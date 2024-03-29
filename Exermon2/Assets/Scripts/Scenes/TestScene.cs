﻿
using System;
using System.Collections.Generic;

using LitJson;

using UnityEngine;

using Core.Systems;

using PlayerModule.Services;

namespace UI.MapSystem {

	/// <summary>
	/// 测试地图场景
	/// </summary>
	public class TestScene : BaseMapScene {

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOthers() {
			base.initializeOthers();
			playerSer.createPlayer("TestPlayer");
		}

		/// <summary>
		/// 场景索引
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene sceneIndex() {
			return SceneSystem.Scene.TestScene;
		}

		/// <summary>
		/// 下一关
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene nextStage() {
			return SceneSystem.Scene.TutorialScene;
		}

		#endregion
	}
}