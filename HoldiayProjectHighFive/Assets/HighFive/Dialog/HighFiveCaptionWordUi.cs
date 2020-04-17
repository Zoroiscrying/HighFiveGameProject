using DialogSystem.Model;
using DialogSystem.View;

namespace HighFive.Dialog
{
    public class HighFiveCaptionWordUi:AbstractWordDialogUI
    {
        public HighFiveCaptionWordUi(DialogUnitInfo unitInfo) : base(unitInfo)
        {
        }

        protected override string SpeakerTextPath => "SpeakerName";
        protected override string CaptionWordTextPath => "Word";
    }
}