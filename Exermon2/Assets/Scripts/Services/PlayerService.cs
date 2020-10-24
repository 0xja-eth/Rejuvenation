using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

using Core.Data;
using Core.Data.Loaders;
using Core.Services;
using Core.Systems;

using Core.UI.Utils;

using BattleModule.Data;

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
		/// 存档名
		/// </summary>
		const string PlayerSaveFilename = ".player";

		/// <summary>
		/// 默认主角名称
		/// </summary>
		public const string DefaultPlayerName = "富文";

		/// <summary>
		/// 玩家
		/// </summary>
		public Player player { get; protected set; }

		/// <summary>
		/// 快捷访问
		/// </summary>
		public Actor actor => player?.actor;
		public RuntimeActor runtimeActor => actor?.runtimeActor;

		public Info info => player?.info;

		/// <summary>
		/// 当前储存的玩家数据
		/// </summary>
		JsonData savedPlayer;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		public SceneSystem sceneSys;

		/// <summary>
		/// 创建角色
		/// </summary>
		/// <param name="name"></param>
		public void createPlayer(string name = DefaultPlayerName) {
			player = new Player(name);
			savePlayer();
		}

		/// <summary>
		/// 读取角色
		/// </summary>
		/// <param name="fileName"></param>
		public void loadPlayer() {
			savedPlayer = StorageSystem.loadJsonFromFile(PlayerSaveFilename);
			player = DataLoader.load<Player>(savedPlayer);
		}

		/// <summary>
		/// 是否存在存档
		/// </summary>
		/// <returns></returns>
		public bool hasPlayer() {
			return StorageSystem.hasFile(PlayerSaveFilename);
		}

		/// <summary>
		/// 开始游戏（启动了异步场景加载）
		/// </summary>
		public IEnumerator startGame(bool load,
			UnityAction<float> onProgress, UnityAction onCompleted = null) {

			if (load && hasPlayer()) loadPlayer();
			else createPlayer();

			sceneSys.pushScene(player.stage, true, true);
			return sceneSys.startAsync(onProgress, onCompleted);
		}

		/// <summary>
		/// 重开本局
		/// </summary>
		public IEnumerator resumeGame(
			UnityAction<float> onProgress, UnityAction onCompleted = null) {
			player = DataLoader.load<Player>(savedPlayer);

			sceneSys.changeScene(player.stage, true, true);
			return sceneSys.startAsync(onProgress, onCompleted);
		}

		/// <summary>
		/// 保存角色（场景开始自动调用）
		/// </summary>
		/// <param name="fileName"></param>
		public void savePlayer() {
			savedPlayer = player.toJson();
			StorageSystem.saveJsonIntoFile(savedPlayer, PlayerSaveFilename);
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