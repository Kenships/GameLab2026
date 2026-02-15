using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Grid
{
    public class BindGridIndicatorToPlayer : MonoBehaviour<IGridService>
    {
        [SerializeField, Range(1, 2)] private int playerID = 1;
        [SerializeField] private Transform frontOfPlayer;
        private IGridService _gridSystem;
        private GameObject _gridIndicator;
        protected override void Init(IGridService gridService)
        {
            _gridSystem = gridService;
        }
        private void Start()
        {
            _gridIndicator = _gridSystem.GetGridIndicator(playerID);
        }
        private void Update()
        {
            _gridIndicator.transform.position = _gridSystem.GetGridWorldPosition(frontOfPlayer.position);
        }
    }
}   
