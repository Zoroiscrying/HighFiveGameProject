using HighFive.Script;

namespace HighFive.Model.Person
{
	/// <summary>
	/// Enemy角色控制类，在角色类加UsePersonController属性的话会自动添加上去，使用InitController初始化
	/// </summary>
	public abstract class HighFiveEnemyController : HighFivePersonController
	{
		public HeadUiCanvas HeadUi;
	}
}
