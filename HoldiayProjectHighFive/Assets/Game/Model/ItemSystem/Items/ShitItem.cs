using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Const;
using Game.Data;
using UnityEngine.Assertions;
namespace Game.Model.ItemSystem
{
    public class ShitItem:AbstractItem
    {
        private float weight;

        public override string Sign
        {
            get
            {
                return DataSign.shitItem;
            }
        }

        internal override void Init(string args)
        {
            var strs = args.Split(TxtManager.SplitChar);
            Assert.IsTrue(strs.Length >= base.BasePropertyCount + 1);
            this.weight = Convert.ToSingle(strs[base.BasePropertyCount].Trim());
            base.Init(string.Join(TxtManager.SplitChar.ToString(), strs, 0, base.BasePropertyCount));
        }
    }
}
