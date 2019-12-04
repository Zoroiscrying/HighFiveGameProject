using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ReadyGamerOne.Utility
{
    public static class FileUtil
    {
        /// <summary>
        /// 当前可执行文件路径
        /// </summary>
        public static string CurrentRunDirectory => Environment.CurrentDirectory;
        /// <summary>
        /// 获取父级目录
        /// </summary>
        /// <param CharacterName="pathNum"></param>
        /// <returns></returns>
        public static string GetRunDirectoryInParentPath(int pathNum=3)
        {
            //    /Server/Server/bin/Debug/server.exe
            DirectoryInfo pathInfo = Directory.GetParent(CurrentRunDirectory);
            while (pathNum > 0 && pathInfo.Parent != null)
            {
                var info = pathInfo.Parent;
                pathNum--;
            }
            //返回一个完整的文件夹路径
            return pathInfo.FullName;
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <returns></returns>
        public static string CreateFolder(string path)
        {
            //如果目录存在则创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;



        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="foldName"></param>
        /// <returns></returns>
        public static string CreateFolder(string path,string foldName)
        {
            var fullPath = path + "//" + foldName;
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            return fullPath;
        }


        /// <summary>
        /// 创建文件（初始化文本）
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="fileName"></param>
        /// <param CharacterName="info"></param>
        public static void CreateFile(string path,string fileName,string info=null)
        {
            CreateFolder(path);
            var fileInfo = new FileInfo(path + "//" + fileName);
            if (fileInfo.Exists)
                fileInfo.Delete();
            var streamWriter = fileInfo.CreateText();

            if(!string.IsNullOrEmpty(info))
                streamWriter.WriteLine(info);

            streamWriter.Close();
            streamWriter.Dispose();
        }
        /// <summary>
        /// 向文件中添加信息
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="fileName"></param>
        /// <param CharacterName="info"></param>
        public static void AddInfoToFile(string path,string fileName,string info)
        {
            CreateFolder(path);

            var fileInfo = new FileInfo(path + "//" + fileName);

            var streamWriter = !fileInfo.Exists ? fileInfo.CreateText() : fileInfo.AppendText();

            streamWriter.WriteLine(info);

            streamWriter.Close();

            streamWriter.Dispose();

        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="fileName"></param>
        /// <returns></returns>
        public static string LoadFile(string path,string fileName)
        {
            if (!FileExist(path, fileName))
                return null;

            if (!path.EndsWith("/"))
                path = path + "/";
            
            var streamReader = new StreamReader(path + fileName);

            var arr = new ArrayList();

            while (true)
            {
                var line = streamReader.ReadLine();
                if (line == null)
                    break;
                arr.Add(line);
            }
            streamReader.Close();
            streamReader.Dispose();
            string ans = "";
            foreach (var str in arr)
                ans += str;
            return ans;
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="fileName"></param>
        /// <returns></returns>
        public static bool FileExist(string path,string fileName)
        {
            if (!Directory.Exists(path))
                return false;
            if (!File.Exists(path + "//" + fileName))
                return false;
            return true;
        }

        public static void CreateConstClassByDictionary(string className, string generateDir, string nameSpace,
            Dictionary<string, string> content)
        {
            var constClassFullPath = generateDir + "/"+className+".cs";
            if(File.Exists(constClassFullPath))
                File.Delete(constClassFullPath);
            var stream = new StreamWriter(constClassFullPath);
            
            var ns = string.IsNullOrEmpty(nameSpace) ? "DefaultNameSpace" : nameSpace;
            
            stream.Write("namespace "+ns+"\n" +
                         "{\n" +
                         "\tpublic partial class "+className+ "\n" +
                         "\t{\n");

            foreach (var kv in content)
            {
                stream.Write("\t\tpublic const string "+kv.Key+" = \""+kv.Value+"\";\n");                
            }
            
            stream.Write("\t}\n" +
                         "}\n");
            stream.Flush();
            stream.Dispose();
            stream.Close();
        }
        
        public static void ReCreateFileNameClassFromDir(string className, string generateDir,string dirWhichContainsFiles, string nameSpace,Action<string,StreamWriter> onOperateFile=null,bool deepSearch=false)
        {
            var panelNameClassPath = generateDir + "/"+className+".cs";
            if(File.Exists(panelNameClassPath))
                File.Delete(panelNameClassPath);
            var stream = new StreamWriter(panelNameClassPath);
            
            var ns = string.IsNullOrEmpty(nameSpace) ? "DefaultNameSpace" : nameSpace;
            
            stream.Write("namespace "+ns+"\n" +
                         "{\n" +
                         "\tpublic partial class "+className+ "\n" +
                         "\t{\n");
            
            if(null==onOperateFile)
                SearchDirectory(dirWhichContainsFiles, fileName =>
                {
                    if (!fileName.EndsWith(".meta"))
                    {
                        var name = Path.GetFileNameWithoutExtension(fileName);
                        stream.Write("\t\tpublic const string "+name+" = @\""+name+"\";\n");
                    }
                },deepSearch);
            else 
                SearchDirectory(dirWhichContainsFiles,fileName=>onOperateFile(fileName,stream),deepSearch);


            stream.Write("\t}\n" +
                         "}\n");
            stream.Flush();
            stream.Dispose();
            stream.Close();
        }

        public static void SearchDirectory(string startDirPath, Action<string> onOperateFile, bool deepSearch = false,Action<string> onoperateDir=null)
        {
            if (!Directory.Exists(startDirPath))
                throw new Exception("起始文件夹不存在");

            foreach (var info in Directory.GetFileSystemEntries(startDirPath))
            {
                var name = info;
                if (Directory.Exists(name))
                {
                    //如果是文件夹
                    onoperateDir?.Invoke(name);
                    if (deepSearch)
                    {
                        SearchDirectory(name,onOperateFile,deepSearch,onoperateDir);
                    }
                }
                else //否则就是文件
                {
                    onOperateFile?.Invoke(name);
                }
            }
        }
        

    }
}