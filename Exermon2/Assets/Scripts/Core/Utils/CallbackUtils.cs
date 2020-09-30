using System;
using System.Collections.Generic;

namespace Core.Utils {

	/// <summary>
	/// 回调管理类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CallbackManager {

		/// <summary>
		/// 回调设置
		/// </summary>
		Dictionary<int, CallbackSetting> callbacks = 
			new Dictionary<int, CallbackSetting>();

		/// <summary>
		/// 注册回调
		/// </summary>
		/// <param name="type"></param>
		/// <param name="action"></param>
		public void registerCallback(Enum type, Action action) {
			registerCallback(type.GetHashCode(), action);
		}
		public void registerCallback(int type, Action action) {
			callbacks[type] = new CallbackSetting(type, action);
		}

		/// <summary>
		/// 批量注册回调
		/// </summary>
		/// <param name="enumType"></param>
		/// <param name="obj"></param>
		public void registerCallbacks(Type enumType, object obj) {
			foreach (int e in Enum.GetValues(enumType)) {
				var oType = obj.GetType();
				var name = "on" + Enum.GetName(enumType, e);
				var info = oType.GetMethod(name, 
					ReflectionUtils.DefaultFlag, null, new Type[0], null);
				var func = (Action)info?.CreateDelegate(typeof(Action), obj);

				registerCallback(e, func);
			}
		}
		public void registerCallbacks<T>(object obj) where T : Enum {
			registerCallbacks(typeof(T), obj);
		}

		/// <summary>
		/// 唤起回调
		/// </summary>
		/// <param name="type"></param>
		public void on(Enum type, bool invoke = true) {
			on(type.GetHashCode(), invoke);
		}
		public void on(int type, bool invoke = true) {
			if (callbacks.ContainsKey(type))
				callbacks[type].on(invoke);
		}

		/// <summary>
		/// 判断回调
		/// </summary>
		/// <param name="type"></param>
		public bool judge(Enum type) {
			return judge(type.GetHashCode());
		}
		public bool judge(int type) {
			return callbacks.ContainsKey(type) &&
				callbacks[type].flag;
		}

		/// <summary>
		/// 重置
		/// </summary>
		public void reset() {
			foreach (var cb in callbacks)
				cb.Value.flag = false;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public CallbackManager() { }
		public CallbackManager(Type enumType, object obj) {

		}
	}

	/// <summary>
	/// 回调设置
	/// </summary>
	public class CallbackSetting {

		/// <summary>
		/// 回调类型
		/// </summary>
		public int type; 

		/// <summary>
		/// 处理函数（立刻）
		/// </summary>
		public Action action;

		/// <summary>
		/// 回调发生标志
		/// </summary>
		public bool flag = false;

		/// <summary>
		/// 构造函数
		/// </summary>
		public CallbackSetting(int type, Action action) {
			this.type = type; this.action = action;
		}

		/// <summary>
		/// 唤起
		/// </summary>
		public void on(bool invoke = true) {
			if (invoke) action?.Invoke();
			flag = true;
		}
	}
}
