
using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using LitJson;

using Core.Systems;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using BattleModule.Data;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 顽疾模块
/// </summary>
namespace PlayerModule { }

/// <summary>
/// 玩家模块数据
/// </summary>
namespace PlayerModule.Data {

	/// <summary>
	/// 物品标志
	/// </summary>
	public class ObjectFlags : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public bool energyBall1 { get; set; } = true;
		[AutoConvert]
		public bool taiqingWater1 { get; set; } = false;
		[AutoConvert]
		public bool taiqingWater2 { get; set; } = false;
		[AutoConvert]
		public Vector2 shipPosition { get; set; }
	}

	/// <summary>
	/// 玩家数据
	/// </summary>
	public class Player : BaseData,
		ParamDisplay.IDisplayDataConvertable {

		/// <summary>
		/// 初始关卡
		/// </summary>
		public const SceneSystem.Scene FirstStage = SceneSystem.Scene.TurialScene;

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public string name { get; protected set; }
		[AutoConvert]
		public string uid { get; protected set; }

		[AutoConvert]
		public Actor actor { get; protected set; }

		[AutoConvert]
		public ObjectFlags flags { get; protected set; } = new ObjectFlags();

		[AutoConvert]
		public SceneSystem.Scene stage { get; set; } = FirstStage;

		/// <summary>
		/// 转化为显示数据
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public JsonData convertToDisplayData(string type = "") {
			return toJson();
		}

		/// <summary>
		/// 生成随机UID
		/// </summary>
		string generateUid() {
			// TODO: 完善uid生成
			return Random.Range(0, 99999).ToString();
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name"></param>
		public Player() { }
		public Player(string name) {
			this.name = name;
			uid = generateUid();
            actor = new Actor(this);
		}
	}
}