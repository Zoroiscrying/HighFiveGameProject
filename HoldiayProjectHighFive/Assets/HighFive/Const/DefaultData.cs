using System.Collections.Generic;
using UnityEngine;

namespace HighFive.Const
{
    public static class DefaultData
    {
        public static readonly string[] shop ={ItemName.LittleBoy,ItemName.Medkit,ItemName.Snail,ItemName.ChenTie,ItemName.DuanYin,ItemName.XueJing,ItemName.Soda};
        
        public static readonly int DefaultPackageSlotCount = 16;
        public static readonly string PlayerName = "Player";
        public static readonly string PlayerPath = "GameObjects/Player";
        public static readonly string PlayerDataFilePath = Application.streamingAssetsPath + "/Data/PlayerData.xml";
        public static Vector3 PlayerPos = new Vector3(0, 0f, -1);
        public static List<string> PlayerDefaultSkills = new List<string>(new[] {  "U_Skill", "U_Skill", "O_Skill","Shot", "L_Skill","ParaShoot" });
    }
}
