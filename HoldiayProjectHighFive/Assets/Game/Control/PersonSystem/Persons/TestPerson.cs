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
        #endregion

        public TestPerson(BaseCharacterInfo characterInfo,Vector3 pos, Transform parent = null) : base(characterInfo,pos, parent)
        {
            new BloodBarUI(this);
        }
    }
}
