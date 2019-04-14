using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Model.ItemSystem
{
    public interface IItemTriggerFactory
    {
        AbstractItem CreateItem(string args);
    }
}
