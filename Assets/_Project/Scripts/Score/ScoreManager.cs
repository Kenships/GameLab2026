using _Project.Scripts.Util.Timer.Timers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using _Project.Scripts.Effects;
using _Project.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private List<ScoreEntry> scoreEntries = new();
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private Color latestScoreColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private TextTyper victoryText;

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
        Bomb,
        None
    }

    private CountdownTimer _timer;
    private string _path;

    void Start()
    {
        _path = Path.Combine(Application.persistentDataPath, "scores.json");
        GameManager.Instance.PauseTimer();

        scoreEntries = LoadFromJson();
        foreach (ScoreEntry entry in scoreEntries)
        {
            if (entry.isMostRecent) entry.isMostRecent = false;
        }

        var newScore = new ScoreEntry
        {
            score = GameManager.Instance.score,
            time = GameManager.Instance.runTime,
            isMostRecent = true
        };
        victoryText.StartTyping(DetermineAdjective(newScore.score) + "!");

        if (newScore.time > 0f)
        {
            scoreEntries.Add(newScore);
        }

        scoreEntries.Sort((y, x) => x.score.CompareTo(y.score));
        while (scoreEntries.Count > 16)
        {
            scoreEntries.Remove(scoreEntries[^1]);
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

            scoreEntry.gameObject.GetComponent<Image>().color = defaultColor;

            if (entry.isMostRecent)
            {
                scoreEntry.gameObject.GetComponent<Image>().color = latestScoreColor;
            }

            Instantiate(scoreEntry, transform);
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }

    private Adj DetermineAdjective(float score)
    {
        Adj adj = Adj.None;

        switch (score)
        {
            case >= 950:
                adj = Adj.Bomb;
                break;
            case >= 875:
                adj = Adj.Tubular;
                break;
            case >= 775:
                adj = Adj.Wicked;
                break;
            case >= 700:
                adj = Adj.Radical;
                break;
            case >= 400:
                adj = Adj.Excellent;
                break;
            case >= 225:
                adj = Adj.Great;
                break;
            case >= 150:
                adj = Adj.Okay;
                break;
            case >= 100:
                adj = Adj.Garbage;
                break;
            case >= 0:
                adj = Adj.Horrid;
                break;
        }

        return adj;
    }


    private void SavetoJson()
    {
        ScoreDataWrapper wrapper = new ScoreDataWrapper();
        wrapper.scoreEntries = scoreEntries;

        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(_path, json);
    }

    public void ResetJson()
    {
        ScoreDataWrapper wrapper = new ScoreDataWrapper();
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(_path, json);
    }

    private List<ScoreEntry> LoadFromJson()
    {
        if (File.Exists(_path))
        {
            string json = File.ReadAllText(_path);
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
