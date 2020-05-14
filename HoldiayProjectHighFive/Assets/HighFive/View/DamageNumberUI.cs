using System.Globalization;
using DefaultNamespace.HighFive.Model.Person;
using HighFive.Const;
using HighFive.Global;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HighFive.View
{
    /// <summary>
    /// 伤害数值
    /// </summary>
    public class DamageNumberUI : AbstractUI
    {
        public GameObject go;
        private Text Text;
        private Transform target;
        private float yOffect;
        private int dir;

        public DamageNumberUI(HighFiveDamage damageInfo, float yOffect,Transform targetPerson, int dir, float dur = 0.5f, float time = 1f)
        {
            Create(UiName.Text_Number);
            this.go = m_TransFrom.gameObject;
            this.target = targetPerson;

            this.yOffect = yOffect;
            this.dir = -dir;

            if (go == null)
                Debug.Log("go实例化失败");

            this.Text = this.m_TransFrom.GetComponent<Text>();
            if (damageInfo.IsCrit)
            {
                Text.color = GameSettings.Instance.critColor;
                Text.fontSize = GameSettings.Instance.critSize;
                Text.text = damageInfo.Damage.ToString(CultureInfo.InvariantCulture);
            }else if (damageInfo.IsMissing)
            {
                Text.color = GameSettings.Instance.missingColor;
                Text.fontSize = GameSettings.Instance.missingSize;
                Text.text = "MISS";
            }
            else
            {
                Text.color = GameSettings.Instance.normalColor;
                Text.fontSize = GameSettings.Instance.normalSize;
                Text.text = damageInfo.Damage.ToString(CultureInfo.InvariantCulture);
            }

            if (!damageInfo.IsPlayer)
                Text.color = GameSettings.Instance.enemyDamageColor;
            
            m_TransFrom.position = Camera.main.WorldToScreenPoint(target.position + new Vector3(0, this.yOffect * 1.5f, 0));


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
