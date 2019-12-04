using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HighFive.Model.ItemSystem.Items.Editor
{
    [CustomEditor(typeof(ItemMgr))]
    public class ItemInfoAssetEditor:UnityEditor.Editor
    {
        [MenuItem("ReadyGamerOne/Global/ShowItemInfos")]
        public static void CreateAsset()
        {
            Selection.activeInstanceID = ItemMgr.Instance.GetInstanceID();
        }

        private Vector2 detailPos;
        private ReorderableList itemList;
        private SerializedProperty itemListProp;
        private ItemMgr asset;
        private int selectedIndex=-1;
        private void OnEnable()
        {
            this.asset = target as ItemMgr;
            this.itemListProp = serializedObject.FindProperty("items");
            this.itemList = new ReorderableList(serializedObject, itemListProp,
                true, true, true, true);
            this.itemList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;

            itemList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var itemProp = itemListProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, itemProp);
            };

            itemList.onSelectCallback = (list) => this.selectedIndex = list.index;

            itemList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "物品信息");

            itemList.onAddDropdownCallback = (rect, list) =>
            {
                var menu = new GenericMenu();
                var enumIndex = -1;
                foreach (var value in EnumUtil.GetValues<ItemType>())
                {
                    enumIndex++;

                    menu.AddItem(new GUIContent(((ItemType) value).ToString()), false, OnAddUnitCallBack, enumIndex);

                }

                menu.ShowAsContext();
            };

        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            this.detailPos = GUILayout.BeginScrollView(this.detailPos ,GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            itemList.DoLayoutList();
            if (selectedIndex != -1 && selectedIndex < itemListProp.arraySize)
            {
                var prop = itemListProp.GetArrayElementAtIndex(selectedIndex);
                var rect = GUILayoutUtility.GetRect(100, EditorGUI.GetPropertyHeight(prop),
                    GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                asset.items[selectedIndex].OnDrawMoveInfo(rect,prop);
            }
            GUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnAddUnitCallBack(object obj)
        {
            var enumIndex = (int) obj;

            var index = itemListProp.arraySize;
            
            itemListProp.arraySize++;
            var triggerProp = itemListProp.GetArrayElementAtIndex(index);
            this.itemList.index = index;
            selectedIndex = index;

            triggerProp.FindPropertyRelative("type").enumValueIndex = enumIndex;

            triggerProp.FindPropertyRelative("id").intValue = asset.GetID();

            serializedObject.ApplyModifiedProperties();
            
        }
    }
    
    
    
}