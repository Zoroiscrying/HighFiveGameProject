using Game.Common;
using Game.Const;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model
{
    /// <summary>
    /// 战斗单位数值系统
    /// </summary>
    public class PersonData : Singleton<PersonData>
    {

        public List<string> rankArgs = new List<string>();
        public Dictionary<string, string> enenyArgs = new Dictionary<string, string>();

        public PersonData()
        {
            LoadPersonArgsFromFile(FilePath.PersonArgsFilePath);
        }

        private void LoadPersonArgsFromFile(string path)
        {
            bool rank = false;
            bool enemy = false;
            bool backet = false;
            path = Application.streamingAssetsPath + "\\" + path;
            var sr = new StreamReader(path);
            do
            {
                string line = sr.ReadLine();
                if (line == null)
                    break;

                line = line.Trim();

                //                Debug.Log(line);
                //注释写法   //注释
                if (line.StartsWith("//") || line == "")
                    continue;

                if (line.StartsWith("Rank"))
                {
                    rank = true;
                    //                    Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);

                    continue;
                }

                if (line.StartsWith("{"))
                {
                    backet = true;
                    //                    Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);

                    continue;
                }

                if (line.StartsWith("Enemy"))
                {
                    enemy = true;
                    //                    Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);

                    continue;
                }
                if (line.EndsWith("}"))
                {
                    backet = false;
                    if (enemy)
                        enemy = false;
                    if (rank)
                        rank = false;
                    //                    Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);

                    continue;
                }

                //                Debug.Log("Enemy: " + enemy + " Rank: " + rank + " Backet: " + backet);
                if (backet)
                {
                    if (rank)
                    {
                        rankArgs.Add(line);
                    }

                    if (enemy)
                    {
                        var strs = line.Split('|');
                        enenyArgs.Add(strs[0].Trim(), string.Join("|", strs, 1, strs.Length - 1));
                    }
                }


            } while (true);
        }
    }
}
