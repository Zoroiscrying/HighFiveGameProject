using System;
using System.Reflection;
using DG.Tweening;
using Game.Scripts;
using HighFive.Const;
using HighFive.Control.EffectSystem;
using HighFive.Math;
using HighFive.Model;
using HighFive.Model.Person;
using HighFive.Model.SpriteObjSystem;
using HighFive.Others;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
#endif

namespace HighFive.Control.SkillSystem.Triggers
{
    [Serializable]
    public enum TriggerType
    {
        Animation,
        Audio,
        SingleBullet,
        CurvedBullet,
        Dash,
        RayDamage,
        Trigger2D,
        Effect,
        LightSword,
    }
    
    [Serializable]
    public class TriggerUnitInfo
    {
        
#pragma warning disable 649
        
        #region Every

        [SerializeField] private SkillInfoAsset skillAsset;
        public int id;
        public int workTimes = 1;
        public float startTime;
        public float lastTime;
        public TriggerType type;
        public bool enable = true;

        private IHighFivePerson self;
        #endregion
        
        #region 共用

        #region 角度

        public float degree;       
        public bool randomDegree;
        public Vector2 degreeRange;
        
        private float GetSuitableDegree(IHighFivePerson self)
        {
            if (!randomDegree)
            {
                var ans = degree;
                if(locateType==LocateType.LocateByBossArea)
                    switch (sideType)
                    {
                        case SideType.Left:
                            ans -= 90;
                            break;
                        case SideType.Right:
                            ans += 90;
                            break;
                        case SideType.Top:
                            ans += 180;
                            break;
                    }
                return ans;
            }
            
            
            var tempRange = degreeRange - 90 * Vector2.one;
            if(locateType==LocateType.LocateByBossArea)
                switch (sideType)
                {
                    case SideType.Left:
                        tempRange -= 90 * Vector2.one;
                        break;
                    case SideType.Right:
                        tempRange += 90 * Vector2.one;
                        break;
                    case SideType.Top:
                        tempRange += 180 * Vector2.one;
                        break;
                }
            return Random.Range(tempRange.x, tempRange.y);
        }        

        #endregion


        #region 定位

        public enum LocateType
        {
            LocateByOffset,
            LocateByBossArea,
        }

        public LocateType locateType;
        
        public Vector3 offset;
        public bool randomPos;
        public float locateInterpolation;
        public Vector2 interpolationRange;        
        public enum SideType
        {
             Top,
             Left,
             Right,
             Bottom,
        }

        public SideType sideType;
        
        private Vector3 GetSuitablePosition(IHighFivePerson self)
        {
            switch (locateType)
            {
                case LocateType.LocateByOffset:
                    return self.position + offset;
                case LocateType.LocateByBossArea:
                    var interpolation = randomPos
                        ? Random.Range(interpolationRange.x, interpolationRange.y)
                        : locateInterpolation;
                    Transform a=null, b=null;
                    switch (sideType)
                    {
                        case SideType.Bottom:
                            a = SceneUtil.Instance.bossLB;
                            b = SceneUtil.Instance.bossRB;
                            break;
                        case SideType.Left:
                            a = SceneUtil.Instance.bossLB;
                            b = SceneUtil.Instance.bossLT;
                            break;
                        case SideType.Top:
                            a = SceneUtil.Instance.bossLT;
                            b = SceneUtil.Instance.bossRT;
                            break;
                        case SideType.Right:
                            a = SceneUtil.Instance.bossRB;
                            b = SceneUtil.Instance.bossRT;
                            break;
                    }

                    return Vector3.Lerp(a.position, b.position, interpolation);
            }
            
            throw new Exception($"Unexpected locateType:{locateType}");
        }        
        
        #endregion

        #endregion
        
        #region Animation

        public AnimationNameChooser animationName;
        public float animationSpeed;

        #endregion

        #region Audio
        
        public StringChooser audioName = new StringChooser(typeof(AudioName));

        #endregion
        
        #region SingleBullet

        public StringChooser bulletName=new StringChooser(typeof(PrefabName));
        public float damageScale=1;
        public float speed=5;
        public float gravityScale = 0;
        public float maxLife = 3.0f;
        
        #endregion

        #region CurvedBullet

        public AnimationCurve aniX;
        public AnimationCurve aniY;
        public bool reverseX = false;
        public bool reverseY = false;

        #endregion

        #region RayDamage

        public Vector2 rayDir;
        public float rayLength;
        public float hitSpeed;
        public LayerMask rayTestLayer;
        public float shineLastTime;
        public float shineDurTime;

        #endregion

        #region Effect

        [SerializeField] private EffectInfoAsset attackEffects;

        #endregion

        #region LightSword

        public StringChooser swordPrefabName=new StringChooser(typeof(PrefabName));
        public AnchorMode anchorMode;
        public Vector2 targetSize;
        public Vector2 startSize;
        public float delayTime;
        public float widenTime;
        public Ease easeType;




        #endregion
        

        

        #region Trigger2D

        public enum DamageType
        {
            PlayerToEnemy,
            EnemyToPlayer
        }

        public DamageType damageType;
        public TransformPathChooser triggerPath;
        private float preLastTime;
        private void OnTriggerEnter(Collider2D col)
        {
            var hitPerson = col.gameObject.GetPersonInfo() as IHighFivePerson;
            if (hitPerson == null)
            {
//                Debug.Log($"打击人物为空:{col.name}");
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
                    self.TryAttack(hitPerson);
                    break;
                case DamageType.EnemyToPlayer:
                    if (hitPerson is IHighFiveCharacter)
                    {
                        self.TryAttack(hitPerson);
                    }

                    break;
            }
        }
        
        #endregion

        public void RunTriggerUnit(IHighFivePerson self,params object[] args)
        {
            if (!enable)
                return;
            this.self = self;
            var tempDegree = GetSuitableDegree(self);
            var dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * tempDegree), Mathf.Sin(Mathf.Deg2Rad * tempDegree));
            MainLoop.Instance.ExecuteLater(() =>
            {
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

                    #region SingleBullet

                    case TriggerType.SingleBullet:

                        var bulletObj = ResourceMgr.InstantiateGameObject(bulletName.StringValue,
                            GetSuitablePosition(self));

                        var bullet = bulletObj.GetComponent<SingleBullet>();
                        if(!bullet)
                            throw new Exception($"{bulletObj.name}没有DirectBullet组建");
                        bullet.initialSpeed = speed * new Vector2(self.Dir * dir.x, dir.y).normalized;
                        bullet.damageScale = damageScale;
                        bullet.gravityScale = gravityScale;
                        bullet.maxLife = maxLife;
                        bullet.Init(self);
                        break;                        

                    #endregion

                    #region CurvedBullet

                    case TriggerType.CurvedBullet:
                        var curveBulletObj = ResourceMgr.InstantiateGameObject(bulletName.StringValue,
                            self.position + new Vector3(0, 0.3f, 0));

                        var curveBullet = curveBulletObj.GetComponent<CurvedBullet>();
                        if(!curveBullet)
                            throw new Exception($"{curveBulletObj.name}没有curveBulletObj组建");
                        curveBullet.aniX = aniX;
                        curveBullet.aniY = aniY;
                        curveBullet.xDir = reverseX ? -self.Dir : self.Dir;
                        curveBullet.yDir = reverseY ? -1 : 1;
                        curveBullet.damageScale = damageScale;
                        curveBullet.gravityScale = gravityScale;
                        curveBullet.maxLife = maxLife;
                        curveBullet.Init(self);
                        break;
                        

                    #endregion

                    #region RayDamage

                    case TriggerType.RayDamage:
                        //面对方向
                        var position = self.gameObject.transform.position;
                        var p = new Vector2(position.x, position.y);
                        
                        var target = this.rayLength * new Vector2(dir.x * self.Dir, dir.y);

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
                            self.TryAttack(hitPerson);
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
                        var sword = self.gameObject.transform.Find(triggerPath.Path);
                        var trigger = sword.GetOrAddComponent<TriggerInputer>();

                        trigger.onTriggerEnterEvent2D += this.OnTriggerEnter;
//                        Debug.Log("添加监听");
                        MainLoop.Instance.ExecuteLater(() =>
                        {
//                            Debug.Log("移除监听");
                            trigger.onTriggerEnterEvent2D -= this.OnTriggerEnter;
                        }, lastTime);
                   
                        break;                        

                    #endregion

                    #region LightSword

                    case TriggerType.LightSword:
                        var swordObj = ResourceMgr.InstantiateGameObject(swordPrefabName.StringValue);
                        
                        var lightSword = swordObj.GetComponent<LightSword>();
                        if(!lightSword)
                            throw new Exception($"{swordObj.name}没有LightSword组建");

                        lightSword.anchorMode = anchorMode;
                        lightSword.damageScale = damageScale;
                        lightSword.delayTime = delayTime;
                        lightSword.easeType = easeType;
                        lightSword.startSize = startSize;
                        lightSword.targetSize = targetSize;
                        lightSword.widenTime = widenTime;
                        lightSword.HandlePosition = GetSuitablePosition(self);
                        lightSword.RotateDegree = GetSuitableDegree(self);
                        lightSword.Init(self);

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
            var workTimeProp = property.FindPropertyRelative("workTimes");
            if (workTimeProp.intValue == 0)
                workTimeProp.intValue = 1;
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), workTimeProp);
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("startTime"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("lastTime"));            

            #endregion


            SerializedProperty randomDegreeProp = null;
            SerializedProperty lightTypeProp = null;
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

                    break;

                #endregion
                
                #region RayDamage

                    
                case TriggerType.RayDamage:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("degree"));
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
                
                #region SingleBullet
                case TriggerType.SingleBullet:
                    var bulletProp = property.FindPropertyRelative("bulletName");
                    EditorUtil.InitSerializedStringArray(bulletProp.FindPropertyRelative("values"), typeof(PrefabName));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),bulletProp);
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("damageScale"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("gravityScale"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("maxLife"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("speed"));
                    

                    lightTypeProp = property.FindPropertyRelative("locateType");
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), lightTypeProp);

                    switch (EnumUtil.GetEnumValue<LocateType>(lightTypeProp.enumValueIndex))
                    {
                        case (int)LocateType.LocateByOffset:
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("offset"));
                            break;
                        case (int)LocateType.LocateByBossArea:
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("sideType"));

                            var randomPosProp = property.FindPropertyRelative("randomPos");
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("randomPos"));

                            if (randomPosProp.boolValue)
                                EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                                    property.FindPropertyRelative("interpolationRange"));
                            else
                            {
                                var interpProp = property.FindPropertyRelative("locateInterpolation");
                                EditorGUI.Slider(position.GetRectAtIndex(index++), interpProp.displayName,
                                    interpProp.floatValue, 0, 1);
                            }
                            break;
                    }

                    randomDegreeProp = property.FindPropertyRelative("randomDegree");
                            
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), randomDegreeProp);
                    if(randomDegreeProp.boolValue)
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("degreeRange"));
                    else 
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("degree"));

                    
                    break;
                #endregion
                
                #region LightSword

                case TriggerType.LightSword:
                    var swordNameProp = property.FindPropertyRelative("swordPrefabName");
                    EditorUtil.InitSerializedStringArray(swordNameProp.FindPropertyRelative("values"), typeof(PrefabName));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),swordNameProp);
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("anchorMode"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("damageScale"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("targetSize"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("startSize"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("delayTime"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("widenTime"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("easeType"));
                    
                    lightTypeProp = property.FindPropertyRelative("locateType");
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), lightTypeProp);

                    switch (EnumUtil.GetEnumValue<LocateType>(lightTypeProp.enumValueIndex))
                    {
                        case (int)LocateType.LocateByOffset:
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("offset"));
                            break;
                        case (int)LocateType.LocateByBossArea:
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("sideType"));

                            var randomPosProp = property.FindPropertyRelative("randomPos");
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("randomPos"));

                            if (randomPosProp.boolValue)
                                EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                                    property.FindPropertyRelative("interpolationRange"));
                            else
                            {
                                var interpProp = property.FindPropertyRelative("locateInterpolation");
                                EditorGUI.Slider(position.GetRectAtIndex(index++), interpProp.displayName,
                                    interpProp.floatValue, 0, 1);
                            }
                            break;
                    }

                    randomDegreeProp = property.FindPropertyRelative("randomDegree");
                            
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), randomDegreeProp);
                    if(randomDegreeProp.boolValue)
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("degreeRange"));
                    else 
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("degree"));

                    break;

                #endregion

                #region CurveBullet

                case TriggerType.CurvedBullet:
                    var curveBulletProp = property.FindPropertyRelative("bulletName");
                    EditorUtil.InitSerializedStringArray(curveBulletProp.FindPropertyRelative("values"), typeof(PrefabName));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),curveBulletProp);
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("damageScale"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("gravityScale"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("maxLife"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("aniX"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("aniY"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("reverseX"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("reverseY"));

                    
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
                    var audioProp = property.FindPropertyRelative("audioName");
                    EditorUtil.InitSerializedStringArray(audioProp.FindPropertyRelative("values"), typeof(AudioName));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),audioProp);
                    break;

                #endregion
            }
        }
#endif


#pragma warning restore 649
    }
}