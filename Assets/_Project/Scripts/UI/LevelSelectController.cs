using _Project.Scripts.Core.Player;
using _Project.Scripts.UI.UIElements;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class LevelSelectController : MonoBehaviour
    {
        [SerializeField] ModuleSelectPlayer[] players;

        [SerializeField] private LevelSelectPanel[] panels;
        
        private int _p1SelectedModuleNumber, _p2SelectedModuleNumber;
        private bool _p1Confirm, _p2Confirm;

        private void Start()
        {
            foreach (var player in players)
            {
                player.OnConfirm += PlayerOnConfirm;
                player.OnMove += PlayerOnMove;
            }
            
            HandleSelect(_p1SelectedModuleNumber, PlayerData.PlayerID.Player1);
            HandleSelect(_p2SelectedModuleNumber, PlayerData.PlayerID.Player2);
        }

        private void PlayerOnMove((PlayerData.PlayerID playerID, int dir) arg)
        {
            if (arg.playerID == PlayerData.PlayerID.Player1)
            {
                HandleSelectedPanelNumber(ref _p1SelectedModuleNumber, arg.dir);
                HandleSelect(_p1SelectedModuleNumber, arg.playerID);
            }
            else if (arg.playerID == PlayerData.PlayerID.Player2)
            {
                HandleSelectedPanelNumber(ref _p2SelectedModuleNumber, arg.dir);
                HandleSelect(_p2SelectedModuleNumber, arg.playerID);
            }
        }

        private void HandleSelect(int selectedModuleNumber, PlayerData.PlayerID playerID)
        {
            foreach (var panel in panels)
            {
                panel.Deselect(playerID);
            }
            
            panels[selectedModuleNumber].Select(playerID);
        }

        private void PlayerOnConfirm(PlayerData.PlayerID playerID)
        {
            if (playerID == PlayerData.PlayerID.Player1)
            {
                _p1Confirm = true;
                panels[_p1SelectedModuleNumber].Confirm(playerID);
            }
            else if (playerID == PlayerData.PlayerID.Player2)
            {
                _p2Confirm = true;
                panels[_p2SelectedModuleNumber].Confirm(playerID);
            }
            
            if (_p1Confirm && _p2Confirm && _p1SelectedModuleNumber == _p2SelectedModuleNumber)
            {
                panels[_p1SelectedModuleNumber].LoadLevel();
            }
        }
        
        private void HandleSelectedPanelNumber(ref int selectedModuleNumber, int delta)
        {
            if (delta == 0) return;
            selectedModuleNumber = (selectedModuleNumber + delta) % panels.Length;
            
            if (selectedModuleNumber < 0)
            {
                selectedModuleNumber += panels.Length;
            }
        }
    }
}
