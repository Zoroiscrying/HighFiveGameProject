#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Reflection;
using HighFive.Const;
using HighFive.Model.Person;
using HighFive.View;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace HighFive.Control.EffectSystem.Effects
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
#pragma warning disable 649
        private static Dictionary<GameObject, ObjPoor<GameObject>> poorDic =
            new Dictionary<GameObject, ObjPoor<GameObject>>();

        private static ObjPoor<GameObject> GetPoor(GameObject prefab)
        {
            if (!poorDic.ContainsKey(prefab))
            {
                if(prefab==null)
                    throw new Exception("prefab is null ??????");
                var p = new ObjPoor<GameObject>(
                    onInit: () => Object.Instantiate(prefab),
                    onGet: obj =>
                    {
                        if(obj==null)
                             throw new Exception("wtf");
                        obj.SetActive(true);
                    },
                    onRelease: obj => obj.SetActive(false));
                poorDic.Add(prefab, p);
            }

            return poorDic[prefab];
        }

        private static GameObject GetPrefabGo(GameObject prefab, GameObject target, Vector3? localPosition = null,Vector3? localScale=null)
        {
            var pool = GetPoor(prefab);
            var partical = pool.GetObj();

            if (partical.transform.parent != target.transform)
                partical.transform.SetParent(target.transform);
            partical.transform.localPosition = localPosition ?? Vector3.zero;
//            Debug.Log(partical.transform.localPosition);
            partical.transform.localScale = localScale ?? Vector3.one;
            return partical;
        }

        public static void Clear()
        {
            foreach (var obj_pool in poorDic)
            {
                obj_pool.Value.Clear();
            }
        }
        
        [SerializeField] private EffectType type;
        [SerializeField] private EffectInfoAsset EffectInfoAsset;

        #region Audio

        [SerializeField] private StringChooser audioName=new StringChooser(typeof(AudioName));

        #endregion

        #region Shining

        [SerializeField] private float shiningDuring = 0.2f;
        [SerializeField] private Color shiningColor = Color.red;

        #endregion

        #region Partical

        [SerializeField] private GameObject particalPrefab;

        #endregion

        #region Animation

        [SerializeField] private bool causeDamage = false;
        [SerializeField] private AnimationClip clip;
        [SerializeField] private GameObject animationPrefab;
        [SerializeField] private Vector3 localPosition;
        [SerializeField] private Vector3 localScale=Vector3.one;

        #endregion

        public void Play(IEffector<AbstractPerson> ditascher, IEffector<AbstractPerson> receiver)
        {
            var dp = ditascher?.EffectPlayer as IHighFivePerson;
            var rp = receiver?.EffectPlayer as IHighFivePerson;
            switch (type)
            {
                #region Animation

                case EffectType.Animation:
                    Assert.IsTrue(dp != null);

                    var pos = new Vector3(dp.Dir * localPosition.x, localPosition.y, localPosition.z);

                    var aniObj = GetPrefabGo(animationPrefab, dp.gameObject, pos,localScale);
//                    Debug.Log("Animator");
                    
                    var ani = aniObj.GetComponent<Animator>();

                    if (causeDamage)
                    {
                        var trigger = aniObj.GetComponent<TriggerInputer>();
                        if (trigger == null)
                            trigger = aniObj.AddComponent<TriggerInputer>();
                        trigger.Clear();
                        trigger.onTriggerEnterEvent += (col) =>
                        {
                            rp = col.gameObject.GetPersonInfo() as IHighFivePerson;
                            if (rp != null)
                                dp.TryAttack(rp);
                        };

                    }
                    
                    
                    ani.Play(clip.name);
                    MainLoop.Instance.ExecuteLater(
                        () =>
                        {
                            if (aniObj)
                            {
//                                Debug.Log("回收Animation");
                                GetPoor(animationPrefab).ReleaseObj(aniObj);
                            }
                        },clip.length);

                    break;

                #endregion


                #region Partical

                case EffectType.Partical:
                    var psObj = GetPrefabGo(particalPrefab, rp.gameObject);
                    var ps = psObj.GetComponent<ParticleSystem>();
//                    Debug.Log("Partical");
                    ps.Stop();
                    ps.Play();
                    MainLoop.Instance.ExecuteUntilTrue(() =>ps==null || ps.isStopped,
                        () =>
                        {
                            if (psObj)
                            {
//                                Debug.Log("回收Partical");
                                GetPoor(particalPrefab).ReleaseObj(psObj);
                            }
                        });
                    break;

                #endregion

                #region Damage

                case EffectType.Damage:

                    //todo: 这里显示伤害数字才对
//                    dp?.TryAttack(rp);
                    new DamageNumberUI(dp.Attack, 0, 1, Color.red, rp.transform, dp.Dir);
                    break;

                #endregion

                #region Audio

                case EffectType.Audio:
                    switch (EffectInfoAsset.effectorType)
                    {
                        case EffectInfoAsset.EffectorType.Ditascher:
                            Assert.IsTrue(dp != null);
                            AudioMgr.Instance.PlayEffect(this.audioName.StringValue, dp.position);
                            break;
                        case EffectInfoAsset.EffectorType.Receiver:
                            Assert.IsTrue(rp != null);
                            AudioMgr.Instance.PlayEffect(this.audioName.StringValue, rp.position);
                            break;
                    }

                    break;

                #endregion

                #region Shining

                case EffectType.Shining:
                    Assert.IsTrue(rp != null);
                    var sr = rp.gameObject.GetComponent<SpriteRenderer>();
                    var times = rp is IHighFiveCharacter ? rp.DefaultConstTime / shiningDuring : 0.8f;
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

                    Assert.IsTrue(rp != null && dp != null);

                    if (!rp.IgnoreHitback)
                    {
                        var trans = rp.gameObject.transform;
//                        trans.position += new Vector3(dp.Dir * Mathf.Abs(dp.HitBackSpeed.x), dp.HitBackSpeed.y, 0);
                        var hitBack = new Vector2(dp.Dir * Mathf.Abs(dp.HitBackSpeed.x), dp.HitBackSpeed.y);
                        //TODO:需要更好接口
                        rp.Controller.GetComponent<Actor>().ChangeVelBasedOnHitDir(hitBack,15);
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

            EditorGUI.LabelField(position.GetRectFromIndexWithHeight(ref index, 20 + 5), "详细信息", style);

            EditorGUI.LabelField(position.GetRectAtIndex(index++), "类型", type.ToString());

            #endregion

            switch (type)
            {
                case EffectType.Animation:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("causeDamage"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("clip"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("animationPrefab"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("localPosition"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("localScale"));
                    break;
                
                case EffectType.Audio:
                    var audioNameProp = property.FindPropertyRelative("audioName");
                    //TODO:AUDIOpath
//                    InitSerializedStringArray(audioNameProp.FindPropertyRelative("values"), typeof(AudioPath));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), audioNameProp);
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
#pragma warning restore 649
    }
}