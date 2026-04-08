using _Project.Scripts.Core.Player;
using _Project.Scripts.UI.UIElements;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class LevelSelectController : MonoBehaviour
    {
        [SerializeField] ModuleSelectPlayer[] players;

        [SerializeField] private LevelSelectPanel[] panels;
        
        [SerializeField] private Button backButton;
        
        [SerializeField] private Button[] difficultyButtons;
        
        [SerializeField] private FloatVariable difficulty;
        
        private int _p1SelectedModuleNumber = 1, _p2SelectedModuleNumber = 1;
        private bool _p1Confirm, _p2Confirm;

        private void Start()
        {
            foreach (var player in players)
            {
                player.OnConfirm += PlayerOnConfirm;
                player.OnMove += PlayerOnMove;
            }

            switch (difficulty.Value)
            {
                case > 1f:
                    difficultyButtons[0].onClick.Invoke();
                    break;
                case > 0.8f: 
                    difficultyButtons[1].onClick.Invoke();
                    break;
                case > 0.5f:
                    difficultyButtons[2].onClick.Invoke();
                    break;
                default:
                    difficultyButtons[3].onClick.Invoke();
                    break;
            }
            
            HandleSelect(_p1SelectedModuleNumber, PlayerData.PlayerID.Player1);
            HandleSelect(_p2SelectedModuleNumber, PlayerData.PlayerID.Player2);
        }

        private void PlayerOnMove((PlayerData.PlayerID playerID, Vector2Int dir) arg)
        {
            if (arg.dir.x == 0)
            {
                if (arg.playerID == PlayerData.PlayerID.Player1)
                {
                    HandleSelectedPanelNumber(ref _p1SelectedModuleNumber, -arg.dir.y);
                    HandleSelect(_p1SelectedModuleNumber, arg.playerID);
                }
                else if (arg.playerID == PlayerData.PlayerID.Player2)
                {
                    HandleSelectedPanelNumber(ref _p2SelectedModuleNumber, -arg.dir.y);
                    HandleSelect(_p2SelectedModuleNumber, arg.playerID);
                }
            }
            else if (arg.dir.y == 0)
            {
                if (arg.playerID == PlayerData.PlayerID.Player1)
                {
                    HandleSelectedPanelNumber(ref _p1SelectedModuleNumber, arg.dir.x);
                    HandleSelect(_p1SelectedModuleNumber, arg.playerID);
                }
                else if (arg.playerID == PlayerData.PlayerID.Player2)
                {
                    HandleSelectedPanelNumber(ref _p2SelectedModuleNumber, arg.dir.x);
                    HandleSelect(_p2SelectedModuleNumber, arg.playerID);
                }
                
            }
            
            
        }

        private void HandleSelect(int selectedModuleNumber, PlayerData.PlayerID playerID)
        {
            foreach (var panel in panels)
            {
                panel.Deselect(playerID);
            }
            
            EventSystem.current.SetSelectedGameObject(null);

            switch (selectedModuleNumber)
            {
                case 0:
                    backButton.Select();
                    break;
                case 1:
                case 2:
                    panels[selectedModuleNumber - 1].Select(playerID);
                    break;
                case >= 3:
                    difficultyButtons[selectedModuleNumber - 3].Select();
                    break;
            }
        }

        private void PlayerOnConfirm(PlayerData.PlayerID playerID)
        {
            
            if (playerID == PlayerData.PlayerID.Player1)
            {
                switch (_p1SelectedModuleNumber)
                {
                    case 0:
                        backButton.onClick.Invoke();
                        break;
                    case 1:
                    case 2:
                        _p1Confirm = true;
                        panels[_p1SelectedModuleNumber - 1].Confirm(playerID);
                        break;
                    case >= 3:
                        difficultyButtons[_p1SelectedModuleNumber - 3].onClick.Invoke();
                        break;
                }
            }
            else if (playerID == PlayerData.PlayerID.Player2)
            {
                switch (_p1SelectedModuleNumber)
                {
                    case 0:
                        backButton.onClick.Invoke();
                        break;
                    case 1:
                    case 2:
                        _p2Confirm = true;
                        panels[_p2SelectedModuleNumber - 1].Confirm(playerID);
                        break;
                    case >= 3:
                        difficultyButtons[_p1SelectedModuleNumber - 3].onClick.Invoke();
                        break;
                }
            }
            
            if (_p1Confirm && _p2Confirm && _p1SelectedModuleNumber == _p2SelectedModuleNumber)
            {
                panels[_p1SelectedModuleNumber - 1].LoadLevel();
            }
        }
        
        
        private void HandleSelectedPanelNumber(ref int selectedModuleNumber, int delta)
        {
            if (delta == 0) return;
            selectedModuleNumber = (selectedModuleNumber + delta) % (panels.Length + 1 + difficultyButtons.Length);
            
            if (selectedModuleNumber < 0)
            {
                selectedModuleNumber += panels.Length + 1 + difficultyButtons.Length;
            }
        }
    }
}
