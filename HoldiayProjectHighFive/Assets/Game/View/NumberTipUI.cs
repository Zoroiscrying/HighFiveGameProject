using Game.Const;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.View
{
    /// <summary>
    /// 伤害数值
    /// </summary>
    public class NumberTipUI : AbstractUI
    {


        public GameObject go;
        private Text Text;
        private Transform target;
        private float yOffect;
        private float targetX;
        private float dur;
        private int dir;

        public NumberTipUI(int number, float yOffect, int size, Color color, Transform targetPerson, int dir, float dur = 0.5f, float time = 1f)
        {
            Create(UIPath.text_number);
            this.go = m_TransFrom.gameObject;
            this.target = targetPerson;

            this.yOffect = yOffect;
            this.dur = dur;
            this.dir = dir;

            if (go == null)
                Debug.Log("go实例化失败");

            this.Text = this.m_TransFrom.GetComponent<Text>();
            Text.color = color;
            Text.fontSize = size;
            Text.text = number.ToString();

            m_TransFrom.position = Camera.main.WorldToScreenPoint(target.position + new Vector3(0, this.yOffect * 1.5f, 0));
            this.targetX = m_TransFrom.position.x + this.dir * 6f;

            var rig = this.go.AddComponent<Rigidbody2D>();
            rig.gravityScale = 100;
            Show();

            MainLoop.Instance.ExecuteLater<PointerEventData>(DestroyThis, time, null);
        }

        protected override void Update()
        {
            base.Update();
            if (target == null)
                return;
            var r = UnityEngine.Random.Range(1.5f, 4.5f);
            m_TransFrom.position += new Vector3(this.dir * r, 0);
        }
    }
}
