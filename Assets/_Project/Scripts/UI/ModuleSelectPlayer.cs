using System;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using Sisus.Init;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.UI
{
    public class ModuleSelectPlayer : MonoBehaviour<INESActionReader>
    {
        public event UnityAction<PlayerData.PlayerID> OnConfirm;
        public event UnityAction<PlayerData.PlayerID> OnCancel;
        public event UnityAction<(PlayerData.PlayerID, Vector2Int)> OnMove;
        
        private INESActionReader _actionReader;
        private PlayerData _playerData;

        protected override void Init(INESActionReader actionReader)
        {
            _actionReader = actionReader;
        }

        protected override void OnAwake()
        {
            _playerData = GetComponent<PlayerData>();
        }

        private void Start()
        {
            //TODO migrate to loading modules manually
            //Module[] modulesArray = Resources.LoadAll<Module>("Modules");
            
            _actionReader.OnDPadInput += HandlePad;
            _actionReader.OnTapInteract += Select;
            _actionReader.OnTapAltInteract += Cancel;
        }

        private void OnDestroy()
        {
            _actionReader.OnDPadInput -= HandlePad;
            _actionReader.OnTapInteract -= Select;
            _actionReader.OnTapAltInteract -= Cancel;
        }
        
        private void Cancel()
        {
            OnCancel?.Invoke(_playerData.ID);
        }

        private void Select()
        {
            OnConfirm?.Invoke(_playerData.ID);
        }

        private void HandlePad(Vector2 dir)
        {
            OnMove?.Invoke((_playerData.ID, new Vector2Int((int)dir.x, (int)dir.y)));
        }
    }
}
