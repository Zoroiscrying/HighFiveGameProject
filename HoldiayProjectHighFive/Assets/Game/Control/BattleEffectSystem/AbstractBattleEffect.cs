using Game.Control.PersonSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Control.BattleEffectSystem
{
    public abstract class AbstractBattleEffect : IBattleEffect
    {
        public abstract void Execute(AbstractPerson ap);

        public virtual void Release(AbstractPerson ap)
        {
            ap.bfList.Remove(this);
        }
    }
}
