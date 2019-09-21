using System;
using Game.Const;
using Game.Control.PersonSystem;
using Game.Math;
using Game.Model.SpriteObjSystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace Game.Control.SkillSystem
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
        Trigger2D
    }
    
    [Serializable]
    public class TriggerUnitInfo
    {
        #region Every

        public int id;
        public float startTime;
        public float lastTime;
        public TriggerType type;        

        #endregion

        #region Animation

        public string animationName;
        public float animationSpeed;

        #endregion

        #region Audio
        
        public string audioName;

        #endregion

        #region Bullet

        public string bulletName;
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
        
        public float timeToTarget;
        public Vector3 offset;

        #endregion

        #region Trigger2D

        public Vector2 personOffect;
        public Vector2 size;
        public float beginDegree;
        public float endDegree;

        #endregion

        public void RunTriggerUnit(AbstractPerson self)
        {
            MainLoop.Instance.ExecuteLater(() =>
            {
                switch (type)
                {
                    #region Animation

                    case TriggerType.Animation:
                        if (self == null)
                        {
                            throw new Exception("SkillCore为空");
                        }

                        var animator = GameAnimator.GetInstance(self.obj.GetComponent<Animator>());

                        if (animator == null)
                        {
                            throw new Exception(self.CharacterName + "没有Animator组件");
                        }

                        animator.speed = this.animationSpeed*self.AttackSpeed;


                        animator.Play(Animator.StringToHash(this.animationName), AnimationWeight.High);
                        
                        
                        break;
                        

                    #endregion

                    #region Audio
                    
                    case TriggerType.Audio:
                        AudioMgr.Instance.PlayEffect(this.audioName, self.obj.transform.position);
                        break;

                        

                    #endregion

                    #region Bullet

                    case TriggerType.Bullet:
                        this.dir = new Vector2(self.Dir * Mathf.Abs(this.dir.x), this.dir.y);
                        new DirectLineBullet(GameMath.Damage(self.BaseAttack), this.dir, self.Pos + new Vector3(0, 0.3f * self.Scanler, 0)
                            , self, DirPath.BulletDir + this.bulletName, speed:this.bulletSpeed,maxLife:this.maxLife);
                        break;                        

                    #endregion

                    #region Parabloa


                    case TriggerType.Parabloa:
                        new ParabloaBullet(this.damage, this.bulletSpeed, this.timeToTarget, self.Pos + offset,
                            self.Pos, self, Const.DirPath.BulletDir+this.bulletName, this.maxLife);
                        break;                        

                    #endregion

                    #region RayDamage

                    case TriggerType.RayDamage:
                        
                        //临时数据
                        var mainc = self.obj.GetComponent<MainCharacter>();
                        //面对方向
                        var position = self.obj.transform.position;
                        var p = new Vector2(position.x, position.y);
                        
                        var target = this.rayLength * self.Scanler * new Vector2(this.dir.x * self.Dir, this.dir.y);

                        //调整身高偏移
                        p += new Vector2(0, 0.1f * self.obj.transform.localScale.y);
                        Debug.DrawLine(p, p + target, Color.red);
                        var rescult = Physics2D.LinecastAll(p, p + target, this.rayTestLayer);

                        if (rescult.Length == 0)
                        {
                            return;
                        }

                        foreach (var r in rescult)
                        {
                            //对打击到的目标进行操作，添加各种效果
                            var hitPerson = AbstractPerson.GetInstance(r.transform.gameObject);
                            if (hitPerson == null)
                            {
                                Debug.Log(r.transform.gameObject);
                                continue;
                            }
                            self.OnCauseDamage(GameMath.Damage(self.BaseAttack));
                            hitPerson.TakeBattleEffect(self.AttackEffect);
                        }
                        break;                        

                    #endregion
                    

                    case TriggerType.Trigger2D:
                        break;
                    
                }                
            },this.startTime);
        }

        public void Reset()
        {
            
        }
       

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
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("startTime"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("lastTime"));            

            #endregion

            switch (type)
            {
                #region Trigger2D

                    
                case TriggerType.Trigger2D:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("personOffect"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("size"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("beginDegree"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("endDegree"));
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
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("offset"));
                    
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

    }
}