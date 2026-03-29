using TMPro;
using UnityEngine;
using System.Collections;

public class TextTyper : MonoBehaviour
{
    private TextMeshProUGUI dialogueText;
    private string fullText;

    public float delay = 0.03f;

    void Start()
    {
        dialogueText = GetComponent<TextMeshProUGUI>();
    }


    IEnumerator ShowTextRoutine()
    {
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.text = fullText;


        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(delay);
        }
    }

    public void StartTyping(string textToType)
    {
        fullText = textToType;
        dialogueText.text = "";
        StopAllCoroutines();
        StartCoroutine(ShowTextRoutine());
    }
}
