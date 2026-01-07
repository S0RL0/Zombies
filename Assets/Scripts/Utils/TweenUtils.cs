using DG.Tweening;
using UnityEngine;

public static class TweenUtils
{
    public static Tween LerpTween(GameObject obj, Vector3 targetPosition, Quaternion targetRotation, float duration, Ease ease = Ease.InOutCubic)
    {
        Transform t = obj.transform;
        t.DOKill();

        if (duration <= 0f)
        {
            t.localPosition = targetPosition;
            t.localRotation = targetRotation;
            return null;
        }

        return DOTween.Sequence()
            .Join(t.DOLocalMove(targetPosition, duration).SetEase(ease))
            .Join(t.DOLocalRotateQuaternion(targetRotation, duration).SetEase(ease));
    }
}
