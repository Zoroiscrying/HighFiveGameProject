using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Game.View.PanelSystem
{
    /// <inheritdoc />
    /// <summary>
    /// 可用栈盛放的栈窗口
    /// </summary>
    public abstract class AbstractPanel : AbstractUI, IStackPanel
    {
        #region Static

        private static Dictionary<string, AbstractPanel> panelDic =
            new Dictionary<string, AbstractPanel>();

        internal static AbstractPanel GetPanel(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("PanelName 为空！！！");
                return null;
            }

            if (!panelDic.ContainsKey(name))
            {
                Debug.LogError("你好像没有在GameMgr_RegisterUiPanel中注册这个Panel类："+name);
                return null;
            }
            return panelDic[name];
        }

        public static void RegisterPanel<T>(string panelName) where T : AbstractPanel, new()
        {
            var p = new T();
            p.Init(panelName);
            panelDic.Add(panelName, p);
        }


        #endregion

        private string panelName;
        public event Action<string> onPanelShow;
        public event Action<string> onPanelHide;
        public event Action<string> onPanelDestory;

        /// /////////////////     继承接口      ////////////////////////
        public string PanelName => this.panelName;

        public sealed override void DestroyThis(PointerEventData eventData = null)
        {
            base.DestroyThis(eventData);
        }

        public virtual void Enable()
        {
            if (m_TransFrom == null)
                this.Load();
            this.Show();
            if (onPanelShow != null)
                onPanelShow(panelName);
        }

        public virtual void Disable()
        {
            this.Hide();
            if (onPanelHide != null)
                onPanelHide(panelName);
        }


        public virtual void Destory()
        {
            if (onPanelDestory != null)
                onPanelDestory(this.panelName);
            this.Disable();
            this.DestroyThis();
        }

        protected void Init(string panelName)
        {
            this.panelName = panelName;
        }

        protected abstract void Load();
    }
}
