using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model.SpiritItems
{
    public class TestSpirit : AbstractSpiritItem
    {
        private UpdateTestPair pair;

        private void Execute()
        {
            Debug.Log("灵器检测成功");
        }

        public override void OnEnable()
        {
            pair = new UpdateTestPair(() => Input.GetKeyDown(KeyCode.K), Execute);
            MainLoop.Instance.AddUpdateTest(pair);
        }

        public override void OnDisable()
        {
            MainLoop.Instance.RemoveUpdateTest(pair);

            pair = null;
        }
        
    }
}
