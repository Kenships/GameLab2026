using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core.InputManagement;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Project.Scripts.Core.SceneLoading;
using Obvious.Soap;
using PrimeTween;
using UnityEngine.InputSystem; // �����Ҫ���ð�ť

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private FloatVariable hapticsIntensity;
    [SerializeField] private NESActionReader[] readers;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button startButton;
    private VideoPlayer videoPlayer;
    private SceneLoader sceneLoader;
    private RawImage rawImage;

    private List<Gamepad> _gamepads = new();

    void Start()
    {
        foreach (var reader in readers)
        {
            if (!reader.TryGetGamePad(out var gamepad))
            {
                continue;
            }
            _gamepads.Add(gamepad);
        }
        
        startButton.onClick.AddListener(PlayVideo);

        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.Prepare();
        videoPlayer.loopPointReached += OnVideoFinished;

        sceneLoader = GetComponent<SceneLoader>();

        rawImage = GetComponent<RawImage>();
    }

    public void PlayVideo()
    {
        startButton.gameObject.SetActive(false);

        Color c = rawImage.color;
        c.a = 1;
        rawImage.color = c;

        videoPlayer.time = 0;
        videoPlayer.Play();
    }
    
    private void Update()
    {
        if (!videoPlayer.isPlaying)
        {
            return;
        }
        
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * 2);

        if (videoPlayer.time / videoPlayer.clip.length < 0.4f)
            return;

        if (videoPlayer.time / videoPlayer.clip.length > 0.8f)
        {
            foreach (var gamepad in _gamepads)
            {
                gamepad.SetMotorSpeeds((1f - (float)(videoPlayer.time / videoPlayer.clip.length)) * hapticsIntensity.Value, (1f - (float)(videoPlayer.time / videoPlayer.clip.length)) * hapticsIntensity.Value);
            }

            return;
        }

        foreach (var gamepad in _gamepads)
        {
            gamepad.SetMotorSpeeds((float) (videoPlayer.time / videoPlayer.clip.length) * hapticsIntensity.Value, 0);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        foreach (var gamepad in _gamepads)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
        sceneLoader.LoadScene();
    }

    void OnDestroy()
    {
        if (videoPlayer != null) videoPlayer.loopPointReached -= OnVideoFinished;
        if (startButton != null) startButton.onClick.RemoveListener(PlayVideo);
        foreach (var gamepad in _gamepads)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }
}
