using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TweenExtensions {

    public static IEnumerator MoveTo(this MonoBehaviour v, EaseType easeType, float duration, Vector3 to) {
        Vector3 from = v.transform.localPosition;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.transform.localPosition = Vector3.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator ScaleTo(this MonoBehaviour v, EaseType easeType, float duration, Vector3 to) {
        Vector3 from = v.transform.localScale;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.transform.localScale = Vector3.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

#if BOOT_NGUI_SUPPORT 
    public static IEnumerator AlphaTo(this UIPanel v, EaseType easeType, float duration, float to) {
        float from = v.alpha;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.alpha = Mathf.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator ColorTo(this UIWidget v, EaseType easeType, float duration, Color to) {
        Color from = v.color;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.color = Color.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator AlphaTo(this UIWidget v, EaseType easeType, float duration, float to) {
        Color from = v.color;
        Color toClolor = v.color;
        toClolor.a = to;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.color = Color.Lerp(from, toClolor, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }
#endif
}