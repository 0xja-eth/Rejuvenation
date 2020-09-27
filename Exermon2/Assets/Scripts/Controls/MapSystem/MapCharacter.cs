using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using CollFunc = UnityEngine.Events.UnityAction<UnityEngine.Collider2D>;
using CollFuncList = System.Collections.Generic.List<
	UnityEngine.Events.UnityAction<UnityEngine.Collider2D>>;

namespace UI.Common.Controls.MapSystem {

	using Common.Controls.AnimationSystem;

	/// <summary>
	/// 地图上的行走实体
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(AnimatorExtend))]
	[RequireComponent(typeof(AnimationExtend))]
	public class MapCharacter : MapEntity {

		/// <summary>
		/// 类型
		/// </summary>
		public enum Type {
			NPC, Enemy, Player
		}

		/// <summary>
		/// 常量定义
		/// </summary>
		const float ZeroVelocityThreshold = 0.001f;
		static readonly float Sqrt2 = Mathf.Sqrt(2);

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
		/// 方向位移
		/// </summary>
		public static readonly float[] dirX = new float[] {
			-Sqrt2, 0, Sqrt2, -1, 0, 1, -Sqrt2, 0, Sqrt2
		};
		public static readonly float[] dirY = new float[] {
			-Sqrt2, -1, -Sqrt2, 0, 0, 0, Sqrt2, 1, Sqrt2
		};

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public float defaultMoveSpeed = 2; // 默认移动速度

		/// <summary>
		/// 内部控件设置
		/// </summary>
		[RequireTarget] [HideInInspector]
		public new Rigidbody2D rigidbody;
		[RequireTarget] [HideInInspector]
		public SpriteRenderer sprite;

		[RequireTarget] [HideInInspector]
		public AnimatorExtend animator;
		[RequireTarget] [HideInInspector]
		public new AnimationExtend animation;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		protected bool moving = false;

		/// <summary>
		/// 类型
		/// </summary>
		public virtual Type type => Type.NPC;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeCollFuncs();
		}

		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected virtual void initializeCollFuncs() { }

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateMoving();
		}

		/// <summary>
		/// 更新移动
		/// </summary>
		void updateMoving() {
			checkMoveStart();
			checkMoving();
			checkMoveEnd();
		}

		#endregion

		#region 碰撞处理

		/// <summary>
		/// 各种碰撞处理函数
		/// </summary>
		CollFuncList onEnterFuncs = new CollFuncList();
		CollFuncList onStayFuncs = new CollFuncList();
		CollFuncList onExitFuncs = new CollFuncList();

		/// <summary>
		/// 注册碰撞处理函数
		/// </summary>
		/// <typeparam name="T">物品类型</typeparam>
		/// <param name="func">绘制函数</param>
		public void registerOnEnterFunc<T>(
			UnityAction<T> func) where T : WorldComponent {
			registerCollFunc(onEnterFuncs, func);
		}
		public void registerOnStayFunc<T>(
			UnityAction<T> func) where T : WorldComponent {
			registerCollFunc(onStayFuncs, func);
		}
		public void registerOnExitFunc<T>(
			UnityAction<T> func) where T : WorldComponent {
			registerCollFunc(onExitFuncs, func);
		}
		void registerCollFunc<T>(CollFuncList funcs,
			UnityAction<T> func) where T : WorldComponent {

			CollFunc func_ = (coll) => {
				var item = SceneUtils.get<T>(coll);
				if (item != null) func?.Invoke(item);
			};
			funcs.Add(func_);
		}

		/// <summary>
		/// 碰撞开始
		/// </summary>
		/// <param name="collision"></param>
		void OnTriggerEnter2D(Collider2D collision) {
			onTrigger(collision, onEnterFuncs);
		}

		/// <summary>
		/// 碰撞持续
		/// </summary>
		/// <param name="collision"></param>
		void OnTriggerStay2D(Collider2D collision) {
			onTrigger(collision, onStayFuncs);
		}

		/// <summary>
		/// 碰撞结束
		/// </summary>
		/// <param name="collision"></param>
		void OnTriggerExit2D(Collider2D collision) {
			onTrigger(collision, onExitFuncs);
		}

		/// <summary>
		/// 统一碰撞回调处理
		/// </summary>
		/// <param name="collision"></param>
		void onTrigger(Collider2D collision, CollFuncList funcs) {
			foreach (var func in funcs) func.Invoke(collision);
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
		void checkMoving() {
			if (isMoving() && moving) onMove();
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
			animator?.setVar("moving", moving = true);
		}

		/// <summary>
		/// 结束移动回调
		/// </summary>
		protected virtual void onMoveEnd() {
			animator.setVar("moving", moving = false);
		}

		/// <summary>
		/// 移动中回调
		/// </summary>
		protected virtual void onMove() { }

		#endregion

		#region 移动控制

		#region 朝向相关
		
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
		public static Direction judgeDirection(Vector2 vec) {
			return judgeDirection(vec.x, vec.y);
		}

		/// <summary>
		/// 获取当前朝向
		/// </summary>
		/// <returns></returns>
		public Direction currentDirection() {
			return judgeDirection(currentVelocity());
		}

		/// <summary>
		/// 朝向判断
		/// </summary>
		/// <returns></returns>
		public bool isFacingLeft() {
			return isFacingLeft(currentVelocity());
		}
		public bool isFacingLeft(Vector2 vec) {
			return vec.x < 0;
		}
		public bool isFacingRight() {
			return isFacingRight(currentVelocity());
		}
		public bool isFacingRight(Vector2 vec) {
			return vec.x > 0;
		}
		public bool isFacingUp() {
			return isFacingUp(currentVelocity());
		}
		public bool isFacingUp(Vector2 vec) {
			return vec.y < 0;
		}
		public bool isFacingDown() {
			return isFacingDown(currentVelocity());
		}
		public bool isFacingDown(Vector2 vec) {
			return vec.y > 0;
		}

		/// <summary>
		/// 更新朝向
		/// </summary>
		public void refreshFacing() {
			if (isFacingRight()) sprite.flipX = true;
			else if (isFacingLeft()) sprite.flipX = false;
		}
		public void refreshFacing(Vector2 vec) {
			if (isFacingRight(vec)) sprite.flipX = true;
			else if (isFacingLeft(vec)) sprite.flipX = false;
		}

		#endregion

		#region 状态判断

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public virtual bool isMoveable() {
			return true;
		}

		/// <summary>
		/// 能否转移
		/// </summary>
		/// <returns></returns>
		public virtual bool isTransferable() {
			return isMoveable();
		}

		/// <summary>
		/// 是否移动中
		/// </summary>
		/// <returns></returns>
		public virtual bool isMoving() {
			return rigidbody.velocity.magnitude > ZeroVelocityThreshold;
		}

		#endregion

		#region 属性获取

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		public virtual float moveSpeed() {
			return defaultMoveSpeed;
		}

		/// <summary>
		/// 当前速度
		/// </summary>
		/// <returns></returns>
		public virtual Vector2 currentVelocity() {
			return rigidbody.velocity;
		}

		#endregion

		#region 移动操作

		/// <summary>
		/// 转移到指定位置
		/// </summary>
		/// <param name="x">x坐标</param>
		/// <param name="y">y坐标</param>
		public void transfer(float x, float y, bool force = false) {
			if (!force && !isMoveable()) return;
			rigidbody.position = new Vector2(x, y);
		}

		/// <summary>
		/// 移动
		/// </summary>
		/// <param name="x">x速度</param>
		/// <param name="y">y速度</param>
		public void move(float x, float y, bool force = false) {
			move(new Vector2(x, y), force);
		}
		public void move(Vector2 vec, bool force = false) {
			if (!force && !isMoveable()) return;

			refreshFacing(rigidbody.velocity = vec);
		}

		/// <summary>
		/// 朝一个方向移动
		/// </summary>
		/// <param name="d"></param>
		public void moveDirection(Direction d, float speed = -1) {
			var index = (int)d - 1;
			if (speed < 0) speed = moveSpeed();
			move(dirX[index] * speed, dirY[index] * speed);
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void stop() {
			rigidbody.velocity = Vector2.zero;
		}

		#endregion

		#endregion

		#region 绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			refreshFacing();
		}

		#endregion
	}
}