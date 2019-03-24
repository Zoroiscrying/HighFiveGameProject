using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Game.Control;


namespace Game.Global
{
    public static class Flag
    {
        public static bool isPlaying = false;
    }
    public static class CGameObjects
    {
        public static  GameObject Canvas;
        //在玩家创建的时候初始化
        public static  GameObject Player;


        public static void Refresh()
        {
            Canvas = GameObject.Find("Canvas");
        }

    }
}

namespace Game.Const
{
    public static class Message
    {
        public static string M_LevelUp = "LevelUp";
        public static string M_ExpChange = "ExpChanged";
        public static string M_DragChange = "DragChanged";
        public static string M_BloodChange(GameObject go)
        {
            return go.GetInstanceID() + "BC";
        }
        public static string M_Destory(GameObject go)
        {
            return go.GetInstanceID() + "D";
        }
    }

    
    public static class FilePath
    {
        public static readonly string SkillFilePath = "SkillData.txt";
        public static readonly string PersonArgsFilePath = "PersonData.txt";
        public static readonly string SQLiteFilePath = Application.streamingAssetsPath + "/SQLite/SQLite4Unity.db";
    }
    
    
    public static class UIPath
    {
        public static readonly string Dir = "UI/";
        public static readonly string window_Tip =Dir+ "Image_Window_Tip";
        public static readonly string text_number =Dir+  "Text_Number";
        public static readonly string window_ItemTips =Dir+  "Image_ItemTips";
        public static readonly string window_ItemUI = Dir+ "Image_ItemUI";
        public static readonly string window_Slot =Dir+  "Image_Slot";
        public static readonly string Slider_BloodBar = Dir + "Slider";
        public static readonly string Panel_Dialog_Close =Dir+  "Image_Dialog_Close";
        public static readonly string Panel_Dialog_OK = Dir+ "Image_Dialog_OK";
        public static readonly string Panel_Dialog_OkConceal =Dir+  "Image_Dialog_OKConceal";
        public static readonly string Panel_Battle = Dir + "Panel_BattleScenery";
    }

    
    public static class BulletPath
    {
        public static readonly string Dir = "Bullet/";
        public static readonly string PlayerBullet = Dir + "bullet_1";
    }

    public static class GameObjectPath
    {
        public static readonly string Dir = "GameObjects/";
        public static readonly string TestPersonPath = Dir+"TestPerson";
        public static readonly string DefaultSprite = Dir+"NullSprite";
    }

    public static class Signal
    {
        public static readonly Vector3 defaultPos=new Vector3(0,0-999);
    }

    public static class PanelName
    {
        public static readonly string battlePanel = "BattlePanel";
//        public static readonly string 
    }

    public static class SceneName
    {
        public static readonly string welcomeScene = "WelcomeScene";
        public static readonly string testScene = "TestScene";
        public static readonly string jbScene = "JbScene";
    }
    
    public static class GameData
    {
        public static readonly string PlayerName = "Player";
        public static readonly string PlayerPath ="GameObjects/Player";
        public static readonly string PlayerDataFilePath = Application.streamingAssetsPath + "/Data/PlayerData.xml";
        public static Vector3 PlayerPos=new Vector3(-15, -1f, -1);
        public static List<string> PlayerDefaultSkills=new List<string>(new []{"L_Skill", "U_Skill", "I_Skill", "O_Skill","H_Skill"});  
    }
    
    
    
}