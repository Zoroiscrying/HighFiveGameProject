using HighFive.Global;
using HighFive.Model.Person;
using ReadyGamerOne.Rougelike.Person;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace HighFive.AI
{
    /// <summary>
    /// 扇形视觉探测器
    /// </summary>
    public class SectorVisualSensor:VisualSensor
    {
        [HideInInspector]public float viewAngle;
        private RaycastHit2D[] hitInfos=new RaycastHit2D[1];
        public override IHighFivePerson Detect()
        {
            var size = Physics2D.CircleCastNonAlloc(CenterPosition, viewDistance, SelfPerson.Dir * Vector2.right, hitInfos, 0, detectLayers);
            if (size == 0)
                return null;
            foreach (var hit in hitInfos)
            {
                if (!hit) 
                    continue;
                var playerDir = hit.point - CenterPosition.ToVector2();
                var distance = playerDir.magnitude;
                var faceDir = Vector3.right * SelfPerson.Dir;
                if (Vector2.Angle(playerDir, faceDir) > viewAngle)
                    continue;
                if (hit.collider.gameObject.GetPersonInfo() is IHighFivePerson person && person.IsAlive)
                {
                    var ans = Physics2D.Raycast(CenterPosition, playerDir, distance, terrainLayers);
                    if (ans.collider == null)
                    {
                        return person;
                    }
                }
            }
            
            return null;
        }

        protected override void OnDrawGizmos()
        {
            if (null==SelfPerson||!SelfPerson.IsAlive)
            {
                return;
            }
            Gizmos.DrawWireSphere(CenterPosition, 0.1f);
            
            var startPos = CenterPosition;
            var dir = SelfPerson.Dir * viewDistance * Vector3.right;
            Gizmos.color=Color.cyan;
            Gizmos.DrawLine(startPos, startPos + dir);
            Gizmos.DrawLine(startPos, startPos + dir.RotateDegree(viewAngle));
            Gizmos.DrawLine(startPos, startPos + dir.RotateDegree(-viewAngle));
        }
    }
}