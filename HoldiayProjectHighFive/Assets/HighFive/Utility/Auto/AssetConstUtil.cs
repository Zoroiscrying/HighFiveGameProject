namespace HighFive.Utility
{
	/// <summary>
	/// 这个类提供了Resources下文件名和路径字典访问方式，同名资源可能引起bug
	/// </summary>
	public class AssetConstUtil : ReadyGamerOne.MemorySystem.AssetConstUtil<AssetConstUtil>
	{
		private System.Collections.Generic.Dictionary<string,string> nameToPath 
			= new System.Collections.Generic.Dictionary<string,string>{
					{ @"ImpuseGenerator Raw Signal" , @"CinemachineNoiseSetting\ImpuseGenerator Raw Signal" },
					{ @"Au1" , @"ClassAudio\Au1" },
					{ @"Au2" , @"ClassAudio\Au2" },
					{ @"Au3" , @"ClassAudio\Au3" },
					{ @"Au4" , @"ClassAudio\Au4" },
					{ @"Au5" , @"ClassAudio\Au5" },
					{ @"beiji" , @"ClassAudio\beiji" },
					{ @"Bg" , @"ClassAudio\Bg" },
					{ @"Fuck" , @"ClassAudio\Fuck" },
					{ @"CharacterData" , @"ClassFile\CharacterData" },
					{ @"DataConfig" , @"ClassFile\DataConfig" },
					{ @"EnemyData" , @"ClassFile\EnemyData" },
					{ @"ItemData" , @"ClassFile\ItemData" },
					{ @"PersonData" , @"ClassFile\PersonData" },
					{ @"RankData" , @"ClassFile\RankData" },
					{ @"ShopItemData" , @"ClassFile\ShopItemData" },
					{ @"AlienHead" , @"ClassItem\AlienHead" },
					{ @"Bear" , @"ClassItem\Bear" },
					{ @"Hook" , @"ClassItem\Hook" },
					{ @"Knife" , @"ClassItem\Knife" },
					{ @"Leave" , @"ClassItem\Leave" },
					{ @"GoldHeart" , @"ClassItem\ShopItem\GoldHeart" },
					{ @"LittleBoy" , @"ClassItem\ShopItem\LittleBoy" },
					{ @"texKnurlIcon" , @"ClassItem\ShopItem\texKnurlIcon" },
					{ @"Snowflower" , @"ClassItem\Snowflower" },
					{ @"texGravBootsIcon" , @"ClassItem\texGravBootsIcon" },
					{ @"texTeslaCoilIcon" , @"ClassItem\texTeslaCoilIcon" },
					{ @"Battle" , @"ClassPanel\Battle" },
					{ @"Loading" , @"ClassPanel\Loading" },
					{ @"Package" , @"ClassPanel\Package" },
					{ @"Shop" , @"ClassPanel\Shop" },
					{ @"Sworder" , @"ClassPerson\TypeCharacter\Sworder" },
					{ @"Boner" , @"ClassPerson\TypeEnemy\Boner" },
					{ @"Spider" , @"ClassPerson\TypeEnemy\Spider" },
					{ @"bullet_1" , @"ClassPrefab\Bullet\bullet_1" },
					{ @"ImpuseGenerator" , @"ClassPrefab\GameObjects\ImpuseGenerator" },
					{ @"OnHitParticle" , @"ClassPrefab\GameObjects\OnHitParticle" },
					{ @"SwordLight" , @"ClassPrefab\GameObjects\SwordLight" },
					{ @"Slot" , @"ClassPrefab\Ui\Slot" },
					{ @"Image_ItemBk" , @"ClassUi\Image_ItemBk" },
					{ @"Image_ItemInfo" , @"ClassUi\Image_ItemInfo" },
					{ @"Image_ItemUI" , @"ClassUi\Image_ItemUI" },
					{ @"Image_MiniMapBackGround" , @"ClassUi\Image_MiniMapBackGround" },
					{ @"Image_Slot" , @"ClassUi\Image_Slot" },
					{ @"Slider" , @"ClassUi\Slider" },
					{ @"Text_Number" , @"ClassUi\Text_Number" },
					{ @"OnHitParticleMat" , @"Materials\OnHitParticleMat" },
				};
		public override System.Collections.Generic.Dictionary<string,string> NameToPath => nameToPath;
	}
}
