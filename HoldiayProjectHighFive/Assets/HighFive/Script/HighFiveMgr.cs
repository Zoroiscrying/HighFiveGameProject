using HighFive.Const;
using HighFive.Control.EffectSystem.Effects;
using ReadyGamerOne.Attributes;
using ReadyGamerOne.Common;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.Rougelike.Mover;
using ReadyGamerOne.View;
using ReadyGamerOne.Script;
using UnityEngine;

namespace HighFive.Script
{
	public partial class HighFiveMgr
	{
		partial void OnSafeAwake()
		{
			
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.T))
			{
				CEventCenter.BroadMessage(Message.M_AddItem, ItemName.Bear, 1);
			}
		}

		protected override void OnAnySceneUnload()
		{
			base.OnAnySceneUnload();
			PanelMgr.Clear();
			MainLoop.Instance.Clear();
			EffectUnitInfo.Clear();
		}


		protected override void RegisterSceneEvent()
		{
			AbstractSceneInfo.RegisterSceneInfo<DefaultSceneInfo>(SceneName.jbScene);
			AbstractSceneInfo.RegisterSceneInfo<DefaultSceneInfo>(SceneName.testScene);
			AbstractSceneInfo.RegisterSceneInfo<DefaultSceneInfo>(SceneName.welcomeScene);
			
			
			AbstractSceneInfo.onAnySceneLoad += this.OnAnySceneLoad;
			AbstractSceneInfo.onAnySceneUnLoaded += this.OnAnySceneUnload;
			
			SceneMgr.LoadActiveScene();
		}
	}
}
