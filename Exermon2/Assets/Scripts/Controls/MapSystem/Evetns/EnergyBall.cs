
using UnityEngine;
using UnityEngine.Events;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 开关
    /// </summary>
    public class EnergyBall : MapEventPage {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public Sprite offSprite, onSprite; // On/Off 状态精灵

		public bool isOn = false;

		public bool reset = false; // 是否在不满足触发条件后重置
		public float resetDuration = 0; // 重置用时

		public bool toggle = false; // 是否在多次满足触发条件时反转

		public UnityEvent onOn, onOff; 

		/// <summary>
		/// 内部变量定义
		/// </summary>
		float resetTime = 0;

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
			if (reset && resetTime > 0) {
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
