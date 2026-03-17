using System.Collections.Generic;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class ModuleSelection : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ModuleSectionUIComponent[] textmodules;
        [SerializeField] private ModuleSelectPlayer[] players;
        [SerializeField] private ScriptableEventGameObject spawnModuleEvent;
        [SerializeField] private List<Module> modules;
        
        private SceneUnloader _sceneUnloader;

        //Zero Indexed
        private int _p1SelectedModuleNumber, _p2SelectedModuleNumber;
        private bool _p1Confirm, _p2Confirm;

        private void Start()
        {
            _sceneUnloader = GetComponent<SceneUnloader>();
        
            //TODO migrate to loading modules manually
            //Module[] modulesArray = Resources.LoadAll<Module>("Modules");

            //modules.AddRange(modulesArray);

            SetModuleSelections();
            
            SelectingModule(PlayerData.PlayerID.Player1, _p1SelectedModuleNumber);
            SelectingModule(PlayerData.PlayerID.Player2, _p2SelectedModuleNumber);

            foreach (var player in players)
            {
                player.OnMove += PlayerOnMove;
                player.OnConfirm += PlayerOnConfirm;
            }
        }

        private void PlayerOnConfirm(PlayerData.PlayerID id)
        {
            switch (id)
            {
                case PlayerData.PlayerID.Player1:
                    ConfirmSelection(id, _p1SelectedModuleNumber);
                    break;
                case PlayerData.PlayerID.Player2:
                    ConfirmSelection(id, _p2SelectedModuleNumber);
                    break;
            }
        }

        private void PlayerOnMove((PlayerData.PlayerID ID, int dir) arg)
        {
            switch (arg.ID)
            {
                case PlayerData.PlayerID.Player1:
                    SelectingModule(arg.ID, _p1SelectedModuleNumber);
                    HandleSelectedModuleNumber(ref _p1SelectedModuleNumber, arg.dir);
                    break;
                case PlayerData.PlayerID.Player2:
                    SelectingModule(arg.ID, _p2SelectedModuleNumber);
                    HandleSelectedModuleNumber(ref _p2SelectedModuleNumber, arg.dir);
                    break;
                default: break;
            }
        }

        private void HandleSelectedModuleNumber(ref int selectedModuleNumber, int delta)
        {
            selectedModuleNumber = (selectedModuleNumber + delta) % textmodules.Length;

            if (selectedModuleNumber < 0)
            {
                selectedModuleNumber += textmodules.Length;
            }
        }


        private Module ReturnRandAndRemove()
        {
            if (modules.Count == 0) return null;
        
            int rand = Random.Range(0, modules.Count);
            Module toReturn = modules[rand];
            modules.Remove(modules[rand]);

            return toReturn;
        }

        private void SetModuleSelections()
        {
            foreach (ModuleSectionUIComponent moduleUI in textmodules)
            {
                SetModuleUI(moduleUI);
            }
        }

        private void SetModuleUI(ModuleSectionUIComponent moduleUI)
        {
            Module selectedModule = ReturnRandAndRemove();
            moduleUI.Initialize(selectedModule);
        }

    
        private void SelectingModule(PlayerData.PlayerID id, int selectedModuleNumber)
        {
            switch (id)
            {
                case PlayerData.PlayerID.Player1:
                    _p1Confirm = false;
                    break;
                case PlayerData.PlayerID.Player2:
                    _p2Confirm = false;
                    break;
            }

            for (int i = 0; i < textmodules.Length; i++)
            {
                if (i == selectedModuleNumber)
                {
                    textmodules[i].Select(id);
                }
                else
                {
                    textmodules[i].Unselect(id);
                }
            }
        }

        private void ConfirmSelection(PlayerData.PlayerID playerID, int selectedModuleNumber)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    textmodules[selectedModuleNumber].Confirm(playerID);
                    _p1Confirm = true;
                    break;
                case PlayerData.PlayerID.Player2:
                    textmodules[selectedModuleNumber].Confirm(playerID);
                    _p2Confirm = true;
                    break;
            }

            if (_p1Confirm && _p2Confirm && _p1SelectedModuleNumber == _p2SelectedModuleNumber)
            {
                AcquireModule();
            }
        }

        private void AcquireModule()
        {
            Module selectedModule = textmodules[_p1SelectedModuleNumber].GetSelectedModule();
            spawnModuleEvent.Raise(selectedModule.gameObject);
            Time.timeScale = 1f;
            _sceneUnloader.UnloadScene();
        }
    }
}
