using System;
using UnityEditor;

namespace ReadyGamerOne.EditorExtension
{
    [Serializable]
    public class ArgChooser
    {
        #region Static

        public static bool typeChangAble = true;
        public static float argTypeWidth = 0.4f;
        
        
        public static string GetShowTextFromSerializedProperty(SerializedProperty property)
        {
            var text = "";

            var argTypeProp = property.FindPropertyRelative("argType");

            switch (argTypeProp.enumValueIndex)
            {
                case 0://int 
                    text = property.FindPropertyRelative("IntArg").intValue.ToString();
                    break;
                case 1://float
                    text = property.FindPropertyRelative("FloatArg").floatValue.ToString();
                    break;
                case 2://string
                    text = property.FindPropertyRelative("StringArg").stringValue;
                    break;
                case 3://bool
                    text = property.FindPropertyRelative("BoolArg").boolValue.ToString();
                    break;
            }

            return text;
        }

        #endregion
        public ArgType argType;

        public bool BoolArg;
        public int IntArg;
        public float FloatArg;
        public string StringArg;

        public void SetValue(ArgChooser arg)
        {
            this.BoolArg = arg.BoolArg;
            this.IntArg = arg.IntArg;
            this.FloatArg = arg.FloatArg;
            this.StringArg = arg.StringArg;
        }
        
    }
}