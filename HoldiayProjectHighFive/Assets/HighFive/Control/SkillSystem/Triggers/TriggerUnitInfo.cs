using System;
using HighFive.Const;
using HighFive.Control.EffectSystem;
using HighFive.Math;
using HighFive.Model.Person;
using HighFive.Model.SpriteObjSystem;
using HighFive.Others;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace HighFive.Control.SkillSystem.Triggers
{
    [Serializable]
    public enum TriggerType
    {
        Animation,
        Audio,
        Bullet,
        Dash,
        RayDamage,
        Parabloa,
        Trigger2D,
        Effect
    }
    
    [Serializable]
    public class TriggerUnitInfo
    {
        #region Every

        [SerializeField] private SkillInfoAsset skillAsset;
        public int id;
        public float startTime;
        public float lastTime;
        public TriggerType type;
        public bool enable = true;

        private IHighFivePerson self;
        #endregion

        #region Animation

        public AnimationNameChooser animationName;
        public float animationSpeed;

        #endregion

        #region Audio
        
        public StringChooser audioName = new StringChooser(typeof(AudioName));

        #endregion

        #region Bullet

        public StringChooser bulletName=new StringChooser(typeof(PrefabName));
        public int damage;
        public Vector3 dir;
        public float bulletSpeed;
        public float maxLife;

        #endregion

        #region RayDamage

        public Vector2 rayDir;
        public float rayLength;
        public float hitSpeed;
        public LayerMask rayTestLayer;
        public float shineLastTime;
        public float shineDurTime;

        #endregion

        #region Parabloa

        public enum TargetType
        {
            Vector3Offset,
            GetFromCache
        }

        public TargetType targetType;
        public float timeToTarget;
        public Vector3 offset;

        #endregion

        #region Effect

        [SerializeField] private EffectInfoAsset attackEffects;

        #endregion

        #region Trigger2D

        public enum DamageType
        {
            PlayerToEnemy,
            EnemyToPlayer
        }

        public DamageType damageType;
        public string triggerPath;
        public Vector2 HitBack;
        private float preLastTime;
        private void OnTriggerEnter(Collider2D col)
        {
            var hitPerson = col.gameObject.GetPersonInfo() as IHighFivePerson;
            if (hitPerson == null)
            {
//                Debug.Log("打击人物为空");
                return;
            }
//            else
//            {
//                Debug.Log("打击到”" + hitPerson.CharacterName);
//            }

            switch (damageType)
            {
                case DamageType.PlayerToEnemy:
                    if (hitPerson is IHighFiveCharacter)
                        break;
                    self.TryAttackSimple(hitPerson);
                    break;
                case DamageType.EnemyToPlayer:
                    if (hitPerson is IHighFiveCharacter)
                    {
                        self.TryAttackSimple(hitPerson);
                    }

                    break;
            }
        }
        
        #endregion

        public void RunTriggerUnit(IHighFivePerson self)
        {
            if (!enable)
                return;
            this.self = self;
            MainLoop.Instance.ExecuteLater(() =>
            {
//                Debug.Log("运行Trigger单元：" + this.type);
                switch (type)
                {
                    #region Effect

                    case TriggerType.Effect:
//                        Debug.Log("技能系统——TriggerType.Effect");
                        self.PlayAttackEffects(attackEffects);
                        break;                        

                    #endregion

                    #region Animation

                    case TriggerType.Animation:
                        if (self == null)
                        {
                            throw new Exception("SkillCore为空");
                        }

                        
                        var animator = GameAnimator.GetInstance(self.gameObject.GetComponent<Animator>());

                        if (animator == null)
                        {
                            throw new Exception(self.CharacterName + "没有Animator组件");
                        }

                        animator.speed = this.animationSpeed*self.AttackSpeed;


                        animator.Play(Animator.StringToHash(this.animationName.StringValue), AnimationWeight.High);
                        
                        
                        break;
                        

                    #endregion

                    #region Audio
                    
                    case TriggerType.Audio:
                        if (self.gameObject != null)
                        {
                            AudioMgr.Instance.PlayEffect(this.audioName.StringValue, self.gameObject.transform.position);
                            
                        }
                        break;

                    #endregion

                    #region Bullet

                    case TriggerType.Bullet:
                        var d = new Vector2(self.Dir * Mathf.Abs(this.dir.x), this.dir.y);
                        new DirectLineBullet(GameMath.Damage(self.Attack), d, self.position + new Vector3(0, 1, 0)
                            , self, this.bulletName.StringValue, speed:this.bulletSpeed,maxLife:this.maxLife);
                        break;                        

                    #endregion

                    #region Parabloa


                    case TriggerType.Parabloa:
                        switch (targetType)
                        {
                            case TargetType.Vector3Offset:                        
                                new ParabloaBullet(this.damage, this.bulletSpeed, this.timeToTarget, self.position + offset,
                                    self.position, self, this.bulletName.StringValue, this.maxLife);
                                break;
                            case TargetType.GetFromCache:
//                                Debug.Log($"SkillAsset【{skillAsset.skillName.StringValue}】.Vector3Cache:"+skillAsset.Vector3Cache);
                                new ParabloaBullet(this.damage, this.bulletSpeed, this.timeToTarget,skillAsset.Vector3Cache ,
                                    self.position, self, this.bulletName.StringValue, this.maxLife);
                                break;
                        }

                        break;                        

                    #endregion

                    #region RayDamage

                    case TriggerType.RayDamage:
                        //面对方向
                        var position = self.gameObject.transform.position;
                        var p = new Vector2(position.x, position.y);
                        
                        var target = this.rayLength * new Vector2(this.dir.x * self.Dir, this.dir.y);

                        //调整身高偏移
                        p += new Vector2(0, 0.1f * self.gameObject.transform.localScale.y);
                        Debug.DrawLine(p, p + target, Color.red);
                        var results = Physics2D.LinecastAll(p, p + target, this.rayTestLayer);

                        if (results.Length == 0)
                        {
                            return;
                        }

                        foreach (var result in results)
                        {
                            //对打击到的目标进行操作，添加各种效果
                            var hitPerson = result.transform.gameObject.GetPersonInfo() as IHighFivePerson;
                            if (hitPerson == null)
                            {
                                Debug.Log(result.transform.gameObject);
                                continue;
                            }
                            self.TryAttackSimple(hitPerson);
                        }
                        break;                        

                    #endregion

                    #region Trigger2D

                    case TriggerType.Trigger2D:
                        if (Mathf.Abs(preLastTime) < 0.01f)
                        {
                            preLastTime = lastTime;
                        }
                        lastTime /= self.AttackSpeed;
                        if (self.gameObject == null)
                            break;
                        var sword = self.gameObject.transform.Find(triggerPath);
                        var trigger = (sword.GetComponent<TriggerInputer>() ??
                                       sword.gameObject.AddComponent<TriggerInputer>());

                        trigger.onTriggerEnterEvent2D += this.OnTriggerEnter;
//                        Debug.Log("添加监听");
                        MainLoop.Instance.ExecuteLater(() =>
                        {
//                            Debug.Log("移除监听");
                            trigger.onTriggerEnterEvent2D -= this.OnTriggerEnter;
                        }, lastTime);
                   
                        break;                        

                    #endregion
                }                
            },this.startTime);
        }

        public void Reset()
        {
            switch (type)
            {
                case TriggerType.Trigger2D:
                    lastTime = preLastTime;
                    
                    break;
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
            EditorGUI.LabelField(position.GetRectAtIndex(index++), "ID", id.ToString());
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("enable"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("startTime"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("lastTime"));            

            #endregion

            switch (type)
            {
                #region Effect

                case TriggerType.Effect:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("attackEffects"));
                    break;

                #endregion
                
                #region Trigger2D

                    
                case TriggerType.Trigger2D:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("damageType"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("triggerPath"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("hitBack"));
                    break;

                #endregion
                
                #region Parabloa

                    
                case TriggerType.Parabloa:


                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("bulletName"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("damage"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("bulletSpeed"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("maxLife"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("timeToTarget"));                    
                    var typeProp = property.FindPropertyRelative("targetType");
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),typeProp);
                    switch (typeProp.enumValueIndex)
                    {
                        case 0:
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("offset"));
                            break;
                    }
                    break;

                #endregion
                
                #region RayDamage

                    
                case TriggerType.RayDamage:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("rayDir"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("rayLength"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("hitSpeed"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("rayTestLayer"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("shineLastTime"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("shineDurTime"));
                    break;

                #endregion
                
                #region Bullet
                case TriggerType.Bullet:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("bulletName"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("damage"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("dir"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("bulletSpeed"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("maxLife"));
                    
                    break;

                    

                #endregion
                
                #region Dash

                case TriggerType.Dash:
                    break;
                    

                #endregion
                
                #region Animation

                    
                case TriggerType.Animation:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("animationName"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("animationSpeed"));
                    break;

                #endregion
                
                #region Auido

                    
                case TriggerType.Audio:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("audioName"));
                    break;

                #endregion
            }

        }        
#endif


    }
}