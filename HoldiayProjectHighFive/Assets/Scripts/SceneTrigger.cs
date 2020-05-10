using System;
using System.IO;
using HighFive.Const;
using HighFive.Global;
using HighFive.Model.Person;
using HighFive.Script;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;

#endif

namespace HighFive.Scripts
{
    
    /// <summary>
    /// 场景切换触发器
    /// </summary>
    [RequireComponent(typeof(ShowCollider2D))]
    public class SceneTrigger : MonoBehaviour
    {
        public SceneTriggerChooser targetChooser=new SceneTriggerChooser(typeof(SceneTrigger).FullName);

#if UNITY_EDITOR
        private GUIStyle style;
        private GUIStyle Style
        {
            get
            {
                if (null == style)
                {
                    style = new GUIStyle();
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor=Color.cyan;
                }

                return style;
            }
        }
#endif
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!GameSettings.Instance.enableSceneTrigger)
                return;

            if (!(other.gameObject.GetPersonInfo() is IHighFiveCharacter))
                return;

            DefaultData.PlayerPos = this.targetChooser.TargetPosition;
            GameSettings.Instance.enableSceneTrigger = false;
//            Debug.Log($"禁用场景触发器");
            SceneMgr.LoadScene(targetChooser.SceneName);

        }

        private void OnTriggerExit2D(Collider2D col)
        {
//            Debug.Log($"启用场景触发器");
            GameSettings.Instance.enableSceneTrigger = true;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Handles.Label(transform.position,$"[SceneTrigger:{name}]",Style);
#endif
        }


        [ContextMenu("ShowPath")]
        private void Test()
        {
            Debug.Log(targetChooser.ScenePath);
            Debug.Log(Path.GetFileNameWithoutExtension(targetChooser.ScenePath));
        }
    }
}
