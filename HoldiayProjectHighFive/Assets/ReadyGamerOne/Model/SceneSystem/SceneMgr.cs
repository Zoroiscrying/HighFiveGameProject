using System;
using System.Collections;
using ReadyGamerOne.Script;
using ReadyGamerOne.View.PanelSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReadyGamerOne.Model.SceneSystem
{
    public static class SceneMgr
    {

        #region Private


        private static string currentSceneName;
        
        private static IEnumerator _LoadSceneAsync(AbstractSceneInfo newScene,bool stopTime, Action<int> onLoading,Action onLeaveOldScene,Action onEnterNewScene)
        {
            if (stopTime)
                Time.timeScale = 0;
            
            //显示用的进度
            int displayPro = 0;
            
            //实际的进度
            int realPro = 0;
            
            //开始加载下一个场景
            var async = SceneManager.LoadSceneAsync(newScene.sceneName);
            
            //暂时不进入下一个产经
            async.allowSceneActivation = false;
            
            
            
            //在加载不足90%时，进度条缓慢增长动画
            while (async.progress < 0.9f)
            {
                realPro = (int) (async.progress * 100.0f);
                
                //如果显示进度尚未达到实际进度，每帧增加1%
                while (displayPro < realPro)
                {
                    displayPro++;
                    
                    onLoading?.Invoke(displayPro);
                    
                    yield return new WaitForEndOfFrame();
                }
            }
            
            //加载最后一段
            realPro = 100;
            while (displayPro < realPro)
            {
                displayPro++;
                
                onLoading?.Invoke(displayPro);
                
                yield return new WaitForEndOfFrame();
            }

            
            onLeaveOldScene?.Invoke();
            
            if(!string.IsNullOrEmpty(currentSceneName))
                AbstractSceneInfo.GetScene(currentSceneName).OnSceneUnLoaded();
            
            
            //允许场景切换
            async.allowSceneActivation = true;
            
            //等待场景真正加载完毕
            while (!async.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            if (stopTime)
                Time.timeScale = 1;

            currentSceneName = newScene.sceneName;
            
            //throw new Exception("!!!!!");
            
            newScene.OnSceneLoad();
            
            onEnterNewScene?.Invoke();
        }
        
        private static IEnumerator _LoadScene(AbstractSceneInfo newScene,Action onLeaveOldScene,Action onEnterNewScene)
        {
            //throw new Exception("SceneMgr_LoadScene");
            //开始加载下一个场景
            var async = SceneManager.LoadSceneAsync(newScene.sceneName);
            
            //暂时不进入下一个产经
            async.allowSceneActivation = false;

            while (async.progress < 0.9f)
            {
                yield return new WaitForFixedUpdate();
            }
            
            onLeaveOldScene?.Invoke();
            
            if(!string.IsNullOrEmpty(currentSceneName))
                AbstractSceneInfo.GetScene(currentSceneName).OnSceneUnLoaded();
            
            //允许场景切换
            async.allowSceneActivation = true;
            
            //等待场景真正加载完毕
            while (!async.isDone)
            {
                yield return new WaitForFixedUpdate();
            }

            currentSceneName = newScene.sceneName;
            
            newScene.OnSceneLoad();
            
            onEnterNewScene?.Invoke();
        }
        

        #endregion
        
        
        /// <summary>
        /// 当前SceneInfo,可以添加事件响应函数
        /// </summary>
        public static AbstractSceneInfo CurrentScene => AbstractSceneInfo.GetScene(currentSceneName);
        
        /// <summary>
        /// 当前场景名字
        /// </summary>
        public static string CurrentSceneName => currentSceneName;

        /// <summary>
        /// 无资源加载场景（伪同步）
        /// </summary>
        /// <param CharacterName="newSceneName">新场景名字</param>
        /// <param CharacterName="onLeaveOldScene">离开原场景回调</param>
        /// <param CharacterName="onEnterNewScene">进入新场景回调</param>
        public static void LoadScene(string newSceneName, Action onLeaveOldScene = null, Action onEnterNewScene = null)
        {
            if(currentSceneName==newSceneName)
                return;
            
            MainLoop.Instance.StartCoroutine(_LoadScene(AbstractSceneInfo.GetScene(newSceneName), 
                onLeaveOldScene,
                onEnterNewScene));
        }

        /// <summary>
        /// 加载场景，并加载新UI
        /// </summary>
        /// <param CharacterName="newSceneName"></param>
        /// <param CharacterName="newPanelName"></param>
        public static void LoadScene(string newSceneName, string newPanelName)
        {
            if(currentSceneName==newSceneName)
                return;
            
            MainLoop.Instance.StartCoroutine(_LoadScene(AbstractSceneInfo.GetScene(newSceneName), 
                null,
                ()=>PanelMgr.PushPanel(newPanelName)));
        }

        /// <summary>
        /// 异步无资源加载场景
        /// </summary>
        /// <param CharacterName="newSceneName">新场景名字</param>
        /// <param CharacterName="onLoading">正在加载的回调</param>
        /// <param CharacterName="onLeaveOldScene">离开原场景回调</param>
        /// <param CharacterName="onEnterNewScene">进入新场景回调</param>
        public static void LoadSceneAsync(string newSceneName, bool stopTime=true, Action<int> onLoading=null,Action onLeaveOldScene=null,Action onEnterNewScene=null)
        {
            if(currentSceneName==newSceneName)
                return;
            
            MainLoop.Instance.StartCoroutine(_LoadSceneAsync(AbstractSceneInfo.GetScene(newSceneName),
                stopTime,
                onLoading,
                onLeaveOldScene,
                onEnterNewScene));
        }

        
        /// <summary>
        /// 打开已经开启的场景
        /// </summary>
        public static void LoadActiveScene()
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            AbstractSceneInfo.GetScene(currentSceneName).OnSceneLoad();
        }
    }
}
