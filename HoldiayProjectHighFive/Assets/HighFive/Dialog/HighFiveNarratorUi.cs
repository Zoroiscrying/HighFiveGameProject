using DialogSystem.Model;
using DialogSystem.View;

namespace HighFive.Dialog
{
    public class HighFiveNarratorUi:AbstractNarratorDialogUI
    {
        public HighFiveNarratorUi(DialogUnitInfo info) : base(info)
        {
        }

        protected override string TextPath => "";
    }
}