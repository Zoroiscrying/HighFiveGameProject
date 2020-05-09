using ReadyGamerOne.Data;
using ReadyGamerOne.Rougelike.Person;
using HighFive.Const;
namespace HighFive.Model.Person
{
	[UsePersonController(typeof(DefenderController))]
	public partial class Defender :
		HighFiveEnemy<Defender>
	{
		public override string ResKey => PersonName.Defender;
	}
}
