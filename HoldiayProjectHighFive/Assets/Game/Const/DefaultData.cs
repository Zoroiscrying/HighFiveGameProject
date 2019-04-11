using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Const
{
    public static class DefaultData
    {
        public static readonly string PlayerName = "Player";
        public static readonly string PlayerPath = "GameObjects/Player";
        public static readonly string PlayerDataFilePath = Application.streamingAssetsPath + "/Data/PlayerData.xml";
        public static Vector3 PlayerPos = new Vector3(0, 0f, -1);
        public static List<string> PlayerDefaultSkills = new List<string>(new[] {  "U_Skill", "O_Skill","Shot", "L_Skill" });
    }
}
