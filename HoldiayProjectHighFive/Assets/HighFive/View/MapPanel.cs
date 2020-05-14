using HighFive.Global;
using HighFive.Script;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HighFive.View
{
	public partial class MapPanel
	{

		private RectTransform map;
		partial void OnLoad()
		{
			//do any thing you want
			map = GetComponent<RectTransform>("MapBg/Map");
			map.GetComponent<RawImage>().texture =
				ResourceMgr.GetAsset<Texture2D>(
					$"Map_{SceneManager.GetActiveScene().name}");
		}


		public override void Enable()
		{
			base.Enable();
			var playerPos = GlobalVar.G_Player.position;
			var min = SceneUtil.Instance.mapMin;
			var max = SceneUtil.Instance.mapMax;
			var xS = (playerPos.x - min.x) / (max.x - min.x)-0.5f;
			var yS = (playerPos.y - min.y) / (max.y - min.y)-0.5f;
			var rectSize = map.rect.size;
			var offset = new Vector3(rectSize.x * xS, rectSize.y * yS);
			map.transform.localPosition = -offset;
		}
	}
}
