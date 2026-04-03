using System;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.UI;
using Obvious.Soap;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Multiplayer
{
    public class DevicePairingUIController : MonoBehaviour<IDevicePairingService>
    {
        [SerializeField] private PlayerMenuAnimationController player1AnimationController;
        [SerializeField] private PlayerMenuAnimationController player2AnimationController;

        [SerializeField] private ModuleSelectPlayer player1;
        [SerializeField] private ModuleSelectPlayer player2;

        [SerializeField] private GameObject player1SelectVisual;
        [SerializeField] private GameObject player2SelectVisual;

        [SerializeField] private GameObject player1ConfirmVisual;
        [SerializeField] private GameObject player2ConfirmVisual;

        [SerializeField] private Transform[] treys;
        
        private bool _player1Ready;
        private bool _player2Ready;

        private int _player1Index;
        private int _player2Index;
        
        private IDevicePairingService _devicePairingService;
        private SceneLoader _sceneLoader;
        
        protected override void Init(IDevicePairingService devicePairingService)
        {
            _devicePairingService = devicePairingService;
            _sceneLoader = GetComponent<SceneLoader>();
        }

        private void OnEnable()
        {
            _player1Index = 1;
            _player2Index = 1;
            
            player1.OnMove += PlayerOnMove;
            player2.OnMove += PlayerOnMove;
            
            player1.OnConfirm += PlayerOnConfirm;
            player2.OnConfirm += PlayerOnConfirm;
            
            _devicePairingService.OnPlayer1Paired += DevicePairingServiceOnPlayer1Paired;
            _devicePairingService.OnPlayer2Paired += DevicePairingServiceOnPlayer2Paired;
            _devicePairingService.OnPlayer1Unpaired += DevicePairingServiceOnPlayer1Unpaired;
            _devicePairingService.OnPlayer2Unpaired += DevicePairingServiceOnPlayer2Unpaired;
        }

        private void PlayerOnConfirm(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    if (_player1Index != 1 && _player1Index != _player2Index)
                        _player1Ready = true;
                    break;
                case PlayerData.PlayerID.Player2:
                    if (_player2Index != 1 && _player2Index != _player1Index)
                        _player2Ready = true;
                    break;
            }
            UpdateUI();
        }
        
        private bool UpdateSelectIndex(ref int selectedModuleNumber, int delta)
        {
            int newIndex = Mathf.Clamp(selectedModuleNumber + delta, 0, treys.Length - 1);

            if (newIndex != selectedModuleNumber)
            {
                selectedModuleNumber = newIndex;
                return true;
            }
            
            return false;
        }

        private void PlayerOnMove((PlayerData.PlayerID id, int dir) arg)
        {
            switch (arg.id)
            {
                case PlayerData.PlayerID.Player1:
                    if (UpdateSelectIndex(ref _player1Index, arg.dir))
                    {
                        _player1Ready = false;
                    }
                    break;
                case PlayerData.PlayerID.Player2:
                    if (UpdateSelectIndex(ref _player2Index, arg.dir))
                    {
                        _player2Ready = false;
                    }
                    break;
            }
            UpdateUI();
        }

        private void OnDisable()
        {
            if (_devicePairingService == null)
            {
                Debug.LogWarning("DevicePairingService is null");
                return;
            }
            
            _devicePairingService.OnPlayer1Paired -= DevicePairingServiceOnPlayer1Paired;
            _devicePairingService.OnPlayer2Paired -= DevicePairingServiceOnPlayer2Paired;
            _devicePairingService.OnPlayer1Unpaired -= DevicePairingServiceOnPlayer1Unpaired;
            _devicePairingService.OnPlayer2Unpaired -= DevicePairingServiceOnPlayer2Unpaired;
        }

        private void Update()
        {
            UpdateUI();

            if (_player1Ready && _player2Ready)
            {
                _devicePairingService.SwapPlayers = _player1Index > _player2Index;
                _sceneLoader.LoadScene();
            }
        }

        //Currently they all do the same thing
        private void DevicePairingServiceOnPlayer1Paired()
        {
            UpdateUI();
        }
        
        private void DevicePairingServiceOnPlayer2Paired()
        {
            UpdateUI();
        }
        
        private void DevicePairingServiceOnPlayer1Unpaired()
        {
            UpdateUI();
        }
        
        private void DevicePairingServiceOnPlayer2Unpaired()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            player1SelectVisual.SetActive(_devicePairingService.IsPlayer1Paired);
            player2SelectVisual.SetActive(_devicePairingService.IsPlayer2Paired);
            
            player1ConfirmVisual.SetActive(_player1Ready);
            player2ConfirmVisual.SetActive(_player2Ready);
            
            player1SelectVisual.transform.SetParent(treys[_player1Index]);
            player2SelectVisual.transform.SetParent(treys[_player2Index]);
        }
    }
}

