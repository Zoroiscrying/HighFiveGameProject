using System;
using System.Collections.Generic;
using Game.Control.PersonSystem;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace Game.Scripts
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

        private AbstractPerson sss;
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
                        sss= new TestPerson(VARIABLE.characterInfo,VARIABLE.position);
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
//                Gizmos.DrawWireSphere(VARIABLE.position, signalSize);
                GizmosUtil.DrawSign(VARIABLE.position, VARIABLE.color, signalSize);
            }
        }
    }
}