using HighFive.Model.Person;
using UnityEngine;

namespace HighFive.Model
{
    /// <summary>
    /// 子弹类
    /// 需求：
    ///    子弹：
    ///         1、造成伤害
    ///             input:damageScale,HighFivePerson
    ///         2、特效
    ///             击中音效、爆炸震屏、粒子效果
    ///         3、打到地面回调，打到敌人回调
    ///         4、Input:
    ///              enemyLayer,terrainLayer
    ///         5、单体伤害/AOE伤害，半径多少
    ///         6、子弹如何飞行
    /// 
    ///         
    ///         
    /// 
    ///     直线子弹：
    ///         input：dir,speed,shotThrough
    ///         思路：无视重力，设置初始速度后，匀速直线飞行
    ///     抛物线子弹：
    ///         input:initialSpeed,gravityScale
    ///         思路：考虑重力，速度会受重力影响，抛物线运动
    ///     追踪子弹：
    ///         input:target,speed
    ///         思路：
    ///     弹道子弹：
    ///         input：具体看情况
    ///         思路：使用数学公式计算其每一时刻速度
    /// </summary>
    public class AbstractBullet:MonoBehaviour
    {
        protected IHighFivePerson selfPerson;
        
        
    }
}