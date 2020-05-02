using System;
using DG.Tweening;
using HighFive.Model.Person;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighFive.Model.SpriteObjSystem
{
    /// <summary>
    /// 光刀脚本，像控制刀一样控制DamageZone
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class LightSword:DamageZone
    {
        [Header("目标Sprite大小")]
        public Vector2 targetSize;
        [Header("初始状态Sprite大小")]
        public Vector2 startSize;
        [Header("延迟多久光柱开始变粗")]
        public float delayTime = 1.0f;
        [Header("变粗时间")]
        public float widenTime = 0.3f;
        public Ease easeType = Ease.InOutElastic;

        #region Debug

        public float rotate;
        [ContextMenu("SetRotate")]
        private void SetRotate()
        {
            RotateDegree = rotate;
        }
        
        public Vector2 size;
        [ContextMenu("SetSize")]
        private void SetScale()
        {
            SpriteSize = size;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(HandlePosition, 1);
        }        

        #endregion

        #region LocalScale为0的时候图像大小

        private Vector2? _originSize;
        public Vector2 OriginSize
        {
            get
            {
                if (null == _originSize)
                {
                    var transform1 = transform;
                    var beforeEur = transform1.localEulerAngles;
                    var beforeScale = transform1.localScale;
                    transform1.localEulerAngles=Vector3.zero;
                    transform1.localScale=Vector3.one;
                    _originSize = _spriteRenderer.bounds.size;
                    transform1.localScale = beforeScale;
                    transform1.localEulerAngles = beforeEur;
                }

                return _originSize.Value;
            }
        }        

        #endregion

        #region 现在图像大小

        public Vector2 SpriteSize
        {
            get
            {
                var localScale = transform.localScale;
                return new Vector2(
                    localScale.x * OriginSize.x,
                    localScale.y * OriginSize.y);
            }
            set
            {
                var deltaY = value.y - transform.localScale.y*OriginSize.y;
                transform.position += deltaY/2f * transform.up;
                transform.localScale =new Vector3(
                        value.x / OriginSize.x,
                        value.y / OriginSize.y);
            }
        }
        

        #endregion

        #region SpriteRenderer

        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer SpriteRenderer
        {
            get
            {
                if (!_spriteRenderer)
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                    Assert.IsTrue(_spriteRenderer);
                }
                return _spriteRenderer;
            }
        }

        #endregion

        /// <summary>
        /// Z轴旋转角度
        /// </summary>
        public float RotateDegree
        {
            set
            {
                var tf = SpriteRenderer.transform;
                var y = OriginSize.y * tf.localScale.y;
                var pos =  HandlePosition + (Vector3.up * y/2).RotateDegree(value);
                tf.localEulerAngles = new Vector3(0, 0, value);
                tf.position = pos;
            }
            get => SpriteRenderer.transform.localEulerAngles.z;
        }

        /// <summary>
        /// 旋转点/刀柄的位置
        /// </summary>
        public Vector3 HandlePosition
        {
            get
            {
                var tf = SpriteRenderer.transform;
                var y = OriginSize.y * tf.localScale.y;
                return SpriteRenderer.transform.position - transform.up * y/2;
            }
            set
            {
                var tf = SpriteRenderer.transform;
                var y = OriginSize.y * tf.localScale.y;
                SpriteRenderer.transform.position = value + transform.up * y/2;
            }
        }
        

        public override void Init(IHighFivePerson self)
        {
            base.Init(self);
            SpriteRenderer.enabled = true;
            SpriteSize = startSize;
            enableTrigger = false;
            MainLoop.Instance.ExecuteLater(
                () =>
                {
                    if (this == null)
                        return;
                    enableTrigger = true;
                    DOTween.To(
                            () => SpriteSize,
                            value => SpriteSize = value,
                            targetSize,
                            widenTime)
                        .SetEase(easeType)
                        .onComplete += OnFinish;

                }, delayTime);
        }
        
        private void OnFinish()
        {
            if(this)
                Destroy(gameObject);
        }
    }
}