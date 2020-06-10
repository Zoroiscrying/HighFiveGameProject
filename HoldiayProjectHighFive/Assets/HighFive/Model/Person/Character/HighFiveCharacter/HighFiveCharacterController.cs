using System;
using HighFive.Control.SkillSystem;
using System.Collections.Generic;
using HighFive.Const;
using HighFive.Global;
using ReadyGamerOne.View;
using UnityEngine;

namespace HighFive.Model.Person
{
	[Serializable]
	public class CommonSkillInfo
	{
		public KeyCode key;
		public SkillInfoAsset skillAsset;
	}
    
	[Serializable]
	public class ComboSkillInfo
	{
		[Tooltip("技能资源")]
		public SkillInfoAsset skillAsset;
		[Tooltip("技能进行什么程度开始连击检测")]
		[Range(0, 1)] [SerializeField]private float beginComboTest;
		[Tooltip("技能释放多少秒后角色可以解除移动禁锢")]
		[SerializeField]private float canMoveTime;
		[Tooltip("技能释放期间是否无视用户输入")]
		[SerializeField]private bool ignoreInput;
		[Tooltip("连击检测容错时间")]
		[SerializeField]private float faultToleranceTime;

		public string SkillName => skillAsset.skillName.StringValue;
		public float StartTime => skillAsset.startTime;
		public float LastTime => skillAsset.LastTime/GlobalVar.G_Player.AttackSpeed;
		public float CanMoveTime => canMoveTime / GlobalVar.G_Player.AttackSpeed;
		public float FaultToleranceTime => faultToleranceTime / GlobalVar.G_Player.AttackSpeed;
		public float BeginComboTest => beginComboTest;
		public bool IgnoreInput => ignoreInput;
		

		public void RunSkill(IHighFiveCharacter self, bool ignoreInput = false, float startTimer = 0f) =>
			skillAsset.RunSkill(self, ignoreInput, startTimer);
	}
	/// <summary>
	/// Character角色控制类，在角色类加UsePersonController属性的话会自动添加上去，使用InitController初始化
	/// </summary>
	public abstract class HighFiveCharacterController : HighFivePersonController
	{
		[Header("攻击键")]
		public KeyCode comboKey=KeyCode.J;
		[Header("强化键")]
		public KeyCode superKey = KeyCode.Z;
		[Header("背包键")]
		public KeyCode bagKey = KeyCode.Tab;
		[Header("地图键")]
		public KeyCode mapKey = KeyCode.E;
        
		[Header("当前玩家拥有的金钱")]
		public int money = 0;
		[Header("释放技能造成的水平位移")]
		public float airXMove;
		[Header("玩家可以拥有的最大灵器数量")]
		public int MaxSpiritNum = 1;
		
		/// <summary>
		/// 常规技能列表
		/// </summary>
		public List<CommonSkillInfo> commonSkillInfos=new List<CommonSkillInfo>();
		/// <summary>
		/// 连击技能列表
		/// </summary>
		public List<ComboSkillInfo> comboSkillInfos=new List<ComboSkillInfo>();

		/// <summary>
		/// 是否显示地图
		/// </summary>
		private bool mapped = false;
		protected override void Update()
		{
			base.Update();
			if (Input.GetKeyDown(mapKey))
			{
				if (mapped)
					PanelMgr.PopPanel();
				else
					PanelMgr.PushPanel(PanelName.MapPanel);
				mapped = !mapped;
			}
		}
	}
}
