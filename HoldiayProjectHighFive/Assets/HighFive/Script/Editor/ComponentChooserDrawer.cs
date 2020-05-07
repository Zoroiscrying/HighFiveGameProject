using System;
using System.Collections.Generic;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HighFive.Script
{
    
    [CustomPropertyDrawer(typeof(SceneTriggerChooser))]
    public class ComponentChooserDrawer:PropertyDrawer
    {
        private List<Object> objects = new List<Object>();
        private string[] names;
        private Vector3[] positions;
        private string scenePath;

        private Object scene;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0f;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("scene"));
            height += EditorGUIUtility.singleLineHeight;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneProp = property.FindPropertyRelative("scene");
            var scenePathProp = sceneProp.FindPropertyRelative("scenePath");
            var sceneAssetProp = sceneProp.FindPropertyRelative("sceneAsset");
            var positionProp = property.FindPropertyRelative("targetPosition");
            var index = 0;
//            var guidProp = property.FindPropertyRelative("guid");
////            var guid = guidProp.stringValue;
//            var sceneAssetProp = property.FindPropertyRelative("sceneAsset");
//            var sceneNameProp = property.FindPropertyRelative("sceneName");
//            var sa=sceneAssetProp.objectReferenceValue as SceneAsset;
//            if (sa == null && !string.IsNullOrEmpty(guidProp.stringValue))
//            {
//                var path = AssetDatabase.GUIDToAssetPath(guidProp.stringValue);
//                if (string.IsNullOrEmpty(path))
//                {
//                    Debug.LogError($"guid获取路径异常:{path}");
//                    return;
//                }
//                sa = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
//                if (sa == null)
//                {
//                    Debug.LogError($"guid获取路径异常:{path}, guid:{guidProp.stringValue}");
//                    return;
//                }
//            }
////            else if (sa && string.IsNullOrEmpty(scenePath))
////            {
////                var tempGuid = "";
////                var tempLocalId = 0L;
////                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sa, out tempGuid, out tempLocalId))
////                {
////                    guid = tempGuid;
////                    guidProp.stringValue = tempGuid;
////                    scenePath = AssetDatabase.GUIDToAssetPath(guid);
////                    sceneNameProp.stringValue = sa.name;
////                }
////                else
////                {
////                    Debug.LogError($"获取guid异常:{sa.name}");
////                    return;
////                }
////            }
//            EditorGUI.LabelField(position.GetLeft(SceneTriggerChooser.LabelWidth), property.displayName);
//            
//            EditorGUI.BeginChangeCheck();
//            sceneAssetProp.objectReferenceValue = EditorGUI.ObjectField( 
//                position.GetLeft(SceneTriggerChooser.LabelWidth + SceneTriggerChooser.ObjectFieldWidth,
//                    SceneTriggerChooser.LabelWidth+0.015f), 
//                sceneAssetProp.objectReferenceValue, 
//                typeof(SceneAsset),
//                false) as SceneAsset;
//            sa=(SceneAsset) sceneAssetProp.objectReferenceValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(
                position.GetRectFromIndexWithHeight(ref index, EditorGUI.GetPropertyHeight(sceneProp)), sceneProp);
            
            index = string.IsNullOrEmpty(scenePathProp.stringValue) ? 2 : 3;
            
            var refresh = false;
            if (EditorGUI.EndChangeCheck())
            {
                if (scene != sceneAssetProp.objectReferenceValue)
                {
                    scene = sceneAssetProp.objectReferenceValue;
                    if (scene)
                        scenePathProp.stringValue = AssetDatabase.GetAssetPath(scene);
                }
                
                if (string.IsNullOrEmpty(scenePathProp.stringValue))
                {// scene is none
                    positionProp.vector3Value=Vector3.zero;
                    names=null;
                    positions=null;
                    EditorGUI.HelpBox(position.GetRectFromIndex(index), "请选择Scene", MessageType.Error);
                    return;
                }
                
                Debug.Log($"scenePath:{scenePath}, scenePathProp.stringValue:{scenePathProp.stringValue}");
                
                if(scenePath!=scenePathProp.stringValue)
                {
                    Debug.Log("Refresh");
                    refresh = true;
                }
                
//                if (sa != null)
//                {
//                    var tempGuid = "";
//                    var tempLocalId = 0L;
//                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sa, out tempGuid, out tempLocalId))
//                    {
//                        guidProp.stringValue = tempGuid;
//                        sceneNameProp.stringValue = sa.name;
//                    }
//                }
//                else
//                {
//                    guidProp.stringValue = null;
//                    sceneNameProp.stringValue = null;
//                }      
            }

            
            
            if (string.IsNullOrEmpty(scenePathProp.stringValue))
            {// scene is none
                positionProp.vector3Value=Vector3.zero;
                names=null;
                positions=null;
                EditorGUI.HelpBox(position.GetRectFromIndex(index), "请选择Scene", MessageType.Error);
                return;
            }
            
            
            var selectedIndexProp = property.FindPropertyRelative("selectedIndex");
            
//            var enumWidth = 1 - SceneTriggerChooser.LabelWidth - SceneTriggerChooser.ObjectFieldWidth;

//            if (sa == null)
//            {
//                EditorGUI.HelpBox(position.GetRight(enumWidth), "请选择Scene", MessageType.Error);
//                return;
//            }

//            if (string.IsNullOrEmpty(sa.name))
//            {
//                EditorGUI.HelpBox(position.GetRight(enumWidth), "sceneName is null", MessageType.Error);
//                return;
//            }


            if (Application.isPlaying)
                return;

            if (refresh||names==null)
            {
                
                var scene = EditorSceneManager.OpenScene(scenePathProp.stringValue, OpenSceneMode.Additive);
               if (!scene.IsValid())
               {
                   EditorGUI.HelpBox(position.GetRectFromIndex(index),$"Scene is InValid", MessageType.Error);
                   EditorSceneManager.UnloadSceneAsync(scene);
                   return;
               }

               var typeName = property.FindPropertyRelative("typeName").stringValue;
               if (string.IsNullOrEmpty(typeName))
               {
                   EditorGUI.HelpBox(position.GetRectFromIndex(index),$"typeName is empty or null", MessageType.Error);
                   EditorSceneManager.UnloadSceneAsync(scene);
                   return;
               }

               var targetType = EditorUtil.GetType(typeName);
               if (null == targetType)
               {
                   EditorGUI.HelpBox(position.GetRectFromIndex(index), $"failed to get type info : {typeName}", MessageType.Error);
                   EditorSceneManager.UnloadSceneAsync(scene);
                   return;
               }

               if (!scene.isLoaded)
                   return;
               objects.Clear();
               foreach (var go in scene.GetRootGameObjects())
               {
                   objects.AddRange(go.GetComponentsInChildren(targetType));
               }

               if (objects.Count == 0)
               {
                   EditorGUI.HelpBox(position.GetRectFromIndex(index),"该Scene中没有SceneTrigger", MessageType.Error);
                   EditorSceneManager.UnloadSceneAsync(scene);
                   return;
               }
               
               names = new string[objects.Count];
               positions=new Vector3[objects.Count];
               for (var i = 0; i < names.Length; i++)
               {
                   names[i] = objects[i].name;
                   positions[i] = ((Component) objects[i]).transform.position;
               }

               if (selectedIndexProp.intValue >= names.Length)
               {
                   Debug.LogWarning($"selectIndex过大，已经取0");
                   selectedIndexProp.intValue = 0;
               }
               
               EditorSceneManager.UnloadSceneAsync(scene);
            }

//            if (positions == null)
//                return;
            EditorGUI.BeginChangeCheck();
            selectedIndexProp.intValue = EditorGUI.Popup(position.GetRectFromIndex(index), selectedIndexProp.intValue, names);
            positionProp.vector3Value = positions[selectedIndexProp.intValue];
            if(EditorGUI.EndChangeCheck())
                Debug.Log($"choose: {names[selectedIndexProp.intValue]}");

            scenePath = scenePathProp.stringValue;
        }
    }
}