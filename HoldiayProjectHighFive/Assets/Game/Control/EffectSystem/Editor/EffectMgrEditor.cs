using UnityEditor;
using UnityEditorInternal;

namespace Game.Control.EffectSystem.Editor
{
    [CustomEditor(typeof(EffectMgr))]
    public class EffectMgrEditor:UnityEditor.Editor
    {
        private SerializedProperty effectInfoMsgAssetsProp;
        private ReorderableList effectInfoMsgList;
        private EffectMgr EffectMgr;
        private void OnEnable()
        {
            this.EffectMgr=target as EffectMgr;
            this.effectInfoMsgAssetsProp = serializedObject.FindProperty("effectInfoMsgs");
            this.effectInfoMsgList=new ReorderableList(serializedObject,effectInfoMsgAssetsProp,true,true,true,true);

            this.effectInfoMsgList.elementHeight = 3 * EditorGUIUtility.singleLineHeight;
            this.effectInfoMsgList.drawElementCallback = (rect, index, a, b) =>
            {
                var prop = effectInfoMsgAssetsProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, prop);
            };
            this.effectInfoMsgList.drawHeaderCallback = (rect) =>
                EditorGUI.LabelField(rect, "效果信息");

            this.effectInfoMsgList.onAddCallback = OnAddEffectInfoMsg;
        }

        private void OnAddEffectInfoMsg(ReorderableList list)
        {
            var index = effectInfoMsgAssetsProp.arraySize;
            effectInfoMsgAssetsProp.arraySize++;
            effectInfoMsgList.index = index;
            var prop = effectInfoMsgAssetsProp.GetArrayElementAtIndex(index);
            prop.FindPropertyRelative("id").intValue = EffectMgr.GetID();
            this.serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            effectInfoMsgList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}