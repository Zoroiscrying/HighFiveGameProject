namespace HighFive.Model.Person
{
	public class AngryBallController :
		HighFiveBossController
	{
		public override int Dir
		{
			get => 1;
			set { }
		}

		public override void SetMoveable(bool state)
		{
			throw new System.NotImplementedException();
		}
	}
}
