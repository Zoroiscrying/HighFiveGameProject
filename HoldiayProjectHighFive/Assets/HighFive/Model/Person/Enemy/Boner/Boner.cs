using ReadyGamerOne.Rougelike.Person;
using HighFive.Const;
namespace HighFive.Model.Person
{
	[UsePersonController(typeof(BonerController))]
	public partial class Boner :
		HighFiveEnemy<Boner>
	{
		public override string ResPath => PersonName.Boner;
	}
}
