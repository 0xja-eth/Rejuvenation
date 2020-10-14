
using UnityEngine;

using MapModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.MapSystem.Controls {

	/// <summary>
	/// 选项容器
	/// </summary>
	public class OptionContainer : SelectableContainerDisplay<DialogOption>{
		
		/// <summary>
		/// 外部组件设置
		/// </summary>
		public MessageDisplay messageDisplay;
	}
}
