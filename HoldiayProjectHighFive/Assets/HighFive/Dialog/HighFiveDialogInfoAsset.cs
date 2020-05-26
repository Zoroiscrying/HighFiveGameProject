using System;
using DialogSystem.Model;
using DialogSystem.ScriptObject;
using HighFive.Const;
using UnityEngine;
#if UNITY_EDITOR
using ReadyGamerOne.Utility;
using UnityEditor;
#endif

namespace HighFive.Dialog
{
    public class HighFiveDialogInfoAsset:AbstractDialogInfoAsset
    {

#if UNITY_EDITOR
        [MenuItem("ReadyGamerOne/DialogSystem/Create/HighFiveDialogInfoAsset #&I")]
        public static void CreateAsset()
        {
            EditorUtil.CreateAsset<HighFiveDialogInfoAsset>("DialogAsset");
        }         
#endif

        //这里只展示用法，详细注释请看父类

        #region 基础

        public override Func<bool> CanGoToNext =>
            () => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return);
        
        public override Action<DialogUnitInfo> CreateWordUI =>(info)=>new HighFiveCaptionWordUi(info);
        public override string[] CaptionWordUiKeys => new[]
        {
            UiName.Caption_Word_1,
            UiName.Caption_Word_2,
        };
        public override Action<DialogUnitInfo> CreateChooseUi =>(info)=>new HighFiveCaptionChooseUi(info);

        public override string ChooseUiKeys => UiName.Caption_Choose_1;

        public override Action<DialogUnitInfo> CreateNarratorUI => (info)=> new HighFiveNarratorUi(info);

        public override string NarratorUiKeys => UiName.NarratorUi;

        #endregion



        #region 高级

        public override Func<bool> IfInteract =>()=> Input.GetKeyDown(KeyCode.E);
//        public override Type MessageType => typeof(Message);
//        
//        public override Type PanelType => typeof(PanelName);
        
//        public override Type SceneType=>typeof(SceneName);
        #endregion
        

    }
}