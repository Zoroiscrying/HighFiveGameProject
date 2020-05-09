using HighFive.Control.Movers.Interfaces;
using UnityEngine;

namespace Game.Scripts
{
    public class TempActorMover:TempMover,IActorBaseControl
    {
        public void SetMovable(bool enableMove)
        {
            Rig.isKinematic = !enableMove;
        }

        public bool IsGrounded => false;
        public float VelocityX
        {
            get => Velocity.x;
            set => Velocity = new Vector2(value, VelocityY);
        }
        public float VelocityY
        {
            get => Velocity.y;
            set => Velocity = new Vector2(VelocityX, value);
        }
        public void ChangeVelBasedOnHitDir(Vector2 hitDir, float multiplier = 1)
        {
            Velocity = hitDir.normalized * multiplier;
        }

        public void ReverseMovementInputX()
        {
            throw new System.NotImplementedException();
        }

        public void ReverseMovementInputY()
        {
            throw new System.NotImplementedException();
        }

        public void MoveToward(Vector2 target)
        {
            throw new System.NotImplementedException();
        }

        public void StopHorizontallyInput()
        {
            throw new System.NotImplementedException();
        }

        public void StopVerticallyInput()
        {
            throw new System.NotImplementedException();
        }

        public void StopMoverInput()
        {
            throw new System.NotImplementedException();
        }
    }
}