using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Const;
using Game.Control;
using Game.Modal;
using Game.Script;
using Game.Serialization;
using Game.View;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneManger : MonoSingleton<SceneManger>
{


	List<AbstractPerson> list=new List<AbstractPerson>();
	private Player player;
	public string UiPanelName;
	private SQLiteHelper sql;
	
	void Awake()
	{
		Game.Global.CGameObjects.Refresh();
		this.gameObject.AddComponent<MainLoop>();
		this.gameObject.AddComponent<AudioMgr>();
		InitBehavic();
		CreateTestPeople();
		BaseSceneInfo.InitPlayer();
		UIManager.Instance.PushPanel(this.UiPanelName);
	}	

	// Use this for initialization
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
     			SceneMgr.Instance.LoadScene(Game.Const.SceneName.welcomeScene);
		
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
	private bool InitBehavic()
    {
     	Debug.Log("InitBehavic");
     	behaviac.Workspace.Instance.FilePath = Application.dataPath + "/Scripts/behaviac/exported/behaviac_generated/types";
     	behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;
     	return true;
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

    void Start () 
    {
        //创建名为sqlite4unity的数据库
//        sql = new SQLiteHelper();
//        //关闭数据库连接
//        sql.CloseConnection();
//        //创建名为sqlite4unity的数据库
//        sql = new SQLiteHelper();
//        //关闭数据库连接
//        sql.CloseConnection();        //创建名为sqlite4unity的数据库
//        sql = new SQLiteHelper();
//        //读取整张表
//        SqliteDataReader reader = sql.ReadFullTable ("table1");
//        while(reader.Read()) 
//        {
//            //读取ID
//            Debug.Log(reader.GetInt32(reader.GetOrdinal("ID")));
//            //读取Name
//            Debug.Log(reader.GetString(reader.GetOrdinal("Name")));
//            //读取Age
//            Debug.Log(reader.GetInt32(reader.GetOrdinal("Age")));
//            //读取Email
//            Debug.Log(reader.GetString(reader.GetOrdinal("Email")));
//        }
//        //关闭数据库连接
//        sql.CloseConnection();
        //创建名为table1的数据表
        //sql.CreateTable("table1",new string[]{"ID","Name","Age","Email"},new string[]{"INTEGER","TEXT","INTEGER","TEXT"});

        //插入两条数据
//        sql.InsertValues("table1",new string[]{"'1'","'张三'","'22'","'Zhang3@163.com'"});
//        sql.InsertValues("table1",new string[]{"'2'","'李四'","'25'","'Li4@163.com'"});
//
//        //更新数据，将Name="张三"的记录中的Name改为"Zhang3"
//        sql.UpdateValues("table1", new string[]{"Name"}, new string[]{"'Zhang3'"}, "Name", "=", "'张三'");
//
//        //插入3条数据
//        sql.InsertValues("table1",new string[]{"3","'王五'","25","'Wang5@163.com'"});
//        sql.InsertValues("table1",new string[]{"4","'王五'","26","'Wang5@163.com'"});
//        sql.InsertValues("table1",new string[]{"5","'王五'","27","'Wang5@163.com'"});

        //删除Name="王五"且Age=26的记录,DeleteValuesOR方法类似
//        sql.DeleteValuesAND("table1", new string[]{"Name","Age"}, new string[]{"=","="}, new string[]{"'王五'","'26'"});
//
        

//        //读取数据表中Age>=25的所有记录的ID和Name
//        reader = sql.ReadTable ("table1", new string[]{"ID","Name"}, new string[]{"Age"}, new string[]{">="}, new string[]{"'25'"});
//        while(reader.Read()) 
//        {
//            //读取ID
//            Debug.Log(reader.GetInt32(reader.GetOrdinal("ID")));
//            //读取Name
//            Debug.Log(reader.GetString(reader.GetOrdinal("Name")));
//        }
//
//        //自定义SQL,删除数据表中所有Name="王五"的记录
//        sql.ExecuteQuery("DELETE FROM table1 WHERE NAME='王五'");

    }
}
