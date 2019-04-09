using Game.View;
using Game.View.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model.Scenes
{
    public class JbSceneInfo : BaseSceneInfo
    {
        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            Debug.Log("jbScene卸载");
            UIManager.Instance.PopPanel();
        }
    }
}
