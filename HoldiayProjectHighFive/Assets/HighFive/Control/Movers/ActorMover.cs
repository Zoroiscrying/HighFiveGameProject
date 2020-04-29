using System;
using UnityEngine;
using zoroiscrying;

namespace HighFive.Control.Movers
{
    /// <summary>
    /// 角色移动器
    /// </summary>
    public class ActorMover:BaseMover
    {
        #region IMover2D

        /// <summary>
        /// 如果你不希望外部使用某个访问器，可以在里面抛异常提示
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override float GravityScale
        {
            get
            {
                // do something;
                return default;
            }
            set
            {
                throw new Exception("Actor不允许修该GravityScale");
            }
        }

        /// <summary>
        /// 可以中专其他组建的值
        /// </summary>
        private CharacterController2D cc;
        public override LayerMask ColliderLayers
        {
            get => cc.platformMask;
            set => cc.platformMask = value;
        }
                

        #endregion


        #region Actor_特有接口

        /// <summary>
        /// 角色的面朝方向，右 1 ，左 - 1
        /// </summary>
        public virtual int FaceDir { get; set; }

        
        /// <summary>
        /// 速度的缩放
        /// 默认为1，表示正常速度，为0的时候静止不动，速度为0
        /// 这样方便我方便的实现人物的加速和减速，SpeedScale为1.5就意味着现在提速50%
        /// </summary>
        public virtual float SpeedScale { get; set; }        

        /// <summary>
        /// 控制是否可以移动的开关
        /// </summary>
        /// <param name="arg"></param>
        public virtual void SetMovable(bool arg)
        {
            
        }
        #endregion
    }
}