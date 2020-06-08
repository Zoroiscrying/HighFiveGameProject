using System.Collections;
using HighFive.Const;
using UnityEngine;
using HighFive.Global;
using HighFive.Model.Person;
using UnityEngine.Assertions;

namespace HighFive.Script
{
	/// <summary>
	/// 背景图片移动控制器
	/// </summary>
	public class BackGroundController : MonoBehaviour
	{
		private IHighFiveCharacter player;
		public float X_Speed;
		public float Y_Speed;
		private Vector3 beforePos;

		private bool inited;
		private void Start()
		{
			StartCoroutine(InitSettings());
		}

		private IEnumerator InitSettings()
		{
			while (null == GlobalVar.G_Player)
				yield return null;
			this.player = GlobalVar.G_Player;
			this.beforePos = this.player.position;
			inited = true;
		}
		
		
		// Update is called once per frame
		void Update ()
		{
			if (!inited)
				return;
			var nowPos = this.player.position;
			if (nowPos == Signal.defaultPos)
				return;
			var temp = (nowPos - this.beforePos);
			temp = new Vector3(temp.x * this.X_Speed, temp.y * this.Y_Speed, 0);
			this.transform.Translate(temp*0.01f,Space.World);
			this.beforePos = nowPos;
		}
	}
}


