
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

namespace UI.Common.Controls.Entities {

	/// <summary>
	/// 地图上的实体
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class MapEntity : WorldComponent {

		/// <summary>
		/// 属性
		/// </summary>
		public float x => transform.position.x;
		public float y => transform.position.y;

		/// <summary>
		/// 内部组件定义
		/// </summary>
		protected new Rigidbody2D rigidbody;
		protected SpriteRenderer sprite;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			rigidbody = SceneUtils.get<Rigidbody2D>(gameObject);
			sprite = SceneUtils.get<SpriteRenderer>(gameObject);
		}

		#endregion

	}
}