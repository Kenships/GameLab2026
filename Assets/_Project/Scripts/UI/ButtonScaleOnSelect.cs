using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaleOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Vector3 addScale = new Vector3(1.2f, 1.2f, 1.2f);
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale += addScale;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = originalScale;
    }
}