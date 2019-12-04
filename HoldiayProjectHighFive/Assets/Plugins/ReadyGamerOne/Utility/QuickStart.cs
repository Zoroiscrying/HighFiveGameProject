using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReadyGamerOne.Global;
using ReadyGamerOne.View;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace ReadyGamerOne.Utility
{
    public class QuickStart
#if UNITY_EDITOR
        :IEditorTools
#endif
    {

        public static void RegisterUi(Type rigisterType)
        {
            AbstractPanel.RegisterPanels(rigisterType);
        }
        
#if UNITY_EDITOR
        private static List<string> autoClassName=new List<string>();
        private static string Title = "QuickStart";
        private static Dictionary<string, string> nameToPath = new Dictionary<string, string>();
        private static bool createGameMgr = true;
        private static bool createPanelClasses = true;
        static void OnToolsGUI(string rootNs, string viewNs, string constNs, string dataNs, string autoDir,string scriptNs)
        {
            var resources = Application.dataPath + "/Resources";
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("自动常量文件生成目录",Application.dataPath+"/"+rootNs+"/"+constNs+"/"+autoDir);
            EditorGUILayout.Space();
            
            
            createPanelClasses = EditorGUILayout.Toggle("是否自动生成Panel类型", createPanelClasses);
            if(createPanelClasses)
                EditorGUILayout.LabelField("自动生成Panel文件路径",Application.dataPath+"/"+rootNs+"/"+viewNs);
            EditorGUILayout.Space();
            
            
            createGameMgr = EditorGUILayout.Toggle("是否生成GameMgr", createGameMgr);
            if(createGameMgr)
                EditorGUILayout.LabelField("自动生成"+rootNs+ "Mgr文件路径",Application.dataPath+"/"+rootNs+"/"+scriptNs);
            EditorGUILayout.Space();
            
            
            if (GUILayout.Button("开启自动生成",  GUILayout.Height(2*EditorGUIUtility.singleLineHeight)))
            {
                autoClassName.Clear();
                var rootDir = Application.dataPath + "/" + rootNs;
                FileUtil.CreateFolder(rootDir);
                FileUtil.CreateFolder(rootDir + "/" + constNs);
                FileUtil.CreateFolder(rootDir + "/" + constNs+"/"+autoDir);
                nameToPath.Clear();
                 if (!Directory.Exists(resources))
                 {
                     Directory.CreateDirectory(resources);
                     return;
                 }
                 FileUtil.SearchDirectory(resources,
                     OprateFile,false,
                     filPath=> OprateDir(filPath,rootNs,constNs,autoDir)
                     );

                 if (nameToPath.Count > 0)
                 {
                     FileUtil.CreateConstClassByDictionary("OtherResPath", rootDir + "/" + constNs + "/" + autoDir,
                         rootNs + "." + constNs, nameToPath);
                     FileUtil.CreateConstClassByDictionary("OtherResName", rootDir + "/" + constNs + "/" + autoDir,
                         rootNs + "." + constNs, nameToPath.Select(pair => pair.Key).ToDictionary(name => name));
                     autoClassName.Add("OtherRes");
                 }


                 if (autoClassName.Contains("Panel"))
                     CreatePanelFile(Application.dataPath + "/Resources/ClassPanel", viewNs, constNs, rootNs, autoDir);

                 if (createGameMgr)
                 {
                     CreateMgr(rootNs,constNs,scriptNs,autoDir);
                 }

                 AssetDatabase.Refresh();
                 Debug.Log("生成结束");
            }

            EditorGUILayout.Space();
            
            if (autoClassName.Count > 0)
            {
                var str = "生成的类型有：";
                foreach (var name in autoClassName)
                {
                    str += "\n" + name + "Name" + "\t\t" + name + "Path";
                }
                EditorGUILayout.HelpBox(str,MessageType.Info);
            }

        }


        /// <summary>
        /// 创建GameMgr
        /// </summary>
        /// <param name="rootNs"></param>
        /// <param name="constNs"></param>
        /// <param name="scriptNs"></param>
        /// <param name="autoDirName"></param>
        private static void CreateMgr(string rootNs,string constNs,string scriptNs,string autoDirName)
        {
            var safePartDir = Application.dataPath + "/" + rootNs + "/" + scriptNs;
            var autoPartDir = safePartDir + "/" + autoDirName;
            FileUtil.CreateFolder(safePartDir);
            FileUtil.CreateFolder(autoPartDir);
            var fileName = rootNs + "Mgr";
            
            var safePartPath = safePartDir + "/" + fileName + ".cs";
            var autoPartPath = autoPartDir + "/" + fileName + ".cs";

            #region AutoPart

            if (File.Exists(autoPartPath))
                File.Delete(autoPartPath);
            
            var stream = new StreamWriter(autoPartPath);

            stream.Write("using ReadyGamerOne.Script;\n");
            
            stream.Write("namespace "+rootNs+"."+scriptNs+"\n"+
                         "{\n" +
                         "\tpublic partial class "+fileName+" : AbstractGameMgr<"+fileName+">\n" +
                         "\t{\n");

            
            stream.Write("\t\tpartial void OnSafeAwake();\n");

            stream.Write("\t\tprotected override void Awake()\n" +
                         "\t\t{\n" +
                         "\t\t\tbase.Awake();\n" +
                         "\t\t\tOnSafeAwake();\n");
            
            stream.Write("\t\t}\n" +
                         "\t}\n" +
                         "}\n");
            
            stream.Dispose();
            stream.Close();            
            

            #endregion

            #region SafePart

            if (File.Exists(safePartPath))
                return;
          
            stream=new StreamWriter(safePartPath);
            
            var usePanel = autoClassName.Contains("Panel");
            var useAudio = autoClassName.Contains("Audio");
            if ( useAudio || usePanel )
            {
                stream.Write("using ReadyGamerOne.EditorExtension;\n");
                stream.Write("using "+rootNs+"."+constNs+";\n");
            }
            
            if(usePanel)
                stream.Write("using ReadyGamerOne.View;\n" +
                             "using ReadyGamerOne.Script;\n");

            stream.Write("namespace " + rootNs + "." + scriptNs + "\n" +
                         "{\n" +
                         "\tpublic partial class " + fileName + "\n" +
                         "\t{\n");
            if (usePanel)
            {
                stream.Write("\t\tpublic StringChooser startPanel = new StringChooser(typeof(PanelName));\n");
            }

            if (useAudio)
            {
                stream.Write("\t\tpublic StringChooser startBgm = new StringChooser(typeof(AudioPath));\n");
            }

            stream.Write("\t\tpartial void OnSafeAwake()\n" +
                         "\t\t{\n");
                         
            if(usePanel)
                stream.Write("\t\t\tPanelMgr.PushPanel(startPanel.StringValue);\n");
            if(useAudio)
                stream.Write("\t\t\tAudioMgr.Instance.PlayBgm(startBgm.StringValue);\n");
            
            stream.Write("\t\t\t//do any thing you want\n" +
                         "\t\t}\n" +
                         "\t}\n" +
                         "}\n");
            
            
            stream.Dispose();
            stream.Close();
            
            #endregion
        }
        
        /// <summary>
        /// 创建Panel类
        /// </summary>
        /// <param name="panelPrefabDir"></param>
        /// <param name="viewNs"></param>
        /// <param name="constNs"></param>
        /// <param name="rootNs"></param>
        /// <param name="autoDirName"></param>
        /// <returns></returns>
        private static bool CreatePanelFile(string panelPrefabDir, string viewNs,string constNs,string rootNs,string autoDirName)
        {
            if (!Directory.Exists(panelPrefabDir))
            {
                Debug.LogWarning("panelPrefabDir is not exist ! ");
                return false;
            }
            foreach (var fullPath in Directory.GetFiles(panelPrefabDir))
            {
                if(fullPath.EndsWith(".meta"))
                    continue;
                if(!fullPath.EndsWith(".prefab"))
                    continue;
                
                var prefabName=Path.GetFileNameWithoutExtension(fullPath);
                var fileName = prefabName;
                var className = prefabName;
                var safePartDir = Application.dataPath + "/" + rootNs + "/" + viewNs;
                var autoPartDir = safePartDir + "/" + autoDirName;
//                Debug.Log(safePartDir);
                FileUtil.CreateFolder(safePartDir);
                FileUtil.CreateFolder(autoPartDir);
                var safePartPath = safePartDir + "/" + fileName + ".cs";
                var autoPartPath = autoPartDir + "/" + fileName + ".cs";

                #region AutoPart

                if(File.Exists(autoPartPath))
                    File.Delete(autoPartPath);
                
                var stream = File.CreateText(autoPartPath);

                stream.Write("using ReadyGamerOne.View;\n" +
                             "using "+rootNs+"."+constNs+";\n" +
                             "namespace " + rootNs+"."+viewNs + "\n" +
                             "{\n" +
                             "\tpublic partial class " + className + " : AbstractPanel\n" +
                             "\t{\n" +
                             "\t\tpartial void OnLoad();\n" +
                             "\n" +
                             "\t\tprotected override void Load()\n" +
                             "\t\t{\n" +
                             "\t\t\tCreate(PanelPath." + className + ");\n" +
                             "\t\t\tOnLoad();" +
                             "\t\t}\n" +
                             "\t}\n" +
                             "}\n");


                stream.Flush();
                stream.Dispose();
                stream.Close();                

                #endregion

                #region SafePart
                
                if(File.Exists(safePartPath))
                    continue;
                
                stream=new StreamWriter(safePartPath);
                
                stream.Write("namespace "+rootNs+"."+viewNs+"\n" +
                             "{\n" +
                             "\tpublic partial class "+fileName+"\n" +
                             "\t{\n" +
                             "\t\tpartial void OnLoad()\n" +
                             "\t\t{\n" +
                             "\t\t\t//do any thing you want\n" +
                             "\t\t}\n" +
                             "\t}\n" +
                             "}\n");
                
                stream.Dispose();
                stream.Close();
                

                #endregion
                

            }
            
            return true;
        }
        
        private static void OprateFile(string filePath)
        {
            if(filePath.EndsWith(".meta"))
                return;
            var loadPath =  filePath.GetAfterSubstring("Resources").GetBeforeSubstring(Path.GetExtension(filePath));
            nameToPath.Add(Path.GetFileNameWithoutExtension(filePath),loadPath);
        }

        private static void OprateDir(string dirPath,string rootNs,  string constNs, string autoDir)
        {
            var dirName = dirPath.GetAfterLastChar('\\');
            if(dirName.StartsWith("Global"))
                return;
            if (dirName.StartsWith("Class"))
            {
                autoClassName.Add(dirName.GetAfterSubstring("Class"));
                FileUtil.ReCreateFileNameClassFromDir(
                    dirName.GetAfterSubstring("Class") + "Path",
                    Application.dataPath+"/"+rootNs+"/"+constNs+"/"+autoDir,
                    dirPath,
                    rootNs+"."+constNs,
                    (filaName,stream) =>
                    {
                        if(!filaName.EndsWith(".meta"))
                        {
                            var name = Path.GetFileNameWithoutExtension(filaName);
                            var loadPath = filaName.GetAfterSubstring("Resources\\");
                            loadPath =loadPath.GetBeforeSubstring(Path.GetExtension(filaName));
                            stream.Write("\t\tpublic const string "+name+" = @\""+loadPath+"\";\n");
                        }
                    },true);

                var className = dirName.GetAfterSubstring("Class") + "Name";
                FileUtil.ReCreateFileNameClassFromDir(
                    className,
                    Application.dataPath+"/"+rootNs+"/"+constNs+"/"+autoDir,
                    dirPath,
                    rootNs+"."+constNs,
                    (filaName,stream) =>
                    {
                        if(!filaName.EndsWith(".meta"))
                        {
                            var name = Path.GetFileNameWithoutExtension(filaName);
                            var loadPath = filaName.GetAfterSubstring("Resources\\");
                            loadPath =loadPath.GetBeforeSubstring(Path.GetExtension(filaName));
                            stream.Write("\t\tpublic const string "+name+" = \""+name+"\";\n");
                        }
                    },true);
            }
            else
                FileUtil.SearchDirectory(dirPath, OprateFile,true);
        }
#endif

    }
}