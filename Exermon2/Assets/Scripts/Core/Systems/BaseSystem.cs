using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;

using Core.Utils;

/// <summary>
/// 核心系统
/// </summary>
namespace Core.Systems {

	/// <summary>
	/// 状态机
	/// </summary>
	public class StateMachine {

		/// <summary>
		/// 状态字典 (state, action)
		/// </summary>
		protected Dictionary<int, List<UnityAction>> stateDict =
			new Dictionary<int, List<UnityAction>>();
		protected Dictionary<int, List<UnityAction>> stateEnters =
			new Dictionary<int, List<UnityAction>>();
		protected Dictionary<int, List<UnityAction>> stateExits =
			new Dictionary<int, List<UnityAction>>();

		/// <summary>
		/// 状态改变字典 (<state, state>, action)
		/// </summary>
		protected Dictionary<Tuple<int, int>, List<UnityAction>> stateChanges =
			new Dictionary<Tuple<int, int>, List<UnityAction>>();

		/// <summary>
		/// 当前状态
		/// </summary>
		public int state { get; protected set; } = -1;
		public int lastState { get; protected set; } = -1;

		/// <summary>
		/// 状态改变回调
		/// </summary>
		public UnityAction onStateChanged { get; set; } = null;
		
		#region 更新控制

		/// <summary>
		/// 更新（每帧）
		/// </summary>
		public virtual void update() {
			updateState();
			updateStateChange();
		}

		/// <summary>
		/// 更新状态机
		/// </summary>
		void updateState() {
			if (hasState(state) && stateDict[state] != null) 
				foreach(var action in stateDict[state]) action?.Invoke();
		}

		/// <summary>
		/// 更新状态变化
		/// </summary>
		void updateStateChange() {
			if (isStateChanged()) {
				onStateChanged?.Invoke();

				processStateExit(lastState);
				processStateChange(lastState, state);
				processStateEnter(state);
			}

			lastState = state;
		}

		/// <summary>
		/// 处理状态变化
		/// </summary>
		void processStateChange(int from, int to) {
			var actions = getStateChange(from, to);
			if (actions != null)
				foreach (var action in actions) action?.Invoke();
		}

		/// <summary>
		/// 处理状态进入
		/// </summary>
		/// <param name="to"></param>
		void processStateEnter(int to) {
			var actions = getStateEnter(to);
			if (actions != null)
				foreach (var action in actions) action?.Invoke();
		}

		/// <summary>
		/// 处理状态退出
		/// </summary>
		/// <param name="from"></param>
		void processStateExit(int from) {
			var actions = getStateExit(from);
			if (actions != null)
				foreach (var action in actions) action?.Invoke();
		}

		#endregion

		#region 状态字典

		/// <summary>
		/// 添加状态字典
		/// </summary>
		/// <param name="state">状态</param>
		/// <param name="action">动作</param>
		public void addStateDict(int state, UnityAction action = null) {
			addListDict(stateDict, state, action);
		}
		public void addStateDict(Enum state, UnityAction action = null) {
			addStateDict(state.GetHashCode(), action);
		}
		public void addStateDict<E>() where E : Enum {
			addStateDict(typeof(E));
		}
		public void addStateDict(Type enumType) {
			foreach (int e in Enum.GetValues(enumType))
				addStateDict(e);
		}

		/// <summary>
		/// 是否存在状态
		/// </summary>
		/// <param name="state">状态名</param>
		/// <returns>是否存在</returns>
		public bool hasState(int state) {
			return hasListDict(stateDict, state);
		}
		public bool hasState(Enum state) {
			return hasState(state.GetHashCode());
		}

		/// <summary>
		/// 是否处于状态
		/// </summary>
		/// <param name="state">状态名</param>
		/// <returns>是否存在</returns>
		public bool isState(int state) {
			return this.state == state;
		}
		public bool isState(Enum state) {
			return isState(state.GetHashCode());
		}

		/// <summary>
		/// 状态是否改变
		/// </summary>
		/// <returns>状态改变</returns>
		public bool isStateChanged() {
			return lastState != state;
		}

		/// <summary>
		/// 改变状态
		/// </summary>
		/// <param name="state">新状态</param>
		public void changeState(int state, bool force = false) {
			//Debug.Log("changeState: " + GetType() + ": " + this.state + " -> " + state);
			if ((force || hasState(state)) && this.state != state)
				this.state = state;
		}
		public void changeState(Enum state, bool force = false) {
			changeState(state.GetHashCode(), force);
		}

		#endregion

		#region 状态改变字典

		/// <summary>
		/// 注册状态切换函数
		/// </summary>
		/// <param name="from">始状态</param>
		/// <param name="to">末状态</param>
		/// <param name="action">动作</param>
		public void addStateChange(int from, int to, UnityAction action) {
			if (from == to) return;
			addListDict(stateChanges, new Tuple<int, int>(from, to), action);
		}
		public void addStateChange(Enum from, Enum to, UnityAction action) {
			addStateChange(from.GetHashCode(), to.GetHashCode(), action);
		}

		/// <summary>
		/// 注册状态进入函数
		/// </summary>
		/// <param name="enumType">类型</param>
		/// <param name="to">初状态</param>
		/// <param name="action">动作</param>
		public void addStateEnter(int to, UnityAction action) {
			addListDict(stateEnters, to, action);
		}
		public void addStateEnter(Enum to, UnityAction action) {
			addStateEnter(to.GetHashCode(), action);
		}

		/// <summary>
		/// 注册状态退出函数
		/// </summary>
		/// <param name="enumType">类型</param>
		/// <param name="from">初状态</param>
		/// <param name="action">动作</param>
		public void addStateExit(int from, UnityAction action) {
			addListDict(stateExits, from, action);
		}
		public void addStateExit(Enum from, UnityAction action) {
			addStateExit(from.GetHashCode(), action);
		}
		
		/// <summary>
		/// 是否存在状态变更
		/// </summary>
		public bool hasStateChange(int from, int to) {
			var key = new Tuple<int, int>(from, to);
			return hasListDict(stateChanges, key);
		}
		public bool hasStateChange(Enum from, Enum to) {
			return hasStateChange(from.GetHashCode(), to.GetHashCode());
		}

		/// <summary>
		/// 是否存在状态变更
		/// </summary>
		public List<UnityAction> getStateChange(int from, int to) {
			var key = new Tuple<int, int>(from, to);
			return getListDict(stateChanges, key);
		}
		public List<UnityAction> getStateChange(Enum from, Enum to) {
			return getStateChange(from.GetHashCode(), to.GetHashCode());
		}

		/// <summary>
		/// 是否存在状态进入
		/// </summary>
		public List<UnityAction> getStateEnter(int to) {
			return getListDict(stateEnters, to);
		}
		public List<UnityAction> getStateEnter(Enum to) {
			return getStateEnter(to.GetHashCode());
		}

		/// <summary>
		/// 是否存在状态退出
		/// </summary>
		public List<UnityAction> getStateExit(int to) {
			return getListDict(stateExits, to);
		}
		public List<UnityAction> getStateExit(Enum to) {
			return getStateExit(to.GetHashCode());
		}

		#endregion

		#region 列表字典工具函数

		/// <summary>
		/// 添加一个列表字典
		/// </summary>
		public static void addListDict<T1, T2>(
			Dictionary<T1, List<T2>> dict, T1 key, T2 value) {
			getListDict(dict, key, true).Add(value);
		}

		/// <summary>
		/// 是否存在键
		/// </summary>
		public static bool hasListDict<T1, T2>(
			Dictionary<T1, List<T2>> dict, T1 key) {
			return dict.ContainsKey(key);
		}

		/// <summary>
		/// 获取值（数组）（如果没有键可以创建）
		/// </summary>
		public static List<T2> getListDict<T1, T2>(
			Dictionary<T1, List<T2>> dict, T1 key, bool create = false) {
			if (dict.ContainsKey(key)) return dict[key];
			if (create) return dict[key] = new List<T2>();
			return null;
		}

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public StateMachine() { }
		public StateMachine(Type type) {
			addStateDict(type);
		}
	}

	/// <summary>
	/// BaseSystem<>父类
	/// </summary>
	public class BaseSystem : StateMachine {

		/// <summary>
		/// 初始化标志
		/// </summary>
		public static bool initialized { get; protected set; } = false;
		public bool isInitialized() { return initialized; }
		
		/// <summary>
		/// 初始化（只执行一次）
		/// </summary>
		protected void initialize() {
			initialized = true;
			initializeStateDict();
			initializeSystems();
			initializeOthers();
		}

		/// <summary>
		/// 初始化状态字典
		/// </summary>
		protected virtual void initializeStateDict() { }

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		void initializeSystems() {
			ReflectionUtils.processMember<FieldInfo, BaseSystem>(
				GetType(), (field) => {
					var fType = field.FieldType;
					var getFunc = fType.GetMethod("Get",
						ReflectionUtils.DefaultStaticFlag);
					var val = getFunc.Invoke(null, null);

					field.SetValue(this, val);
				});
		}

		/// <summary>
		/// 其他初始化工作
		/// </summary>
		protected virtual void initializeOthers() { }

		#region 更新控制

		/// <summary>
		/// 更新（每帧）
		/// </summary>
		public override void update() {
			base.update();
			updateOthers();
			updateSystems();
		}

		/// <summary>
		/// 更新外部系统
		/// </summary>
		protected virtual void updateSystems() { }

		/// <summary>
		/// 更新其他
		/// </summary>
		protected virtual void updateOthers() { }

		#endregion
		
	}

	/// <summary>
	/// 业务控制类（父类）（单例模式）
	/// </summary>
	public class BaseSystem<T> : BaseSystem where T : BaseSystem<T>, new() {

		/// <summary>
		/// 多例错误
		/// </summary>
		class MultCaseException : Exception {
			const string ErrorText = "单例模式下不允许多例存在！";
			public MultCaseException() : base(ErrorText) { }
		}

		/// <summary>
		/// 单例函数
		/// </summary>
		protected static T _self;
        public static T Get() {
            if (_self == null) {
                _self = new T();
                _self.initialize();
            }
            return _self;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected BaseSystem() {
            if (_self != null) throw new MultCaseException();
        }
		
    }
}