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
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneTrigger : MonoBehaviour
    {
        public enum BoundryShowType
        {
            NeverShow,
            ShowOnSelect,
            ShowAlways
        }
    
        public BoundryShowType boundryShowType=BoundryShowType.ShowOnSelect;
    
        public StringChooser newSceneName = new StringChooser(typeof(SceneName));
        public StringChooser newUIName = new StringChooser(typeof(PanelName));
        public Vector3 newPosition;
    
        void OnTriggerExit2D(Collider2D col)
        {
            if ( !(col.gameObject.GetPersonInfo() is IHighFiveCharacter))
                return;
            DefaultData.PlayerPos = this.newPosition;
    
            SceneMgr.LoadScene(newSceneName.StringValue, newUIName.StringValue);
        }
    
        private BoxCollider2D _collider2D;
    
        private void OnDrawGizmos()
        {
            if(boundryShowType==BoundryShowType.ShowAlways)
                DrawBoundry();
        }
    
        private void OnDrawGizmosSelected()
        {
            if(boundryShowType==BoundryShowType.ShowOnSelect)
                DrawBoundry();
        }
    
        private void DrawBoundry()
        {
            if (_collider2D == null)
                _collider2D = GetComponent<BoxCollider2D>();
            var offset = _collider2D.offset;
            var size = _collider2D.size;
            var scale = transform.localScale;
            var startPos = transform.position;
    
            var c = startPos + new Vector3(scale.x * offset.x, scale.y * offset.y);
    
            var height = size.y * scale.y;
            var width = size.x * scale.x;
    
            var lt = c + new Vector3(-width / 2, height / 2);
            Gizmos.DrawLine(lt, lt + new Vector3(width, 0, 0));
            Gizmos.DrawLine(lt + new Vector3(0, -height, 0), lt + new Vector3(width, -height, 0));
            Gizmos.DrawLine(lt, lt + new Vector3(0, -height, 0));
            Gizmos.DrawLine(lt + new Vector3(width, 0, 0), lt + new Vector3(width, -height, 0));
        }
    }

}
