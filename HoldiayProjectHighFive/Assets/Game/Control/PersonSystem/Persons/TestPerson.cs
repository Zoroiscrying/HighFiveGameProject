using System.Collections.Generic;
using Game.Const;
using Game.View;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Control.PersonSystem
{
    /// <summary>
    /// 测试草人
    /// </summary>
    public class TestPerson : AbstractPerson
    {
        #region Private

        private float shineLastTime = 1;
        private float shineDurTime = 0.1f;
        private float hitback = 0.05f;

        #endregion

        public TestPerson(BaseCharacterInfo characterInfo,Vector3 pos, Transform parent = null) : base(characterInfo,pos, parent)
        {
            new BloodBarUI(this);
        }
    }
}
