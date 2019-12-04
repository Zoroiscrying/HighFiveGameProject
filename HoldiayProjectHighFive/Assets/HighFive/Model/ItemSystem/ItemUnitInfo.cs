using System;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace HighFive.Model.ItemSystem
{
    public enum ItemType
    {
        Base,
        Commercial
    }
    [Serializable]
    public class ItemUnitInfo
    {

        #region Every

        [SerializeField] private int id;
        [SerializeField] private ItemType type;
        public ItemType Type => type;
        public int ID => id;

        [SerializeField] private string name;
        public string Name => name;

        [SerializeField] private int capacity;
        public int Capacity => capacity;

        [SerializeField] private ResourcesPathChooser spritePath;
        public string SpritePath => spritePath.Path;
        
        [SerializeField] private string description;
        public string Description => description;

        #endregion

        #region Commercial

        [SerializeField] private int price;
        public int Price => price;

        #endregion

        public void OnDrawMoveInfo(Rect position, SerializedProperty property)
        {
            
            #region Every

            var index = 0;
            GUIStyle style = new GUIStyle()
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
            };
            
            EditorGUI.LabelField(position.GetRectFromIndexWithHeight(ref index,20+5),"详细信息",style);
            
            EditorGUI.LabelField(position.GetRectAtIndex(index++),"类型",type.ToString());
            EditorGUI.LabelField(position.GetRectAtIndex(index++), "ID", id.ToString());
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("name"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("capacity"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("spritePath"));            
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("description"));            

            #endregion

            switch (type)
            {
                case ItemType.Base:
                    break;
                case ItemType.Commercial:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("price")); 
                    break;
            }
        }

    }
}
