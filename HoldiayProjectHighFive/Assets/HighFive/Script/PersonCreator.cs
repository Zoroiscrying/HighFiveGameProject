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
        Defender,
    }

    [Serializable]
    public class CharacterCreateInfo
    {
        public bool enable = false;
        public PersonType _personType;
        public Vector3 position;
        public Color color;
    }
    /// <summary>
    /// 用于调试控制角色生成脚本
    /// </summary>
    public class PersonCreator : UnityEngine.MonoBehaviour
    {
        public float signalSize = 1.0f;
        public List<CharacterCreateInfo> createInfos=new List<CharacterCreateInfo>();

        private void Start()
        {

            if (!gameObject.activeSelf || !enabled)
                return;
            
            foreach (var createInfo in createInfos)
            {
                if(!createInfo.enable)
                    continue;
                switch (createInfo._personType)
                {
                    case PersonType.Boner:
                        Boner.GetInstance(createInfo.position);
                        break;
                    case PersonType.Player:
                        if (GlobalVar.UsePlayerCachePos)
                        {
                            Sworder.GetInstance(DefaultData.PlayerPos);
                        }
                        else
                        {
                            Sworder.GetInstance(createInfo.position);
                            GlobalVar.UsePlayerCachePos = true;
                        }
                        break;
                    case PersonType.Spider:
                        Spider.GetInstance(createInfo.position);
                        break;
                    case PersonType.Boss:
                        AngryBall.GetInstance(createInfo.position);
                        break;
                    case PersonType.Defender:
                        Defender.GetInstance(createInfo.position);
                        break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!gameObject.activeSelf || !enabled)
                return;
            foreach (var VARIABLE in createInfos)
            {
                if(VARIABLE.enable==false)
                    continue;
                Gizmos.color = VARIABLE.color;
                GizmosUtil.DrawSign(VARIABLE.position, signalSize);
            }
        }
    }
}