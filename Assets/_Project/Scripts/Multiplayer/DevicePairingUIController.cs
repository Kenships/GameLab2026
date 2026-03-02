using System;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Multiplayer
{
    public class DevicePairingUIController : MonoBehaviour<IDevicePairingService>
    {
        [SerializeField] private GameObject bothPlayersPairedUI;
        [SerializeField] private GameObject player1PairedUI;
        [SerializeField] private GameObject player2PairedUI;
        [SerializeField] private GameObject player1UnpairedUI;
        [SerializeField] private GameObject player2UnpairedUI;
        
        
        
        private IDevicePairingService _devicePairingService;
        
        protected override void Init(IDevicePairingService devicePairingService)
        {
            _devicePairingService = devicePairingService;
        }

        private void OnEnable()
        {
            _devicePairingService.OnBothPlayersPaired += DevicePairingServiceOnBothPlayersPaired;
            _devicePairingService.OnPlayer1Paired += DevicePairingServiceOnPlayer1Paired;
            _devicePairingService.OnPlayer2Paired += DevicePairingServiceOnPlayer2Paired;
            _devicePairingService.OnPlayer1Unpaired += DevicePairingServiceOnPlayer1Unpaired;
            _devicePairingService.OnPlayer2Unpaired += DevicePairingServiceOnPlayer2Unpaired;
        }

        private void OnDisable()
        {
            if (_devicePairingService == null)
            {
                Debug.LogWarning("DevicePairingService is null");
                return;
            }
            
            _devicePairingService.OnBothPlayersPaired -= DevicePairingServiceOnBothPlayersPaired;
            _devicePairingService.OnPlayer1Paired -= DevicePairingServiceOnPlayer1Paired;
            _devicePairingService.OnPlayer2Paired -= DevicePairingServiceOnPlayer2Paired;
            _devicePairingService.OnPlayer1Unpaired -= DevicePairingServiceOnPlayer1Unpaired;
            _devicePairingService.OnPlayer2Unpaired -= DevicePairingServiceOnPlayer2Unpaired;
        }

        private void Update()
        {
            UpdateUI();
        }

        //Currently they all do the same thing
        private void DevicePairingServiceOnBothPlayersPaired()
        {
            UpdateUI();
        }

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
            bothPlayersPairedUI.SetActive(_devicePairingService.IsPlayer1Paired && _devicePairingService.IsPlayer2Paired);
            
            player1PairedUI.SetActive(_devicePairingService.IsPlayer1Paired);
            player2PairedUI.SetActive(_devicePairingService.IsPlayer2Paired);
            
            player1UnpairedUI.SetActive(!_devicePairingService.IsPlayer1Paired);
            player2UnpairedUI.SetActive(!_devicePairingService.IsPlayer2Paired);
        }
    }
}

