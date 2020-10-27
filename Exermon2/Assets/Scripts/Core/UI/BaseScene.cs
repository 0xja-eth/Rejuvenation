
using System.Collections;

using LitJson;

using UnityEngine;

using Config;

using Core.Systems;
using Core.UI.Utils;

using UI.Common.Windows;
using PlayerModule.Services;

namespace Core.UI {

	/// <summary>
	/// 场景基类
	/// </summary>
	/// <remarks>
	/// 所有场景类的基类，场景脚本均需要从该类派生
	/// 每个场景原则上都需要有一个场景脚本，用于对场景进行控制管理
	/// 该类定义了 alertWindow 和 loadingWindow，用于配置该场景的提示窗口和加载窗口
	/// 一般来说该脚本挂载在 MainCamera，请不要忘记给 promptLayer 赋值
	/// </remarks>
	public abstract class BaseScene : BaseComponent {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PromptLayer promptLayer; // 提示窗口

		/// <summary>
		/// BGM相关
		/// </summary>
		public AudioSource audioSource;
		public AudioClip bgmClip; // BGM
		
        /// <summary>
        /// 内部系统声明
        /// </summary>
        protected GameSystem gameSys;
        protected SceneSystem sceneSys;
        protected PlayerService playerSer;

        /// <summary>
        /// 内部变量设置
        /// </summary>
        protected bool acceptData = false;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public abstract SceneSystem.Scene sceneIndex();

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initializeSceneUtils();
            initializeOthers();
            checkFirstScene();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void initializeSceneUtils() {
            SceneUtils.initialize(this, audioSource);
        }
		
        /// <summary>
        /// 初始化其他项
        /// </summary>
        protected virtual void initializeOthers() { }

        /// <summary>
        /// 检查初始场景
        /// </summary>
        /// <returns></returns>
        public void checkFirstScene() {
            var first = Deploy.FirstScene;
            if (!playerSer.hasPlayer() && sceneIndex() != first)
                sceneSys.gotoScene(first);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();

            if (acceptData = sceneSys.tunnelData != null) {
				processTunnelData(sceneSys.tunnelData);
				sceneSys.clearTunnelData();
			}
        }

        /// <summary>
        /// 处理通道数据
        /// </summary>
        /// <param name="data">数据</param>
        protected virtual void processTunnelData(JsonData data) { }

        #endregion

        #region 更新控制

        protected override void update() {
            base.update(); SceneUtils.update();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 场景结束回调
        /// </summary>
        public virtual void onTerminated() { }

		#endregion

		#region 场景控制

		#region BGM控制

		/// <summary>
		/// 播放BGM
		/// </summary>
		public void playBGM() {
			SceneUtils.audioSource?.Play();
		}

		/// <summary>
		/// 暂停BGM
		/// </summary>
		public void pauseBGM() {
			SceneUtils.audioSource?.Pause();
		}

		/// <summary>
		/// 反转BGM
		/// </summary>
		public void toggleBGM() {
			if (SceneUtils.audioSource.isPlaying)
				pauseBGM();
			else
				playBGM();
		}

		#endregion

		/// <summary>
		/// 返回场景
		/// </summary>
		public virtual void popScene() {
            sceneSys.popScene();
        }

        #endregion
    }
}