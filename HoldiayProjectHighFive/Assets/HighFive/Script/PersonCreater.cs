using System;
using System.Collections.Generic;
using HighFive.Const;
using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace HighFive.Script
{
    public enum PersonType
    {
        Player,
        Boner,
        Spider,
        Boss,
        Null,
    }

    [Serializable]
    public class CharacterCreateInfo
    {
        public PersonType _personType;
        public Vector3 position;
        public Color color;
    }
    /// <summary>
    /// 用于调试控制角色生成脚本
    /// </summary>
    public class PersonCreater : UnityEngine.MonoBehaviour
    {
        public float signalSize = 1.0f;
        public List<CharacterCreateInfo> CharacterInfos=new List<CharacterCreateInfo>();

        private void Start()
        {

            if (!gameObject.activeSelf || !enabled)
                return;
            
            foreach (var VARIABLE in CharacterInfos)
            {
                switch (VARIABLE._personType)
                {
                    case PersonType.Boner:
                        Boner.GetInstance(VARIABLE.position);
                        break;
                    case PersonType.Player:
                        if (GlobalVar.UsePlayerCachePos)
                        {
                            Sworder.GetInstance(DefaultData.PlayerPos);
                        }
                        else
                        {
                            Sworder.GetInstance(VARIABLE.position);
                            GlobalVar.UsePlayerCachePos = true;
                        }
                        break;
                    case PersonType.Spider:
                        Spider.GetInstance(VARIABLE.position);
                        break;
                    case PersonType.Boss:
                        AngryBall.GetInstance(VARIABLE.position);
                        break;
                }
            }
            
        }

        private void OnDrawGizmos()
        {
            if (!gameObject.activeSelf || !enabled)
                return;
            foreach (var VARIABLE in CharacterInfos)
            {
                if(VARIABLE._personType==PersonType.Null)
                    continue;
                Gizmos.color = VARIABLE.color;
                GizmosUtil.DrawSign(VARIABLE.position, signalSize);
            }
        }
    }
}