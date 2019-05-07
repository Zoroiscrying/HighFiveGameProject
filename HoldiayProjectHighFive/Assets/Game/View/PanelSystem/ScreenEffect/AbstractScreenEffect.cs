using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.View.PanelSystem
{
    public abstract class AbstractScreenEffect
    {
        protected float time;

        public AbstractScreenEffect(float time)
        {
            this.time = time;
        }
        public abstract void OnBegin(AbstractPanel panelBefore, AbstractPanel newPanel);
        public abstract void OnEnd(AbstractPanel panelbefore, AbstractPanel newPanel);
    }
}

