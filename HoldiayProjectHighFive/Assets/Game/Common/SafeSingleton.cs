﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Common
{
    /// <summary>
    /// 安全单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SafeSingleton<T>
        where T : class, new()
    {
        private static bool isInsciated = false;

        public SafeSingleton()
        {
            if (isInsciated)
                throw new Exception("这是安全单例，不可多次初始化");
            isInsciated = true;
        }
    }
}
