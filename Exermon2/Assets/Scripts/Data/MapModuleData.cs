
using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

using LitJson;

using Config;

using Core.Data;
using Core.Utils;
using Core.Systems;
using Core.Data.Loaders;

///// <summary>
///// Unity拓展
///// </summary>
//namespace UnityEngine.Events {

//	/// <summary>
//	/// 条件
//	/// </summary>
//	[RequiredByNativeCode]
//	public class UnityCondition : UnityEventBase {

//		public UnityCondition() { }

//		public void AddListener(UnityAction call) { }

//		public void Invoke() { }

//		public void RemoveListener(UnityAction call) { }
//		protected override MethodInfo FindMethod_Impl(string name, object targetObj) { }

//		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction) {
//			throw new NotImplementedException();
//		}
//	}

//}

/// <summary>
/// 地图模块
/// </summary>
namespace MapModule { }

/// <summary>
/// ITU模块数据
/// </summary>
namespace MapModule.Data {

	/// <summary>
	/// 时空类型
	/// </summary>
	public enum TimeType {
		Present, Past
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
        /// 行走图常量定义
        /// </summary>
        public const int XCnt = 3, YCnt = 3;

        const int DefaultPattern = 1;
        const float PatternFrequency = 0.2f;

        /// <summary>
        /// 方向位移
        /// </summary>
        public static readonly float[] dirX = new float[] {
            -Sqrt2, 0, Sqrt2, -1, 0, 1, -Sqrt2, 0, Sqrt2
        };
        public static readonly float[] dirY = new float[] {
            -Sqrt2, -1, -Sqrt2, 0, 0, 0, Sqrt2, 1, Sqrt2
        };
        public static readonly float[] angles = new float[] {
            45, 90, 135, 0, 0, 180, 315, 270, 225
        };

        /// <summary>
        /// 朝向
        /// </summary>
        public enum Direction {
            LeftUp = 7, Up = 8, RightUp = 9,
            Left = 4, None = 5, Right = 6,
            LeftDown = 1, Down = 2, RightDown = 3
        }
        public const int DirectionCount = 9;

        const float DirectionAngle = 45;

        /// <summary>
        /// 判断方向（静态）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Direction vec2Dir8(float x, float y) {
            if (x == 0 && y == 0) return Direction.None;

            if (x == 0 && y > 0) return Direction.Up;
            if (x == 0 && y < 0) return Direction.Down;

            if (x > 0 && y == 0) return Direction.Right;
            if (x < 0 && y == 0) return Direction.Left;

            var angle = getAngle(x, y);

            if (judgeAngle(angle, 0)) return Direction.Right;
            if (judgeAngle(angle, 1)) return Direction.RightUp;
            if (judgeAngle(angle, 2)) return Direction.Up;
            if (judgeAngle(angle, 3)) return Direction.LeftUp;
            if (judgeAngle(angle, 4)) return Direction.Left;
            if (judgeAngle(angle, 5)) return Direction.LeftDown;
            if (judgeAngle(angle, 6)) return Direction.Down;
            if (judgeAngle(angle, 7)) return Direction.RightDown;
			if (judgeAngle(angle, 8)) return Direction.Right;

			Debug.Log("vec2Dir => None: " + angle + "(" + x + "," + y + ")");

            return Direction.None;
        }
        static float getAngle(float x, float y) {
            var res = Mathf.Atan(y / x) / Mathf.PI * 180;

            if (x > 0 && y < 0) res = 360 + res;
            else if (x < 0) res = 180 + res;

            return res;
        }
        static bool judgeAngle(float angle, int n) {
            var stdA = DirectionAngle / 2; // 22.5
            angle = angle - n * DirectionAngle; // 标准化

            return -stdA < angle && angle <= stdA;
        }

        public static Direction vec2Dir8(Vector2 vec) {
            return vec2Dir8(vec.x, vec.y);
        }
        public static Vector2 dir82Vec(Direction d) {
            var index = (int)d - 1;
            return new Vector2(dirX[index], dirY[index]);
        }
        public static float dir82Angle(Direction d) {
            return angles[(int)d - 1];
        }
        public static int dir2Index(Direction d) {
            switch (d) {
                case Direction.Down: return 0;
                case Direction.Left:
                case Direction.Right: return 1;
                case Direction.Up: return 2;
                default: return -1;
            }
        }
        public static bool isDir4(Direction d) {
            return d == Direction.Down || d == Direction.Up ||
                d == Direction.Left || d == Direction.Right;
        }
        public static bool isRightDir(Direction d) {
            return d == Direction.Right || d == Direction.RightUp || d == Direction.RightDown;
        }
        public static bool isLeftDir(Direction d) {
            return d == Direction.Left || d == Direction.LeftUp || d == Direction.LeftDown;
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
		[AutoConvert] public float x { get; set; }
		[AutoConvert] public float y { get; set; }
		[AutoConvert]
		public TimeType timeType { get; set; } = TimeType.Present; // 时空类型
		[AutoConvert]
        public Direction direction { get; set; } = Direction.Down;

        public int state {
            get => stateMachine.state;
            set { stateMachine.changeState(value); }
        }

		public Vector2 velocity { get; protected set; } = new Vector2();

		/// <summary>
		/// 初次更新
		/// </summary>
		bool isFirstUpdate = true;

        /// <summary>
        /// 状态机
        /// </summary>
        public StateMachine stateMachine { get; protected set; } = new StateMachine();

        /// <summary>
        /// 回调类型
        /// </summary>
        public enum CbType { }

        /// <summary>
        /// 回调枚举类型
        /// </summary>
        protected virtual Type cbType => typeof(CbType);

        /// <summary>
        /// 回调管理器
        /// </summary>
        protected CallbackManager callbackManager;

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
            initializeCallbacks();
            initializeOthers();
        }

        /// <summary>
        /// 初始化状态
        /// </summary>
        protected virtual void initializeStates() {
            addStateDict(State.Idle, updateIdle);
            addStateDict(State.Moving, updateMoving);

            changeState(State.Idle);
        }

        /// <summary>
        /// 初始化回调管理
        /// </summary>
        void initializeCallbacks() {
            callbackManager = new CallbackManager(cbType, this);
        }

        /// <summary>
        /// 初始化其他
        /// </summary>
        protected virtual void initializeOthers() { }

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
			Debug.Log(this + ".transfer => " + _transferPoint);
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
            direction = vec2Dir8(velocity = vec);
        }

        /// <summary>
        /// 朝一个方向移动
        /// </summary>
        /// <param name="d"></param>
        public void moveDirection(Direction d,
            float speed = -1, bool force = false) {
            if (speed < 0) speed = moveSpeed();
            var vec = dir82Vec(d) * speed;

            move(vec, force);
            direction = d;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void stop() {
            velocity = Vector2.zero;
        }

        #endregion

        #region 图案控制

        /// <summary>
        /// 图案
        /// </summary>
        float patternTime = 0;
        int _pattern = DefaultPattern;
        public int pattern => _pattern >= XCnt ? 1 : _pattern;

        /// <summary>
        /// 更新图案
        /// </summary>
        void updatePattern() {
            patternTime += Time.deltaTime;
            if (patternTime > PatternFrequency) {
                _pattern = (_pattern + 1) % (XCnt + 1);
                patternTime = 0;
            }
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
		/// 注册状态切换函数
		/// </summary>
		/// <param name="state">状态</param>
		/// <param name="action">行动</param>
		public void addStateChange(Enum from, Enum to, UnityAction action) {
			stateMachine.addStateChange(from, to, action);
		}

		/// <summary>
		/// 注册状态进入函数
		/// </summary>
		/// <param name="enumType">类型</param>
		/// <param name="to">初状态</param>
		/// <param name="action">动作</param>
		public void addStateEnter(Enum to, UnityAction action) {
			stateMachine.addStateEnter(to, action);
		}

		/// <summary>
		/// 注册状态退出函数
		/// </summary>
		/// <param name="enumType">类型</param>
		/// <param name="from">初状态</param>
		/// <param name="action">动作</param>
		public void addStateExit(Enum from, UnityAction action) {
			stateMachine.addStateExit(from, action);
		}

        /// <summary>
        /// 注册状态更新函数
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="action">行动</param>
        public void removeStateDict(Enum state, UnityAction action) {
            stateMachine.removeStateDict(state, action);
        }

        /// <summary>
        /// 注册状态切换函数
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="action">行动</param>
        public void removeStateChange(Enum from, Enum to, UnityAction action) {
            stateMachine.removeStateChange(from, to, action);
        }

        /// <summary>
        /// 注册状态进入函数
        /// </summary>
        /// <param name="enumType">类型</param>
        /// <param name="to">初状态</param>
        /// <param name="action">动作</param>
        public void removeStateEnter(Enum to, UnityAction action) {
            stateMachine.removeStateEnter(to, action);
        }

        /// <summary>
        /// 注册状态退出函数
        /// </summary>
        /// <param name="enumType">类型</param>
        /// <param name="from">初状态</param>
        /// <param name="action">动作</param>
        public void removeStateExit(Enum from, UnityAction action) {
            stateMachine.removeStateExit(from, action);
        }

        /// <summary>
        /// 更换状态
        /// </summary>
        /// <param name="state"></param>
        public void changeState(Enum state) {
			Debug.Log("changeState: " + this + ": " +
				Enum.ToObject(state.GetType(), this.state) + " -> " + state);
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
            _pattern = DefaultPattern;
        }

        /// <summary>
        /// 更新移动状态
        /// </summary>
        protected virtual void updateMoving() {
            if (!isMoving()) changeState(State.Idle);
            updatePattern();
        }

        #endregion

        #endregion

        #region 更新

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="round">回合数</param>
        public virtual void update() {
            if (isFirstUpdate) firstUpdate();

            updateStateMachine();
            updateDirection();
			
            //callbackManager?.reset();
        }

        /// <summary>
        /// 初次更新
        /// </summary>
        protected virtual void firstUpdate() {
            isFirstUpdate = false;
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
            if (isMoving()) direction = vec2Dir8(velocity);
        }

        #endregion
    }

	/// <summary>
	/// 对话框选项
	/// </summary>
	[Serializable]
	public class DialogOption : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        public string text = "";

        /// <summary>
        /// 动作
        /// </summary>
        //public List<UnityAction> actions = new List<UnityAction>();
        public UnityEvent actions = new UnityEvent();

        /// <summary>
        /// 添加动作
        /// </summary>
        /// <param name="action"></param>
        public void addAction(UnityAction action) {
            actions.AddListener(action);
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void invoke() {
            actions?.Invoke();
            //foreach (var action in actions) action?.Invoke();
        }

        /// <summary>
        /// 获取测试数据
        /// </summary>
        /// <returns></returns>
        public static DialogOption testData(int index) {
            var opt = new DialogOption();
            opt.text = "选项" + index;
            opt.addAction(() => Debug.Log("You selected: " + index));

            return opt;
        }
    }

	/// <summary>
	/// 对话框信息
	/// </summary>
	[Serializable]
	public class DialogMessage : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [TextArea(0, 100)]
        public string message = "";
        public string name = "";
		[SerializeField]
		public List<DialogOption> options = new List<DialogOption>();

        /// <summary>
        /// 立绘
        /// </summary>
        [AutoConvert]
        public int bustId { get; protected set; } // 立绘ID

        /// <summary>
        /// Editor中赋值
        /// </summary>
        [SerializeField] Sprite _bust = null;

        /// <summary>
        /// 获取立绘实例
        /// </summary>
        /// <returns></returns>
        protected CacheAttr<Sprite> bust_ = null;
        protected Sprite _bust_() {
            return AssetLoader.loadAsset<Sprite>(
                Asset.Type.Bust, bustId);
        }
        public Sprite bust() {
            return _bust ?? bust_?.value();
        }

        /// <summary>
        /// 获取测试数据
        /// </summary>
        /// <returns></returns>
        public static DialogMessage testData(
            string message, string name = "", int bustId = 0) {
            var msg = new DialogMessage();
            msg.message = message;
            msg.name = name;

            msg.bustId = bustId;

            for (int i = 0; i < UnityEngine.Random.Range(0, 4); ++i)
                msg.options.Add(DialogOption.testData(i));

            return msg;
        }
    }

}
