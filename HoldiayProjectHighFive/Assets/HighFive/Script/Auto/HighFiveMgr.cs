using ReadyGamerOne.Script;
namespace HighFive.Script
{
	public partial class HighFiveMgr : AbstractGameMgr<HighFiveMgr>
	{
		partial void OnSafeAwake();
		protected override void Awake()
		{
			base.Awake();
			OnSafeAwake();
		}
	}
}
