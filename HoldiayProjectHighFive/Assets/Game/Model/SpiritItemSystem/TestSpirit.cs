using ReadyGamerOne.Script;
using UnityEngine;

namespace Game.Model.SpiritItemSystem
{
    public class TestSpirit : AbstractSpiritItem
    {
        private MainLoop.UpdateTestPair pair;

        private void Execute()
        {
            Debug.Log("灵器检测成功");
        }

        public override void OnEnable()
        {
            pair = new MainLoop.UpdateTestPair(() => Input.GetKeyDown(KeyCode.K), Execute);
            MainLoop.AddUpdateTest(pair);
        }

        public override void OnDisable()
        {
            MainLoop.RemoveUpdateTest(pair);

            pair = null;
        }
        
    }
}
