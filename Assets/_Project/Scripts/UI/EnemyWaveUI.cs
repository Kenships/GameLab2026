using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Enemies;
using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core.InputManagement;
using Obvious.Soap;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class EnemyWaveUI : MonoBehaviour
    {
        [SerializeField] private FloatVariable hapticsIntensity;
        [SerializeField] private NESActionReader[] players;
        [SerializeField] private ScriptableEventNoParam perfectWaveEvent;
        [SerializeField] private ScriptableEventWaveData waveData;
        [SerializeField] private TextMeshProUGUI waveTitle;
        [SerializeField] private TextMeshProUGUI waveCompletedText;
        [SerializeField] private TextMeshProUGUI perfectWaveText;
        [SerializeField] private TextMeshProUGUI countdownText;
        
        [SerializeField] private Slider healthSlider;
        
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI scoreTextLarge;

        private List<Gamepad> _gamepads = new ();
        
        private Coroutine currentCountdownCoroutine;

        private int _internalScore;

        private bool _perfectWave;

        private void Start()
        {
            perfectWaveEvent.OnRaised += PerfectWaveEventOnRaised;
            waveData.OnRaised += WaveDataOnRaised;
            scoreText.enabled = false;

            foreach (var player in players)
            {
                if (player.TryGetGamePad(out Gamepad gamepad))
                {
                    _gamepads.Add(gamepad);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var gamepad in _gamepads)
            {
                gamepad.SetMotorSpeeds(0,0);
            }
        }

        private void PerfectWaveEventOnRaised()
        {
            Debug.Log("Perfect Wave!");
            _perfectWave = true;
        }

        private void WaveDataOnRaised(EnemyWaveSpawner.Wave wave)
        {
            _perfectWave = false;
            waveTitle.text = wave.waveName;
        }
        
        public IEnumerator FinalWaveCompleted()
        {
            if (waveCompletedText == null) yield break;

            waveCompletedText.text = "Boss Defeated";
            
            // Start fully transparent
            waveCompletedText.color = new Color(waveCompletedText.color.r, waveCompletedText.color.g, waveCompletedText.color.b, 0f);
            waveCompletedText.enabled = true;

            if (_perfectWave)
            {
                waveCompletedText.text += "\n<size=50>Perfect Wave!</size>";
                _perfectWave = false;
            }

            // fade in -> hold -> fade out
            var sequence = Sequence.Create()
                .Chain(Tween.Alpha(waveCompletedText, 1f, 0.5f))
                .ChainDelay(2f)
                .Chain(Tween.Alpha(waveCompletedText, 0f, 0.5f));

            // Wait for the entire sequence to complete
            yield return ScoreDisplay();
            yield return HealthBonus();
            yield return sequence.ToYieldInstruction();
            
            waveCompletedText.enabled = false;

            waveCompletedText.text = "Wave Completed";
        }

        public IEnumerator ShowWaveCompleted()
        {
            if (waveCompletedText == null) yield break;

            // Start fully transparent
            waveCompletedText.color = new Color(waveCompletedText.color.r, waveCompletedText.color.g, waveCompletedText.color.b, 0f);
            waveCompletedText.enabled = true;

            if (_perfectWave)
            {
                waveCompletedText.text += "\n<size=50>Perfect Wave!</size>";
                _perfectWave = false;
            }

            // fade in -> hold -> fade out
            var sequence = Sequence.Create()
                .Chain(Tween.Alpha(waveCompletedText, 1f, 0.5f))
                .ChainDelay(1f)
                .Chain(Tween.Alpha(waveCompletedText, 0f, 0.5f));

            // Wait for the entire sequence to complete
            yield return ScoreDisplay();
            yield return sequence.ToYieldInstruction();
            
            waveCompletedText.enabled = false;

            waveCompletedText.text = "Wave Completed";
        }

        private IEnumerator HealthBonus()
        {
            scoreText.enabled = false;
            scoreTextLarge.enabled = true;
            
            int finalScore = Mathf.CeilToInt(GameManager.Instance.FinalScore);
            int currentHealth = Mathf.CeilToInt(GameManager.Instance.Score);
            
            float timer = 0;

            while (timer < 1f)
            {
                timer += Time.deltaTime;
                
                float progress = timer;
                
                foreach (var gamepad in _gamepads)
                {
                    gamepad.SetMotorSpeeds(0.5f * progress * hapticsIntensity.Value, 1f * progress * hapticsIntensity.Value);
                }
                
                healthSlider.value = (1f - progress) * currentHealth / 1000f;

                if (healthSlider.value <= 0.001f)
                {
                    healthSlider.value = 0f;
                    healthSlider.gameObject.SetActive(false);
                }
                
                scoreTextLarge.text = "Score: " + (int) (_internalScore + (finalScore - _internalScore) * progress);
                yield return null;
            }
            
            
            
            _internalScore = finalScore;
            scoreTextLarge.text = "score: " + _internalScore;
            scoreText.text = "Score: " + _internalScore;

            foreach (var gamepad in _gamepads)
            {
                gamepad.SetMotorSpeeds(0,0);
            }
            
            yield return new WaitForSeconds(0.5f);

            scoreText.enabled = true;
            scoreTextLarge.enabled = false;
        }
        
        private IEnumerator ScoreDisplay()
        {
            scoreText.enabled = false;
            scoreTextLarge.enabled = true;
            
            scoreTextLarge.text = "Score: " + _internalScore;

            int finalScore = Mathf.CeilToInt(GameManager.Instance.BonusScore);

            float timer = 0;

            while (timer < 1.5f)
            {
                timer += Time.deltaTime;
                
                float progress = timer / 1.5f;
                
                foreach (var gamepad in _gamepads)
                {
                    gamepad.SetMotorSpeeds(0.5f * progress * hapticsIntensity.Value, 0.5f * progress * hapticsIntensity.Value);
                }
                
                scoreTextLarge.text = "Score: " + (int) (_internalScore + (finalScore - _internalScore) * progress);
                yield return null;
            }
            _internalScore = finalScore;
            scoreTextLarge.text = "Score: " + _internalScore;
            scoreText.text = "Score: " + _internalScore;

            foreach (var gamepad in _gamepads)
            {
                gamepad.SetMotorSpeeds(0,0);
            }
            
            yield return new WaitForSeconds(0.5f);
            
            scoreText.enabled = true;
            scoreTextLarge.enabled = false;
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
                countdownText.text = secondsToShow.ToString();
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
