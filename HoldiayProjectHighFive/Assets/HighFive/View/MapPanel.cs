using HighFive.Global;
using HighFive.Script;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HighFive.View
{
	public partial class MapPanel
	{
		private RectTransform map;
		private RawImage mapRawImage;
		private MapController mapController;
		
		partial void OnLoad()
		{
			//do any thing you want
			map = GetComponent<RectTransform>("MapBg/Map");
			mapController = map.GetComponent<MapController>();
			mapRawImage=map.GetComponent<RawImage>();
			mapRawImage.texture =
				ResourceMgr.GetAsset<Texture2D>(
					$"Map_{SceneManager.GetActiveScene().name}");
			mapController.SetSize();
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
