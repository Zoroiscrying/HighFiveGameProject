using System;
using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class ComponentExtension
    {
        public static T GetComponent<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            var temp = self.transform.Find(transformPath);
            if (!temp)
            {
                throw new Exception($"{self.gameObject.name} 获取组建失败：" + transformPath);
            }
            return temp.GetComponent<T>();
        }
        public static T[] GetComponents<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            var temp = self.transform.Find(transformPath);
            if (!temp)
            {
                throw new Exception($"{self.gameObject.name} 获取组建失败：" + transformPath);
            }
            return temp.GetComponents<T>();
        }
        public static T GetComponentInChildren<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            var temp = self.transform.Find(transformPath);
            if (!temp)
            {
                throw new Exception($"{self.gameObject.name} 获取组建失败：" + transformPath);
            }
            return temp.GetComponentInChildren<T>();
        }
        public static T[] GetComponentsInChildren<T>(this Component self, string transformPath)
            where T : Component
        {
            if (!self)
                return null;
            var temp = self.transform.Find(transformPath);
            if (!temp)
            {
                throw new Exception($"{self.gameObject.name} 获取组建失败：" + transformPath);
            }
            return temp.GetComponentsInChildren<T>();
        }
    }
}