using Game.Control.PersonSystem;

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
