using ReadyGamerOne.EditorExtension;
using HighFive.Const;
using HighFive.Model.RankSystem;
using HighFive.Model.RankSystem.LargeRank;
using HighFive.Model.RankSystem.SmallRank;
using ReadyGamerOne.Common;
using ReadyGamerOne.Data;
using ReadyGamerOne.View;
using ReadyGamerOne.Script;

namespace HighFive.Script
{
	public partial class HighFiveMgr
	{
		public StringChooser startPanel = new StringChooser(typeof(PanelName));
		public StringChooser startBgm = new StringChooser(typeof(AudioPath));
		partial void OnSafeAwake()
		{
			RegisterAll();
			PanelMgr.PushPanel(startPanel.StringValue);
//			AudioMgr.Instance.PlayBgm(startBgm.StringValue);
			//do any thing you want
		}
		
		private void RegisterAll()
		{
			print("GameMgr RegisterAll");
			RegisterData();
//			RegisterSpiritItem();
			LoadDataFromFile();	
		}
        
		protected override void OnAnySceneUnload()
		{
			base.OnAnySceneUnload();
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
    
		/// <summary>
		/// 注册Data工厂
		/// </summary>
		void RegisterData()
		{
			TxtManager.RegisterDataFactory<L1Rank>(TxtSign.L1Rank);
			TxtManager.RegisterDataFactory<L2Rank>(TxtSign.L2Rank);
			TxtManager.RegisterDataFactory<S1Rank>(TxtSign.S1Rank);
			TxtManager.RegisterDataFactory<S2Rank>(TxtSign.S2Rank);
		}
    
		/// <summary>
		/// 从文件中加载数据
		/// </summary>
		void LoadDataFromFile()
		{
			TxtManager.LoadDataFromFile<AbstractLargeRank>(FilePath.RankFilePath,
				(largeRank) => { RankMgr.LargeRankList.Add(largeRank); });
            
		}
	}
}
