using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Game;
using UnityEngine;
using Game.Control;
using Game.Const;
using Game.Modal;
using Game.Serialization;
using Game.View;
using UnityEditor;



public class DemoTest : MonoBehaviour
{
	List<AbstractPerson> list=new List<AbstractPerson>();

	public Player player;
	// Use this for initialization
	void Start()
	{
		CreateTestPeople();
		if (File.Exists(GameData.PlayerDataFilePath))
		{
			this.player = XmlManager.LoadData<Player>(GameData.PlayerDataFilePath);
			CEventCenter.BroadMessage(Message.M_LevelUp,this.player.rank);
		}
	    else
			this.player=new Player(GameData.PlayerName,GameData.PlayerPath,GameData.PlayerPos,GameData.PlayerDefaultSkills);
	}
	void CreateTestPeople()
	{
		var go = new GameObject("TestPeople");
		for (int i = 5; i > 0; i--)
		{
			var x = new TestPerson("TestPerson", GameObjectPath.TestPersonPath,
				new Vector3(-25 + Random.Range(0, 20), 1.28f , -1), new List<string>(new []{"O_Skill","H_Skill","L_Skill"}), go.transform);
			list.Add(x);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			foreach (var t in list)
				if(t.obj!=null)
					new Bullet(10, t.Dir, t.Pos+new Vector3(0,0.3f,0), t);
		}



		if (Input.GetKeyDown(KeyCode.S))
		{
			XmlManager.SaveData(this.player,GameData.PlayerDataFilePath);
			AssetDatabase.Refresh();
		}
	}
}
