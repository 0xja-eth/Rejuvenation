
using System;
using System.Collections.Generic;

using LitJson;

using UnityEngine;

using Core.Systems;

using Core.UI;
using Core.UI.Utils;

using UI.Common.Windows;
using UI.Common.Controls.MapSystem;

namespace UI.BaseMapScene {

	/// <summary>
	/// 地图场景基类
	/// </summary>
	public abstract class BaseMapScene : BaseScene {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Map map1, map2;


	}
}