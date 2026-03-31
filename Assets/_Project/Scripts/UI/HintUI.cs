using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class HintUI : MonoBehaviour
{
    private Tween arrowBackAndForthTween;
    public void PlayArrowBackAndForth(RawImage arrow, float offset = 30f, float duration = 0.5f)
    {
        RectTransform rectTransform = arrow.rectTransform;
        Vector3 startPos = rectTransform.localPosition;
        Vector3 localUpDir = rectTransform.localRotation * Vector3.up;
        Vector3 targetPos = startPos + localUpDir * offset;

        arrowBackAndForthTween = Tween.LocalPosition(
            rectTransform,
            targetPos,
            duration,
            cycles: -1,
            cycleMode: CycleMode.Yoyo
        );
    }

    public void StopArrowBackAndForth()
    {
        if (arrowBackAndForthTween.isAlive) arrowBackAndForthTween.Stop();
    }
}
