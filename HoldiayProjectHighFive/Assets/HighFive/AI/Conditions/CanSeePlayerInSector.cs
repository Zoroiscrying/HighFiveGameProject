using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace HighFive.AI.Conditions
{
    [TaskDescription("使用曲线控制Y向速度的方式，在指定时间内往指定方向跳跃指定距离")]
    public class CanSeePlayerInSector:Detector
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("视觉半径")]
        public SharedFloat viewDistance;
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("视觉角度")]
        public SharedFloat viewAngle;
        
        protected override bool InitSensor(AbstractSensor sensor)
        {
            if (sensor is SectorVisualSensor sectorVisualSensor)
            {
                sectorVisualSensor.viewDistance = viewDistance.Value;
                sectorVisualSensor.viewAngle = viewAngle.Value;
                return true;
            }

            return false;
        }
    }
}