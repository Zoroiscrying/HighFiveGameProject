namespace ReadyGamerOne.View.PanelSystem
{
    public interface IStackPanel
    {
        //        string panelName { get; set; }
        string PanelName { get; }
        void Enable();
        void Disable();
        void Destory();
    }
}
