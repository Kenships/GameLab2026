using _Project.Scripts.Core.AudioPooling;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;
using Sisus.Init;


namespace _Project.Scripts.Effects
{
    public class TextTyper : MonoBehaviour<AudioPooler>
    {
        private TextMeshProUGUI dialogueText;
        private string fullText;

        public float delay = 0.03f;

        [Header("Audio")]
        [SerializeField] private AudioClip typeSound;
        [SerializeField] private float typeSoundVolume = 0.1f;
        [SerializeField] private bool hasSFX = false;

        private AudioPooler _audioPooler;
        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;

        }

        void Start()
        {
            dialogueText = GetComponent<TextMeshProUGUI>();
        }

        IEnumerator ShowTextRoutine()
        {
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.text = fullText;


            for (int i = 0; i < fullText.Length; i++)
            {
                if (hasSFX == true)
                {
                    _audioPooler.New2DAudio(typeSound).OnChannel(AudioType.Sfx).SetVolume(typeSoundVolume).RandomizePitch(0.1f, 0.5f).Play();
                }
                dialogueText.maxVisibleCharacters = i + 1;
                yield return new WaitForSeconds(delay);
            }
        }

        public void StartTyping(string textToType)
        {
            fullText = textToType;
            dialogueText.text = "";

            if (!gameObject.activeInHierarchy)
            {
                return;
            }
        
            StopAllCoroutines();
            StartCoroutine(ShowTextRoutine());
        }
    }
}
