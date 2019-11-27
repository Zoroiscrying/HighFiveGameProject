using System;
using System.Collections.Generic;

namespace Test
{
    [Serializable]
    public class BaseUnit
    {
        public int base_Int=2;
    }
    public class Test : UnityEngine.MonoBehaviour
    {
        public List<BaseUnit> units=new List<BaseUnit>();
    }
}