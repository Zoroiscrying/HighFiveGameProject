namespace HighFive.View
{
//    /// <summary>
//    /// 血条
//    /// </summary>
//    public class BloodBarUI : AbstractUI
//    {
//        public Slider slider;
//        private IHighFivePerson ap;
//        private Text bloodText;
//        public BloodBarUI(IHighFivePerson ap)
//        {
//            this.ap = ap;
//            base.Create(UiName.Slider);
//            m_TransFrom.position = Camera.main.WorldToScreenPoint(ap.position + new Vector3(0, 1, 0));
//            this.slider = m_TransFrom.GetComponent<Slider>();
//            this.slider.value = (float)this.ap.Hp / this.ap.MaxHp;
//            this.bloodText = m_TransFrom.Find("Text").GetComponent<Text>();
//            this.bloodText.text = ap.Hp + "/" + ap.MaxHp;
//            Show();
//        }
//
//        protected override void OnAddListener()
//        {
//            base.OnAddListener();
//            CEventCenter.AddListener<int>(Message.M_BloodChange(ap.gameObject), OnBloodChanged);
//            CEventCenter.AddListener<PointerEventData>(Message.M_Destory(ap.gameObject), DestroyThis);
//        }
//
//        protected override void OnRemoveListener()
//        {
//            base.OnRemoveListener();
//            CEventCenter.RemoveListener<int>(Message.M_BloodChange(ap.gameObject), OnBloodChanged);
//            CEventCenter.RemoveListener<PointerEventData>(Message.M_Destory(ap.gameObject), DestroyThis);
//        }
//
//        protected override void Update()
//        {
//            base.Update();
//            if (ap.gameObject == null)
//                return;
//            m_TransFrom.position = Camera.main.WorldToScreenPoint(ap.position + new Vector3(0, 1, 0));
//        }
//
//        void OnBloodChanged(int change)
//        {
//            this.slider.value = (float)this.ap.Hp / this.ap.MaxHp;
//            this.bloodText.text = ap.Hp + "/" + ap.MaxHp;
//        }
//    }
}
