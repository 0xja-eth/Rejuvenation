
using UnityEngine;
using UnityEngine.Events;

using PlayerModule.Data;

using PlayerModule.Services;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 开关
    /// </summary>
    public class MapButton : MapEventPage {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public Sprite offSprite, onSprite; // On/Off 状态精灵

		public Info.Switches relatedSwitch = Info.Switches.None; // 关联的开关信息

		public bool reset = false; // 是否在不满足触发条件后重置
		public float resetDuration = 0; // 重置用时

		public bool toggle = false; // 是否在多次满足触发条件时反转

		public UnityEvent onOn, onOff;

		bool _isOn = false;
		[SerializeField]
		public bool isOn {
			get => _isOn;
			set {
				_isOn = value;
				// 同步开关
				if (relatedSwitch != Info.Switches.None)
					playerSer.info.setSwitch(relatedSwitch, _isOn);
			}
		}

		/// <summary>
		/// 内部变量定义
		/// </summary>
		float resetTime = 0;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		PlayerService playerSer;

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateReset();
			updateState();
		}

		/// <summary>
		/// 更新重置
		/// </summary>
		void updateReset() {
			if (reset && resetTime >= 0) {
				resetTime -= Time.deltaTime;
				if (resetTime <= 0) isOn = false;
			}
		}

		/// <summary>
		/// 更新状态
		/// </summary>
		void updateState() {
			picture = isOn ? onSprite : offSprite;
		}

		/// <summary>
		/// 执行
		/// </summary>
		protected override void invokeCustom() {
			base.invokeCustom();
			isOn = toggle ? !isOn : true;

			if (reset) resetTime = resetDuration;

			(isOn ? onOn : onOff).Invoke();
		}

	}
}
