
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Systems;
using Core.Data.Loaders;

/// <summary>
/// 地图模块
/// </summary>
namespace MapModule { }

/// <summary>
/// ITU模块数据
/// </summary>
namespace MapModule.Data {

	/// <summary>
	/// 事件
	/// </summary>
	public class Event : BaseData {

		/// <summary>
		/// 触发类型
		/// </summary>
		public enum TriggerType {

			Never, // 不会触发

			CollEnter, // 碰撞开始
			CollStay, // 碰撞盒内持续
			CollSearch, // 碰撞盒内按下搜索键
			CollExit, // 碰撞结束

			Always, // 总是执行
		}

		/// <summary>
		/// 条件函数
		/// </summary>
		/// <returns></returns>
		public delegate bool Condition();

		/// <summary>
		/// 触发类型
		/// </summary>
		public TriggerType triggerType { get; protected set; } = TriggerType.Never;

		/// <summary>
		/// 条件
		/// </summary>
		public List<Condition> conditions { get; protected set; } = new List<Condition>();

		/// <summary>
		/// 事件
		/// </summary>
		public List<UnityAction> actions { get; protected set; } = new List<UnityAction>();

		/// <summary>
		/// 显示的图片
		/// </summary>
		public Sprite picture { get; protected set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		public Event() { }
		public Event(TriggerType triggerType,
			List<Condition> conditions = null, List<UnityAction> actions = null) {
			this.triggerType = triggerType;
			if (conditions != null) this.conditions = conditions;
			if (actions != null) this.actions = actions;
		}

		/// <summary>
		/// 是否在条件内
		/// </summary>
		/// <returns></returns>
		public bool isValid() {
			foreach (var cond in conditions)
				if (cond != null && !cond.Invoke()) return false;
			return true;
		}

		/// <summary>
		/// 处理行动事件
		/// </summary>
		public void process() {
			foreach (var action in actions) action?.Invoke();
		}
	}

	/// <summary>
	/// 行走人物（处理行走相关的静态变量/方法）
	/// </summary>
	public class RuntimeCharacter : RuntimeData {

		#region 静态定义

		/// <summary>
		/// 计算常量定义
		/// </summary>
		const float ZeroVelocityThreshold = 0.001f;
		static readonly float Sqrt2 = 1f / Mathf.Sqrt(2);

		static float DefaultMoveSpeed = 2;

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
		/// 朝向
		/// </summary>
		public enum Direction {
			LeftUp = 7, Up = 8, RightUp = 9,
			Left = 4, None = 5, Right = 6,
			LeftDown = 1, Down = 2, RightDown = 3
		}

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
		public static Vector2 judgeVector(Direction d) {
			var index = (int)d - 1;
			return new Vector2(dirX[index], dirY[index]);
		}

		#endregion

		/// <summary>
		/// 状态
		/// </summary>
		public enum State {
			Idle, // 空闲
			Moving, // 移动中
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public Vector2 velocity { get; protected set; } = new Vector2();
		[AutoConvert]
		public Direction direction { get; protected set; } = Direction.Down;

		[AutoConvert]
		public int state {
			get => stateMachine.state;
			set { stateMachine.changeState(value); }
		}

		/// <summary>
		/// 状态机
		/// </summary>
		public StateMachine stateMachine { get; protected set; } = new StateMachine();

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		public RuntimeCharacter() { initialize(); }

		/// <summary>
		/// 初始化
		/// </summary>
		void initialize() {
			initializeStates();
		}

		/// <summary>
		/// 初始化状态
		/// </summary>
		protected virtual void initializeStates() {
			addStateDict(State.Idle, updateIdle);
			addStateDict(State.Moving, updateMoving);

			changeState(State.Idle);
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
		public bool isMoving() {
			return velocity.magnitude > ZeroVelocityThreshold;
		}

		/// <summary>
		/// 是否空闲
		/// </summary>
		/// <returns></returns>
		public virtual bool isIdle() {
			return !isMoving();
		}

		#endregion

		#region 属性控制

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		public virtual float moveSpeed() {
			return DefaultMoveSpeed;
		}

		#endregion

		#region 移动控制

		/// <summary>
		/// 转移位置
		/// </summary>
		Vector2? _transferPoint = null;
		public Vector2? transferPoint {
			get {
				Vector2? res = _transferPoint;
				_transferPoint = null; return res;
			}
		}

		/// <summary>
		/// 转移到指定位置
		/// </summary>
		/// <param name="x">x坐标</param>
		/// <param name="y">y坐标</param>
		public void transfer(float x, float y, bool force = false) {
			if (!force && !isMoveable()) return;
			_transferPoint = new Vector2(x, y);
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
			velocity = vec;
		}

		/// <summary>
		/// 朝一个方向移动
		/// </summary>
		/// <param name="d"></param>
		public void moveDirection(Direction d, 
			float speed = -1, bool force = false) {
			if (speed < 0) speed = moveSpeed();
			var vec = judgeVector(d) * speed;
			move(vec, force);
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void stop() {
			velocity = Vector2.zero;
		}

		#endregion

		#region 状态控制

		/// <summary>
		/// 注册状态更新函数
		/// </summary>
		/// <param name="state">状态</param>
		/// <param name="action">行动</param>
		public void addStateDict(Enum state, UnityAction action) {
			stateMachine.addStateDict(state, action);
		}

		/// <summary>
		/// 注册状态更新函数
		/// </summary>
		/// <param name="state">状态</param>
		/// <param name="action">行动</param>
		public void addStateChange(Enum from, Enum to, UnityAction action) {
			stateMachine.addStateChange(from, to, action);
		}

		/// <summary>
		/// 更换状态
		/// </summary>
		/// <param name="state"></param>
		public void changeState(Enum state) {
			Debug.Log(this + " changeState: " + this.state + " => " + state);
			stateMachine.changeState(state);
		}

		/// <summary>
		/// 更换状态
		/// </summary>
		/// <param name="state"></param>
		public bool isState(Enum state) {
			return stateMachine.isState(state);
		}

		#region 状态更新

		/// <summary>
		/// 更新空闲状态
		/// </summary>
		protected virtual void updateIdle() {
			if (isMoving()) changeState(State.Moving);
		}

		/// <summary>
		/// 更新移动状态
		/// </summary>
		protected virtual void updateMoving() {
			if (!isMoving()) changeState(State.Idle);
		}

		#endregion

		#endregion

		#region 更新

		/// <summary>
		/// 每帧更新
		/// </summary>
		/// <param name="round">回合数</param>
		public virtual void update() {
			updateStateMachine();
			updateDirection();
		}

		/// <summary>
		/// 更新状态机
		/// </summary>
		void updateStateMachine() {
			stateMachine.update();
		}

		/// <summary>
		/// 更新朝向
		/// </summary>
		void updateDirection() {
			if (isMoving()) direction = judgeDirection(velocity);
		}

		#endregion
	}
}

