using Game.Const;
using Game.Control.Person;
using Game.Serialization;
using System.IO;
using UnityEngine;

namespace Game.Global
{
    public static class GlobalVar
    {
        public static GameObject Canvas;
        //在玩家创建的时候初始化
        public static Player Player;

        public static void Refresh()
        {
            Canvas = GameObject.Find("Canvas");

            #region 初始化主角
            if (File.Exists(DefaultData.PlayerDataFilePath))
            {
                Debug.Log("player comes from files");
                Player = XmlManager.LoadData<Player>(DefaultData.PlayerDataFilePath);
                //                Debug.Log(player);
                //AbstractPerson.GetInstance<Player>(Global.CGameObjects.Player);
                CEventCenter.BroadMessage(Message.M_LevelUp, GlobalVar.Player.rank);
            }
            else
            {
                Debug.Log("player comes from new");
                Player = new Player(DefaultData.PlayerName, DefaultData.PlayerPath, DefaultData.PlayerPos, DefaultData.PlayerDefaultSkills);
            }

            #endregion
        }
    }
}
