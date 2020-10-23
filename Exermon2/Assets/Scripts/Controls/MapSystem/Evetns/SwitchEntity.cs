
using UnityEngine;
using UnityEngine.Events;

using PlayerModule.Services;
using PlayerModule.Data;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 通过开关驱动的实体
    /// </summary>
    public class SwitchEntity : MapEntity {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public GameObject[] objects; // 对象
		public SpriteRenderer[] sprites; // 图片
		public Collider2D[] colliders; // 碰撞盒

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public Info.Switches[] relatedSwitches = new Info.Switches[0]; // 关联的开关信息（AND）

		public bool inverse = true; // 反向，即开关开启时才关闭挡板

		/// <summary>
		/// 外部系统设置
		/// </summary>
		protected PlayerService playerSer;

		/// <summary>
		/// 是否激活
		/// </summary>
		/// <returns></returns>
		public virtual bool isActive() {
			var flag = isAllSwitchesOn();
			return inverse ? !flag : flag;
		}

		/// <summary>
		/// 是否所有开关都打开
		/// </summary>
		/// <returns></returns>
		bool isAllSwitchesOn() {
			foreach (var s in relatedSwitches)
				// 如果有一个开关是 false
				if (s != Info.Switches.None && 
					!playerSer.info.getSwitch(s)) return false;
			return true;
		}

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			var active = isActive();
			foreach (var obj in objects)
				obj.SetActive(active);
			foreach (var sprite in sprites)
				sprite.enabled = active;
			foreach (var collider in colliders)
				collider.enabled = active;
		}
	}
}
