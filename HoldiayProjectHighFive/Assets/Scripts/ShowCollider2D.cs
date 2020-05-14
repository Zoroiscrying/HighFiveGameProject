using UnityEngine;

namespace HighFive.Script
{
	[RequireComponent(typeof(BoxCollider2D))]
    public class ShowCollider2D : MonoBehaviour
    {
    
    	private BoxCollider2D box;
        public Color color = Color.red;
    	private void OnDrawGizmos()
    	{
    		if (this.box == null)
    			this.box = GetComponent<BoxCollider2D>();

            Gizmos.color = color;
            var transform1 = this.transform;
            var scale = transform1.localScale;
    		var offset = scale * box.offset;
    		var size = scale * box.size;
            var position = transform1.position;
            var center = new Vector2(position.x + offset.x, position.y + offset.y);
    		var lt = center + new Vector2(-size.x / 2, size.y / 2);
    		var lb = center + new Vector2(-size.x / 2, -size.y / 2);
    		var rt = center + new Vector2(size.x / 2, size.y / 2);
    		var rb = center + new Vector2(size.x / 2, -size.y / 2);
            Gizmos.DrawLine(lt, rt);
            Gizmos.DrawLine(lt, lb);
            Gizmos.DrawLine(rt, rb);
            Gizmos.DrawLine(lb,rb);
    	}
    }
}


