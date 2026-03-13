using System;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Enemies;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class EnemyWaveUI : MonoBehaviour
    {
        [SerializeField] private ScriptableEventWaveData waveData;

        private TextMeshProUGUI _text;
        
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();    
        }

        private void Start()
        {
            waveData.OnRaised += WaveDataOnRaised;
        }

        private void WaveDataOnRaised(EnemyWaveSpawner.Wave wave)
        {
            _text.text = wave.waveName;
        }
    }
}
