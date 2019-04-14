using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Control.SkillSystem
{
    /// <summary>
    /// 触发器管理者类
    /// </summary>
    public static class SkillTriggerMgr
    {
        private static Dictionary<string, ISkillTriggerFactory> factoryDic = new Dictionary<string, ISkillTriggerFactory>();

        /// <summary>
        /// 注册不同种类的触发器
        /// </summary>
        /// <param name="skillType"></param>
        /// <param name="factory"></param>
        public static void RegisterTriggerFactory(string skillType, ISkillTriggerFactory factory)
        {
            if (!factoryDic.ContainsKey(skillType))
                factoryDic.Add(skillType, factory);
        }

        /// <summary>
        /// 创建触发器
        /// </summary>
        /// <param name="skillType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ISkillTrigger CreateTrigger(string args)//skillType,int id,float startTime, float lastTime,string args)
        {
            var strs = args.Split('|');
            var type = strs[0].Trim();
            if (!factoryDic.ContainsKey(type))
                throw new Exception("工厂中没有这个触发器类型"+args);
            return factoryDic[type].CreateTrigger(args);//skillType,id,startTime,lastTime,args);
        }

        /// <summary>
        /// AbstractPerson通过技能名从这里获取技能索引
        /// </summary>
        public static Dictionary<string, SkillInstance> skillInstanceDic = new Dictionary<string, SkillInstance>();


        /// <summary>
        /// 文件读取技能
        /// </summary>
        /// <param name="path"></param>
        public static void LoadSkillsFromFile(string path)
        {
            path = Application.streamingAssetsPath + "\\" + path;
            var sr = new StreamReader(path);

            bool bracket = false;
            SkillInstance skill = null;
            do
            {
                string line = sr.ReadLine();
                if (line == null)
                    break;

                line = line.Trim();

                //注释写法   //注释
                if (line.StartsWith("//") || line == "")
                    continue;

                //解析文件
                if (line.StartsWith("skill"))//技能行
                {
                    //技能行写法  skill|"技能名"
                    var strs = line.Split('|');
                    skill = new SkillInstance(strs[1].Trim());
                    skillInstanceDic.Add(strs[1].Trim(), skill);
                }
                else if (line.StartsWith("{"))//开始大括号
                {
                    bracket = true;
                }
                else if (line.StartsWith("}"))//结束大括号
                {
                    bracket = false;
                }
                else//触发器块
                {
                    //触发器写法   触发器名|开始时间|持续时间|args（以，分隔）    如：AnimationTrigger|0|0.12|BeginAnimation
                    if (skill != null && bracket == true)
                    {

                        ISkillTrigger trigger = SkillTriggerMgr.CreateTrigger(line);//strs[0].Trim(), Convert.ToInt32(strs[2].Trim()),Convert.ToSingle(strs[1].Trim()),Convert.ToSingle(strs[3].Trim()),strs[4].Trim());
                        if (trigger != null)
                        {
                            skill.AddTrigger(trigger);
                        }
                    }
                }
            } while (true);
        }

    }
}
