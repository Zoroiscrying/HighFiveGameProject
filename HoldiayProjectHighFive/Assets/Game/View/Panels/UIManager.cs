using Game.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Game.View.Panels
{
    /// <summary>
    /// 总UI管理者
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        public UIManager()
        {

        }

        private Stack<IStackPanel> panelStack = new Stack<IStackPanel>();

        public IStackPanel CurrentPanel
        {
            get
            {
                if (panelStack.Count == 0)
                    return null;
                return panelStack.Peek();
            }
        }

        public void PushPanel(string name)
        {
            if (panelStack.Count != 0)
                panelStack.Peek().Disable();

            var panel = AbstractPanel.GetPanel(name);
            panel.Enable();
            panelStack.Push(panel);
            Debug.Log("入栈：" + this.panelStack.Count + " name:" + name);
        }


        public void PopPanel()
        {
            //Debug.Log("1出栈：" + this.panelStack.Count + " name:" +CurrentPanel.PanelName);
            panelStack.Peek().Destory();
            //Debug.Log("2出栈：" + this.panelStack.Count + " name:" + CurrentPanel.PanelName);
            panelStack.Pop();
            if (panelStack.Count > 0)
                panelStack.Peek().Enable();
        }


    }
}
