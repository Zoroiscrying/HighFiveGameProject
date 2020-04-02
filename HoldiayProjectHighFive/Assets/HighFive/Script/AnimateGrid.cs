using DG.Tweening;
using ReadyGamerOne.Script;
using UnityEngine;

namespace HighFive.Script
{
    public class AnimateGrid : FreeGrid
    {
        public Ease easeType=Ease.InOutExpo;
        public float easeTime = 0.5f;

        protected override void SetPosition(RectTransform childRect, Vector2 targetAnchoredPosition)
        {
            DOTween
                .To(
                    () => childRect.anchoredPosition,
                    value => childRect.anchoredPosition = value,
                    targetAnchoredPosition,
                    easeTime)
                .SetEase(easeType);
        }
    }
}