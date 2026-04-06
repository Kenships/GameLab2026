using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Project.Scripts.Core.SceneLoading; // 如果需要引用按钮

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    private VideoPlayer videoPlayer;
    private SceneLoader sceneLoader;
    private RawImage rawImage;


    void Start()
    {
        startButton.onClick.AddListener(PlayVideo);

        videoPlayer = GetComponent<VideoPlayer>();
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

    private void OnVideoFinished(VideoPlayer vp)
    {
        sceneLoader.LoadScene();
    }

    void OnDestroy()
    {
        if (videoPlayer != null) videoPlayer.loopPointReached -= OnVideoFinished;
    }
}