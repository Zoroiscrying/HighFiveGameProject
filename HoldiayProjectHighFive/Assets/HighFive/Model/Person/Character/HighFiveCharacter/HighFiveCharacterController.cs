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
		public SkillInfoAsset skillAsset;
		[Range(0, 1)] [SerializeField]private float beginComboTest;
		[SerializeField]private float canMoveTime;
		[SerializeField]private bool ignoreInput;
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
		public KeyCode comboKey=KeyCode.J;
		public KeyCode superKey = KeyCode.Z;
		public KeyCode bagKey = KeyCode.Tab;
		public KeyCode mapKey = KeyCode.E;
        
		public int Maxdrag;   //最大药引上限
		public int money = 0;
		public float airXMove;
		public int MaxSpiritNum = 1;
		
		public List<CommonSkillInfo> commonSkillInfos=new List<CommonSkillInfo>();
		public List<ComboSkillInfo> comboSkillInfos=new List<ComboSkillInfo>();

		public bool mapped = false;
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
