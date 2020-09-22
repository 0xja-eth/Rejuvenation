using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

using Core.Data;
using Core.Services;
using Core.Systems;

using Core.UI.Utils;

/// <summary>
/// 基本系统
/// </summary>
namespace PlayerModule.Services {

	using Data;

    /// <summary>
    /// 玩家服务类
    /// </summary>
    public class PlayerService : BaseService<PlayerService> {

        /// <summary>
        /// 玩家
        /// </summary>
        public Player player { get; protected set; }

		/// <summary>
		/// 创建角色
		/// </summary>
		/// <param name="name"></param>
        public void createPlayer(string name) {
			player = new Player(name);
		}

		/// <summary>
		/// 读取角色
		/// </summary>
		/// <param name="fileName"></param>
		public void loadPlayer(string fileName) {
			// TODO: 实现读取存档功能
		}

		/// <summary>
		/// 玩家退出
		/// </summary>
		public void clearPlayer() {
			player = null;
		}

		/// <summary>
		/// 游戏是否开始
		/// </summary>
		/// <returns></returns>
		public bool isPlaying() {
			return player != null;
		}
    }

}