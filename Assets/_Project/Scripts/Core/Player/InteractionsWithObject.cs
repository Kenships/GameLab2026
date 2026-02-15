using _Project.Scripts.Core.Grid;
using _Project.Scripts.Core.InputManagement.Interfaces;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    public class InteractionsWithObject : MonoBehaviour<INESActionReader,IGridService>
    {
        [SerializeField] private bool allowDiagonal = false;
        [SerializeField] private Transform frontOfPlayer;
        private GameObject _currentHoldingObject;
        private INESActionReader _inputReader;
        private IGridService _gridService;
        protected override void Init(INESActionReader NESActionReader, IGridService gridService)
        {
            _inputReader = NESActionReader;
            _gridService = gridService;
        }
        private void OnEnable()
        {
            _inputReader.OnDoubleTapAltInteract += PickUpOrPutDown;
            _inputReader.OnHoldAltInteract += FastFowrad;
            _inputReader.OnHoldInteract += Rewind;
        }

        private void OnDisable()
        {
            _inputReader.OnDoubleTapAltInteract -= PickUpOrPutDown;
            _inputReader.OnHoldAltInteract -= FastFowrad;
            _inputReader.OnHoldInteract -= Rewind;
        }
        // Double tap A
        private void PickUpOrPutDown()
        {
            // Pick Up
            if (_currentHoldingObject == null)
            {
                GameObject obj = _gridService.GetObjectOnGrid(frontOfPlayer.position);
                if (obj != null)
                {
                    obj.layer = LayerMask.NameToLayer("Ignore Raycast");
                    obj.transform.position = frontOfPlayer.position;
                    obj.transform.SetParent(frontOfPlayer);
                    obj.GetComponent<Collider>().enabled = false;
                    _currentHoldingObject = obj;
                }
            }
            // Put Down
            else
            {
                _currentHoldingObject.transform.SetParent(null);
                _currentHoldingObject.layer = LayerMask.NameToLayer("Object On Grid");
                _currentHoldingObject.transform.position = _gridService.GetGridWorldPosition(frontOfPlayer.position);
                if (!allowDiagonal)
                {
                    Vector3 currentRotation = _currentHoldingObject.transform.eulerAngles;
                    _currentHoldingObject.transform.rotation = Quaternion.Euler(
                        currentRotation.x,
                        AdjustIfDiagonal(currentRotation.y),
                        currentRotation.z
                    );
                }
                _currentHoldingObject.GetComponent<Collider>().enabled = true;
                _currentHoldingObject = null;
            }
        }
        // Hold A
        private void FastFowrad()
        {
            GameObject objOnGrid = _gridService.GetObjectOnGrid(frontOfPlayer.position);
            if(objOnGrid != null)
            {
                ITimeControllable timeControllable = objOnGrid.GetComponent<ITimeControllable>();
                if(timeControllable != null)
                {
                    timeControllable.FastForward();
                }
            }
        }
        // Hold B
        private void Rewind()
        {
            GameObject objOnGrid = _gridService.GetObjectOnGrid(frontOfPlayer.position);
            if (objOnGrid != null)
            {
                ITimeControllable timeControllable = objOnGrid.GetComponent<ITimeControllable>();
                if (timeControllable != null)
                {
                    timeControllable.Rewind();
                }
            }
        }
        private float AdjustIfDiagonal(float angle)
        {
            float tolerance = 1f;

            angle = (angle % 360f + 360f) % 360f;

            // Check if within tolerance of axes (0бу, 90бу, 180бу, 270бу, 360бу)
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
