
using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 战斗模块
/// </summary>
namespace BattleModule { }

/// <summary>
/// 战斗模块数据
/// </summary>
namespace BattleModule.Data {

	/// <summary>
	/// 敌人
	/// </summary>
	public class Enemy : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public string name { get; protected set; }
		[AutoConvert]
		public int mhp { get; protected set; }

	}

	/// <summary>
	/// 战斗者
	/// </summary>
	public class RuntimeBattler : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int hp { get; protected set; }


	}

	/// <summary>
	/// 战斗玩家
	/// </summary>
	public class RuntimeActor : RuntimeBattler {

		/// <summary>
		/// 属性
		/// </summary>


	}

	/// <summary>
	/// 战斗敌人
	/// </summary>
	public class RuntimeEnemy : RuntimeBattler {

		/// <summary>
		/// 属性
		/// </summary>


	}

}

