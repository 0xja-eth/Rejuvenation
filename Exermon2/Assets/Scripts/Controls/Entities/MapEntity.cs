
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

namespace UI.Common.Controls.Entities {

	/// <summary>
	/// 地图上的实体
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class MapEntity : WorldComponent {

		/// <summary>
		/// 属性
		/// </summary>
		public float x => transform.position.x;
		public float y => transform.position.y;

		/// <summary>
		/// 内部控件设置
		/// </summary>
		[RequireTarget]
		protected new Collider2D collider;
		
	}
}