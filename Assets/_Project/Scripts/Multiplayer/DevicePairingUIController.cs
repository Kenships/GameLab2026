using System;
using _Project.Scripts.Core.SceneLoading;
using Obvious.Soap;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Multiplayer
{
    public class DevicePairingUIController : MonoBehaviour<IDevicePairingService>
    {
        [SerializeField] private ScriptableEventBool player1ReadyEvent;
        [SerializeField] private ScriptableEventBool player2ReadyEvent;
        
        [SerializeField] private GameObject player1ReadyProgress;
        [SerializeField] private GameObject player2ReadyProgress;
        [SerializeField] private GameObject player1ReadyText;
        [SerializeField] private GameObject player2ReadyText;
        [SerializeField] private GameObject player1UnreadyText;
        [SerializeField] private GameObject player2UnreadyText;
        [SerializeField] private GameObject player1PairedUI;
        [SerializeField] private GameObject player2PairedUI;
        [SerializeField] private GameObject player1UnpairedUI;
        [SerializeField] private GameObject player2UnpairedUI;
        
        private bool _player1Ready;
        private bool _player2Ready;
        
        private IDevicePairingService _devicePairingService;
        private SceneLoader _sceneLoader;
        
        protected override void Init(IDevicePairingService devicePairingService)
        {
            _devicePairingService = devicePairingService;
            _sceneLoader = GetComponent<SceneLoader>();
        }

        private void OnEnable()
        {
            player1ReadyEvent.OnRaised += OnPlayer1ReadyRaised;
            player2ReadyEvent.OnRaised += OnPlayer2ReadyRaised;
            
            _devicePairingService.OnPlayer1Paired += DevicePairingServiceOnPlayer1Paired;
            _devicePairingService.OnPlayer2Paired += DevicePairingServiceOnPlayer2Paired;
            _devicePairingService.OnPlayer1Unpaired += DevicePairingServiceOnPlayer1Unpaired;
            _devicePairingService.OnPlayer2Unpaired += DevicePairingServiceOnPlayer2Unpaired;
        }

        private void OnPlayer1ReadyRaised(bool ready)
        {
            _player1Ready = ready;
        }
        
        private void OnPlayer2ReadyRaised(bool ready)
        {
            _player2Ready = ready;
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
            player1ReadyProgress.SetActive(_devicePairingService.IsPlayer1Paired);
            player2ReadyProgress.SetActive(_devicePairingService.IsPlayer2Paired);
            player1ReadyText.SetActive(_devicePairingService.IsPlayer1Paired && _player1Ready);
            player2ReadyText.SetActive(_devicePairingService.IsPlayer2Paired && _player2Ready);
            
            player1UnreadyText.SetActive(_devicePairingService.IsPlayer1Paired && !_player1Ready);
            player2UnreadyText.SetActive(_devicePairingService.IsPlayer2Paired && !_player2Ready);
            
            player1PairedUI.SetActive(_devicePairingService.IsPlayer1Paired);
            player2PairedUI.SetActive(_devicePairingService.IsPlayer2Paired);
            
            player1UnpairedUI.SetActive(!_devicePairingService.IsPlayer1Paired);
            player2UnpairedUI.SetActive(!_devicePairingService.IsPlayer2Paired);
        }
    }
}

