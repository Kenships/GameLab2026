using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Grid
{
    public class BindGridIndicatorToPlayer : MonoBehaviour<IGridService>
    {
        [SerializeField] private Transform frontOfPlayer;
        private IGridService _gridSystem;
        [SerializeField] private GameObject gridIndicator;
        protected override void Init(IGridService gridService)
        {
            _gridSystem = gridService;
        }
        
        private void Update()
        {
            gridIndicator.transform.position = _gridSystem.GetGridWorldPosition(frontOfPlayer.position);
        }
    }
}   
