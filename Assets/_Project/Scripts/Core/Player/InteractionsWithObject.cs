using System.Collections.Generic;
using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.InputManagement.Interfaces;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    public class InteractionsWithObject : MonoBehaviour<INESActionReader,IGridService>
    {
        [SerializeField] private Transform frontOfPlayer;
        private GameObject _currentHoldingObject;
        private INESActionReader _inputReader;
        private IGridService _gridService;
        
        private readonly List<ITimeControllable> _currentlyTimeControlledObjects = new ();
        
        protected override void Init(INESActionReader NESActionReader, IGridService gridService)
        {
            _inputReader = NESActionReader;
            _gridService = gridService;
        }
        
        private void OnEnable()
        {
            _inputReader.OnDoubleTapAltInteract += PickUpOrPutDown;
            
            _inputReader.OnHoldInteract += FastForward;
            _inputReader.OnReleaseInteract += CancelFastForward;
            
            _inputReader.OnHoldAltInteract += Rewind;
            _inputReader.OnReleaseAltInteract += CancelRewind;
        }
        
        //Maybe merge the Cancel interactions into one
        
        private void CancelFastForward()
        {
            foreach (var timeControllable in _currentlyTimeControlledObjects)
            {
                timeControllable.CancelFastForward();
            }
            
            _currentlyTimeControlledObjects.Clear();
        }
        
        
        private void CancelRewind()
        {
            foreach (var timeControllable in _currentlyTimeControlledObjects)
            {
                timeControllable.CancelRewind();
            }
            _currentlyTimeControlledObjects.Clear();
            
        }

        

        private void OnDisable()
        {
            _inputReader.OnDoubleTapAltInteract -= PickUpOrPutDown;
            _inputReader.OnHoldAltInteract -= FastForward;
            _inputReader.OnHoldInteract -= Rewind;
        }
        
        // Double tap A
        private void PickUpOrPutDown()
        {
            // Pick Up
            if (!_currentHoldingObject)
            {
                GameObject[] objects = _gridService.GetObjectsInRadius(frontOfPlayer.position);
                // Maybe do some logic to check if the object can be picked up IHoldable interface
                
                var obj = ChooseItemToPickUp(objects);
                
                obj.layer = LayerMask.NameToLayer("Ignore Raycast");
                obj.transform.position = frontOfPlayer.position;
                obj.transform.SetParent(frontOfPlayer);
                obj.GetComponent<Collider>().enabled = false;
                
                _currentHoldingObject = obj;
            }
            // Put Down
            else
            {
                _currentHoldingObject.transform.SetParent(null);
                _currentHoldingObject.layer = LayerMask.NameToLayer("Object On Grid");
                
                _gridService.PlaceObjectOnGrid(_currentHoldingObject, frontOfPlayer.position);
                
                _currentHoldingObject.GetComponent<Collider>().enabled = true;
                _currentHoldingObject = null;
            }
        }

        private GameObject ChooseItemToPickUp(GameObject[] objects)
        {
            // Current strategy is to find the object in the direction the player is facing
            
            Vector3 myPosition = transform.position;
            Vector3 myDirection = transform.forward;
            
            float bestDotProduct = Vector3.Dot((objects[0].transform.position - myPosition).normalized, myDirection);
            int bestIndex = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                float dot = Vector3.Dot((objects[i].transform.position - myPosition).normalized, myDirection);

                if (dot > bestDotProduct)
                {
                    bestIndex = i;
                }
            }
            
            return objects[bestIndex];
        }

        // Hold A
        private void FastForward()
        {
            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }
            
            
            GameObject[] objectsOnGrid = _gridService.GetObjectsInRadius(frontOfPlayer.position);

            foreach (var objOnGrid in objectsOnGrid)
            {
                if(objOnGrid && objOnGrid.TryGetComponent(out ITimeControllable timeControllable))
                {
                    timeControllable.FastForward();
                    _currentlyTimeControlledObjects.Add(timeControllable);
                }
            }
            
        }

        // Hold B
        private void Rewind()
        {
            //Some default logic to determine if Interact is possible right now
            if (!CanInteract())
            {
                return;
            }
            
            GameObject[] objectsOnGrid = _gridService.GetObjectsInRadius(frontOfPlayer.position);

            foreach (var objOnGrid in objectsOnGrid)
            {
                if (objOnGrid && objOnGrid.TryGetComponent(out ITimeControllable timeControllable))
                {
                    timeControllable.Rewind();
                    _currentlyTimeControlledObjects.Add(timeControllable);
                }
            }
        }
        
        private bool CanInteract()
        {
            return !_currentHoldingObject;
        }
        
        private float AdjustIfDiagonal(float angle)
        {
            float tolerance = 1f;

            angle = (angle % 360f + 360f) % 360f;

            // Check if within tolerance of axes (0��, 90��, 180��, 270��, 360��)
            float axisRemainder = angle % 90f;
            bool isNearAxis = Mathf.Min(axisRemainder, 90f - axisRemainder) <= tolerance;

            // If not near an axis, rotate counter-clockwise to next axis
            if (!isNearAxis)
            {
                // Calculate next axis (counter-clockwise direction)
                float nextAxis = Mathf.Ceil(angle / 90f) * 90f;
                return nextAxis >= 360f ? 0f : nextAxis;
            }

            return angle;
        }
    }
}
