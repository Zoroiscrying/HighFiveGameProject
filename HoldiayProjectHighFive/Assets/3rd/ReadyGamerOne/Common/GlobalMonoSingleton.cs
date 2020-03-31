using System;
using UnityEngine;
namespace ReadyGamerOne.Common
{
    /// <summary>
    /// MonoBehavior全局单例泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GlobalMonoSingleton<T> : MonoSingleton<T>
        where T : GlobalMonoSingleton<T>
    {
        /// <summary>
        /// 全局单例，如果重复，直接干掉
        /// </summary>
        protected override void OnStateIsOthers()
        {
            base.OnStateIsOthers();
            GameObject.DestroyImmediate(this.gameObject);
        }
    }
}
