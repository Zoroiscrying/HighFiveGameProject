using ReadyGamerOne.EditorExtension;
using HighFive.Const;
using ReadyGamerOne.Common;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.View;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.SceneManagement;

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
			CEventCenter.Clear();
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
		/// <summary>
		/// 注册灵器
		/// </summary>
//		void RegisterSpiritItem()
//		{
//			AbstractSpiritItem.RegisterSpiritItem<TestSpirit>(SpiritName.C_First);
//			AbstractSpiritItem.RegisterSpiritItem<TestSpirit>(SpiritName.C_Second);
//		}

	}
}
