using UnityEngine;
namespace ReadyGamerOne.Common
{
    /// <summary>
    /// MonoBehavior单例泛型类
    /// </summary>
    /// <typeparam CharacterName="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour
        where T : MonoSingleton<T>, new()
    {
        private static T instance;
        public static T Instance
        {
            get { return instance; }
        }
        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}
