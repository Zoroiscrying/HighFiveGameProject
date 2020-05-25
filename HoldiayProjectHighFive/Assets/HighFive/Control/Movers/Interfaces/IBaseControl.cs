using System.Collections.Generic;
using ReadyGamerOne.Rougelike.Mover;
using UnityEngine;

namespace HighFive.Control.Movers.Interfaces
{
    public interface IBaseControl:IMover2D
    {
        /// <summary>
        /// 检测指定方向指定距离内是否有碰撞
        /// </summary>
        /// <param name="dir">方向向量，只取方向</param>
        /// <param name="distance">检测距离，如果为null，就是float.Max</param>
        /// <param name="layers">检测Layer，如果为null，就使用Mover自带的CollisionLayers</param>
        /// <returns></returns>
        IEnumerable<GameObject> Raycast(Vector2 dir, float? distance, LayerMask? layers);
        /// <summary>
        /// 控制角色移动的输入
        /// </summary>
        Vector2 MoverInput { set; }
    }
}