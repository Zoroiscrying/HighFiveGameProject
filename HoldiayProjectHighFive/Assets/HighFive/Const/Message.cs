﻿namespace HighFive.Const
{
    public static class Message
    {
        #region 背包
        public static readonly string M_AddItem = "at";       //    string (id) int (count)
        public static readonly string M_RemoveItem = "rt";    //    string (id) int (count)
        #endregion
        
        #region 等级

        public static readonly string M_LevelUp = "LevelUp";    
        
        #endregion
        
        public static readonly string M_InitSuper = "InitSuper";    //    void
        public static readonly string M_ExitSuper = "ExitSuper";    //    void

        public static readonly string M_MoneyChange = "mc";          //    int 

        public static readonly string M_LoadSceneAsync = "LSA";    //      string newSceneName


        public static readonly string M_OnTryBut="OnTryBuy";    // int itemID

        public static readonly string M_PlayerBloodChange = "PlayerBloodChange";// int change
        public static readonly string M_PlayerExpChange = "PlayerExpChange";// int change
        public static readonly string M_PlayerDragChange = "PlayerDragChange";// int change
        
        
    }
}
