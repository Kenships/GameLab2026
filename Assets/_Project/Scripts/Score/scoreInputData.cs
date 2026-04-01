using TMPro;
using UnityEngine;

public class scoreInputData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI adjective;

    public int Rank;
    public float Score;
    public float Time;
    public string adj;


    private void Start()
    {
        rank.text = "#" + Rank.ToString();
        score.text = Score.ToString();
        time.text = Time.ToString() + " seconds";
        adjective.text = adj.ToString() + "!";
    }
}
