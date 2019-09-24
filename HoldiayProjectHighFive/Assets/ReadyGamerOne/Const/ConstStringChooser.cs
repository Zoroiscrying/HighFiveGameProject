using System;
using ReadyGamerOne.Const;
using UnityEngine;

namespace ReadyGamerOne.Const
{
    [Serializable]
    public class ConstStringChooser
    {
        [SerializeField] private int selectedIndex;
        public string StringValue => ConstStringAsset.Instance.constStrings[selectedIndex];
    }
}