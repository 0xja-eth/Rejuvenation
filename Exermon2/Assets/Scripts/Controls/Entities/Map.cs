using System;
using System.Collections.Generic;

using Core.UI;

using UnityEngine;

namespace UI.Common.Controls.Entities {

	/// <summary>
	/// 地图类
	/// </summary>
	public class Map : WorldComponent {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public new Camera camera;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		List<MapEntity> entities = new List<MapEntity>();

		#region 实体管理

		/// <summary>
		/// 添加实体
		/// </summary>
		/// <param name="entity"></param>
		public void addEntity(MapEntity entity) {
			entities.Add(entity);
		}

		/// <summary>
		/// 地图上的所有事件
		/// </summary>
		/// <returns></returns>
		public List<MapEvent> events() {

		}

		#endregion
	}
}
