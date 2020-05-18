using UnityEngine;

namespace HighFive.AI
{
    /// <summary>
    /// 视觉探测器
    /// </summary>
    public abstract class VisualSensor:AbstractSensor
    {
        [HideInInspector]public float viewDistance;

        protected override void OnDrawGizmos()
        {
            if (SelfPerson == null||!SelfPerson.IsAlive)
                return;
            base.OnDrawGizmos();
            Gizmos.color=Color.cyan;
            var position = CenterPosition;
            Gizmos.DrawLine(position,position+viewDistance*SelfPerson.Dir*Vector3.right);
        }
    }
}