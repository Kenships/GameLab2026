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
        float rectSize = _cachedRectTransform.sizeDelta.x;
        
        Vector3 leftPosition = _cachedRectTransform.position - new Vector3(rectSize/2f + offset, 0, 0);
        Vector3 rightPosition = _cachedRectTransform.position + new Vector3(rectSize/2f + offset, 0, 0);
        
        leftVisual.transform.position = leftPosition;
        rightVisual.transform.position = rightPosition;
    }
}
