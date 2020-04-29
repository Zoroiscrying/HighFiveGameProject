using HighFive.Const;
using UnityEngine;
using HighFive.Global;
using HighFive.Model.Person;

namespace Game.Scripts{
	/// <summary>
	/// 背景图片移动控制器
	/// </summary>
	public class BackGroundController : MonoBehaviour
	{
		private IHighFiveCharacter player;
		public float X_Speed;
		public float Y_Speed;
		private Vector3 beforePos;

		void Start()
		{
			this.player = GlobalVar.G_Player;
			this.beforePos = this.player.position;
		}
		// Update is called once per frame
		void Update ()
		{
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


