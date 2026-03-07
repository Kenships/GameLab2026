using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectVisual : MonoBehaviour
{
    [SerializeField] private GameObject leftVisual;
    [SerializeField] private GameObject rightVisual;
    [SerializeField] private float offset = 50f;

    private RectTransform _leftVisualRect;
    private RectTransform _rightVisualRect;
    
    private RectTransform _cachedRectTransform;
    private GameObject _currentGameObject;

    private void Awake()
    {
        _leftVisualRect = leftVisual.GetComponent<RectTransform>();
        _rightVisualRect = rightVisual.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!EventSystem.current.currentSelectedGameObject)
        {
            leftVisual.SetActive(false);
            rightVisual.SetActive(false);
            return;
        }
        
        leftVisual.SetActive(true);
        rightVisual.SetActive(true);
        
        var selected = EventSystem.current.currentSelectedGameObject;

        if (selected != _currentGameObject)
        {
            _cachedRectTransform = selected.GetComponent<RectTransform>();
            _currentGameObject = selected;
        }
        float rectSize = _cachedRectTransform.rect.width;
        
        RectTransform parent = (RectTransform)leftVisual.transform.parent;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            RectTransformUtility.WorldToScreenPoint(null, _cachedRectTransform.position),
            null,
            out Vector2 selectedPosInParent);

        
        Vector3 leftPosition = selectedPosInParent + new Vector2(-(rectSize * .5f + offset), 0);
        Vector3 rightPosition = selectedPosInParent + new Vector2(rectSize * .5f + offset, 0);
        
        _leftVisualRect.anchoredPosition = leftPosition;
        _rightVisualRect.anchoredPosition = rightPosition;
    }
}
