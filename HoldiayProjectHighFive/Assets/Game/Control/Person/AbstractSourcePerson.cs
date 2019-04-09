using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Control.Person
{
    /// <summary>
    /// 人物基类
    /// </summary>
    public abstract class AbstractSourcePerson
    {
        [HideInInspector]
        public GameObject obj;

        protected AbstractSourcePerson()
        {
        }

        protected AbstractSourcePerson(string path)
        {
            if (path == null)
                Debug.Log("预制体路径不对");
            else
                this.obj = Resources.Load<GameObject>(path);
        }
    }
}
