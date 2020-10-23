using System;
using System.Collections.Generic;

using Core.Systems;

using Core.Data.Loaders;

/// <summary>
/// 配置
/// </summary>
namespace Config {

	/// <summary>
	/// 资源相关
	/// </summary>
	public class Asset {

		/// <summary>
		/// 资源类型枚举
		/// </summary>
		public enum Type {
			Animation, Character, Bust, Battler
		}

		/// <summary>
		/// 资源类型
		/// </summary>
		public static AssetSetting[] Settings = new AssetSetting[] {
			new AssetSetting(Type.Animation,
				AssetLoader.SystemPath, "Animations", "Animation_{0}"),
			new AssetSetting(Type.Character, "Characters/", "Character_{0}"),
			new AssetSetting(Type.Bust, "Busts/", "Bust_{0}"),
			new AssetSetting(Type.Battler, "Battlers/", "Battler_{0}"),
		};

	}

}