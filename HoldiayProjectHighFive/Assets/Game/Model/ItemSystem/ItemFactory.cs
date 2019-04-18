using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Model.ItemSystem
{
    public class ItemFactory<T> : Singleton<ItemFactory<T>>, IItemTriggerFactory
        where T:AbstractItem,new()
    {
        public AbstractItem CreateItem(string args)
        {
            var item= new T();
            item.Init(args);
            return item;
            
        }
    }
}
