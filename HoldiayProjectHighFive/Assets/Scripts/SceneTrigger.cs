using HighFive.Const;
using HighFive.Model.Person;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.Rougelike.Person;
using UnityEngine;

namespace Game.Scripts
{
    
    /// <summary>
    /// 场景切换触发器
    /// </summary>
    [RequireComponent(typeof(ShowCollider2D))]
    public class SceneTrigger : MonoBehaviour
    {
    
        public StringChooser newSceneName = new StringChooser(typeof(SceneName));
        public Vector3 newPosition;
    
        void OnTriggerExit2D(Collider2D col)
        {
            if ( !(col.gameObject.GetPersonInfo() is IHighFiveCharacter))
                return;
            
            DefaultData.PlayerPos = this.newPosition;
    
            SceneMgr.LoadScene(newSceneName.StringValue);
        }

    }

}
