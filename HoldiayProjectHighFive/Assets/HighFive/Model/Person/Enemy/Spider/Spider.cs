using ReadyGamerOne.Rougelike.Person;
using HighFive.Const;
namespace HighFive.Model.Person
{
	[UsePersonController(typeof(SpiderController))]
	public partial class Spider :
		HighFiveEnemy<Spider>
	{
		public override string ResPath => PersonName.Spider;
	}
}
