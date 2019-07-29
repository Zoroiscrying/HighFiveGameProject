using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Common
{
    [CustomPropertyDrawer(typeof(StringChooser))]
    public class StringChooseDrawer : PropertyDrawer
    {
        private List<string> values;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = property.FindPropertyRelative("selectedIndex");
            if (values == null)
            {
                values = new List<string>();
                var vs = property.FindPropertyRelative("values");
                Assert.IsTrue(vs.isArray);
                for (int i = 0; i < vs.arraySize; i++)
                {
                    values.Add(vs.GetArrayElementAtIndex(i).stringValue);
                }
            }

            //base.OnGUI(position, property, label);
            EditorGUI.BeginChangeCheck();
            //得到描述在列表中的下标
            index.intValue = EditorGUI.Popup(position, property.displayName, index.intValue, values.ToArray());
//            if (EditorGUI.EndChangeCheck())
//            {
//                if (values == null)
//                    throw new Exception("values is null");
//                var sp = property.FindPropertyRelative("selectedValue");
//                if(null==sp)
//                    throw new Exception("没有找到这个属性："+"selectedValue");
//                sp.stringValue = values[index];
//
//                //根据下标从id列表中找出技能id进行赋值
//
////                Debug.Log(property.FindPropertyRelative("selectedValue").stringValue);
//                
//            }
        }
    }
}