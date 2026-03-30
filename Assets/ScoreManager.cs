using _Project.Scripts.Util.Timer.Timers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private List<ScoreEntry> scoreEntries = new();
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private Color LatestScoreColor;
    [SerializeField] private Color DefaultColor;
    [SerializeField] private TextTyper VictoryText;

    private enum Adj
    {
        Horrid,
        Garbage,
        Okay,
        Great,
        Excellent,
        Radical,
        Wicked,
        Tubular,
        Bomb
    }

    private CountdownTimer _timer;
    private string path;

    void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "scores.json");
        GameManager.Instance.PauseTimer();

        scoreEntries = LoadFromJson();
        foreach (ScoreEntry entry in scoreEntries)
        {
            if (entry.isMostRecent) entry.isMostRecent = false;
        }

        ScoreEntry newScore = new ScoreEntry();
        newScore.score = GameManager.Instance.score;
        newScore.time = GameManager.Instance.runTime;
        newScore.isMostRecent = true;
        VictoryText.StartTyping(DetermineAdjective(newScore.score).ToString() + "!");

        if (newScore.time > 0f)
        {
            scoreEntries.Add(newScore);
        }

        scoreEntries.Sort((y, x) => x.score.CompareTo(y.score));
        while (scoreEntries.Count > 16)
        {
            scoreEntries.Remove(scoreEntries[scoreEntries.Count - 1]);
        }

        StartCoroutine(DisplayScores());
        SavetoJson();
    }

    IEnumerator DisplayScores()
    {
        int rankC = 0;
        int tempRankC = 1;

        float prevScore = 0;

        foreach (ScoreEntry entry in scoreEntries)
        {
            GameObject scoreEntry = scorePrefab;
            scoreInputData scoreEntryData = scoreEntry.GetComponent<scoreInputData>();

            scoreEntryData.Score = entry.score;
            scoreEntryData.Time = (int)(entry.time + 0.5f);
            scoreEntryData.adj = DetermineAdjective(entry.score).ToString();

            rankC++;

            if (Mathf.Approximately(prevScore, entry.score))
            {
                scoreEntryData.Rank = tempRankC;
            }
            else
            {
                tempRankC = rankC;
                scoreEntryData.Rank = rankC;
                prevScore = entry.score;
            }

            scoreEntry.gameObject.GetComponent<Image>().color = DefaultColor;

            if (entry.isMostRecent)
            {
                scoreEntry.gameObject.GetComponent<Image>().color = LatestScoreColor;
            }

            Instantiate(scoreEntry, transform);
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }

    private Adj DetermineAdjective(float score)
    {
        Adj adj = new Adj();

        switch (score)
        {
            case var n when n >= 0:
                adj = Adj.Horrid;
                break;
            case var n when n >= 100:
                adj = Adj.Garbage;
                break;
            case var n when n >= 150:
                adj = Adj.Okay;
                break;
            case var n when n >= 225:
                adj = Adj.Great;
                break;
            case var n when n >= 400:
                adj = Adj.Excellent;
                break;
            case var n when n >= 700:
                adj = Adj.Radical;
                break;
            case var n when n >= 775:
                adj = Adj.Wicked;
                break;
            case var n when n >= 875:
                adj = Adj.Tubular;
                break;
            case var n when n >= 950:
                adj = Adj.Bomb;
                break;
        }

        return adj;
    }


    private void SavetoJson()
    {
        ScoreDataWrapper wrapper = new ScoreDataWrapper();
        wrapper.scoreEntries = scoreEntries;

        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(path, json);
    }

    public void ResetJson()
    {
        ScoreDataWrapper wrapper = new ScoreDataWrapper();
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
    }

    private List<ScoreEntry> LoadFromJson()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ScoreDataWrapper wrapper = JsonUtility.FromJson<ScoreDataWrapper>(json);
            return wrapper.scoreEntries;
        }

        return new List<ScoreEntry>();
    }
}
[Serializable]
public class ScoreEntry
{
    public float score;
    public float time;
    public bool isMostRecent;
}

[Serializable]
public class ScoreDataWrapper
{
    public List<ScoreEntry> scoreEntries = new();
}
