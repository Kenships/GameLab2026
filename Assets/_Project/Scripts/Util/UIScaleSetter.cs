using UnityEngine;

public class UIScaleSetter : MonoBehaviour
{
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetScale(float scale)
    {
        rectTransform.localScale = new Vector3(scale, scale, scale);
    }
}
