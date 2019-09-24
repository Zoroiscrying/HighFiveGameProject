using System;
using ReadyGamerOne.Const;
using UnityEngine;

namespace ReadyGamerOne.Const
{
    [Serializable]
    public class ConstStringChooser
    {
        [SerializeField] private int selectedIndex;
        public string label = "选择字符串";
        public string StringValue => ConstStringAsset.Instance.constStrings[selectedIndex];
    }
}