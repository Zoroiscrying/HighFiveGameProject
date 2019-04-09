using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model
{
    /// <summary>
    /// 2D图片基类
    /// </summary>
    public abstract class AbstractSpriteObj
    {
        protected GameObject go;

        protected AbstractSpriteObj(string path, Vector3 pos, Transform parent = null)
        {
            var res = Resources.Load<GameObject>(path);
            if (res == null)
                Debug.LogError("图片路径错误");
            go = GameObject.Instantiate(res, pos, Quaternion.identity, parent);

            Init();
        }

        protected virtual void Update()
        {

        }

        public virtual void Init()
        {
            MainLoop.Instance.AddUpdateFunc(Update);
        }

        public virtual void Release()
        {
            MainLoop.Instance.RemoveUpdateFunc(Update);
        }

        protected virtual void DestoryThis()
        {
            if (this.go == null)
                return;
            Release();
            GameObject.Destroy(this.go);

        }
    }
}
