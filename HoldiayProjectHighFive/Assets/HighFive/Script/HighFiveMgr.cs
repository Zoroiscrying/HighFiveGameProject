using ReadyGamerOne.EditorExtension;
using HighFive.Const;
using ReadyGamerOne.Common;
using ReadyGamerOne.View;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HighFive.Script
{
	public partial class HighFiveMgr
	{
		public StringChooser startPanel = new StringChooser(typeof(PanelName));
		public StringChooser startBgm = new StringChooser(typeof(AudioName));
		public StringChooser ItemChooser=new StringChooser(typeof(ItemName));
		partial void OnSafeAwake()
		{
			PanelMgr.PushPanel(startPanel.StringValue);
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.T))
			{
				CEventCenter.BroadMessage(Message.M_AddItem, ItemName.Bear, 1);
			}
		}

		protected override void OnAnySceneUnload(Scene scene)
		{
			base.OnAnySceneUnload(scene);		
			PanelMgr.Clear();
			MainLoop.Instance.Clear();
			CEventCenter.Clear();
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
