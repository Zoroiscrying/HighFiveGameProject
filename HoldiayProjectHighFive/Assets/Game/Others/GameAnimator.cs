using Game.Const;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class GameAnimator
    {
        private static Dictionary<Animator, GameAnimator> instances = new Dictionary<Animator, GameAnimator>();

        public static GameAnimator GetInstance(Animator ani)
        {
            if (instances.ContainsKey(ani))
            {
                return instances[ani];
            }

            var a = new GameAnimator(ani);
            instances.Add(ani, a);
            return a;
        }

        private Animator ani;

        public AnimationWeight weight;

        private GameAnimator(Animator ani)
        {
            this.ani = ani;
            if (this.ani == null)
                throw new Exception("未获取到Animator");
            CEventCenter.AddListener(Message.M_Destory(ani.gameObject), _Remomve);
        }


        public void Play(int stateNameHash, AnimationWeight weight = AnimationWeight.Normal)
        {
            //            Debug.Log("this.weight"+(int)this.weight+" weight"+(int)weight);
            if ((int)weight < (int)this.weight)
            {
                //                Debug.Log("返回");
                return;
            }
            if ((int)weight == (int)this.weight)
            {
                ani.Play(stateNameHash);
                return;
            }

            //            Debug.Log("走到这里");

            //            Debug.Log(this.weight.ToString()+" "+weight.ToString());
            this.weight = weight;
            //            Debug.Log("修改后："+this.weight.ToString()+" "+weight.ToString());

            ani.Play(stateNameHash);

            var aniInfo = ani.GetCurrentAnimatorStateInfo(0);


            MainLoop.Instance.ExecuteLater(
                () => { this.weight = AnimationWeight.Normal; },
                aniInfo.length / ani.speed);

        }

        public float speed
        {
            get { return ani.speed; }
            set { ani.speed = value; }
        }

        public Animator Animator
        {
            get { return this.ani; }
        }

        private void _Remomve()
        {
            instances.Remove(this.ani);
        }
    }
}
