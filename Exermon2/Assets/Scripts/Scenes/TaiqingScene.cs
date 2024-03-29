﻿
using System;
using System.Collections.Generic;

using LitJson;

using UnityEngine;

using Core.Systems;

using PlayerModule.Services;
using GameModule.Services;

namespace UI.MapSystem {
    using Controls;
    /// <summary>
    /// 测试地图场景
    /// </summary>
    public class TaiqingScene : BaseMapScene {

        /// <summary>
        /// 外部变量
        /// </summary>

        /// <summary>
        /// 外部系统定义
        /// </summary>
        GameService gameSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
        }

        /// <summary>
        /// 场景索引
        /// </summary>
        /// <returns></returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.TaiqingScene;
        }

		/// <summary>
		/// 下一关
		/// </summary>
		/// <returns></returns>
		public override SceneSystem.Scene nextStage() {
			return SceneSystem.Scene.NoneScene;
		}

		#endregion

		#region 事件控制

        #endregion
    }
}