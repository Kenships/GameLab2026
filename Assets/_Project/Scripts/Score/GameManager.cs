using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float BonusScore { get; set; }
    public float Score { get; set; }
    
    public float FinalScore => Score + BonusScore;
    
    public float runTime = 0f;

    private bool timerOn = false;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Destroy this duplicate
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(timerOn)
        {
            runTime += Time.deltaTime;
        }

    }

    public void StartTimer()
    {
        timerOn = true;
    }

    public void PauseTimer()
    {
        timerOn = false;
    }

    public void RestartTimer()
    {
        runTime = 0f;
    }


}
