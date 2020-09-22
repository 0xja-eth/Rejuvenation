using System;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 游戏模块
/// </summary>
namespace GameModule { }

/// <summary>
/// 游戏模块数据
/// </summary>
namespace GameModule.Data {

    /// <summary>
    /// 游戏静态配置数据
    /// </summary>
    public class GameStaticData : BaseData {

		/// <summary>
		/// 本地版本
		/// </summary>
		//public string localVersion = PlayerSettings.bundleVersion;
		public const string LocalMainVersion = DeployConfig.LocalMainVersion; // "0.3.2";
        public const string LocalSubVersion = DeployConfig.LocalSubVersion; // "20200527";

        /// <summary>
        /// 后台版本
        /// </summary>
        [AutoConvert]
        public GameVersionData curVersion { get; protected set; }

        /// <summary>
        /// 历史版本
        /// </summary>
        [AutoConvert]
        public List<GameVersionData> lastVersions { get; protected set; }

        /// <summary>
        /// 游戏配置
        /// </summary>
        public GameConfigure configure { get; protected set; }

        /// <summary>
        /// 游戏资料（数据库）
        /// </summary>
        public GameDatabase data { get; protected set; }

        /// <summary>
        /// 读取标志
        /// </summary>
        bool loaded = false;
        public bool isLoaded() { return loaded; }

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 生成更新日志
        /// </summary>
        /// <returns>更新日志文本</returns>
        public string generateUpdateNote() {
            string updateNote = "当前版本：\n" + curVersion.generateUpdateNote();
            updateNote += "\n历史版本：\n";
            foreach (var ver in lastVersions)
                updateNote += ver.generateUpdateNote();
            return updateNote;
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);

            if (curVersion == null) return;
            Debug.Log("curVersion: " + curVersion.generateUpdateNote());

            // 如果没有版本变更且数据已读取（本地缓存），则直接返回
            if (curVersion.mainVersion == LocalMainVersion &&
                curVersion.subVersion == LocalSubVersion && loaded) return;

            configure = DataLoader.load(configure, json, "configure");
            data = DataLoader.load(data, json, "data");

            loaded = true;
        }

        /// <summary>
        /// 转化自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void convertCustomAttributes(ref JsonData json) {
            base.convertCustomAttributes(ref json);
            json["configure"] = DataLoader.convert(configure);
            json["data"] = DataLoader.convert(data);
        }
    }

    /// <summary>
    /// 游戏动态数据
    /// </summary>
    public class GameDynamicData : BaseData {

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 属性
        /// </summary>
    }

	/// <summary>
	/// 游戏版本数据
	/// </summary>
	public class GameVersionData : BaseData {

        /// <summary>
        /// 更新日志格式
        /// </summary>
        public const string UpdateNoteFormat = "版本号：{1}.{2}\n更新日期：{0}\n更新内容：\n{3}\n";

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string mainVersion { get; protected set; }
        [AutoConvert]
        public string subVersion { get; protected set; }
        [AutoConvert]
        public string updateNote { get; protected set; }
        [AutoConvert]
        public DateTime updateTime { get; protected set; }
        [AutoConvert]
        public string description { get; protected set; }

        /// <summary>
        /// 生成单个版本的更新日志
        /// </summary>
        /// <returns>更新日志文本</returns>
        public string generateUpdateNote() {
            string time = updateTime.ToString(DataLoader.SystemDateFormat);
            return string.Format(GameVersionData.UpdateNoteFormat, mainVersion,
                subVersion, time, updateNote, description);
        }

    }

    /// <summary>
    /// 游戏系统配置数据
    /// </summary>
    public class GameConfigure : BaseData {

        /// <summary>
        /// 基本术语
        /// </summary>
        [AutoConvert]
        public string name { get; protected set; }
        [AutoConvert]
        public string engName { get; protected set; }
        [AutoConvert]
        public string gold { get; protected set; }
	}

	/// <summary>
	/// 游戏资料数据
	/// </summary>
	public class GameDatabase : BaseData {

    }
}