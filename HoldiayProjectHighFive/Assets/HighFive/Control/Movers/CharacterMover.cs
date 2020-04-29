namespace HighFive.Control.Movers
{
    /// <summary>
    /// 玩家角色移动器
    /// </summary>
    public class CharacterMover:ActorMover
    {
        #region Character_特有接口

        public virtual float VelocityX { get; set; }
        public virtual float VelocityY { get; set; }        

        #endregion

    }
}