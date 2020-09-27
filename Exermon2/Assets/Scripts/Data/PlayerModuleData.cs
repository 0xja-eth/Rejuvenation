
using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using LitJson;

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
	/// 基本物品数据
	/// </summary>
	public class Player : BaseData,
		ParamDisplay.IDisplayDataConvertable {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public string name { get; protected set; }
		[AutoConvert]
		public string uid { get; protected set; }

		[AutoConvert]
		public Actor actor { get; protected set; }

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
		void generateUid() {
			// TODO: 完善uid生成
			uid = Random.Range(0, 99999).ToString();
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name"></param>
		public Player() { }
		public Player(string name) {
			this.name = name;
            generateUid();
            actor = new Actor(this);
		}
	}
}