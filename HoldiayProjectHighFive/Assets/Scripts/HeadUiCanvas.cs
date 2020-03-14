using UnityEngine;

namespace Game.Scripts
{
    public class HeadUiCanvas : UnityEngine.MonoBehaviour
    {
        public Canvas canvas;
        private void Start()
        {
            if(canvas && canvas.renderMode==RenderMode.WorldSpace)
                canvas.worldCamera=Camera.main;
        }
    }
}