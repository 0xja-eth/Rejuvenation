
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

namespace UI.Common.Controls.Entities {

	using Common.Controls.AnimationSystem;

	/// <summary>
	/// 地图上的玩家实体
	/// </summary>
	public class MapPlayer : MapCharacter {

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public bool moveable = true;

		/// <summary>
		/// 类型
		/// </summary>
		public override Type type => Type.Player;

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
			updateInput();
		}

		/// <summary>
		/// 更新玩家输入事件
		/// </summary>
		void updateInput() {
			updateMovement();
		}
		
		/// <summary>
		/// 更新移动
		/// </summary>
		void updateMovement() {
			if (xDelta == 0 && yDelta == 0) stop();
			else {
				var force = new Vector2(xDelta, yDelta);
				force *= Time.deltaTime * moveSpeed();

				move(force.x, force.y);
			}
		}

		#endregion

		#region 移动控制

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public override bool isMoveable() {
			return base.isMoveable() && map.active && moveable ;
		}

		#endregion
	}
}