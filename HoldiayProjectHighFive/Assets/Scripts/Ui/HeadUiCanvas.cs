using UnityEngine;

namespace HighFive.Script
{
    /// <summary>
    /// 怪物头顶Ui的Canvas必备脚本
    /// </summary>
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