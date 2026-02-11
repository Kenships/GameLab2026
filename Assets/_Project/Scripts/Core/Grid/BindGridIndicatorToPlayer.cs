using UnityEngine;
using Sisus.Init;

public class BindGridIndicatorToPlayer : MonoBehaviour<IGridService>
{
    [SerializeField] private Transform frontOfPlayer;
    private IGridService gridSystem;
    private GameObject gridIndicator;
    protected override void Init(IGridService argument)
    {
        gridSystem = argument;
    }
    private void Start()
    {
        gridIndicator = gridSystem.GetGridIndicator();
    }
    private void Update()
    {
        gridIndicator.transform.position = gridSystem.GetGridWorldPosition(frontOfPlayer.position);
    }
}   
