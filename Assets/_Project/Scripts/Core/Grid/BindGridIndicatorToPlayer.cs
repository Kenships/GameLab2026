using _Project.Scripts.Core.Player;
using Sisus.Init;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Core.Grid
{
    public class BindGridIndicatorToPlayer : MonoBehaviour<IGridService>
    {
        [SerializeField] private Color invalidColor = Color.red;
        [SerializeField] private Color validColor = Color.green;
        
        
        [SerializeField] private Transform frontOfPlayer;
        [SerializeField] private GameObject gridIndicator;
        private IGridService _gridSystem;
        private PlayerInteractionController _playerController;
        private Image _gridIndicatorImage;
        
        protected override void Init(IGridService gridService)
        {
            _gridSystem = gridService;
            gridIndicator.transform.SetParent(null);
            _playerController = GetComponent<PlayerInteractionController>();
            _gridIndicatorImage = gridIndicator.GetComponentInChildren<Image>();
            _gridIndicatorImage.enabled = false;
        }
        
        private void Update()
        {
            var gridWorldPosition = _gridSystem.GetGridWorldPosition(frontOfPlayer.position);
            
            gridIndicator.transform.position = new Vector3(gridWorldPosition.x, gridIndicator.transform.position.y, gridWorldPosition.z);
            if (!_playerController.IsHoldingObject)
            {
                _gridIndicatorImage.enabled = false;
                return;
            }
            
            _gridIndicatorImage.enabled = true;
            
            if (_gridSystem.GetObjectOnGrid(frontOfPlayer.position) != null)
            {
                _gridIndicatorImage.color = invalidColor;
            }
            else
            {
                _gridIndicatorImage.color = validColor;
            }
        }
    }
}   
