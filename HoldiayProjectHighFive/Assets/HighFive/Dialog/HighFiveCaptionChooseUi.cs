using System;
using DialogSystem.Model;
using DialogSystem.View;
using UnityEngine;

namespace HighFive.Dialog
{
    public class HighFiveCaptionChooseUi:AbstractChooseDialogUI
    {
        public HighFiveCaptionChooseUi(DialogUnitInfo dialogUnitInfo) : base(dialogUnitInfo)
        {
        }

        protected override string SpeakerTextPath => "SpeakerName";
        protected override string TitleTextPath => "Title";
        protected override Func<Transform> onGetChoiceParent => () => m_TransFrom.Find("Choices");
        protected override string choiceObjPath => "ChoiceBtn";
        protected override string textPathOnChoice => "Text";
    }
}