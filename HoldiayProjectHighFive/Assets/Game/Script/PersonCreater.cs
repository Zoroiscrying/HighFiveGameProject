using System;
using Game.Control.PersonSystem;
using UnityEngine;

namespace Game.Scripts
{
    public class PersonCreater : UnityEngine.MonoBehaviour
    {
        public BaseCharacterInfo CharacterInfo;
        public bool createOnStart = true;

        private void Awake()
        {
            if(createOnStart)
                if (CharacterInfo is PlayerInfo)
                    new Player(CharacterInfo);
                else
                {
                    new TestPerson(CharacterInfo);
                }
        }
        
    }
}