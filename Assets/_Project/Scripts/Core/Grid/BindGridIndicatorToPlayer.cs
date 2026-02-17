using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Grid
{
    public class BindGridIndicatorToPlayer : MonoBehaviour<IGridService>
    {
        [SerializeField] private Transform frontOfPlayer;
        [SerializeField] private GameObject gridIndicator;
        private IGridService _gridSystem;
        
        protected override void Init(IGridService gridService)
        {
            _gridSystem = gridService;
            gridIndicator.transform.SetParent(null);
        }
        
        private void Update()
        {
            gridIndicator.transform.position = _gridSystem.GetGridWorldPosition(frontOfPlayer.position);
        }
    }
}   
