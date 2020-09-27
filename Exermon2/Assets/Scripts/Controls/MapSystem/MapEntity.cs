
using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

namespace UI.Common.Controls.MapSystem {

	/// <summary>
	/// 地图上的实体
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class MapEntity : WorldComponent {

		/// <summary>
		/// YZ转换系数
		/// </summary>
		const float YZConvert = 0.0001f;

		/// <summary>
		/// 属性
		/// </summary>
		public float x => transform.position.x;
		public float y => transform.position.y;

		/// <summary>
		/// 内部控件设置
		/// </summary>
		[RequireTarget]
		protected new Collider2D collider;
		protected Map map;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeMap();
		}

		/// <summary>
		/// 初始化地图配置
		/// </summary>
		void initializeMap() {
			map = findParent<Map>();
			map?.addEntity(this);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateZCoord();
		}

		/// <summary>
		/// 更新Z坐标
		/// </summary>
		void updateZCoord() {
			var cz = map.camera.transform.position.z;
			var pos = transform.position;

			pos.z = mapY2Z(pos.y, cz);
			transform.position = pos;
		}

		/// <summary>
		/// 地图上Y坐标转化为Z坐标
		/// </summary>
		/// <returns></returns>
		public static float mapY2Z(float y, float cz = -1) {
			return (float)CalcService.Common.sigmoid(-y * YZConvert) * cz;
		}

		#endregion
	}
}