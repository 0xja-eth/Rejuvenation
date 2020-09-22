using System;
using System.Collections.Generic;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using GameModule.Data;

using System.Reflection;

namespace GameModule.Services {

	/// <summary>
	/// 系统数据控制类
	/// </summary>
	public partial class DataService : BaseService<DataService> {

		/// <summary>
		/// 操作文本设定
		/// </summary>
		public new const string FailTextFormat = "{0}发生错误，错误信息：\n{{0}}"; // \n选择“重试”进行重试，选择“取消”退出游戏。";

		public const string Initializing = "初始化数据";
		public const string Refresh = "刷新数据";

		/// <summary>
		/// 业务操作
		/// </summary>
		public enum Oper {
			Static, Dynamic, Refresh
		}

		/// <summary>
		/// 状态
		/// </summary>
		public enum State {
			Unload,
			LoadingStatic,
			LoadingDynamic,
			Loaded,
		}
		public bool isLoaded() {
			return state == (int)State.Loaded;
		}

		/// <summary>
		/// 游戏数据
		/// </summary>
		public GameStaticData staticData { get; protected set; } = new GameStaticData();
		public GameDynamicData dynamicData { get; protected set; } = new GameDynamicData();

		/// <summary>
		/// 接受失败函数（初始加载用）
		/// </summary>
		UnityAction unacceptFunc = null;

		/// <summary>
		/// 外部系统
		/// </summary>
		StorageSystem storageSys;

		#region 初始化

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			storageSys = StorageSystem.get();
		}

		/// <summary>
		/// 初始化状态字典
		/// </summary>
		protected override void initializeStateDict() {
			base.initializeStateDict();
			addStateDict<State>();
		}

		/// <summary>
		/// 初始化操作字典
		/// </summary>
		protected override void initializeOperDict() {
			base.initializeOperDict();
			//addOperDict(Oper.Static, Initializing, NetworkSystem.Interfaces.LoadStaticData);
			//addOperDict(Oper.Dynamic, Initializing, NetworkSystem.Interfaces.LoadDynamicData);
			//addOperDict(Oper.Refresh, Refresh, NetworkSystem.Interfaces.LoadDynamicData);
		}

		/// <summary>
		/// 其他初始化工作
		/// </summary>
		protected override void initializeOthers() {
			base.initializeOthers();
			changeState(State.Unload);
		}

		#endregion
	}
}
