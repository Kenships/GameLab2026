using _Project.Scripts.Core.Player;
using Sisus.Init;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace _Project.Scripts.Multiplayer
{
    [Service(typeof(IDevicePairingService), LoadScene = 0)]
    public class DevicePairingService : MonoBehaviour, IDevicePairingService
    {
        public NESActions Value => new();

        public bool TryGetFor(Component client, out NESActions value)
        {
            if (client is null)
            {
                value = null;
                return false;
            }

            if (client.TryGetComponent(out PlayerData playerData))
            {
                if (playerData.ID == PlayerData.PlayerID.Player1)
                {
                    Debug.Log("Player 1 Actions Given");
                    _player1Actions ??= new NESActions();
                    value = _player1Actions;
                    return true;
                }
                
                if (playerData.ID == PlayerData.PlayerID.Player2)
                {
                    Debug.Log("Player 2 Actions Given");
                    _player2Actions ??= new NESActions();
                    value = _player2Actions;
                    return true;
                }
            }

            value = null;
            return false;
        }

        public event UnityAction OnPlayer1Paired;
        public event UnityAction OnPlayer2Paired;
        public event UnityAction OnBothPlayersPaired;
        public event UnityAction OnPlayer1Unpaired;
        public event UnityAction OnPlayer2Unpaired;
        
        public bool IsPlayer1Paired => _player1Device != null || allowKeyboard;
        public bool IsPlayer2Paired => _player2Device != null;
        
        [SerializeField] private bool allowKeyboard;
        
        private InputUser _player1, _player2;
        private Gamepad _player1Device, _player2Device;
        private NESActions _player1Actions, _player2Actions;

        private void OnEnable()
        {
            _player1 = InputUser.CreateUserWithoutPairedDevices();
            _player2 = InputUser.CreateUserWithoutPairedDevices();
            
            _player1Actions ??= new NESActions();
            _player2Actions ??= new NESActions();
            
            _player1.AssociateActionsWithUser(_player1Actions);
            _player2.AssociateActionsWithUser(_player2Actions);

            if (allowKeyboard)
            {
                if (Keyboard.current != null)
                {
                    InputUser.PerformPairingWithDevice(Keyboard.current, _player1);
                }
                
                if (Mouse.current != null)
                {
                    InputUser.PerformPairingWithDevice(Mouse.current, _player1);
                }
            }

            if (Gamepad.all.Count >= 2)
            {
                Debug.Log("Gamepads Found");
            }
            
            Debug.Log($"Gamepads Found: {Gamepad.all.Count}");

            foreach (Gamepad device in Gamepad.all)
            {
                if (!device.enabled) continue;
                
                Debug.Log($"Gamepad Found: id: {device.deviceId} hash: {device.GetHashCode()} layout: {device.layout}");
                
                TryAssign(device);
            }
            
            InputSystem.onDeviceChange += InputSystemOnDeviceChange;
            
        }
        
        private void OnDisable()
        {
            _player1.UnpairDevicesAndRemoveUser();
            _player2.UnpairDevicesAndRemoveUser();
            
            InputSystem.onDeviceChange -= InputSystemOnDeviceChange;
        }

        private void InputSystemOnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            Debug.Log($"[DeviceChange] {change} | {device.displayName} | layout={device.layout} | id={device.deviceId}");
            if (device is not Gamepad gamepad) return;

            if (change == InputDeviceChange.Added || 
                change == InputDeviceChange.Reconnected ||
                change == InputDeviceChange.Enabled)
            {
                TryAssign(gamepad);
            }
            else if (change == InputDeviceChange.Disconnected ||
                change == InputDeviceChange.Removed ||
                change == InputDeviceChange.Disabled)
            {
                if (gamepad == _player1Device)
                {
                    _player1Device = null;
                    OnPlayer1Unpaired?.Invoke();
                }
                else if (gamepad == _player2Device)
                {
                    _player2Device = null;
                    OnPlayer2Unpaired?.Invoke();
                }
            }
        }

        private void TryAssign(Gamepad device)
        {
            if (_player1Device == device || _player2Device == device) return;
            
            
            // If keyboard is allowed pair player 2 first
            if (allowKeyboard)
            {
                if (_player2Device == null)
                {
                    Pair(_player2, ref _player2Device, device);
                    OnPlayer2Paired?.Invoke();
                }
                else if (_player1Device == null)
                {
                    Pair(_player1, ref _player1Device, device);
                    OnPlayer1Paired?.Invoke();
                }
            }
            else
            {
                if (_player1Device == null)
                {
                    Pair(_player1, ref _player1Device, device);
                    OnPlayer1Paired?.Invoke();
                }
                else if (_player2Device == null)
                {
                    Pair(_player2, ref _player2Device, device);
                    OnPlayer2Paired?.Invoke();
                }
            }

            if ((_player1Device != null || allowKeyboard) && _player2Device != null)
            {
                OnBothPlayersPaired?.Invoke();
            }
        }

        private void Pair(InputUser user, ref Gamepad slotPad, Gamepad device)
        {
            user.UnpairDevices();
            
            InputUser.PerformPairingWithDevice(device, user);
            
            Debug.Log($"[Pairing] {device.displayName} | layout={device.layout} | id={device.deviceId} with {user.id}");
            
            slotPad = device;
        }
    }
}
