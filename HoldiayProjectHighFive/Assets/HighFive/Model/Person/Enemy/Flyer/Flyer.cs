using ReadyGamerOne.Data;
using ReadyGamerOne.Rougelike.Person;
using HighFive.Const;
namespace HighFive.Model.Person
{
	[UsePersonController(typeof(FlyerController))]
	public partial class Flyer :
		HighFiveEnemy<Flyer>
	{
		public override string ResKey => PersonName.Flyer;
	}
}
