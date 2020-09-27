using System;
using System.Collections.Generic;

using Core.UI;

using UnityEngine;

namespace UI.Common.Controls.MapSystem {

	using BattleSystem;

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
		public bool active = true;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		[HideInInspector]
		public MapPlayer player;
		List<MapEntity> entities = new List<MapEntity>();

		#region 实体管理

		/// <summary>
		/// 添加实体
		/// </summary>
		/// <param name="entity"></param>
		public void addEntity(MapEntity entity) {
			var player = entity as MapPlayer;
			if (player) {
				if (this.player) return;
				this.player = player;
			}

			entities.Add(entity);
		}

		/// <summary>
		/// 过滤特定类型的实体
		/// </summary>
		/// <returns></returns>
		List<T> filterEntities<T>(Predicate<T> match = null) where T : MapEntity {
			var res = new List<T>();
			foreach (var e in entities) {
				var t = e as T;
				if (t != null && (match == null || 
					match.Invoke(t))) res.Add(t);
			}
			return res;
		}

		/// <summary>
		/// 地图上的所有事件
		/// </summary>
		/// <returns></returns>
		public List<MapEvent> events() {
			return filterEntities<MapEvent>();
		}

		/// <summary>
		/// 地图上的所有敌人
		/// </summary>
		/// <returns></returns>
		public List<MapEnemy> enemies() {
			return filterEntities<MapEnemy>();
		}

		/// <summary>
		/// 地图上的所有移动实体
		/// </summary>
		/// <returns></returns>
		public List<MapCharacter> characters() {
			return filterEntities<MapCharacter>();
		}
		public List<MapCharacter> characters(MapCharacter.Type type) {
			return filterEntities<MapCharacter>(e => e.type == type);
		}

		/// <summary>
		/// 获取NPC
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public List<MapCharacter> npcs() {
			return characters(MapCharacter.Type.NPC);
		}

		#endregion
	}
}
