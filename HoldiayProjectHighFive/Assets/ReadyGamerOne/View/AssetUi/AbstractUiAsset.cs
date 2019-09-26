using System;
using System.IO;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Global;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR

#endif

namespace ReadyGamerOne.View.AssetUi
{
    /// <summary>
    /// UI资源基类
    /// <dependences>MainLoop,GlobalVar.G_Canvas</dependences>
    /// </summary>
    public class BaseUiAsset:ScriptableObject
    {
        #region Editor

#if UNITY_EDITOR

        [MenuItem("ReadyGamerOne/Create/UI/BaseUiAsset")]
        public static void CreateAsset()
        {
            string[] strs = Selection.assetGUIDs;

            string path = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (path.Contains("."))
            {
                path=path.Substring(0, path.LastIndexOf('/'));
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var targetFullPath = path + "/NewBaseUiAsset";

            if (File.Exists(targetFullPath + ".asset"))
                targetFullPath += "(1)";
            
            AssetDatabase.CreateAsset(CreateInstance<BaseUiAsset>(), targetFullPath + ".asset");
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<BaseUiAsset>(targetFullPath + ".asset");
        }
        
#endif        

            #endregion
            
        [HideInInspector] public Transform m_TransFrom;
        public ResourcesPathChooser prefabPath;
        private bool m_bIsVisible=false;

        public bool IsVisable => m_bIsVisible;

        /// <summary>
        /// 实例化出来物体，但还是不可见的，显示的话，还需要额外调用Show
        /// </summary>
        /// <param name="parent"></param>
        public virtual void InitializeObj(Transform parent)
        {

            if (string.IsNullOrEmpty(prefabPath.Path))
                throw new Exception("资源路径为空");
            


            var canvas = Global.GlobalVar.G_Canvas.transform;

            if (null == canvas)
                Debug.Log("画布获取失败");

            var obj = MemoryMgr.InstantiateGameObject(prefabPath.Path, parent);

            if (obj == null)
            {
                Debug.LogError("Window Create Error ");
                return;
            }

            m_TransFrom = obj.transform;

            m_TransFrom.gameObject.SetActive(false);
        }

        /// <summary>
        /// 销毁实例化出来的UI实例
        /// </summary>
        /// <param name="data"></param>
        public virtual void DestoryObj(PointerEventData data = null)
        {
            Hide();
            if (m_TransFrom)
            {
                Destroy(m_TransFrom.gameObject);
                m_TransFrom = null;
            }
        }
        
        /// <summary>
        /// 显示当前UI
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Show()
        {
            if (!m_TransFrom)//如果实现没有初始化就会默认以Canvas为父物体来创建
            {
                InitializeObj(GlobalVar.G_Canvas.transform);
            }
            if(m_TransFrom.gameObject.activeSelf==false)
                OnShow();
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void Hide()
        {
            if (m_TransFrom && m_TransFrom.gameObject.activeSelf == true)
            {
                OnHide();
            }

            m_bIsVisible = false;
        }
        
        /// <summary>
        /// 更新回调函数
        /// </summary>
        protected virtual void OnUpdate()
        {
            
        }

        /// <summary>
        /// 显示时的回调
        /// </summary>
        protected virtual void OnShow()
        {
            MainLoop.AddUpdateFunc(OnUpdate);
            m_TransFrom.gameObject.SetActive(true);
            m_TransFrom.SetAsLastSibling();//设置在最上面
            m_bIsVisible = true;
            OnAddListener();
        }

        /// <summary>
        /// 隐藏时的回调
        /// </summary>
        protected virtual void OnHide()
        {
            OnRemoveListener();
            MainLoop.RemoveUpdateFunc(OnUpdate);
            m_TransFrom.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 添加事件监听
        /// </summary>
        protected virtual void OnAddListener()
        {
            
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        protected virtual void OnRemoveListener()
        {
            
        }
        
        
    }
}