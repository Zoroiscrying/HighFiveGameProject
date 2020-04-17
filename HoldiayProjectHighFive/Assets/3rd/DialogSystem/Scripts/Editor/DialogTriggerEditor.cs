using ReadyGamerOne.Editor;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Scripts.Editor
{
    [CustomEditor(typeof(DialogTrigger))]
    public class DialogTriggerEditor:UnityEditor.Editor
    {
        private SerializedProperty targetDialogSystemPorp;
        private SerializedProperty dialogNameToTriggerProp;
        private SerializedProperty triggerOnlyOnce;

        private SerializedProperty triggerTypeProp;
        private SerializedProperty startTypeProp;

        private SerializedProperty sequenceRangeProp;
        private SerializedProperty disableObjProp;

        private SerializedProperty workTypeProp;
        private SerializedProperty messageToEnableThisProp;
        private SerializedProperty messageToStartProp;

        private SerializedProperty allowProgressRangeProp;
        
        
        
        
        private DialogTrigger mb;
        private void OnEnable()
        {
            this.mb = target as DialogTrigger;
            this.triggerTypeProp = serializedObject.FindProperty("triggerType");
            this.startTypeProp = serializedObject.FindProperty("startType");
            this.targetDialogSystemPorp = serializedObject.FindProperty("targetDialogSystem");
            this.dialogNameToTriggerProp=serializedObject.FindProperty("dialogNameToTrigger");
            this.triggerOnlyOnce = serializedObject.FindProperty("triggerOnlyOnce");
            this.sequenceRangeProp = serializedObject.FindProperty("sequenceRange");
            this.disableObjProp = serializedObject.FindProperty("disableThisGameObjectOnEnd");
            this.messageToStartProp = serializedObject.FindProperty("messageToStart");
            this.messageToEnableThisProp=serializedObject.FindProperty("messageToEnableThis");
            this.workTypeProp = serializedObject.FindProperty("workType");
            this.allowProgressRangeProp = serializedObject.FindProperty("allowProgressRange");


        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(targetDialogSystemPorp);
            if (mb.UsedDialogSystem == null)
                return;
            EditorUtil.InitSerializedStringArray(this.dialogNameToTriggerProp.FindPropertyRelative("values"), mb.UsedDialogSystem.GetAssetNames());

            EditorGUILayout.PropertyField(this.workTypeProp);
            switch (workTypeProp.enumValueIndex)
            {
                case 1://WorkAfterMessage
                    EditorUtil.InitSerializedStringArray(messageToEnableThisProp.FindPropertyRelative("values"),mb.UsedDialogSystem.DialogInfoAssets[0].MessageType);
                    EditorGUILayout.PropertyField(this.messageToEnableThisProp);
                    break;
                case 2://WorkAtProgressRange
                    EditorGUILayout.PropertyField(this.allowProgressRangeProp);
                    break;
            }
            
            EditorGUILayout.PropertyField(this.startTypeProp);

            switch (startTypeProp.enumValueIndex)
            {
                case 3://MessagetToStart
                    EditorUtil.InitSerializedStringArray(messageToStartProp.FindPropertyRelative("values"),mb.UsedDialogSystem.DialogInfoAssets[0].MessageType);
                    EditorGUILayout.PropertyField(messageToStartProp);
                    break;
            }
            EditorGUILayout.PropertyField(this.triggerTypeProp);

            if (startTypeProp.enumValueIndex == 0 &&
                (triggerTypeProp.enumValueIndex == 1
                 ||startTypeProp.enumValueIndex==3
                 ||workTypeProp.enumValueIndex==1))
                EditorGUILayout.HelpBox("设置不合理", MessageType.Warning);
            

            switch (this.triggerTypeProp.enumValueIndex)
            {
                case 0:// Single
                EditorGUILayout.PropertyField(this.dialogNameToTriggerProp);
                    break;
                case 1://Sequence
                    EditorGUILayout.PropertyField(this.sequenceRangeProp);
                    var x = this.sequenceRangeProp.FindPropertyRelative("x").intValue;
                    var y = this.sequenceRangeProp.FindPropertyRelative("y").intValue;
                    if (x < 0 || x >= mb.UsedDialogSystem.DialogInfoAssets.Count || y < 0 ||
                        y >= mb.UsedDialogSystem.DialogInfoAssets.Count)
                        EditorGUILayout.HelpBox("设置错误，索引越界", MessageType.Error);
                    break;
                case 2:// Interact
                    break;
            }
            
            var collider2D = mb.gameObject.GetComponent<Collider2D>();
            if((this.startTypeProp.enumValueIndex==1
                ||this.startTypeProp.enumValueIndex==2)
                && !(collider2D && collider2D.isTrigger))
            {
                EditorGUILayout.HelpBox("使用2D触发器需要给此物体添加触发器！", MessageType.Error);
            }

            if (this.startTypeProp.enumValueIndex == 4 //点击触发
                && this.mb.gameObject.layer != LayerMask.NameToLayer("UI") //不是UI
                && !collider2D)
            {
                
                EditorGUILayout.HelpBox("使用2D触发器需要给此物体添加触发器或碰撞体", MessageType.Error);
            }
            
            EditorGUILayout.PropertyField(triggerOnlyOnce);

            if (triggerOnlyOnce.boolValue)
            {
                EditorGUILayout.PropertyField(disableObjProp);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
