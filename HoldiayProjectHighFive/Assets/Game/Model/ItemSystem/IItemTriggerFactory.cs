namespace Game.Model.ItemSystem
{
    public interface IItemTriggerFactory
    {
        AbstractItem CreateItem(string args);
    }
}
