using HighFive.Control.PersonSystem.Persons;
using ReadyGamerOne.Script;
using UnityEngine;

namespace HighFive.Model.SpriteObjSystem
{
    class ParabloaBullet : BulletSpriteObj
    {

        #region 可配置参数
        public float ShotSpeed = 10; // 抛出的速度
        public Vector3 pointA; // 起点
        public Vector3 pointB; // 终点
        public float g = -10; // 重力加速度
        #endregion

        private Vector3 speed; // 初速度向量
        private Vector3 Gravity; // 重力向量
        private Vector3 currentAngle; // 当前角度
        public ParabloaBullet(int damage,float shotSpeed,float time,Vector3 targetPos,Vector3 startPos,AbstractPerson ap,string path,float maxLife=4f, Transform parent = null) 
            : base(damage,ap,path, startPos,maxLife, parent)
        {
            this.ShotSpeed = shotSpeed;
            this.pointA = startPos;
            this.pointB = targetPos;
            time = Vector3.Distance(pointA, pointB) / ShotSpeed;

            // 设置起始点位置为A
            this.obj.transform.position = pointA;

            // 通过一个式子计算初速度
            speed = new Vector3((pointB.x - pointA.x) / time,
                (pointB.y - pointA.y) / time - 0.5f * g * time,
                (pointB.z - pointA.z) / time);
            // 重力初始速度为0
            Gravity = Vector3.zero;
            MainLoop.Instance.AddFixedUpdateFunc(FixedUpdate);
        }
        
        private float dTime = 0;

        // Update is called once per frame
        void FixedUpdate()
        {
            // v=gt
            Gravity.y = g * (dTime += Time.fixedDeltaTime);

            //模拟位移
            this.obj.transform.position += (speed + Gravity) * Time.fixedDeltaTime;

            // 弧度转度：Mathf.Rad2Deg
            currentAngle.z = 90 - Mathf.Atan((speed.y + Gravity.y) / speed.x) * Mathf.Rad2Deg;

            // 设置当前角度
            this.obj.transform.eulerAngles = currentAngle;
        }

        public override void Release()
        {
            base.Release();
            MainLoop.Instance.RemoveFixedUpdateFunc(FixedUpdate);
        }
    }
}
