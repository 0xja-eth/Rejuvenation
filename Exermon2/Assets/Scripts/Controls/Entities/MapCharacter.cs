
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

namespace UI.Common.Controls.Entities {

	using Common.Controls.AnimationSystem;

	/// <summary>
	/// 地图上的行走实体
	/// </summary>
	[RequireComponent(typeof(AnimatorExtend))]
	public class MapCharacter : MapEntity {

		/// <summary>
		/// 移动
		/// </summary>
		protected const string MovingAttr = "moving";

		/// <summary>
		/// 朝向
		/// </summary>
		public enum Direction {
			LeftUp = 7,   Up = 8,   RightUp = 9,
			Left = 4,     None = 5, Right = 6,
			LeftDown = 1, Down = 2, RightDown = 3
		}

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public float moveSpeed = 0.5f;

		/// <summary>
		/// 内部控件设置
		/// </summary>
		protected AnimatorExtend animator;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		protected bool moving = false;

		/// <summary>
		/// 移动目标位置
		/// </summary>
		float targetX_ = 0, targetY_ = 0;
		protected float targetX {
			get => targetX_;
			set { targetX_ = value; checkMoveStart(); }
		}
		protected float targetY {
			get => targetY_;
			set { targetY_ = value; checkMoveStart(); }
		}

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			targetX = x; targetY = y;
			animator = SceneUtils.get<AnimatorExtend>(gameObject);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
		}

		/// <summary>
		/// 更新移动
		/// </summary>
		void updateMoving() {
			if (!isMoving()) return;

			processMoveOnUpdate();
			checkMoveEnd();
		}

		/// <summary>
		/// 执行一帧的移动
		/// </summary>
		void processMoveOnUpdate() {
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 检查是否开始移动
		/// </summary>
		/// <returns></returns>
		void checkMoveStart() {
			if (isMoving() && !moving) onMoveStart();
		}

		/// <summary>
		/// 检查是否结束移动
		/// </summary>
		/// <returns></returns>
		void checkMoveEnd() {
			if (!isMoving() && moving) onMoveEnd();
		}

		/// <summary>
		/// 开始移动回调
		/// </summary>
		protected virtual void onMoveStart() {
			animator.setVar("moving", true);
			moving = true;
		}

		/// <summary>
		/// 结束移动回调
		/// </summary>
		protected virtual void onMoveEnd() {
			animator.setVar("moving", false);
		}

		#endregion

		#region 移动控制

		/// <summary>
		/// 判断方向（静态）
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Direction judgeDirection(float x, float y) {
			if (x == 0 && y > 0) return Direction.Up;
			if (x == 0 && y < 0) return Direction.Down;

			if (x > 0 && y == 0) return Direction.Right;
			if (x > 0 && y > 0) return Direction.RightUp;
			if (x > 0 && y < 0) return Direction.RightDown;

			if (x < 0 && y == 0) return Direction.Left;
			if (x < 0 && y > 0) return Direction.LeftUp;
			if (x < 0 && y < 0) return Direction.LeftDown;

			return Direction.None;
		}

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public virtual bool isMoveable() {
			return true;
		}

		/// <summary>
		/// 是否移动中
		/// </summary>
		/// <returns></returns>
		public virtual bool isMoving() {
			return x != targetX || y != targetY;
		}

		/// <summary>
		/// 移动
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void moveDelta(float x, float y, bool immediately = false) {
			if (!isMoveable()) return;
			moveDeltaForce(x, y, immediately);
		}
		public void moveDeltaForce(float x, float y, bool immediately = false) {
			if (immediately)

			x *= moveSpeed; y *= moveSpeed;
			targetX += x; targetY += y;
		}



		/// <summary>
		/// 朝一个方向移动
		/// </summary>
		/// <param name="d"></param>
		public void moveDirection(Direction d) {

		}

		#endregion
	}
}