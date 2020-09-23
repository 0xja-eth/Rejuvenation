
using UnityEngine;

using Core.UI;

namespace UI.Common.Controls.Entities {

    /// <summary>
    /// 地图上的实体
    /// </summary>
    public class MapEntity : WorldComponent {

		/// <summary>
		/// 属性
		/// </summary>
		public float x => transform.position.x;
		public float y => transform.position.y;

	}
}