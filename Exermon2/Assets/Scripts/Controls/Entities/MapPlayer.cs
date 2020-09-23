
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

namespace UI.Common.Controls.Entities {

	using Common.Controls.AnimationSystem;

	/// <summary>
	/// 地图上的行走实体
	/// </summary>
	public class MapPlayer : MapCharacter {

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public bool moveable = true;

		/// <summary>
		/// 属性
		/// </summary>
		protected float xDelta => Input.GetAxis("Horizontal");
		protected float yDelta => Input.GetAxis("Vertical");

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
		}

		#endregion

		#region 移动控制

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public override bool isMoveable() {
			return moveable;
		}

		/// <summary>
		/// 更新移动
		/// </summary>
		void updateMovement() {
			moveDelta(xDelta, yDelta);
		}

		#endregion
	}
}