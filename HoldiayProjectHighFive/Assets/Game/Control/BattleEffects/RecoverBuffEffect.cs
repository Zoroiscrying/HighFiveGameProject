using Game.Const;
using Game.Control.Person;
using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Control.BattleEffects
{
    /// <summary>
    /// 持续回血效果，这也是个例子
    /// </summary>
    public class RecoverBuffEffect : AbstractBattleEffect
    {
        private int amount;
        private int times;
        private float duringTime;

        public RecoverBuffEffect(int amount, int times, float duringTime)
        {
            this.amount = amount;
            this.times = times;
            this.duringTime = duringTime;
        }

        public override void Execute(AbstractPerson ap)
        {
            MainLoop.Instance.ExecuteEverySeconds(_Recover, this.times, this.duringTime, ap, Release);
        }
        private void _Recover(AbstractPerson ap)
        {
            CEventCenter.BroadMessage(Message.M_BloodChange(ap.obj), this.amount);
        }
    }
}
