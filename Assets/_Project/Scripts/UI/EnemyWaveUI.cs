using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Enemies;
using PrimeTween;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class EnemyWaveUI : MonoBehaviour
    {
        [SerializeField] private ScriptableEventWaveData waveData;
        [SerializeField] private TextMeshProUGUI waveTitle;
        [SerializeField] private TextMeshProUGUI waveCompletedText;
        [SerializeField] private TextMeshProUGUI countdownText;

        private Coroutine currentCountdownCoroutine;

        private void Start()
        {
            waveData.OnRaised += WaveDataOnRaised;
        }

        private void WaveDataOnRaised(EnemyWaveSpawner.Wave wave)
        {
            waveTitle.text = wave.waveName;
        }

        public IEnumerator ShowWaveCompleted()
        {
            if (waveCompletedText == null) yield break;

            // Start fully transparent
            waveCompletedText.color = new Color(waveCompletedText.color.r, waveCompletedText.color.g, waveCompletedText.color.b, 0f);
            waveCompletedText.enabled = true;

            // fade in -> hold -> fade out
            var sequence = Sequence.Create()
                .Chain(Tween.Alpha(waveCompletedText, 1f, 0.5f))
                .ChainDelay(1f)
                .Chain(Tween.Alpha(waveCompletedText, 0f, 0.5f));

            // Wait for the entire sequence to complete
            yield return sequence.ToYieldInstruction();
            waveCompletedText.enabled = false;
        }

        public void StartCountdown(float duration)
        {
            if (countdownText == null) return;

            if (currentCountdownCoroutine != null)
                StopCoroutine(currentCountdownCoroutine);

            currentCountdownCoroutine = StartCoroutine(CountdownRoutine(duration));
        }

        private IEnumerator CountdownRoutine(float duration)
        {
            countdownText.enabled = true;
            float remaining = duration;

            while (remaining > 0f)
            {
                int secondsToShow = Mathf.CeilToInt(remaining);
                countdownText.text = "Incoming: " + secondsToShow.ToString();
                yield return null;
                remaining -= Time.deltaTime;
            }

            countdownText.enabled = false;
            currentCountdownCoroutine = null;
        }

        public IEnumerator BlinkArrowSmooth(RawImage arrow, int blinkCount, float fadeIn, float hold, float fadeOut, float initialDelay)
        {
            if (arrow == null) yield break;

            if (initialDelay > 0f)
                yield return new WaitForSeconds(initialDelay);

            arrow.enabled = true;

            for (int i = 0; i < blinkCount; i++)
            {
                yield return StartCoroutine(FadeRawImage(arrow, 0f, 1f, fadeIn));

                yield return new WaitForSeconds(hold);

                yield return StartCoroutine(FadeRawImage(arrow, 1f, 0f, fadeOut));
            }

            SetRawImageAlpha(arrow, 0f);

            arrow.enabled = false;
        }

        public IEnumerator FadeRawImage(RawImage img, float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float alpha = Mathf.Lerp(from, to, t);
                SetRawImageAlpha(img, alpha);
                yield return null;
            }
            SetRawImageAlpha(img, to);
        }

        public void SetRawImageAlpha(RawImage img, float alpha)
        {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
        }
    }
}
