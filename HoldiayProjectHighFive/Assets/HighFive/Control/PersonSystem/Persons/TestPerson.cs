using HighFive.View;
using UnityEngine;

namespace HighFive.Control.PersonSystem.Persons
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
