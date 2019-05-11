using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Const
{
    public static class UIPath
    {
        #region Dir
        public static readonly string Dir = "UI/";
        public static readonly string PanelDir = Dir + "Panels/";
        public static readonly string LittleUiPath = Dir + "LittleUI/";
        #endregion

        public static readonly string text_number = LittleUiPath + "Text_Number";

        public static readonly string Slider_BloodBar = LittleUiPath + "Slider";

        public static readonly string Image_MiniMap = LittleUiPath + "Image_MiniMapBackGround";

        public static readonly string Image_ItemData = LittleUiPath + "Image_ItemBk";
        
        #region 背包
        public static readonly string Image_ItemInfo = LittleUiPath + "Image_ItemInfo";
        public static readonly string Image_ItemUI = LittleUiPath + "Image_ItemUI";
        public static readonly string Image_Slot = LittleUiPath + "Image_Slot";
        #endregion


    }
}
