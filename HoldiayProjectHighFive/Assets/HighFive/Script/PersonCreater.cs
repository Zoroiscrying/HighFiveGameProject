using System;
using System.Collections.Generic;
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
    }

    [Serializable]
    public class CharacterCreateInfo
    {
        public PersonType _personType;
        public Vector3 position;
        public Color color;
    }
    public class PersonCreater : UnityEngine.MonoBehaviour
    {
        public float signalSize = 1.0f;
        public List<CharacterCreateInfo> CharacterInfos=new List<CharacterCreateInfo>();

        private void Start()
        {

            foreach (var VARIABLE in CharacterInfos)
            {
                switch (VARIABLE._personType)
                {
                    case PersonType.Boner:
                        Boner.GetInstance(VARIABLE.position);
                        break;
                    case PersonType.Player:
                        Sworder.GetInstance(VARIABLE.position);
                        break;
                    case PersonType.Spider:
                        Spider.GetInstance(VARIABLE.position);
                        break;
                }
            }
            
        }

        private void OnDrawGizmos()
        {
            foreach (var VARIABLE in CharacterInfos)
            {
//                if(VARIABLE.characterInfo==null)
//                    continue;
                Gizmos.color = VARIABLE.color;
                GizmosUtil.DrawSign(VARIABLE.position, signalSize);
            }
        }
    }
}