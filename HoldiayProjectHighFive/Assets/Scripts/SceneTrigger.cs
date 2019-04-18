using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Control;
using Game.Control.Person;
using Game.Model.SceneSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{

	public string newSceneName;
	public Vector3 newPosition;
    private void OnDrawGizmos()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col!=null)
        {
            var offect = col.offset;
            offect.x *= transform.localScale.x;
            offect.y *= transform.localScale.y;
            var size = col.size;
            size.x *= transform.localScale.x;
            size.y *= transform.localScale.y;
            var center = transform.position + new Vector3(offect.x,offect.y,0);
            var lt = new Vector3(center.x - size.x / 2, center.y + size.y / 2, center.z);
            var lb = new Vector3(center.x - size.x / 2, center.y - size.y / 2, center.z);
            var rt = new Vector3(center.x + size.x / 2, center.y + size.y / 2, center.z);
            var rb = new Vector3(center.x + size.x / 2, center.y - size.y / 2, center.z);
            Debug.DrawLine(lt, rt);
            Debug.DrawLine(lt, lb);
            Debug.DrawLine(rb, rt);
            Debug.DrawLine(rb, lb);
        }
        //Debug.DrawLine(transform.position + Vector3.left * this.signalSize, transform.position + Vector3.right * this.signalSize);
        //Debug.DrawLine(transform.position + Vector3.up * this.signalSize, transform.position + Vector3.down * this.signalSize);
    }

    void OnTriggerExit2D(Collider2D col)
	{
		if (null == AbstractPerson.GetInstance<Player>(col.gameObject))
			return;
		Game.Const.DefaultData.PlayerPos = this.newPosition;
        //SceneMgr.Instance.LoadScene(this.newSceneName);
        SceneManager.LoadScene(this.newSceneName);
	}
	
	
}
