using Game.View;
using Game.View.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model.SceneSystem
{
    public class TestSceneInfo : BaseSceneInfo
    {


        protected override void OnSceneUnload()
        {
            base.OnSceneUnload();
            Debug.Log("TestScene卸载");
            UIManager.Instance.PopPanel();
        }
    }
}
