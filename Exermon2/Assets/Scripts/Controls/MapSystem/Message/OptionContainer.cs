
using UnityEngine;

using MapModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.MapSystem.Controls {

	/// <summary>
	/// 选项容器
	/// </summary>
	[RequireComponent(typeof(MessageDisplay))]
	public class OptionContainer : SelectableContainerDisplay<DialogOption>{
		
		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		[HideInInspector]
		public MessageDisplay messageDisplay;
	}
}
