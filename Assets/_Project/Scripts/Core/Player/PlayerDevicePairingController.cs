using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Multiplayer;
using Obvious.Soap;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    [RequireComponent(typeof(PlayerData))]
    public class PlayerDevicePairingController : MonoBehaviour<INESActionReader, IDevicePairingService>
    {
        [SerializeField] private ScriptableEventBool readyEvent;
        [SerializeField] private FloatVariable readyProgress;
        [SerializeField] private float speed;
        private INESActionReader _nesActionReader;
        private IDevicePairingService _devicePairingService;
        private PlayerData _playerData;

        private bool _isHolding;
        private bool _isReady;
        
        protected override void Init(INESActionReader actionReader,IDevicePairingService devicePairingService)
        {
            _nesActionReader = actionReader;
            _devicePairingService = devicePairingService;
            _playerData = GetComponent<PlayerData>();
        }

        private void Start()
        {
            if (_playerData.ID == PlayerData.PlayerID.Player1)
            {
                _devicePairingService.OnPlayer1Unpaired += DevicePairingServiceOnPlayerUnpaired;
            }
            else
            {
                _devicePairingService.OnPlayer2Unpaired += DevicePairingServiceOnPlayerUnpaired;
            }
                        
            _nesActionReader.OnHoldInteract += OnHoldInteract;
            _nesActionReader.OnReleaseInteract += OnReleaseInteract;
        }
        
        private void OnDestroy()
        {
            if (_playerData.ID == PlayerData.PlayerID.Player1)
            {
                _devicePairingService.OnPlayer1Unpaired -= DevicePairingServiceOnPlayerUnpaired;
            }
            else
            {
                _devicePairingService.OnPlayer2Unpaired -= DevicePairingServiceOnPlayerUnpaired;
            }
            
            _nesActionReader.OnHoldInteract -= OnHoldInteract;
            _nesActionReader.OnReleaseInteract -= OnReleaseInteract;
        }

        private void DevicePairingServiceOnPlayerUnpaired()
        {
            readyEvent.Raise(false);
            readyProgress.Value = 0;
            _isReady = false;
        }
        
        private void OnHoldInteract()
        {
            _isHolding = true;
        }

        private void OnReleaseInteract()
        {
            _isHolding = false;
        }

        

        

        private void Update()
        {
            if (_isHolding && !_isReady)
            {
                readyProgress.Value += Time.deltaTime * speed;
            }
            else if (!_isHolding && !_isReady)
            {
                readyProgress.Value -= Time.deltaTime * speed;
            }

            readyProgress.Value = Mathf.Clamp(readyProgress.Value, 0, 100f);
            
            if (readyProgress.Value >= 100f)
            {
                readyEvent.Raise(true);
                _isReady = true;
            }
        }
    }
}
