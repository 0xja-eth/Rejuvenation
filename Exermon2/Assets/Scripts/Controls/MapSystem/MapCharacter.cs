using System;

using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using MapModule.Data;

namespace UI.MapSystem.Controls {

	using Common.Controls.AnimationSystem;

	/// <summary>
	/// 地图上的行走实体（可移动的实体，拥有刚体组件）
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

		/// <summary>
		/// 载具
		/// </summary>
		public MapVehicle vehicle { get; set; } // 对外不能用 set

		#region 初始化

		/// <summary>
		/// 启动
		/// </summary>
		protected override void start() {
			base.start();
			configureStateChanges();
		}

		/// <summary>
		/// 配置运行时角色
		/// </summary>
		protected virtual void configureStateChanges(bool isSetup = true) {
            if (isSetup) {
                runtimeCharacter?.addStateEnter(
                  RuntimeCharacter.State.Moving, onMoveStart);
                runtimeCharacter?.addStateExit(
                    RuntimeCharacter.State.Moving, onMoveEnd);
                runtimeCharacter?.addStateDict(
                    RuntimeCharacter.State.Moving, onMove);
            }
            else {
                runtimeCharacter?.removeStateEnter(
                    RuntimeCharacter.State.Moving, onMoveStart);
                runtimeCharacter?.removeStateExit(
                    RuntimeCharacter.State.Moving, onMoveEnd);
                runtimeCharacter?.removeStateDict(
                    RuntimeCharacter.State.Moving, onMove);
            }
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
			var rot = directionController.localEulerAngles;
			rot.z = RuntimeCharacter.dir82Angle(direction);
			directionController.localEulerAngles = rot;
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

		/// <summary>
		/// 计算新坐标
		/// </summary>
		protected override float calcZCoord(Vector3 pos) {
			return vehicle ? vehicle.transform.position.z : base.calcZCoord(pos);
		}

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

		#endregion

		#region 移动操作

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		public virtual float moveSpeed() {
			return vehicle ? 0 : defaultMoveSpeed;
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

        #region 组件控制
        /// <summary>
        /// 销毁控制
        /// 销毁时删除注册函数
        /// </summary>
        /// <param name="force"></param>
        public override void destroy(bool force = false) {
            base.destroy(force);
            configureStateChanges(false);
        }
        #endregion
    }
}