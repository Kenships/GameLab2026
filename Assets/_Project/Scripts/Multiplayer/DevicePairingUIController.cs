using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.UI;
using PrimeTween;
using Sisus.Init;
using UnityEngine;
using UnityEngine.UI;

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
        
        [SerializeField] private GameObject player1GhostVisual;
        [SerializeField] private GameObject player2GhostVisual;

        [SerializeField] private GameObject redConfirmVisual;
        [SerializeField] private GameObject blueConfirmVisual;

        [SerializeField] private Transform[] treys;

        private bool _player1Init;
        private bool _player2Init;
        
        private bool _player1Ready;
        private bool _player2Ready;

        private int _player1Index;
        private int _player2Index;
        
        private IDevicePairingService _devicePairingService;
        private SceneLoader _sceneLoader;

        private bool _bufferFrame;
        
        protected override void Init(IDevicePairingService devicePairingService)
        {
            _devicePairingService = devicePairingService;
            _sceneLoader = GetComponent<SceneLoader>();
        }

        private void OnEnable()
        {
            player1.OnMove += PlayerOnMove;
            player2.OnMove += PlayerOnMove;
            
            player1.OnConfirm += PlayerOnConfirm;
            player2.OnConfirm += PlayerOnConfirm;

            player1.OnCancel += PlayerOnCancel;
            player2.OnCancel += PlayerOnCancel;
            
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
            
            player1.OnMove -= PlayerOnMove;
            player2.OnMove -= PlayerOnMove;
            
            player1.OnConfirm -= PlayerOnConfirm;
            player2.OnConfirm -= PlayerOnConfirm;
            
            player1.OnCancel -= PlayerOnCancel;
            player2.OnCancel -= PlayerOnCancel;
            
            _devicePairingService.OnPlayer1Paired -= DevicePairingServiceOnPlayer1Paired;
            _devicePairingService.OnPlayer2Paired -= DevicePairingServiceOnPlayer2Paired;
            _devicePairingService.OnPlayer1Unpaired -= DevicePairingServiceOnPlayer1Unpaired;
            _devicePairingService.OnPlayer2Unpaired -= DevicePairingServiceOnPlayer2Unpaired;
        }

        private void PlayerOnCancel(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    _player1Ready = false;
                    break;
                case PlayerData.PlayerID.Player2:
                    _player2Ready = false;
                    break;
            }
            UpdateUI();
        }

        private void PlayerOnConfirm(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    if (!_player1Init)
                        return;
                    if (_player1Index != _player2Index || !_player2Init){
                        PlayerConfirm(_player1Index);
                        _player1Ready = true;
                    }
                    break;
                case PlayerData.PlayerID.Player2:
                    if (!_player2Init)
                        return;
                    if (_player2Index != _player1Index || !_player1Init)
                    {
                        PlayerConfirm(_player2Index);
                        _player2Ready = true;
                    }
                    break;
            }
            UpdateUI();
        }

        private void PlayerConfirm(int index)
        {
            if (index == 0)
            {
                player1AnimationController.Select();
            }

            if (index == treys.Length - 1)
            {
                player2AnimationController.Select();
            }
        }
        
        private void UpdateSelectIndex(ref int selectedModuleNumber, int delta)
        {
            selectedModuleNumber = Mathf.Clamp(selectedModuleNumber + delta, 0, treys.Length - 1);
        }

        private void PlayerOnMove((PlayerData.PlayerID id, int dir) arg)
        {
            if (arg.dir == 0)
                return;
            
            switch (arg.id)
            {
                case PlayerData.PlayerID.Player1:
                    if (_player1Ready)
                    {
                        return;
                    }
                    _player1Init = true;
                    UpdateSelectIndex(ref _player1Index, arg.dir);
                    break;
                case PlayerData.PlayerID.Player2:
                    if (_player2Ready)
                    {
                        return;
                    }
                    _player2Init = true;
                    UpdateSelectIndex(ref _player2Index, arg.dir);
                    break;
            }
            UpdateUI();
        }

        private bool _loading;

        private void Update()
        {
            if (_player1Ready && _player2Ready && !_loading)
            {
                _loading = true;
                _devicePairingService.SwapPlayers = _player1Index > _player2Index;
                _sceneLoader.LoadScene();
            }
        }

        //Currently they all do the same thing
        private void DevicePairingServiceOnPlayer1Paired()
        {
            _player1Init = true;
            UpdateUI();
        }
        
        private void DevicePairingServiceOnPlayer2Paired()
        {
            _player2Init = true;
            UpdateUI();
        }
        
        private void DevicePairingServiceOnPlayer1Unpaired()
        {
            _player1Init = false;
            UpdateUI();
        }
        
        private void DevicePairingServiceOnPlayer2Unpaired()
        {
            _player2Init = false;
            UpdateUI();
        }

        private void UpdateUI()
        {
            player1SelectVisual.SetActive(_player1Init && _devicePairingService.IsPlayer1Paired);
            player1GhostVisual.SetActive(_player1Init && _devicePairingService.IsPlayer1Paired);
            player2SelectVisual.SetActive(_player2Init && _devicePairingService.IsPlayer2Paired);
            player2GhostVisual.SetActive(_player2Init && _devicePairingService.IsPlayer2Paired);
            
            blueConfirmVisual.SetActive((_player1Ready && _devicePairingService.IsPlayer1Paired && _player1Index == 0)
                || (_player2Ready && _devicePairingService.IsPlayer2Paired && _player2Index == 0));
            redConfirmVisual.SetActive((_player1Ready && _devicePairingService.IsPlayer1Paired && _player1Index == treys.Length - 1)
                || (_player2Ready && _devicePairingService.IsPlayer2Paired && _player2Index == treys.Length - 1));

            player1GhostVisual.transform.SetParent(treys[_player1Index]);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) treys[0]);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) treys[1]);
            player1GhostVisual.transform.localPosition = 
                new Vector3(player1GhostVisual.transform.localPosition.x, player1GhostVisual.transform.localPosition.y, 0);
            
            
            player2GhostVisual.transform.SetParent(treys[_player2Index]);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) treys[0]);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) treys[1]);
            player2GhostVisual.transform.localPosition = 
                new Vector3(player2GhostVisual.transform.localPosition.x, player2GhostVisual.transform.localPosition.y, 0);

            
            Tween.Position(
                    target: player1SelectVisual.transform,
                    endValue: player1GhostVisual.transform.position,
                    duration: 0.2f,
                    ease: Ease.Linear
                ).Group(
                    Tween.Rotation(
                        target: player1SelectVisual.transform,
                        endValue: player1GhostVisual.transform.rotation,
                        duration: 0.2f,
                        ease: Ease.Linear
                    )
                );

            Tween.Position(
                    target: player2SelectVisual.transform,
                    endValue: player2GhostVisual.transform.position,
                    duration: 0.2f,
                    ease: Ease.Linear
                ).Group(
                    Tween.Rotation(
                        target: player2SelectVisual.transform,
                        endValue: player2GhostVisual.transform.rotation,
                        duration: 0.2f,
                        ease: Ease.Linear
                    )
                );
        }
    }
}

