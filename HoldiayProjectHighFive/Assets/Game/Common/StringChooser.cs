using System;
using System.Reflection;
using UnityEngine;

namespace Game.Common
{
    [Serializable]
    public class StringChooser
    {
        //private static Type typeToShow;
        [SerializeField] 
        private string[] values;

        [SerializeField] private int selectedIndex = 0;
        public string StringValue => values[selectedIndex];

        public StringChooser(Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            values=new string[fieldInfos.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = fieldInfos[i].GetValue(null) as string;
            }
        }
    }
}