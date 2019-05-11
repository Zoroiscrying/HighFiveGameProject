﻿using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.MemorySystem;
using UnityEngine;

namespace Game.Model.SpriteObjSystem
{
    /// <summary>
    /// 2D图片基类
    /// </summary>
    public abstract class AbstractSpriteObj
    {
        protected GameObject obj;

        protected AbstractSpriteObj(string path, Vector3 pos, Transform parent = null)
        {
            obj = MemoryMgr.Instantiate(path, pos, Quaternion.identity, parent);
            if(obj==null)
            {
                throw new Exception("子弹生成失败");
            }
            MainLoop.Instance.AddUpdateFunc(Update);
        }

        protected virtual void Update()
        {

        }


        public virtual void Release()
        {
            MainLoop.Instance.RemoveUpdateFunc(Update);
        }

        protected void DestoryThis()
        {
            if (this.obj == null)
                return;
            Release();
            //throw new Exception("怎么来的？");
            GameObject.Destroy(this.obj);

        }
    }
}
