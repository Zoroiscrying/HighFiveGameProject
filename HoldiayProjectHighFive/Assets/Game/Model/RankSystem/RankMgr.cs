using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Game.Common;
using Game.Const;
using Game.Data;
using Game.Model.ItemSystem;
using Game.Model.LimitSystem;
using UnityEngine;

namespace Game.Model.RankSystem
{
	public class RankMgr:SafeSingleton<RankMgr>,IXmlSerializable
	{
		#region Static
		
		public static List<AbstractLargeRank> LargeRankList = new List<AbstractLargeRank>();
	
		#endregion

		public RankMgr()
		{
			CEventCenter.AddListener<int>(Message.M_ChangeSmallLevel,this.OnChangeSmallRankAdder);
			CEventCenter.AddListener<int>(Message.M_AchieveLargeLevel,LargeLevelUp);
			CEventCenter.AddListener<int>(Message.M_AchieveSmallLevel,SmallLevelUp);
			CEventCenter.AddListener<int,int>(Message.M_RankAwake,OnRankAwake);
		}
		
		
		#region 字段
		
		//当前等级唯一标记
		private int currentLargeRank;
		private int currentSmallRank;
		//数值控制小等级升级
		public int Adder;
		
		
		#endregion
		
		
		#region 属性
		
		//获取当前小等级和大等级
		public AbstractSmallRank SmallRank
		{
			get { return LargeRankList[this.currentLargeRank].smallRanks[this.currentSmallRank]; }
		}
		public AbstractLargeRank LargeRank
		{
			get { return LargeRankList[this.currentLargeRank]; }
		}
		public int SmallRankIndex
		{
			get { return currentSmallRank; }
			private set
			{
				currentSmallRank = value;
			}
		}
		public int LargeRankIndex
		{
			get { return currentLargeRank; }
			private set{ currentLargeRank = value; }
		}
		public string LargeRankName
		{
			get { return LargeRankList[this.currentLargeRank].name; } 
		}
		public string SmallRankName 
		{
			get { return LargeRankList[this.currentLargeRank].smallRanks[this.currentSmallRank].name; }
		}
		//当前最大经验
    	public int Max
    	{
    		get { return SmallRank.max; }
    	}	
		//是否突破当前大等级瓶颈
		private bool BreakRank
		{
			get { return LargeRank.BreakLimit(); }
		}
		#endregion
		

		
		

		//获得小经验
		private void OnChangeSmallRankAdder(int change)
		{
			this.Adder += change;
			if (Adder <= 0)
				Adder = 0;
			else if (Adder >= Max)
			{
				//升级！

				if (this.SmallRankIndex == SmallRank.max - 1)
				{
					//巅峰
					if (LargeRank.BreakLimit())
					{
						//大突破
						CEventCenter.BroadMessage(Message.M_AchieveLargeLevel,this.LargeRankIndex+1);
					}
					else
					{
						//等待突破
					}
				}
				else
				{
					//小升级
					CEventCenter.BroadMessage(Message.M_AchieveSmallLevel,this.SmallRankIndex+1);
				}
			}
		}

		private void OnRankAwake(int large, int small)
		{
			this.currentLargeRank = large;
			this.currentSmallRank = small;
			this.Adder = 0;
		}
		
		//小升级
		void SmallLevelUp(int newRankIndex)
		{
			SmallRank.ImprovePlayer();
			this.currentSmallRank = newRankIndex;
			this.Adder = 0;
		}
		
		//大升级
		void LargeLevelUp(int newRankIndex)
		{
			LargeRank.ImprovePlayer();
			this.currentLargeRank = newRankIndex;
			
			this.Adder = 0;
		}
		
		
		
		#region IXmlSerializable

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			var intSer = new XmlSerializer(typeof(int));

			reader.Read();
			reader.ReadStartElement("smallIndex");
			this.currentSmallRank = (int) intSer.Deserialize(reader);
			reader.ReadEndElement();

			reader.ReadStartElement("largeIndex");
			this.currentLargeRank = (int) intSer.Deserialize(reader);
			reader.ReadEndElement();

			reader.ReadStartElement("adder");
			this.Adder = (int) intSer.Deserialize(reader);
			reader.ReadEndElement();
			
			reader.ReadEndElement();


		}

		public void WriteXml(XmlWriter writer)
		{
			var intSer = new XmlSerializer(typeof(int));


			writer.WriteStartElement("smallIndex");
			intSer.Serialize(writer, this.currentSmallRank);
			writer.WriteEndElement();

			writer.WriteStartElement("largeIndex");
			intSer.Serialize(writer, this.currentLargeRank);
			writer.WriteEndElement();

			writer.WriteStartElement("adder");
			intSer.Serialize(writer, this.Adder);
			writer.WriteEndElement();
		}
		
		#endregion
	}
}

