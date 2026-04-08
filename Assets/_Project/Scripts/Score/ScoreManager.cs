using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Effects;
using _Project.Scripts.UI;
using _Project.Scripts.Util.Timer.Timers;
using Sisus.Init;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

public class ScoreManager : MonoBehaviour<AudioPooler>
{
    [SerializeField] private List<ScoreEntry> scoreEntries = new();
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private Color latestScoreColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private TextTyper victoryText;

    [SerializeField] private AudioClip scoreInputSound;
    [SerializeField] private float scoreInputSoundVolume = 0.1f;

    [Header("Player Score Display (Always Visible)")]
    [SerializeField] private TMP_Text playerScoreText;

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

    private AudioPooler _audioPooler;
    protected override void Init(AudioPooler audioPooler)
    {
        _audioPooler = audioPooler;
    }

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
            score = GameManager.Instance.FinalScore ,
            time = GameManager.Instance.runTime,
            isMostRecent = true
        };
        victoryText.StartTyping(DetermineAdjective(newScore.score) + "!");

        if (newScore.time > 0f)
        {
            scoreEntries.Add(newScore);
        }

        // UPDATED: Sort by score (descending), then by time (ascending - lower is better)
        scoreEntries.Sort((a, b) => 
        {
            int scoreCompare = b.score.CompareTo(a.score); // Higher score first
            if (scoreCompare != 0) return scoreCompare;
            return a.time.CompareTo(b.time); // Lower time first (faster is better)
        });

        // Display player's score in the TMP text
        if (newScore.time > 0f)
        {
            DisplayPlayerScoreSeparately(newScore);
        }

        while (scoreEntries.Count > 16)
        {
            scoreEntries.Remove(scoreEntries[^1]);
        }

        StartCoroutine(DisplayScores());
        SavetoJson();
    }

    private void DisplayPlayerScoreSeparately(ScoreEntry playerEntry)
    {
        if (playerScoreText == null)
        {
            Debug.LogWarning("Player Score Text (TMP) is not assigned!");
            return;
        }

        // UPDATED: Find rank by position in sorted list
        int rank = 1;
        foreach (ScoreEntry entry in scoreEntries)
        {
            if (entry.isMostRecent)
            {
                break;
            }
            rank++;
        }

        int score = (int)(playerEntry.score + 0.5f);
        int time = (int)(playerEntry.time + 0.5f);
        string adj = DetermineAdjective(playerEntry.score).ToString();

        playerScoreText.text = $"#{rank}    {score}    {time} seconds    {adj}!";
    }

    IEnumerator DisplayScores()
    {
        // UPDATED: Simple incrementing rank - no ties
        int rank = 1;

        foreach (ScoreEntry entry in scoreEntries)
        {
            GameObject scoreEntryObj = Instantiate(scorePrefab, transform);
            scoreInputData scoreEntryData = scoreEntryObj.GetComponent<scoreInputData>();

            scoreEntryData.Score = (int)(entry.score + 0.5f);
            scoreEntryData.Time = (int)(entry.time + 0.5f);
            scoreEntryData.adj = DetermineAdjective(entry.score).ToString();
            scoreEntryData.Rank = rank;

            rank++;

            scoreEntryObj.GetComponent<Image>().color = defaultColor;

            if (entry.isMostRecent)
            {
                scoreEntryObj.GetComponent<Image>().color = latestScoreColor;
                _audioPooler.New2DAudio(scoreInputSound).OnChannel(AudioType.Sfx).SetVolume(scoreInputSoundVolume + 0.35f).RandomizePitch(0.1f, 0.5f).Play();
            }
            else
            {
                _audioPooler.New2DAudio(scoreInputSound).OnChannel(AudioType.Sfx).SetVolume(scoreInputSoundVolume).RandomizePitch(0.1f, 0.5f).Play();
            }

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
