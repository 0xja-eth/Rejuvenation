using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Core.Utils;
using Core.Systems;

namespace Core.UI {

	using Utils;

	/// <summary>
	/// 组件接口
	/// </summary>
	public interface IBaseComponent {

		#region Unity内置元素

		//
		// 摘要:
		//     The ParticleSystem attached to this GameObject. (Null if there is none attached).
		Component particleSystem { get; }
		//
		// 摘要:
		//     The Transform attached to this GameObject.
		Transform transform { get; }
		//
		// 摘要:
		//     The game object this component is attached to. A component is always attached
		//     to a game object.
		GameObject gameObject { get; }
		//
		// 摘要:
		//     The tag of this game object.
		string tag { get; set; }
		//
		// 摘要:
		//     The Rigidbody attached to this GameObject. (Null if there is none attached).
		Component rigidbody { get; }
		//
		// 摘要:
		//     The HingeJoint attached to this GameObject. (Null if there is none attached).
		Component hingeJoint { get; }
		//
		// 摘要:
		//     The Camera attached to this GameObject. (Null if there is none attached).
		Component camera { get; }
		//
		// 摘要:
		//     The Rigidbody2D that is attached to the Component's GameObject.
		Component rigidbody2D { get; }
		//
		// 摘要:
		//     The Animation attached to this GameObject. (Null if there is none attached).
		Component animation { get; }
		//
		// 摘要:
		//     The ConstantForce attached to this GameObject. (Null if there is none attached).
		Component constantForce { get; }
		//
		// 摘要:
		//     The Renderer attached to this GameObject. (Null if there is none attached).
		Component renderer { get; }
		//
		// 摘要:
		//     The AudioSource attached to this GameObject. (Null if there is none attached).
		Component audio { get; }
		//
		// 摘要:
		//     The NetworkView attached to this GameObject (Read Only). (null if there is none
		//     attached).
		Component networkView { get; }
		//
		// 摘要:
		//     The Collider attached to this GameObject. (Null if there is none attached).
		Component collider { get; }
		//
		// 摘要:
		//     The Collider2D component attached to the object.
		Component collider2D { get; }
		//
		// 摘要:
		//     The Light attached to this GameObject. (Null if there is none attached).
		Component light { get; }

		#endregion

		/// <summary>
		/// RectTransform
		/// </summary>
		RectTransform rectTransform { get; }

		/// <summary>
		/// 组件类型判断
		/// </summary>
		/// <returns></returns>
		bool isCanvasComponent();
		bool isWorldComponent();

		/// <summary>
		/// 配置组件
		/// </summary>
		void configure();

		/// <summary>
		/// 开启视窗
		/// </summary>
		void activate();

		/// <summary>
		/// 结束视窗
		/// </summary>
		void deactivate();

		/// <summary>
		/// 请求刷新
		/// </summary>
		void requestRefresh(bool force = false);

		/// <summary>
		/// 请求清除
		/// </summary>
		void requestClear(bool force = false);

	}

	/// <summary>
	/// 组件基类
	/// </summary>
	public abstract class BaseComponent : MonoBehaviour, IBaseComponent {

		/// <summary>
		/// RequireComponent绑定对象特性
		/// </summary>
		[AttributeUsage(AttributeTargets.Field)]
		public class RequireTargetAttribute : Attribute { }

		/// <summary>
		/// 外部变量声明
		/// </summary>
		public bool showDebug = false;

		/// <summary>
		/// 属性
		/// </summary>
		public bool awaked { get; protected set; } = false;
        public bool started { get; protected set; } = false;
		public bool updating { get; protected set; } = false;
		public bool initialized { get; protected set; } = false; // 初始化标志

		/// <summary>
		/// 显示状态
		/// </summary>
		public virtual bool shown {
			get {
				return gameObject.activeSelf;
			}
			protected set {
				gameObject.SetActive(value);
			}
		}

		/// <summary>
		/// 销毁标志
		/// </summary>
		public bool destroyFlag { get; protected set; } = false;

		/// <summary>
		/// 刷新请求
		/// </summary>
		List<StackTrace> refreshRequestedStacks = new List<StackTrace>();
		List<StackTrace> clearRequestedStacks = new List<StackTrace>();

		/// <summary>
		/// RectTransform
		/// </summary>
		public RectTransform rectTransform => transform as RectTransform;

		/// <summary>
		/// 组件类型判断
		/// </summary>
		/// <returns></returns>
		public bool isCanvasComponent() { return rectTransform != null; }
		public bool isWorldComponent() { return !isCanvasComponent(); }

		#region 初始化

		/// <summary>
		/// 初始化（唤醒）
		/// </summary>
		private void Awake() {
            awaked = true;
			debugLog("Awake");
			if (!initialized) initialize();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		void initialize() {
			if (!initialized) {
				initialized = true;
				initializeSystems();
				initializeRequirements();
				initializeOnce();
			}
			initializeEvery();
		}

		#region 内部初始化

		/// <summary>
		/// 初始化系统/服务
		/// </summary>
		void initializeSystems() {
			ReflectionUtils.processMember<FieldInfo, BaseSystem>(
				GetType(), (field) => {
					var fType = field.FieldType;
					var getFunc = fType.GetMethod("Get", 
						ReflectionUtils.DefaultStaticFlag);
					var val = getFunc.Invoke(null, null);

					field.SetValue(this, val);
					debugLog(field.Name + ": " + val);
				});
		}

		/// <summary>
		/// 初始化需要的组件
		/// </summary>
		void initializeRequirements() {
			var reqs = new List<Type>();
			ReflectionUtils.processClassAttribute<RequireComponent>(
				GetType(), (a) => {
					if (a.m_Type0 != null) reqs.Add(a.m_Type0);
					if (a.m_Type1 != null) reqs.Add(a.m_Type1);
					if (a.m_Type2 != null) reqs.Add(a.m_Type2);
				});

			ReflectionUtils.processAttribute<
				FieldInfo, RequireTargetAttribute>(
				GetType(), (field, attr) => {
					var fType = field.FieldType;
					if (reqs.Contains(fType)) {
						field.SetValue(this, get(fType));
						debugLog(field.Name + ": " + get(fType));
					}
				});
		}

		#endregion

		/// <summary>
		/// 初次打开时初始化（子类中重载）
		/// </summary>
		protected virtual void initializeOnce() { }

		/// <summary>
		/// 每次打开时初始化（子类中重载）
		/// </summary>
		protected virtual void initializeEvery() { }

		/// <summary>
		/// 配置
		/// </summary>
		public virtual void configure() { }

        /// <summary>
        /// 初始化（开始）
        /// </summary>
        private void Start() {
			debugLog("Start");
			started = true; start();
        }

        /// <summary>
        /// 初始化（同Start）
        /// </summary>
        protected virtual void start() { }

		/// <summary>
		/// 销毁回调
		/// </summary>
		private void OnDestroy() {
			onDestroy();
		}

		/// <summary>
		/// 销毁回调（同OnDestroy）
		/// </summary>
		protected virtual void onDestroy() { }

		#endregion

		#region 激活和关闭

		/// <summary>
		/// 显示
		/// </summary>
		public virtual void show() {
			requestRefresh();
			shown = true;
			debugLog("show: " + shown);
		}

		/// <summary>
		/// 隐藏
		/// </summary>
		public virtual void hide() {
			debugLog("hide");
			requestClear(true);
			shown = false;
		}

		/// <summary>
		/// 激活
		/// </summary>
		public virtual void activate() {
			initialize(); show();
		}

		/// <summary>
		/// 关闭
		/// </summary>
		public virtual void deactivate() {
			hide();
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		private void FixedUpdate() {
			updating = true;
			fixedUpdate();
			updating = false;
		}

		/// <summary>
		/// 更新（同FixedUpdate）
		/// </summary>
		protected virtual void fixedUpdate() { }

		/// <summary>
		/// 更新
		/// </summary>
		private void Update() {
            updating = true;
            update();
            updating = false;
        }

        /// <summary>
        /// 更新（同Update）
        /// </summary>
        protected virtual void update() {
			updateRefresh();
		}

		/// <summary>
		/// 更新刷新
		/// </summary>
		void updateRefresh() {
			if (isRefreshRequested()) {
				showRefreshStackTrace(); refresh();
			} else if (isClearRequested()) {
				showClearStackTrace(); clear();
			}

			resetRequests();
		}

		/// <summary>
		/// 显示刷新函数的调用堆栈
		/// </summary>
		void showRefreshStackTrace() {
			showStackTraces(refreshRequestedStacks, "RefreshRequested");
		}

		/// <summary>
		/// 显示清除函数的调用堆栈
		/// </summary>
		void showClearStackTrace() {
			showStackTraces(clearRequestedStacks, "ClearRequested");
		}

		/// <summary>
		/// 显示清除函数的调用堆栈
		/// </summary>
		void showStackTraces(List<StackTrace> stacks, string title) {
			var logText = title + ": ";
			foreach (var stack in stacks)
				logText += "\n" + stack.ToString();

			debugLog(logText);
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 请求刷新
		/// </summary>
		public virtual void requestRefresh(bool force = false) {
			if (force) refresh();
			else refreshRequestedStacks.Add(new StackTrace());
		}

		/// <summary>
		/// 请求清除
		/// </summary>
		public virtual void requestClear(bool force = false) {
			if (force) clear();
			else clearRequestedStacks.Add(new StackTrace());
		}

		/// <summary>
		/// 重置所有请求
		/// </summary>
		void resetRequests() {
			clearRequestedStacks.Clear();
			refreshRequestedStacks.Clear();
		}

		/// <summary>
		/// 是否需要刷新
		/// </summary>
		/// <returns>需要刷新</returns>
		protected virtual bool isRefreshRequested() {
			return shown && refreshRequestedStacks.Count > 0;
		}

		/// <summary>
		/// 是否需要清除
		/// </summary>
		/// <returns>需要清除</returns>
		protected virtual bool isClearRequested() {
			return clearRequestedStacks.Count > 0;
		}

		/// <summary>
		/// 刷新视窗
		/// </summary>
		protected virtual void refresh() { }

		/// <summary>
		/// 清除视窗
		/// </summary>
		protected virtual void clear() { }

		#endregion

		#region 组件控制

		/// <summary>
		/// 获取组件
		/// </summary>
		/// <typeparam name="T">组件类型</typeparam>
		/// <returns></returns>
		public T get<T>() {
			return SceneUtils.get<T>(this);
		}
		public Component get(Type type) {
			return SceneUtils.get(this, type);
		}
		public T[] gets<T>() {
			return SceneUtils.gets<T>(this);
		}
		public Component[] gets(Type type) {
			return SceneUtils.gets(this, type);
		}

		/// <summary>
		/// 寻找子组件
		/// </summary>
		/// <typeparam name="T">组件类型</typeparam>
		/// <param name="path">路径</param>
		/// <returns></returns>
		public T find<T>(string path) {
			return SceneUtils.find<T>(this, path);
		}
		public Component find(Type type, string path) {
			return SceneUtils.find(this, type, path);
		}

		/// <summary>
		/// 寻找子组件
		/// </summary>
		/// <typeparam name="T">组件类型</typeparam>
		/// <param name="path">路径</param>
		/// <returns></returns>
		public T findParent<T>() {
			return SceneUtils.findParent<T>(this);
		}
		public Component findParent(Type type) {
			return SceneUtils.findParent(this, type);
		}

		/// <summary>
		/// 销毁
		/// </summary>
		public virtual void destroy(bool force = false) {
			if (force) {Destroy(gameObject); debugLog("destroy force"); }
			else debugLog("destroyReq");
			destroyFlag = true;
		}

		#endregion

		#region 协程控制

		/// <summary>
		/// 协程
		/// </summary>
		protected Coroutine doRoutine(string methodName) {
            return StartCoroutine(methodName);
        }
        protected Coroutine doRoutine(IEnumerator routine) {
            return StartCoroutine(routine);
        }

		#endregion

		#region Rebuild
		
		/// <summary>
		/// 注册布局更新（仅用于挂载 Layout 的物体）
		/// </summary>
		/// <param name="rect">物体 RectTransform</param>
		public void registerUpdateLayout() {
			registerUpdateLayout(rectTransform);
		}
		public void registerUpdateLayout(Transform rect) {
			registerUpdateLayout(rect as RectTransform);
		}
		public void registerUpdateLayout(RectTransform rect) {
			if (rect == null) return;
			SceneUtils.registerUpdateLayout(rect);
		}

		#endregion

		#region 调试

		/// <summary>
		/// 调试输出类型
		/// </summary>
		public enum DebugType {
			Log, Warning, Error
		}

		/// <summary>
		/// 输出位置信息
		/// </summary>
		public virtual void debugPosition() {
			var msg = debugFormat("position", transform.position);
			msg += debugFormat("localPosition", transform.localPosition);

			if (isCanvasComponent()) msg += debugFormat(
				"anchoredPosition", rectTransform.anchoredPosition);

			debugLog(msg);
		}

		/// <summary>
		/// 返回调试格式化字符串
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected string debugFormat(string key, object value) {
			return string.Format("{0}: {1}\n", key, value);
		}

		/// <summary>
		/// 调试日志
		/// </summary>
		/// <param name="obj"></param>
		public void debug(object obj, DebugType type, bool force = false) {
			if (!force && !showDebug) return;
			var format = "{0}: ";
			var msg = string.Format(format, this);

			switch (type) {
				case DebugType.Log:
					UnityEngine.Debug.Log(msg + obj); break;
				case DebugType.Warning:
					UnityEngine.Debug.LogWarning(msg + obj); break;
				case DebugType.Error:
					UnityEngine.Debug.LogError(msg + obj); break;
			}
		}
		public void debugLog(object obj, bool force = false) {
			debug(obj, DebugType.Log, force);
		}
		public void debugError(object obj, bool force = false) {
			debug(obj, DebugType.Error, force);
		}
		public void debugWarning(object obj, bool force = false) {
			debug(obj, DebugType.Warning, force);
		}

		#endregion
	}

	/// <summary>
	/// Canvas物体的组件
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public abstract class CanvasComponent : BaseComponent {

	}

	/// <summary>
	/// 通用组件
	/// </summary>
	public abstract class GeneralComponent : BaseComponent {

	}

	/// <summary>
	/// 世界物体的组件
	/// </summary>
	public abstract class WorldComponent : BaseComponent {

	}
}