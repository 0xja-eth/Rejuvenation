using System;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using MapModule.Data;

using CollFunc = UnityEngine.Events.UnityAction<UnityEngine.Collider2D>;
using CollFuncList = System.Collections.Generic.List<
	UnityEngine.Events.UnityAction<UnityEngine.Collider2D>>;

namespace UI.Common.Controls.MapSystem {

	using Common.Controls.AnimationSystem;

	/// <summary>
	/// 地图上的行走实体
	/// </summary>
	//[RequireComponent(typeof(SpriteRenderer))]
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
		/// 动画常量定义
		/// </summary>
		protected const string IdleStateName = "Idle";
		protected const string WalkStateName = "Walk";

		protected const string MovingAttrName = "moving";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Transform directionController;

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public float defaultMoveSpeed = 2; // 默认移动速度

		/// <summary>
		/// 内部控件设置
		/// </summary>
		[RequireTarget] [HideInInspector]
		public new Rigidbody2D rigidbody;
		//[RequireTarget] [HideInInspector]
		//public SpriteRenderer sprite;

		[RequireTarget] [HideInInspector]
		public AnimatorExtend animator;
		[RequireTarget] [HideInInspector]
		public new AnimationExtend animation;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		//protected bool moving = false;

		/// <summary>
		/// 类型
		/// </summary>
		public virtual Type type => Type.NPC;

		/// <summary>
		/// 运行时敌人
		/// </summary>
		RuntimeCharacter _runtimeCharacter = new RuntimeCharacter();
		public virtual RuntimeCharacter runtimeCharacter => _runtimeCharacter;

		/// <summary>
		/// 速度
		/// </summary>
		public Vector2 velocity => runtimeCharacter.velocity;

		/// <summary>
		/// 朝向
		/// </summary>
		public RuntimeCharacter.Direction direction 
			=> runtimeCharacter.direction;

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

		/// <summary>
		/// 启动
		/// </summary>
		protected override void start() {
			base.start();
			setupStateChanges();
		}

		/// <summary>
		/// 配置运行时角色
		/// </summary>
		protected virtual void setupStateChanges() {
			runtimeCharacter?.addStateChange(
				RuntimeCharacter.State.Idle,
				RuntimeCharacter.State.Moving, onMoveStart);
			runtimeCharacter?.addStateChange(
				RuntimeCharacter.State.Moving,
				RuntimeCharacter.State.Idle, onMoveEnd);
			runtimeCharacter?.addStateDict(
				RuntimeCharacter.State.Moving, onMove);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateCharacter();
			updatePosition();
			updateDirection();
			updateVelocity();
		}

		/// <summary>
		/// 更新战斗者
		/// </summary>
		void updateCharacter() {
			runtimeCharacter.update();
		}

		/// <summary>
		/// 更新位置
		/// </summary>
		void updatePosition() {
			var pos = runtimeCharacter.transferPoint;
			if (pos == null) return;

			rigidbody.position = (Vector2)pos;
		}

		/// <summary>
		/// 更新速度
		/// </summary>
		void updateVelocity() {
			rigidbody.velocity = velocity;
		}

		/// <summary>
		/// 更新朝向
		/// </summary>
		void updateDirection() {
			if (!directionController) return;
			var rot = directionController.rotation;
			rot.z = RuntimeCharacter.dir82Angle(direction);
			directionController.rotation = rot;
		}

		///// <summary>
		///// 更新移动
		///// </summary>
		//protected virtual void updateMoving() {
		//	checkMoveStart();
		//	checkMoving();
		//	checkMoveEnd();
		//}

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

		///// <summary>
		///// 检查是否开始移动
		///// </summary>
		///// <returns></returns>
		//void checkMoveStart() {
		//	if (isMoving() && !moving) onMoveStart();
		//}

		///// <summary>
		///// 检查是否结束移动
		///// </summary>
		///// <returns></returns>
		//void checkMoving() {
		//	if (isMoving() && moving) onMove();
		//}

		///// <summary>
		///// 检查是否结束移动
		///// </summary>
		///// <returns></returns>
		//void checkMoveEnd() {
		//	if (!isMoving() && moving) onMoveEnd();
		//}

		/// <summary>
		/// 开始移动回调
		/// </summary>
		protected virtual void onMoveStart() {
			animator?.setVar(MovingAttrName, true);
		}

		/// <summary>
		/// 结束移动回调
		/// </summary>
		protected virtual void onMoveEnd() {
			animator.setVar(MovingAttrName, false);
		}

		/// <summary>
		/// 移动中回调
		/// </summary>
		protected virtual void onMove() { }

		#endregion

		#region 动画控制

		/// <summary>
		/// 是否处于某动画状态
		/// </summary>
		/// <param name="state">状态名</param>
		/// <returns></returns>
		public bool isAnimationState(string state) {
			return animator != null && animator.isState(state);
		}
		public bool isPlayingIdleAnimation() {
			return isAnimationState(IdleStateName);
		}
		public bool isPlayingWalkAnimation() {
			return isAnimationState(WalkStateName);
		}

		/// <summary>
		/// 切换并播放动画
		/// </summary>
		/// <param name="state">状态名</param>
		/// <param name="attr">控制状态的属性名（Trigger）</param>
		/// <param name="animation">动画</param>
		public void playAnimation(string state, string attr, AnimationClip animation) {
			animator?.changeAni(state, animation);
			animator?.setVar(attr);
		}

		#endregion

		#region 移动控制

		#region 朝向相关

		///// <summary>
		///// 获取当前朝向
		///// </summary>
		///// <returns></returns>
		//public RuntimeCharacter.Direction currentDirection() {
		//	return RuntimeCharacter.vec2Dir8(velocity);
		//}

		///// <summary>
		///// 朝向判断
		///// </summary>
		///// <returns></returns>
		//public bool isFacingLeft() {
		//	return isFacingLeft(velocity);
		//}
		//public bool isFacingLeft(Vector2 vec) {
		//	return vec.x < 0;
		//}
		//public bool isFacingRight() {
		//	return isFacingRight(velocity);
		//}
		//public bool isFacingRight(Vector2 vec) {
		//	return vec.x > 0;
		//}
		//public bool isFacingUp() {
		//	return isFacingUp(velocity);
		//}
		//public bool isFacingUp(Vector2 vec) {
		//	return vec.y < 0;
		//}
		//public bool isFacingDown() {
		//	return isFacingDown(velocity);
		//}
		//public bool isFacingDown(Vector2 vec) {
		//	return vec.y > 0;
		//}

		///// <summary>
		///// 更新朝向
		///// </summary>
		//public void refreshFacing() {
		//	if (isFacingRight()) sprite.flipX = true;
		//	else if (isFacingLeft()) sprite.flipX = false;
		//}
		//public void refreshFacing(Vector2 vec) {
		//	if (isFacingRight(vec)) sprite.flipX = true;
		//	else if (isFacingLeft(vec)) sprite.flipX = false;
		//}
		//public void refreshFacing(RuntimeCharacter.Direction d) {
		//	var vec = RuntimeCharacter.dir82Vec(d);
		//	refreshFacing(vec);
		//}

		#endregion

		#region 状态判断

		/// <summary>
		/// 状态判断
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public bool isState(Enum state) {
			return runtimeCharacter.isState(state);
		}

		/// <summary>
		/// 状态改变（禁用）
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public void changeState(Enum state) {
			runtimeCharacter.changeState(state);
		}

		///// <summary>
		///// 是否移动中
		///// </summary>
		///// <returns></returns>
		//public virtual bool isMoving() {
		//	return 
		//}

		///// <summary>
		///// 是否空闲
		///// </summary>
		///// <returns></returns>
		//public virtual bool isIdle() {
		//	return !isMoving();
		//}

		#endregion

		#region 移动操作

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		public virtual float moveSpeed() {
			return defaultMoveSpeed;
		}

		/// <summary>
		/// 转移到指定位置
		/// </summary>
		/// <param name="x">x坐标</param>
		/// <param name="y">y坐标</param>
		public void transfer(float x, float y, bool force = false) {
			runtimeCharacter?.transfer(x, y, force);
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
			runtimeCharacter?.move(vec, force);
			//refreshFacing(vec);
		}

		/// <summary>
		/// 朝一个方向移动
		/// </summary>
		/// <param name="d"></param>
		public void moveDirection(
			RuntimeCharacter.Direction d, 
			float speed = -1, bool force = false) {
			if (speed < 0) speed = moveSpeed();
			runtimeCharacter?.moveDirection(d, speed, force);
			//refreshFacing(d);
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void stop() {
			runtimeCharacter?.stop();
		}

		#endregion

		#endregion

		#region 绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			//refreshFacing();
		}

		#endregion
	}
}