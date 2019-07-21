using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Game.Const
{
    public static class Message
    {
        #region 背包
        public static readonly string M_AddItem = "at";       //    int (id) int (count)
        public static readonly string M_RemoveItem = "rt";    //    int
        public static readonly string M_TouchItem = "tm";     //    Slot
        public static readonly string M_ReleaseItem = "rm";   //    Slot
        #endregion
        
        #region 等级

        public static readonly string M_RankAwake = "RA";    
        public static readonly string M_AchieveSmallLevel = "SLU";    //    int 
        public static readonly string M_AchieveLargeLevel = "LLU";    //    int 
        public static readonly string M_ChangeSmallLevel = "CSL";     //    int
        
        #endregion
        
        public static readonly string M_InitSuper = "InitSuper";    //    void
        public static readonly string M_ExitSuper = "ExitSuper";    //    void

        public static readonly string M_MoneyChange = "mc";          //    int 
        public static readonly string M_TryChangeMoney = "TryCm";    //    int abstractcommodity

        public static readonly string M_LoadSceneAsync = "LSA";    //      string newSceneName
        

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
