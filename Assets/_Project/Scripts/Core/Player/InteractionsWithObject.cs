using _Project.Scripts.Core.InputManagement.Interfaces;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    public class InteractionsWithObject : MonoBehaviour<INESActionReader,IGridService>
    {
        [SerializeField] private Transform frontOfPlayer;
        private GameObject currentHoldingObject;
        private INESActionReader _inputReader;
        private IGridService gridService;
        protected override void Init(INESActionReader firstArgument, IGridService secondArgument)
        {
            _inputReader = firstArgument;
            gridService = secondArgument;
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
            if (currentHoldingObject == null)
            {
                GameObject obj = gridService.GetObjectOnGridIndicator();
                if (obj != null)
                {
                    obj.layer = LayerMask.NameToLayer("Ignore Raycast");
                    obj.transform.position = frontOfPlayer.position;
                    obj.transform.SetParent(frontOfPlayer);
                    obj.GetComponent<Collider>().enabled = false;
                    currentHoldingObject = obj;
                }
            }
            // Put Down
            else
            {
                currentHoldingObject.transform.SetParent(null);
                currentHoldingObject.layer = LayerMask.NameToLayer("Object On Grid");
                currentHoldingObject.transform.position = gridService.GetGridIndicatorWorldPosition();
                currentHoldingObject.GetComponent<Collider>().enabled = true;
                currentHoldingObject = null;
            }
        }
        // Hold A
        private void FastFowrad()
        {
            GameObject objOnGrid = gridService.GetObjectOnGridIndicator();
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
            GameObject objOnGrid = gridService.GetObjectOnGridIndicator();
            if (objOnGrid != null)
            {
                ITimeControllable timeControllable = objOnGrid.GetComponent<ITimeControllable>();
                if (timeControllable != null)
                {
                    timeControllable.Rewind();
                }
            }
        }
    }
}
