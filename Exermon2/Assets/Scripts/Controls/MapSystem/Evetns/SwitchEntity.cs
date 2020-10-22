
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
		public SpriteRenderer[] sprites; // 图片
		public Collider2D[] colliders; // 碰撞盒

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public Info.Switches relatedSwitch = Info.Switches.None; // 关联的开关信息

		public bool inverse = true; // 反向，即开关开启时才关闭挡板

		/// <summary>
		/// 外部系统设置
		/// </summary>
		PlayerService playerSer;

		/// <summary>
		/// 是否激活
		/// </summary>
		/// <returns></returns>
		public bool isActive() {
			if (relatedSwitch == Info.Switches.None) return false;
			var flag = playerSer.info.getSwitch(relatedSwitch);
			return inverse ? !flag : flag;
		}

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			var active = isActive();
			foreach(var sprite in sprites)
				sprite.enabled = active;
			foreach (var collider in colliders)
				collider.enabled = active;
		}
	}
}
