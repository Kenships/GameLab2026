using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.CameraScripts
{
    public class CinemachineSplit : MonoBehaviour
    {
        [SerializeField] private PlayerInputManager pim;

        private void Reset()
        {
            pim = FindFirstObjectByType<PlayerInputManager>();
        }

        private void OnEnable()
        {
            if (pim != null) pim.onPlayerJoined += OnPlayerJoined;
        }

        private void OnDisable()
        {
            if (pim != null) pim.onPlayerJoined -= OnPlayerJoined;
        }

        private void OnPlayerJoined(PlayerInput playerInput)
        {
            int index = playerInput.playerIndex; // 0,1,2...

            // 1) Find the spawned player camera for this player
            var cam = playerInput.camera; // this is the split-screen camera instance for that player

            // 2) Ensure it has a brain
            var brain = cam.GetComponent<CinemachineBrain>();
            if (brain == null) brain = cam.gameObject.AddComponent<CinemachineBrain>();

            // 3) Find this player's vcam (assumes it's in the player prefab)
            var vcam = playerInput.GetComponentInChildren<CinemachineVirtualCamera>(true);
            if (vcam == null)
            {
                Debug.LogWarning("No CinemachineVirtualCamera found under player prefab.");
                return;
            }

            // 4) Assign a unique channel per player
            // CinemachineChannels uses bitmask. Channel 1 = 1<<0, Channel 2 = 1<<1, etc.
            int channelBit = 1 << index;

            // vcam output channel
            vcam.OutputChannel = (OutputChannels) channelBit;

            // brain channel mask
            brain.ChannelMask = (OutputChannels) channelBit;

            // Optional: set priority to make sure it's active
            vcam.Priority = 10;
        }
    }
}


