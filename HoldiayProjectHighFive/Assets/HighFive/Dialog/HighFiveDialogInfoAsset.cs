using System;
using DialogSystem.Model;
using DialogSystem.ScriptObject;
using UnityEngine;
#if UNITY_EDITOR
    
using UnityEngine.Windows;
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
            string[] strs = Selection.assetGUIDs;

            string path = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (path.Contains("."))
            {
                path=path.Substring(0, path.LastIndexOf('/'));
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var name = path + "/NewDialogInfo";
            while (File.Exists(name + ".asset"))
                name += "(1)";
            

            AssetDatabase.CreateAsset(CreateInstance<HighFiveDialogInfoAsset>(), name + ".asset");
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<HighFiveDialogInfoAsset>(name + ".asset");
        }         
#endif

        //这里只展示用法，详细注释请看父类

        #region 基础

        public override Func<bool> CanGoToNext => () => Input.GetMouseButtonDown(0);
        
        public override Action<DialogUnitInfo> CreateWordUI =>(info)=>new HighFiveCaptionWordUi(info);
        public override string[] CaptionWordUiKeys => new[]
        {
            "Caption_Word_1",
            "Caption_Word_2",
        };
        public override Action<DialogUnitInfo> CreateChooseUi =>(info)=>new HighFiveCaptionChooseUi(info);

        public override string ChooseUiKeys => "Caption_Choose_1";

        public override Action<DialogUnitInfo> CreateNarratorUI => (info)=> new HighFiveNarratorUi(info);

        public override string NarratorUiKeys => "NarratorUi";

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