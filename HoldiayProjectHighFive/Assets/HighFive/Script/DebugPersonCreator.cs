namespace HighFive.Script
{
    public class DebugPersonCreator:PersonCreator
    {
        protected override void Start()
        {
            base.Start();
            Create();
        }
    }
}