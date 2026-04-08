using System.Collections;
using _Project.Scripts.Core.AudioPooling;
using Knot.Localization;
using Knot.Localization.Components;
using Sisus.Init;
using TMPro;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.UI
{
    public class TextTyper : MonoBehaviour<AudioPooler>
    {
        private KnotLocalizedTextMeshProUGUI localizedText;
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
            localizedText = GetComponent<KnotLocalizedTextMeshProUGUI>();
        }

        IEnumerator ShowTextRoutine()
        {
            dialogueText.maxVisibleCharacters = 0;


            for (int i = 0; i < dialogueText.text.Length; i++)
            {
                if (hasSFX)
                {
                    _audioPooler.New2DAudio(typeSound).OnChannel(AudioType.Sfx).SetVolume(typeSoundVolume).RandomizePitch(0.1f, 0.5f).Play();
                }
                dialogueText.maxVisibleCharacters = i + 1;
                yield return new WaitForSeconds(delay);
            }
        }

        public void StartTyping(KnotTextKeyReference key)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            dialogueText.maxVisibleCharacters = 0;
            
            localizedText.KeyReference = key;
            localizedText.ForceUpdateValue();
        
            StopAllCoroutines();
            StartCoroutine(ShowTextRoutine());
        }

        public void StartTyping(string text)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.text = text;
            
            StopAllCoroutines();
            StartCoroutine(ShowTextRoutine());
        }
    }
}
