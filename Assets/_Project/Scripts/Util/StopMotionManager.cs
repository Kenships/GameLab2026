using UnityEngine;

public class StopMotionManager : MonoBehaviour
{
    public static StopMotionManager Instance { get; private set; }

    [SerializeField] private GameObject stopMotionPrefab;
    [SerializeField] private Canvas targetCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnAnimation(Vector3 worldPos, Vector2? size = null)
    {
        GameObject go = Instantiate(stopMotionPrefab);
        go.transform.SetParent(targetCanvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        if (size.HasValue) rect.sizeDelta = size.Value;

        RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();

        Camera mainCam = Camera.main;
        Vector2 screenPos = mainCam.WorldToScreenPoint(worldPos);

        Camera uiCamera = (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                          ? null
                          : targetCanvas.worldCamera;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            uiCamera,
            out localPos
        );

        rect.anchoredPosition = localPos;
        go.GetComponent<StopMotionUI>().Play();
    }
}