using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Game;
using Game.Const;
using Game.Control;
using Game.Serialization;


namespace Game.Global
{
    public static class GlobalFlag
    {
        public static bool isPlaying = false;
    }
    
    public static class GlobalVar
    {
        public static  GameObject Canvas;
        //在玩家创建的时候初始化
        public static  Player Player;

        public static void Refresh()
        {
            Canvas = GameObject.Find("Canvas");
            
            #region 初始化主角
            if (File.Exists(GameData.PlayerDataFilePath))
            {
                Debug.Log("player comes from files");
                Player = XmlManager.LoadData<Player>(GameData.PlayerDataFilePath);
//                Debug.Log(player);
                //AbstractPerson.GetInstance<Player>(Global.CGameObjects.Player);
                CEventCenter.BroadMessage(Message.M_LevelUp,GlobalVar.Player.rank);
            }
            else
            {
                Debug.Log("player comes from new");
                Player=new Player(GameData.PlayerName,GameData.PlayerPath,GameData.PlayerPos,GameData.PlayerDefaultSkills);
            }
            
            #endregion
        }
    }
}

namespace Game.Const
{

    #region 名字/唯一标识
    public static class SpiritName
    {
        public static readonly string C_First = "first";
        public static readonly string C_Second = "second";
    }    
    public static class PanelName
    {
        public static readonly string battlePanel = "BattlePanel";
    }

    public static class SkillTriggerName
    {
        public static readonly string animation="AnimationTrigger";
        public static readonly string instantDamage="InstantDamageTrigger";
        public static readonly string audio="AudioTrigger";
        public static readonly string dash="DashTrigger";
        public static readonly string bullet="BulletTrigger";
        public static readonly string trigger2D="Trigger2DTrigger";
    }
    
    public static class SceneName
    {
        public static readonly string welcomeScene = "WelcomeScene";
        public static readonly string testScene = "TestScene";
        public static readonly string jbScene = "JbScene";
    }
    
    #endregion
    
    
    #region 消息
    
    public static class Message
    {
        public static readonly string M_LevelUp = "LevelUp";
        public static readonly string M_ExpChange = "ExpChanged";
        public static readonly string M_DragChange = "DragChanged";
        public static readonly string M_InitSuper = "InitSuper";
        public static readonly string M_ExitSuper = "ExitSuper";
         
        public static string M_BloodChange(GameObject go)
        {
            return go.GetInstanceID() + "BC";
        }
        public static string M_Destory(GameObject go)
        {
            return go.GetInstanceID() + "D";
        }
    }
    #endregion
    
    
    
    #region 路径
    
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
    
    #endregion
    
    
    #region defaultData

    public static class Signal
    {
        public static readonly Vector3 defaultPos=new Vector3(0,0-999);
    }


    
    public static class GameData
    {
        public static readonly string PlayerName = "Player";
        public static readonly string PlayerPath ="GameObjects/Player";
        public static readonly string PlayerDataFilePath = Application.streamingAssetsPath + "/Data/PlayerData.xml";
        public static Vector3 PlayerPos=new Vector3(0, 0f, -1);
        public static List<string> PlayerDefaultSkills=new List<string>(new []{"L_Skill", "U_Skill", "I_Skill", "O_Skill","H_Skill"});  
    }
    
    #endregion
    
    
}