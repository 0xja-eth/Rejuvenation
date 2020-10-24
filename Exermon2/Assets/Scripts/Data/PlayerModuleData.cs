
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
			TaiqingWater1, // 房间1
			TaiqingWater2,

			TaiqingWater3, // 房间2
			TaiqingWater4,
			TaiqingWater5,
			TaiqingBlock1,
			TaiqingBlock2,

			CloneFlag, // 扶桑长廊

			FusangBlock1, // 扶桑复制
			FusangBlock2,
			FusangBlock3,
			FusangBlock4,
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
		public Dictionary<Switches, bool> switches { get; set; } 
			= new Dictionary<Switches, bool>();
		public Dictionary<Variables, float> variables { get; set; }
			= new Dictionary<Variables, float>();

		#region 数据读取

		/// <summary>
		/// 读取自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			this.switches.Clear();
			this.variables.Clear();
			
			var switches = DataLoader.load(json, "switches");
			var variables = DataLoader.load(json, "variables");

			if (switches != null) {
				switches.SetJsonType(JsonType.Object);
				foreach (KeyValuePair<string, JsonData> pair in switches) {
					var key = (Switches)int.Parse(pair.Key);
					var data = DataLoader.load<bool>(pair.Value);
					Debug.Log("Load switches: " + key + " => " + data);
					this.switches.Add(key, data);
				}
			}
			if (variables != null) {
				variables.SetJsonType(JsonType.Object);
				foreach (KeyValuePair<string, JsonData> pair in variables) {
					var key = (Variables)int.Parse(pair.Key);
					var data = DataLoader.load<float>(pair.Value);
					Debug.Log("Load variables: " + key + " => " + data);
					this.variables.Add(key, data);
				}
			}
		}

		/// <summary>
		/// 转化自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void convertCustomAttributes(ref JsonData json) {
			base.convertCustomAttributes(ref json);

			var switches = new JsonData();
			var variables = new JsonData();

			switches.SetJsonType(JsonType.Object);
			variables.SetJsonType(JsonType.Object);

			foreach (var pair in this.switches)
				switches[pair.Key.GetHashCode().ToString()] = DataLoader.convert(pair.Value);
			foreach (var pair in this.variables)
				variables[pair.Key.GetHashCode().ToString()] = DataLoader.convert(pair.Value);

			json["switches"] = switches;
			json["variables"] = variables;
		}

		#endregion

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
	/// 玩家数据（包含角色数据、存档数据、关卡状态等）
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

        [AutoConvert]
        public bool firstStart { get; set; } = true;

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