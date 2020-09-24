
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

/// <summary>
/// 地图模块
/// </summary>
namespace MapModuleData { }

/// <summary>
/// ITU模块数据
/// </summary>
namespace MapModuleData.Data {

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

}

