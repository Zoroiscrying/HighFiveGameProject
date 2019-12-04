using System;
using System.Collections.Generic;
using HighFive.Control.PersonSystem.Persons;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace HighFive.Script
{
    [Serializable]
    public class CharacterCreateInfo
    {
        public BaseCharacterInfo characterInfo;
        public Vector3 position;
        public Color color;
    }
    public class PersonCreater : UnityEngine.MonoBehaviour
    {
        public bool createOnStart = true;
        public float signalSize = 1.0f;
        public List<CharacterCreateInfo> CharacterInfos=new List<CharacterCreateInfo>();

        private void Awake()
        {
            if (createOnStart)
            {
                foreach (var VARIABLE in CharacterInfos)
                {
                    if (VARIABLE.characterInfo is PlayerInfo)
                    {
                        new Player(VARIABLE.characterInfo,VARIABLE.position);
                    }
                    else
                    {
                        new TestPerson(VARIABLE.characterInfo,VARIABLE.position);
                    }                    
                }
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var VARIABLE in CharacterInfos)
            {
                if(VARIABLE.characterInfo==null)
                    continue;
                Gizmos.color = VARIABLE.color;
                GizmosUtil.DrawSign(VARIABLE.position, signalSize);
            }
        }
    }
}