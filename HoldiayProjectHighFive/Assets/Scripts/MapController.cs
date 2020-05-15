using UnityEngine;
using UnityEngine.UI;

namespace HighFive.Script
{
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(RectTransform))]
    public class MapController : MonoBehaviour
    {
        public float scale = 1.0f;
        
        
        private RawImage _rawImage;
        private RectTransform _rect;

        private RawImage RawImage
        {
            get
            {
                if (!_rawImage)
                    _rawImage = GetComponent<RawImage>();
                return _rawImage;
            }
        }

        private RectTransform Rect
        {
            get
            {
                if (!_rect)
                    _rect = GetComponent<RectTransform>();
                return _rect;
            }
        }

        [ContextMenu("SetSize")]
        public void SetSize()
        {
            var mainTexture = RawImage.mainTexture;
            var nativeSize = new Vector2(
                mainTexture.width,
                mainTexture.height);
            Rect.sizeDelta = nativeSize * scale;
        }
        
    }
}