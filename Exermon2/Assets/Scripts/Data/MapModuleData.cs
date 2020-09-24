
using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

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
			OnCollsion, // 碰撞
			OnSearch, // 搜索键
			Auto, // 自动执行
		}

		/// <summary>
		/// 条件函数
		/// </summary>
		/// <returns></returns>
		public delegate bool Condition();

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public TriggerType triggerType { get; protected set; }
		[AutoConvert]
		public List<Condition> conditions { get; protected set; }


	}

}

