using System;
using System.Collections.Generic;

using Core.Systems;

using Core.Data.Loaders;

/// <summary>
/// 核心配置
/// </summary>
public static class CoreConfig {

	#region 资源相关

	/// <summary>
	/// 资源类型
	/// </summary>
	public static AssetSetting[] AssetSettings = new AssetSetting[] {
		new AssetSetting(AssetLoader.AnimationName, 
			AssetLoader.SystemPath, "Animations", "Animation_{0}")
	};

	#endregion

}