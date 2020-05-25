﻿using System;
using DialogSystem.Model;
using DialogSystem.ScriptObject;
using ReadyGamerOne.Common;
using UnityEngine;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Script;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogSystem.Scripts
{
    /// <summary>
    /// 开启类型
    /// </summary>
    public enum StartType
    {
        StartOnAwake,    //场景Awake时开启
        AutoStart,       //触发器碰到后自动开启
        InteractStart,   //在触发器范围内按交互键开启
        MessageStart,    //接收到指定消息后开启
        ClickStart,      //点击此物体开启
        AppearStart,     //出现在屏幕中后开始对话
    }

    /// <summary>
    /// 如何开启DialogSystem中的对话
    /// </summary>
    public enum TriggerType
    {
        Single,        //只开启你指定的DialogInfoAsset
        Sequence,      //会依次调用DialogSystem中指定范围的DialogInfoAsset,每调用一次触发一个
        All            //一次性全部顺次开启DialogSystem中的DialogInfoAsset
    }        
    
    /// <summary>
    /// 工作模式
    /// </summary>
    public enum WorkType
    {
        WorkAlways,        //始终工作，一直在检测有没有触发
        WorkAfterMessage,  //接收到指定消息后再开始工作
        WorkBetweenProgress     //在指定游戏进度中间工作
    }
    
    /// <summary>
    /// 对话触发器
    /// </summary>
    public class DialogTrigger : MonoBehaviour
    {

        #region UsedDialogSystem

        public DialogSystem targetDialogSystem;

        private DialogSystem _dialogSystem;
        public DialogSystem UsedDialogSystem
        {
            get
            {
                _dialogSystem = targetDialogSystem ? targetDialogSystem : GetComponent<DialogSystem>();

                if (_dialogSystem == null)
                    Debug.LogError("获取DialogSystem失败");
                return _dialogSystem;
            }
        }          

        #endregion

        #region Fields

        /// <summary>
        /// 如何开启DialogSystem中的对话
        /// </summary>
        public TriggerType triggerType=TriggerType.Single;
        
        /// <summary>
        /// 工作模式
        /// </summary>
        public WorkType workType = WorkType.WorkAlways;
        /// <summary>
        /// 开启工作的消息
        /// </summary>
        public StringChooser messageToEnableThis;
        /// <summary>
        /// 触发类型
        /// </summary>
        public StartType startType = StartType.AutoStart;
        /// <summary>
        /// 待触发的对话资源
        /// </summary>
        public StringChooser dialogNameToTrigger=null;

        /// <summary>
        /// 开启对话的消息
        /// </summary>
        public StringChooser messageToStart;

        /// <summary>
        /// 开启DialogSystem中从哪里到哪里的对话资源
        /// </summary>
        public Vector2Int sequenceRange = new Vector2Int(0, 0);
        
        /// <summary>
        /// 是否只触发一次
        /// </summary>
        public bool triggerOnlyOnce = true;

        /// <summary>
        /// 是否在触发后设置此物体不可见
        /// </summary>
        public bool disableThisGameObjectOnEnd = false;

        public ProgressPointRange allowProgressRange;

        #region Private
        
        private bool enableTrigger = false;
        
        private bool _triggered = false;


        private int currentDialogIndex=0;
        
        private bool close=false;    
        

        #endregion
    
        
        #endregion

        #region CEventCenter消息响应

        
        private void OnEnableTrigger() => enableTrigger = true;

        #endregion

        #region MonoBehavior
        
        // Use this for initialization
        private void Start()
        {            
//            Debug.Log(gameObject.name+ " Start:"+gameObject.activeSelf);
            if(workType==WorkType.WorkAfterMessage)
                CEventCenter.AddListener(messageToEnableThis.StringValue,OnEnableTrigger);
            if (startType == StartType.MessageStart)
                CEventCenter.AddListener(messageToStart.StringValue,TriggerDialog);
            else if (startType == StartType.ClickStart)
            {
         
                if (this.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    this.gameObject.AddComponent<UIInputer>().eventOnPointerClick += (obj) =>
                    {
                        switch (workType)
                        {
                            case WorkType.WorkAlways:
                                TriggerDialog();
                                break;
                            case WorkType.WorkAfterMessage:
                                if (enableTrigger)
                                    TriggerDialog();
                                break;
                            case WorkType.WorkBetweenProgress:
                                if(DialogProgressAsset.Instance.CurrentProgress>allowProgressRange.Min
                                   && DialogProgressAsset.Instance.CurrentProgress<allowProgressRange.Max)
                                TriggerDialog();
                                break;
                        }
                    };
                }
                else
                {
                    Debug.Log("添加eventOnCLickObj监听");
                    this.gameObject.AddComponent<UIInputer>().eventOnClickObj += (obj) =>
                    {
                        Debug.Log("点击");
                        switch (workType)
                        {
                            case WorkType.WorkAlways:
                                TriggerDialog();
                                break;
                            case WorkType.WorkAfterMessage:
                                if (enableTrigger)
                                    TriggerDialog();
                                break;
                            case WorkType.WorkBetweenProgress:
                                if(DialogProgressAsset.Instance.CurrentProgress>allowProgressRange.Min
                                   && DialogProgressAsset.Instance.CurrentProgress<allowProgressRange.Max)
                                    TriggerDialog();
                                break;
                        }
                    };
                }
         
            }
            else if(startType==StartType.StartOnAwake)
                switch (workType)
                {
                    case WorkType.WorkAlways:
                        TriggerDialog();
                        break;
                    case WorkType.WorkAfterMessage:
                        if(enableTrigger)
                            TriggerDialog();
                        break;
                    case WorkType.WorkBetweenProgress:
                        if(DialogProgressAsset.Instance.CurrentProgress>allowProgressRange.Min
                           && DialogProgressAsset.Instance.CurrentProgress<allowProgressRange.Max)
                            TriggerDialog();
                        break;
                }
        }


        private void Update()
        {
            if(!close)
                return;
            if(startType==StartType.InteractStart && UsedDialogSystem.IfInteract())
                switch (workType)
                {
                    case WorkType.WorkAlways:
                        TriggerDialog();
                        break;
                    case WorkType.WorkAfterMessage:
                        if(enableTrigger)
                            TriggerDialog();
                        break;
                    case WorkType.WorkBetweenProgress:
                        if(DialogProgressAsset.Instance.CurrentProgress>allowProgressRange.Min
                           && DialogProgressAsset.Instance.CurrentProgress<allowProgressRange.Max)
                            TriggerDialog();
                        break;
                }
        }

        private void OnDestroy()
        {
            if(workType==WorkType.WorkAfterMessage)
                CEventCenter.RemoveListener(messageToEnableThis.StringValue,OnEnableTrigger);
            if (startType == StartType.MessageStart)
                CEventCenter.RemoveListener(messageToStart.StringValue,TriggerDialog);
        }
        
#if UNITY_EDITOR
        private GUIStyle style;
        private GUIStyle Style
        {
            get
            {
                if (null == style)
                {
                    style = new GUIStyle();
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.BoldAndItalic;
                    style.normal.textColor=new Color(1f, 0.42f, 0.81f);
                }

                return style;
            }
        }
#endif

        private void OnDrawGizmos()
        {
            if (!gameObject.activeSelf || !enabled)
                return;
            if (startType == StartType.AppearStart || startType == StartType.AutoStart ||
                startType == StartType.InteractStart)
            {
#if UNITY_EDITOR
                Handles.Label(transform.position,$"[DialogTrigger:{name}]",Style);
#endif                
            }

        }
        #endregion
        
        #region Unity_Trigger

        private void OnBecameVisible()
        {
            if (!Application.isPlaying)
                return;
//            Camera.main.
            if (!enabled)
                return;
            if(startType==StartType.AppearStart)
                switch (workType)
                {
                    case WorkType.WorkAlways:
                        TriggerDialog();
                        break;
                    case WorkType.WorkAfterMessage:
                        if(enableTrigger)
                            TriggerDialog();
                        break;
                    case WorkType.WorkBetweenProgress:
                        if(DialogProgressAsset.Instance.CurrentProgress>allowProgressRange.Min
                           && DialogProgressAsset.Instance.CurrentProgress<allowProgressRange.Max)
                            TriggerDialog();
                        break;
                }
        }

        private void OnTriggerExit2D(Collider2D collider){
            close=false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            close=true;
            if (!this.enabled)
                return;
            if(startType==StartType.AutoStart)
                switch (workType)
                {
                    case WorkType.WorkAlways:
                        TriggerDialog();
                        break;
                    case WorkType.WorkAfterMessage:
                        if(enableTrigger)
                            TriggerDialog();
                        break;
                    case WorkType.WorkBetweenProgress:
                        if(DialogProgressAsset.Instance.CurrentProgress>allowProgressRange.Min
                           && DialogProgressAsset.Instance.CurrentProgress<allowProgressRange.Max)
                            TriggerDialog();
                        break;
                }
        }        

        #endregion

        #region Private_Functions

        private void TriggerDialog()
        {
            if(triggerOnlyOnce && _triggered)
                return;
            StartDialog(this.triggerType);
        }

        private Coroutine StartDialog(TriggerType triggerType)
        {
            switch (triggerType)
            {
                case TriggerType.All:
                    _triggered = true;
                    return UsedDialogSystem.StartDialog(startType==StartType.MessageStart);    
                case TriggerType.Single:
                    _triggered = true;            
                    return UsedDialogSystem.StartDialog(dialogNameToTrigger.StringValue,null,startType==StartType.MessageStart);
                case TriggerType.Sequence:
                    
                    var c = UsedDialogSystem.StartDialog(sequenceRange.x + currentDialogIndex++, () =>
                    {
                        if (currentDialogIndex > sequenceRange.y)
                        {
                            _triggered = true;
                            if(disableThisGameObjectOnEnd)
                                this.gameObject.SetActive(false);
                            else
                            {
                                Destroy(this);
                            }
                        }
                    },startType==StartType.MessageStart);
                    return c;
            }

            throw new Exception("没有处理这种StartTrigger的逻辑：" + triggerType);
        }



        #endregion

    }
}

