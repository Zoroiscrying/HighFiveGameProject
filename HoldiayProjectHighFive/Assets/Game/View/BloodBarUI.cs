using Game.Const;
using Game.Control.Person;
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
    /// 血条
    /// </summary>
    public class BloodBarUI : AbstractUI
    {
        public Slider slider;
        private AbstractPerson ap;
        public BloodBarUI(AbstractPerson ap)
        {
            this.ap = ap;
            base.Create(UIPath.Slider_BloodBar);
            m_TransFrom.position = Camera.main.WorldToScreenPoint(ap.Pos + new Vector3(0, ap.Scanler * 0.3f, 0));
            this.slider = m_TransFrom.GetComponent<Slider>();
            this.slider.value = (float)this.ap.Hp / this.ap.MaxHp;
            Show();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            CEventCenter.AddListener<int>(Message.M_BloodChange(ap.obj), OnBloodChanged);
            CEventCenter.AddListener<PointerEventData>(Message.M_Destory(ap.obj), DestroyThis);
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            CEventCenter.RemoveListener<int>(Message.M_BloodChange(ap.obj), OnBloodChanged);
            CEventCenter.RemoveListener<PointerEventData>(Message.M_Destory(ap.obj), DestroyThis);
        }

        protected override void Update()
        {
            base.Update();
            if (ap.obj == null)
                return;
            m_TransFrom.position = Camera.main.WorldToScreenPoint(ap.Pos + new Vector3(0, ap.Scanler * 0.7f, 0));
        }

        void OnBloodChanged(int change)
        {
            this.slider.value = (float)this.ap.Hp / this.ap.MaxHp;
        }
    }
}
