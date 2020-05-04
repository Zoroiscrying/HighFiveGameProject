using ReadyGamerOne.Script;
using ReadyGamerOne.MemorySystem;
namespace HighFive.Script
{
	public partial class HighFiveMgr : AbstractGameMgr<HighFiveMgr>
	{
		protected override IResourceLoader ResourceLoader => ResourcesResourceLoader.Instance;
		protected override IAssetConstUtil AssetConstUtil => Utility.AssetConstUtil.Instance;
		partial void OnSafeAwake();

		protected override void OnStateIsNull()
		{
			base.OnStateIsNull();
			OnSafeAwake();
		}
	}
}
