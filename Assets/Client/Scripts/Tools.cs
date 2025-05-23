using DG.Tweening;
using TMPro;
using UnityEngine;

public static class DOTweenTMPExtensions
{
    public static Tweener DOText(this TMP_Text text, string endValue, float duration, bool richTextEnabled = true)
    {
        return DOTween.To(
            () => text.text,
            x => text.text = x,
            endValue,
            duration)
            .SetOptions(richTextEnabled);
    }
}