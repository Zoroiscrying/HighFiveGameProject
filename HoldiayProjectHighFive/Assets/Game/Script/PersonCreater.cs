using System;
using System.Collections.Generic;
using Game.Control.PersonSystem;
using UnityEngine;

namespace Game.Scripts
{
    public class PersonCreater : UnityEngine.MonoBehaviour
    {
        public List<BaseCharacterInfo> CharacterInfos=new List<BaseCharacterInfo>();
        public bool createOnStart = true;

        private void Awake()
        {
            if (createOnStart)
            {
                foreach (var VARIABLE in CharacterInfos)
                {
                    if (VARIABLE is PlayerInfo)
                        new Player(VARIABLE);
                    else
                    {
                        new TestPerson(VARIABLE);
                    }                    
                }
            }

        }
        
    }
}