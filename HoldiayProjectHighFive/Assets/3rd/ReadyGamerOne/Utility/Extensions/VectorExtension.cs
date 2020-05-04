using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class VectorExtension
    {
        /// <summary>
        /// 将一个Vector3向量的XY分量旋转角度制degree
        /// </summary>
        /// <param name="self"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Vector3 RotateDegree(this Vector3 self, float degree)
        {
            var sin = Mathf.Sin(degree * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degree * Mathf.Deg2Rad);
            
            return new Vector3(
                self.x*cos -self.y*sin,
                self.x*sin+self.y*cos,
                self.z
                );
        }


        public static Vector2 RotateDegree(this Vector2 self, float degree)
        {
            var sin = Mathf.Sin(degree * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degree * Mathf.Deg2Rad);
            
            return new Vector2(
                self.x*cos -self.y*sin,
                self.x*sin+self.y*cos
            );
        }
    }
}