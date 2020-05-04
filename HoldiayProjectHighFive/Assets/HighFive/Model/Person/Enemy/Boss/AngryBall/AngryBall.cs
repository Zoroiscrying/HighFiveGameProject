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
		public override void LoadData(CsvMgr data)
		{
			//    如果父类没有此方法，删掉就行
			base.LoadData(data);			
			//    在这里需要使用数据对类内变量进行初始化赋值(比如_hp,_maxhp,_attack，这三项自动生成在基类中了)
			//    这个函数人物每次激活时都会调用
		}
	}
}
