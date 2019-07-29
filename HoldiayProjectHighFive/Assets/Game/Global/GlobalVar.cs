using Game.Const;
using Game.Control.PersonSystem;
using Game.Data;
using System.IO;
using UnityEngine;

namespace Game.Global
{
    public static class GlobalVar
    {
        public static GameObject G_Canvas;
        //在玩家创建的时候初始化
        public static Player G_Player;
        public static Vector3 GCanvasButton
        {
            get { return G_Canvas.transform.position - new Vector3(0, Screen.height, 0); }
        }

        public static Vector3 GCanvasTop
        {
            get { return G_Canvas.transform.position + new Vector3(0, Screen.height, 0); }
        }

        public static Vector3 GCanvasLeft
        {
            get { return G_Canvas.transform.position - new Vector3(Screen.width, 0, 0); }
        }

        public static Vector3 GCanvasRight
        {
            get { return G_Canvas.transform.position + new Vector3(Screen.width, 0, 0); }
        }

        public static void Refresh()
        {
            G_Canvas = GameMgr.Instance.gameObject;

            Player.InitPlayer();
        }
        public static void Refresh(Vector3 pos)
        {
//            Debug.Log("GlobalVar: "+pos);
            G_Canvas = GameMgr.Instance.gameObject;

            Player.InitPlayer(pos);
        }
    }
}
