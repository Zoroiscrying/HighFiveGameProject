using ReadyGamerOne.Data;
using ReadyGamerOne.Rougelike.Person;
using HighFive.Const;
namespace HighFive.Model.Person
{
	[UsePersonController(typeof(AngryBallController))]
	public partial class AngryBall :
		HighFiveBoss<AngryBall>
	{
		public override string ResKey => PersonName.AngryBall;
	}
}
