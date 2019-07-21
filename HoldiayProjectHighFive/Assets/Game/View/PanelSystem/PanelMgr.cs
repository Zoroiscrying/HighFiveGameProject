using Game.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Game.View.PanelSystem
{
    /// <summary>
    /// 总UI管理者
    /// </summary>
    public static class PanelMgr
    {
        public static event Action onClear;

        private static Stack<IStackPanel> panelStack = new Stack<IStackPanel>();

        public static IStackPanel CurrentPanel
        {
            get
            {
                if (panelStack.Count == 0)
                    return null;
                return panelStack.Peek();
            }
        }
        
        
        /// <summary>
        /// 加载一个新面板
        /// </summary>
        /// <param name="name"></param>
        public static void PushPanel(string name)
        {
            if (panelStack.Count != 0)
                panelStack.Peek().Disable();

            var panel = AbstractPanel.GetPanel(name);
            panel.Enable();
            panelStack.Push(panel);
//            Debug.Log("入栈：" + panelStack.Count + " name:" + name);
        }

        #region 加载Panel,附带一些效果
        /// <summary>
        /// 加载新面板，带动画
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="transition"></param>
        public static void PushPanel(string panelName, AbstractTransition transition)
        {
            var panel = AbstractPanel.GetPanel(panelName);
            transition.PushPanel(panel);
            panelStack.Push(panel);

//            Debug.Log("入栈：" + panelStack.Count + " name:" + panelName);
        }

        /// <summary>
        /// 加载新面板，带效果
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="abstractScreenEffect"></param>
        public static void PushPanel(string panelName, AbstractScreenEffect abstractScreenEffect)
        {
            var panel = AbstractPanel.GetPanel(panelName);
            abstractScreenEffect.OnBegin(CurrentPanel as AbstractPanel, panel);
            panelStack.Push(panel);
        }

        /// <summary>
        /// 加载新面板，都带
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="transition"></param>
        /// <param name="abstractScreenEffect"></param>
        public static void PushPanel(string panelName, AbstractTransition transition,
            AbstractScreenEffect abstractScreenEffect)
        {
            var panel = AbstractPanel.GetPanel(panelName);
            transition.onBegin += abstractScreenEffect.OnBegin;
            transition.PushPanel(panel);
            panelStack.Push(panel);
        }
        

        #endregion


        #region 加载Panel，同时广播一条消息
        public static void PushPanelWithMessage(string panelName, string message)
        {
            PushPanel(panelName);
            CEventCenter.BroadMessage(message);
        }

        public static void PushPanelWithMessage<T>(string panelName, string message, T arg1)
        {
            PushPanel(panelName);
            CEventCenter.BroadMessage<T>(message,arg1);
        }

        public static void PushPanelWithMessage<T1, T2>(string panelName, string message, T1 arg1, T2 arg2)
        {
            PushPanel(panelName);
            CEventCenter.BroadMessage(message, arg1, arg2);
        }
        

        #endregion



        /// <summary>
        /// 移除当前面板
        /// </summary>
        public static void PopPanel()
        {
            //Debug.Log("1出栈：" + this.panelStack.Count + " name:" +CurrentPanel.PanelName);
            panelStack.Peek().Destory();
            //Debug.Log("2出栈：" + this.panelStack.Count + " name:" + CurrentPanel.PanelName);
            panelStack.Pop();
            if (panelStack.Count > 0)
                panelStack.Peek().Enable();
        }


        /// <summary>
        /// 清空Panel栈
        /// </summary>
        public static void Clear()
        {
            while (panelStack.Count>0)
            {
                PopPanel();
            }
            onClear?.Invoke();
        }

    }
}
