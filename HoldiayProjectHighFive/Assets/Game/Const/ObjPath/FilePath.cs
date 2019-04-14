using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Const
{
    public static class FilePath
    {
        public static readonly string SkillFilePath = "SkillData.txt";
        public static readonly string PersonArgsFilePath = "PersonData.txt";
        public static readonly string ItemFilePath = "ItemData.txt";
        public static readonly string SQLiteFilePath = Application.streamingAssetsPath + "/SQLite/SQLite4Unity.db";
    }
}
