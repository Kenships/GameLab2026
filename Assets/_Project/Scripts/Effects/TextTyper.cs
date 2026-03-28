using TMPro;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    [SerializeField] private string fullText;

    public float delay = 0.05f;

    void Start()
    {
        dialogueText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(ShowTextRoutine());
    }

    // Coroutine to reveal text character by character
    IEnumerator ShowTextRoutine()
    {
        // Clear the text box initially
        dialogueText.text = "";

        // Use maxVisibleCharacters for better layout preservation with TextMeshPro
        dialogueText.maxVisibleCharacters = 0;

        // Set the full text first, then reveal characters
        dialogueText.text = fullText;

        // Loop through each character to make it visible gradually
        for (int i = 0; i < fullText.Length; i++)
        {
            // Increment the number of visible characters
            dialogueText.maxVisibleCharacters = i + 1;

            // Wait for the specified delay before the next character
            yield return new WaitForSeconds(delay);
        }
    }

    // A public method to start the effect from other scripts
    public void StartTyping(string textToType, float typingSpeed)
    {
        fullText = textToType;
        delay = typingSpeed;
        StartCoroutine(ShowTextRoutine());
    }

}
