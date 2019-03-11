using System;
using System.Collections;
using System.Collections.Generic;
using Game.Control;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Modal
{
	public class LinkInfo
	{
		public int count;
		public int itemId;
	}
	
	
	/// <summary>
	/// 消耗品道具
	/// </summary>
	public abstract class CousumeItem : AbstractUIItem
	{
		public abstract void Consume(IBattleEffect bf);
	}

	

	/// <summary>
	/// 货币道具
	/// </summary>
	public abstract class MoneyItem : AbstractUIItem
	{
		public abstract bool Use<T>(int num);
	}


	/// <summary>
	/// 装备道具
	/// </summary>
	public abstract class Equipment : AbstractUIItem
	{
		
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
//	public enum ItemQuality
//	{
//		Common,
//		Rare,
//		Epic,
//		Legendary
//	}
//
//	[Serializable]
//	public class Item
//	{
//		public int ID { get; set; }
//		public string Name { set; get; }
//		public ItemQuality Quality { set; get; }
//		public string Descripts { set; get; }
//		public int Capacity { set; get; }
//		public string Sprite { set; get; }
//
//		public Item()
//		{
//			ID = -1;
//		}
//
//		public Item(int id, string name, ItemQuality quality, string des, int cap, string sprite)
//		{
//			ID = id;
//			Name = name;
//			Quality = quality;
//			Descripts = des;
//			Capacity = cap;
//			Sprite = sprite;
//		}
//
//		public virtual string GetToolTips()
//		{
//			string strItemQua = null;
//			string color = null;
//			switch (Quality)
//			{
//				case ItemQuality.Epic:
//					strItemQua = "史诗级";
//					color = "magenta";
//					break;
//				case ItemQuality.Rare:
//					strItemQua = "稀有";
//					color = "navy";
//					break;
//				case ItemQuality.Common:
//					strItemQua = "普通";
//					color = "white";
//					break;
//				case ItemQuality.Legendary:
//					strItemQua = "传说";
//					color = "orange";
//					break;
//			}
//			return  string.Format("<color={0}>{1}</color>\n<color=yellow><size=10>介绍：{2}</size></color>\n<color=red><size=12>容量：{3}</size></color>\n<color=blue><size=12>物品质量：{4}</size></color>", color, Name, Descripts, Capacity, strItemQua);
//		}
//	}
//	[Serializable]
//	public class Consumable : Item
//	{
//		public int Hp { set; get; }
//		public int Mp { set; get; }
//
//		public Consumable()
//		{
//		}
//
//		public Consumable(int id, string name, ItemQuality quality, string des, int cap, string sprite, int hp, int mp)
//			: base(id, name, quality, des, cap, sprite)
//		{
//			Hp = hp;
//			Mp = mp;
//		}
//
//		public override string GetToolTips()
//		{
//			return string.Format(
//				"{0}\n<color=red>类型：消耗品</color>\n<color=red>加血：{1}HP</color>\n<color=yellow>加魔法：{2}MP</color>",
//				base.GetToolTips(), Hp, Mp);
//		}
//		public override string ToString()
//		{
//			string str = "";
//			str += ID;
//			str += Name;
//			str += "消耗品";
//			str += Quality;
//			str += Descripts;
//			str += Capacity;
//			str += Sprite;
//			str += Hp;
//			str += Mp;
//			return str;
//		}
//	}
//	public enum EquipmentType
//	{
//		Head,
//		Ring,
//		Leg,
//		Chest,
//		Boots,
//		Belt
//	}
//	[Serializable]
//	public class _Equipment:Item
//	{
//		public int Strength { set; get; }
//		public int Intellect { set; get; }
//		public int Hp { set; get; }
//		public int Mp { set; get; }
//		public int Speed { set; get; }
//		public EquipmentType Type { get; set; }
//
//		public _Equipment()
//		{
//			
//		}
//
//		public _Equipment(int id, string name, ItemQuality quality, string des, int cap, string sprite, int strength,
//			int intellect, int hp, int mp, int speed,EquipmentType type) : base(id, name, quality, des, cap, sprite)
//		{
//			Strength = strength;
//			Intellect = intellect;
//			Hp = hp;
//			Mp = mp;
//			Speed = speed;
//			Type = type;
//		}
//		
//		public override string GetToolTips()
//		{
//			string strEquipType = "";
//			switch (Type)
//			{
//				case EquipmentType.Head:
//					strEquipType = "头部的";
//					break;
//				case EquipmentType.Ring:
//					strEquipType = "戒指";
//					break;
//				case EquipmentType.Leg:
//					strEquipType = "腿部的";
//					break;
//				case EquipmentType.Chest:
//					strEquipType = "胸部的";
//					break;
//				case EquipmentType.Boots:
//					strEquipType = "靴子";
//					break;
//				case EquipmentType.Belt:
//					strEquipType = "腰带";
//					break;
//			}
//
//			string text = base.GetToolTips();//调用父类的GetToolTipText()方法
//			string newText = string.Format("{0}\n<color=green>力量：{1}</color>\n<color=yellow>智力：{2}</color>\n<color=white>HP：{3}</color>\n<color=blue>MP：{4}</color>\n<color=red>速度：{5}</color>\n<color=red>装备类型：{6}</color>",
//				text, Strength, Intellect, Hp, Mp,Speed, strEquipType);
//			return newText;
//		}
//	}
//	[Serializable]
//	public class Weapon : Item
//	{
//		public int Damage { set; get; }
//
//		public Weapon()
//		{
//			
//		}
//		public Weapon(int id, string name, ItemQuality quality, string des, int cap, string sprite, int damage) : base(
//			id, name, quality, des, cap, sprite)
//		{
//			Damage = damage;
//		}
//		public override string GetToolTips()
//		{
//			return string.Format(
//				"{0}\n<color=red>类型：武器</color>\n<color=red>伤害：{1}</color>",
//				base.GetToolTips(), Damage);
//		}
//	}

}
