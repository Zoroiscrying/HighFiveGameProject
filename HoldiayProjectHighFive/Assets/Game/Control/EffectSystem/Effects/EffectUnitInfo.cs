using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using Game.Const;
using Game.Control.PersonSystem;
using Game.Math;
using Game.View;
using ReadyGamerOne.Common;
using ReadyGamerOne.Const;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
#if UNITY_EDITOR
    
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Control.EffectSystem
{     
    [Serializable]
     public enum EffectType
     {
         Audio,
         Shake,
         Partical,
         Shining,
         HitBack,
         Animation,
         Damage
     }
    /// <summary>
    /// 抽象特效单元
    /// 针对实现IEffector<AbstractPerson>接口的特效持有者
    /// </summary>
    [Serializable]
    public sealed class EffectUnitInfo
    {
        private static Dictionary<GameObject, ObjPoor<GameObject>> poorDic =
            new Dictionary<GameObject, ObjPoor<GameObject>>();
        private static ObjPoor<GameObject> GetPoor(GameObject prefab)
        {
            if (!poorDic.ContainsKey(prefab))
            {
                var p=new ObjPoor<GameObject>(
                    onInit:()=>GameObject.Instantiate(prefab),
                    onGet:obj=>obj.SetActive(true),
                    onRelease:obj=>obj.SetActive(false));
                poorDic.Add(prefab,p);
            }

            return poorDic[prefab];
        }
        private static GameObject GetParticalGo(GameObject prefab, GameObject target)
        {
            var partical = GetPoor(prefab).GetObj();
            if(partical.transform.parent!=target.transform)
                partical.transform.SetParent(target.transform);
            partical.transform.localPosition=Vector3.zero;
            return partical;
        }




        [SerializeField] private EffectType type;
        [SerializeField] private EffectInfoAsset EffectInfoAsset;

        #region Audio

        
        [SerializeField] private StringChooser audioName;

        #endregion

        #region Shining

        [SerializeField] private float shiningDuring = 0.2f;
        [SerializeField] private Color shiningColor=Color.red;
        
        
        #endregion

        #region Partical


        [SerializeField] private GameObject particalPrefab;

        #endregion
        
        public void Play(IEffector<AbstractPerson> ditascher, IEffector<AbstractPerson> receiver)
        {        
            var dp = ditascher?.EffectPlayer;
            var rp = receiver?.EffectPlayer;
            switch (type)
            {
                #region Partical
                case EffectType.Partical:
                    var psObj = GetParticalGo(particalPrefab, rp.obj);
                    var ps = psObj.GetComponent<ParticleSystem>();
                    Debug.Log("播放");
                    ps.Play();
                    MainLoop.Instance.ExecuteUntilTrue(() => ps.isStopped,
                        () =>
                        {
                            if (psObj)
                            {
                                GetPoor(particalPrefab).ReleaseObj(psObj);
                            }
                        });
                    break;
                        

                #endregion
                
                #region Damage

                case EffectType.Damage:
                    
                    Assert.IsTrue(dp!=null && rp!=null);
                    if (rp.IsConst)
                        break;
                    
                    

                    var damage = GameMath.Damage(dp, rp);
                    
                    //伤害数字
                    new NumberTipUI(damage,0.3f * rp.Scanler, 24, Color.red, rp.obj.transform, rp.Dir, 1);
                    
                    CEventCenter.BroadMessage(Message.M_BloodChange(rp.obj), -damage);
                    rp.OnTakeDamage(damage);
                    if (!(rp is Player))
                    {
                        CEventCenter.BroadMessage(Message.M_ChangeSmallLevel, damage);
                    }
                    break;
                
                #endregion
                
                #region Audio

                case EffectType.Audio:
                    switch (EffectInfoAsset.effectorType)
                    {
                        case EffectInfoAsset.EffectorType.Ditascher:
                            Assert.IsTrue(dp != null);
                            AudioMgr.Instance.PlayEffect(this.audioName.StringValue,dp.Pos);
                            break;
                        case EffectInfoAsset.EffectorType.Receiver: 
                            Assert.IsTrue(rp!=null);
                            AudioMgr.Instance.PlayEffect(this.audioName.StringValue,rp.Pos);
                            break;
                    }
                    break;                    

                #endregion

                #region Shining

                case EffectType.Shining:
                    Assert.IsTrue(rp!=null);
                    var sr = rp.obj.GetComponent<SpriteRenderer>();
                    var times = rp is Player ? rp.DefaultConstTime / shiningDuring : 0.8f;
                    MainLoop.Instance.ExecuteEverySeconds(
                        render =>
                        {
                            if (sr == null)
                                return;
                            sr.color = sr.color == shiningColor ? Color.white : shiningColor;
                        }, times, shiningDuring, sr,
                        render =>
                        {
                            if (sr == null)
                                return;
                            sr.color = Color.white;
                        });
                    break;                    

                #endregion
                
                #region Hitback
                case EffectType.HitBack:

                    Assert.IsTrue(rp!=null&& dp!=null);
                    
                    if (!rp.IgnoreHitback)
                    {
                        var trans = rp.obj.transform;
                        trans.position += new Vector3(dp.Dir*Mathf.Abs(dp.HitBackSpeed.x),dp.HitBackSpeed.y,0);
                    }
                    break;



                #endregion
                
            }
            
        }
#if UNITY_EDITOR
         public void OnDrawMoreInfo(SerializedProperty property, Rect position)
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
 
             #endregion

             switch (type)
             {
                 case EffectType.Audio:
                     var audioNameProp = property.FindPropertyRelative("audioName");
                         InitSerializedStringArray(audioNameProp.FindPropertyRelative("values"),typeof(AudioName));
                     EditorGUI.PropertyField(position.GetRectAtIndex(index++),audioNameProp);
                     break;
                 case EffectType.Shining:
                     EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                         property.FindPropertyRelative("shiningColor"));
                     EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                         property.FindPropertyRelative("shiningDuring"));
                     break;
                 case EffectType.Partical:
                     EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                         property.FindPropertyRelative("particalPrefab"));
                     break;
             }
         }       
         
         
         
         public static void InitSerializedStringArray(SerializedProperty arrProp, Type type)
         {
             arrProp.arraySize = 0;
             FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
             for (int i = 0; i < fieldInfos.Length; i++)
             {
                 arrProp.InsertArrayElementAtIndex(i);
                 arrProp.GetArrayElementAtIndex(i).stringValue = fieldInfos[i].GetValue(null) as string;
             }
         }
         #endif
    }
}