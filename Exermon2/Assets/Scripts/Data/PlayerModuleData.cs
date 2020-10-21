
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
	/// 游戏数据变量
	/// </summary>
	public class Info : BaseData {

		/// <summary>
		/// 开关
		/// </summary>
		public enum Switches {
			None,
			EnergyBall1,
			TaiqingWater1,
			TaiqingWater2
		}

		/// <summary>
		/// 变量
		/// </summary>
		public enum Variables {
			None,
			ShipPosX, ShipPosY
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public Dictionary<Switches, bool> switches { get; set; } 
			= new Dictionary<Switches, bool>();
		[AutoConvert]
		public Dictionary<Variables, float> variables { get; set; }
			= new Dictionary<Variables, float>();

		/// <summary>
		/// 构造函数
		/// </summary>
		public Info() {
			setupSwitches();
			setupVariables();
		}

		/// <summary>
		/// 配置开关字典
		/// </summary>
		void setupSwitches() {
			foreach (var s in Enum.GetValues(typeof(Switches)))
				switches.Add((Switches)s, false);
		}

		/// <summary>
		/// 配置变量字典
		/// </summary>
		void setupVariables() {
			foreach (var v in Enum.GetValues(typeof(Variables)))
				variables.Add((Variables)v, 0);
		}

		/// <summary>
		/// 获取开关值
		/// </summary>
		public bool getSwitch(Switches type) {
			return switches[type];
		}

		/// <summary>
		/// 设置开关值
		/// </summary>
		public bool setSwitch(Switches type, bool val) {
			return switches[type] = val;
		}

		/// <summary>
		/// 获取变量值
		/// </summary>
		public float getVariable(Variables type) {
			return variables[type];
		}

		/// <summary>
		/// 获取开关值
		/// </summary>
		public float setVariable(Variables type, float val) {
			return variables[type] = val;
		}
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
		public Info info { get; protected set; } = new Info();

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