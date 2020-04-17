using System;
using DialogSystem.Model;

namespace DialogSystem.Scripts
{
    public class ActivateBetweenProgressRange : AwakeAtGameProgress
    {
        public ProgressPointRange ProgressPointRange;

        public override Action<float> onAwakeOnProgress => (value) =>
        {
            if (value > ProgressPointRange.Min && value < ProgressPointRange.Max)
            {
                this.gameObject.SetActive(true);
            }
            else
                this.gameObject.SetActive(false);
        };
    }
}