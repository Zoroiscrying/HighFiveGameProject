using ReadyGamerOne.Rougelike.Person;
using HighFive.Const;
namespace HighFive.Model.Person
{
	[UsePersonController(typeof(SworderController))]
	public partial class Sworder :
		HighFiveCharacter<Sworder>
	{
		public override string ResPath => PersonName.Sworder;
	}
}
