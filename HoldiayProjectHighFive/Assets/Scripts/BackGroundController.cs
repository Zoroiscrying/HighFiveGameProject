using System.Collections;
using UnityEngine;
using HighFive.Global;
using UnityEngine.Assertions;

namespace HighFive.Script
{
	/// <summary>
	/// 背景图片移动控制器
	/// </summary>
	public class BackGroundController : MonoBehaviour
	{
		private static Vector3 DefaultPos=new Vector3(0,-99);
		
		public float X_Speed;
		public float Y_Speed;

		[HideInInspector]
		public bool controlByOthers = false;

		#region SpriteRendererSize

		private Vector3? _size;

		private Vector3 SpriteRendererSize
		{
			get
			{
				if (null == _size)
					_size = GetComponent<SpriteRenderer>().bounds.size;
				Assert.IsTrue(null!=_size);
				return _size.Value;
			}
		}		

		#endregion
		
		#region MidTrans

		private Transform _midTrans;

		private Transform MidTrans
		{
			get
			{
				if (!_midTrans)
					_midTrans = transform;
				return _midTrans;
			}
			set { _midTrans = value; }
		}
		

		#endregion

		#region LeftTrans

		private Transform _leftTrans;

		private Transform LeftTrans
		{
			get
			{
				if (controlByOthers)
					return null;
				if (!_leftTrans)
					_leftTrans = GetBgTransform(-1);
				return _leftTrans;
			}
			set { _leftTrans = value; }
		}

		#endregion

		#region RightTrans

		private Transform _rightTrans;

		private Transform RightTrans
		{
			get
			{
				if (controlByOthers)
					return null;
				if (!_rightTrans)
					_rightTrans = GetBgTransform(1);
				return _rightTrans;
			}
			set { _rightTrans = value; }
		}

		#endregion

		private Transform GetBgTransform(int side)
		{
			side = side > 0 ? 1 : -1;
			var instance = Instantiate(gameObject, transform.parent);
			instance.GetComponent<BackGroundController>().controlByOthers = true;
			instance.transform.position = MidTrans.position + side * SpriteRendererSize.x *Vector3.right;
			return instance.transform;
		}
		
		
		
		private Vector3 beforePlayerPos;

		private bool inited;
		private void Start()
		{
			if (controlByOthers)
				return;
			StartCoroutine(InitSettings());
		}

		private IEnumerator InitSettings()
		{
			while (null == GlobalVar.G_Player)
				yield return null;
			this.beforePlayerPos = GlobalVar.G_Player.position;
			inited = true;
		}
		
		
		// Update is called once per frame
		void Update ()
		{
			if (controlByOthers)
				return;
			if (!inited)
				return;
			var nowPos = GlobalVar.G_Player.position;
			if (nowPos == DefaultPos)
				return;


			#region 背景跟随

			var temp = (nowPos - this.beforePlayerPos);
			temp = new Vector3(temp.x * this.X_Speed, temp.y * this.Y_Speed, 0);
			MidTrans.Translate(temp*0.01f,Space.World);
			RightTrans.Translate(temp*0.01f,Space.World);
			LeftTrans.Translate(temp*0.01f,Space.World);						
			
			
			#endregion

			#region 轮替背景

			//如果玩家在右侧
			var halfCameraX = Camera.main.orthographicSize * Camera.main.aspect;
			if (SpriteRendererSize.x / 2 - (RightTrans.position.x - nowPos.x) > halfCameraX)
			{
				var mid = MidTrans;
				MidTrans = RightTrans;
				RightTrans = LeftTrans;
				LeftTrans = mid;
				RightTrans.position = MidTrans.position + SpriteRendererSize.x * Vector3.right;
			}
			//如果玩家在左侧
			if (SpriteRendererSize.x / 2 - (nowPos.x - LeftTrans.position.x) > halfCameraX)
			{
				var mid = MidTrans;
				MidTrans = LeftTrans;
				LeftTrans = RightTrans;
				RightTrans = mid;
				LeftTrans.position = MidTrans.position - SpriteRendererSize.x * Vector3.right;
			}

			#endregion

			this.beforePlayerPos = nowPos;
		}
	}
}


