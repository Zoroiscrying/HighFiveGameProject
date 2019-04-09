using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Game.Const
{
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
}
