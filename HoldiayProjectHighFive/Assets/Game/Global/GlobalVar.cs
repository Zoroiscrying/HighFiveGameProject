using Game.Const;
using Game.Control.Person;
using Game.Serialization;
using System.IO;
using UnityEngine;

namespace Game.Global
{
    public static class GlobalVar
    {
        public static GameObject G_Canvas;
        //在玩家创建的时候初始化
        public static Player G_Player;

        public static void Refresh()
        {
            G_Canvas = GameObject.Find("Canvas");

            Player.InitPlayer();
        }
        public static void Refresh(Vector3 pos)
        {
            G_Canvas = GameObject.Find("Canvas");

            Player.InitPlayer(pos);
        }
    }
}
