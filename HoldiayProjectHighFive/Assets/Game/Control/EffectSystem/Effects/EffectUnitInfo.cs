using System;
using System.Reflection;
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
        
        [SerializeField] private EffectType type;
        [SerializeField] private EffectInfoAsset EffectInfoAsset;

        #region Audio

        
        [SerializeField] private StringChooser audioName;

        #endregion

        #region Shining

        [SerializeField] private float shiningDuring = 0.2f;
        [SerializeField] private Color shiningColor=Color.red;
        
        
        #endregion
        
        public void Play(IEffector<AbstractPerson> ditascher, IEffector<AbstractPerson> receiver)
        {        
            var dp = ditascher?.EffectPlayer;
            var rp = receiver?.EffectPlayer;
            switch (type)
            {
                #region Damage

                case EffectType.Damage:
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
                            AudioMgr.Instance.PlayEffect(this.audioName.StringValue,dp.Pos);
                            break;
                        case EffectInfoAsset.EffectorType.Receiver: 
                            AudioMgr.Instance.PlayEffect(this.audioName.StringValue,rp.Pos);
                            break;
                    }
                    break;                    

                #endregion

                #region Shining

                case EffectType.Shining:
                    var sr = rp.obj.GetComponent<SpriteRenderer>();
                    var times = dp is Player ? dp.DefaultConstTime / shiningDuring : 0.8f;
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
//                     if(audioNameProp==null)
                         InitSerializedStringArray(audioNameProp.FindPropertyRelative("values"),typeof(AudioName));
                     EditorGUI.PropertyField(position.GetRectAtIndex(index++),audioNameProp);
                     break;
                 case EffectType.Shining:
                     EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                         property.FindPropertyRelative("shiningColor"));
                     EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                         property.FindPropertyRelative("shiningDuring"));
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