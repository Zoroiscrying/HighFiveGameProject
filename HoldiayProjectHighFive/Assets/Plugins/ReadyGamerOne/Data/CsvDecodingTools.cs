using System.IO;
using ReadyGamerOne.Global;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.Data
{
#if UNITY_EDITOR
    
    public class CsvDecodingTools:IEditorTools
    {
        private static string csvPath = "";


        private static GUIStyle titleStyle = new GUIStyle
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter
        };

        
        private static string Title = "CSV解析";
        private static void OnToolsGUI(string rootNs,string viewNs,string constNs,string dataNs,string autoDir,string scriptDir)
        {
            EditorGUILayout.Space();

            var generateDir = Application.dataPath + "/" + rootNs + "/" + dataNs + "/" + autoDir;
            
            EditorGUILayout.LabelField("csv数据类生成目录", rootNs + "/" + dataNs + "/" + autoDir);
            
            EditorGUILayout.Space();
            GUILayout.Label("请选择一个合法的CSV文件",titleStyle);

            if(File.Exists(csvPath) && csvPath.EndsWith(".csv"))
                GUILayout.Label(csvPath);
            else
            {
                EditorGUILayout.HelpBox("请选择CSV文件路径", MessageType.Warning);
            }
            if (GUILayout.Button("设置CSV数据文件所在目录"))
                csvPath = EditorUtility.OpenFilePanel("选择CSV文件", Application.dataPath, "csv");
        
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("生成C#协议文件",GUILayout.Height(3*EditorGUIUtility.singleLineHeight)))
            {
                var consDir = Application.dataPath + "/" + rootNs + "/" + constNs + "/" + autoDir;
                if (!Directory.Exists(consDir))
                    Directory.CreateDirectory(consDir);
                if (!Directory.Exists(generateDir))
                    Directory.CreateDirectory(generateDir);
                
                if (File.Exists(csvPath) && csvPath.EndsWith(".csv"))
                {
                    CreatConfigFile(csvPath, generateDir,rootNs+"."+dataNs);
                    Utility.FileUtil.ReCreateFileNameClassFromDir("FileName", consDir,Application.dataPath + "/Resources/GameConfig",rootNs+"."+constNs);
                    AssetDatabase.Refresh();        //这里是一个点
                    Debug.Log("生成完成");
                }
                else
                {
                    Debug.LogError("生成失败——请正确设置所有路径");
                }
            }
        }

        private static void CreatConfigFile(string filePath, string writePath,string nameSpace)
        {
            var targetPath = Application.dataPath + "/Resources/GameConfig";

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            targetPath = targetPath + "/" + fileName + ".csv";
            if(targetPath!=csvPath)
                File.Copy(csvPath, targetPath, true);

            string className = fileName;
            StreamWriter sw = new StreamWriter(writePath + "/" + className + ".cs");

            sw.WriteLine("using UnityEngine;\nusing System.Collections;\nusing ReadyGamerOne.Data;\n");
            var ns = string.IsNullOrEmpty(nameSpace) ? "DefaultNamespace" : nameSpace;
            sw.WriteLine("namespace "+ns+"\n" +
                         "{\n" +
                         "\tpublic class " + className + " : CsvMgr");
            sw.WriteLine("\t{");

            var csr = new CsvStreamReader(csvPath);
            for (int colNum = 1; colNum < csr.ColCount + 1; colNum++)
            {
                string fieldName = csr[1, colNum];
                string fieldType = csr[2, colNum];
                sw.WriteLine("\t\t" + "public " + fieldType + " " + fieldName + ";" + "");
            }
            sw.WriteLine("\t}\n" +
                         "}\n");

            sw.Flush();
            sw.Close();
        }
    }
#endif
    
}

