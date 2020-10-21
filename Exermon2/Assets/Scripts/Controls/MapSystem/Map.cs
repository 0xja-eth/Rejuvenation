using System;
using System.Collections.Generic;

using Core.UI;
using Core.UI.Utils;

using MapModule.Data;

using UI.MapSystem;

using UnityEngine;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

	/// <summary>
	/// 地图类
	/// </summary>
	public class Map : WorldComponent {
		
		/// <summary>
		/// 外部变量定义
		/// </summary>
		public bool active = true;
		public TimeType type = TimeType.Present;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		List<MapEntity> entities = new List<MapEntity>();

		/// <summary>
		/// 属性
		/// </summary>
		public BaseMapScene scene => SceneUtils.getCurrentScene<BaseMapScene>();

		public new Camera camera => scene?.camera;

		[HideInInspector]
		public MapPlayer player; // => scene?.player;

		public Vector2 position {
			get => transform.position;
			set { transform.position = value; }
		}

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void start() {
            base.start();
            //entities.Add(player);
        }

        #endregion

        #region	状态判断

        /// <summary>
        /// 是否繁忙
        /// </summary>
        /// <returns></returns>
        public bool isBusy() {
			return scene && scene.isBusy();
		}

		/// <summary>
		/// 是否激活
		/// </summary>
		/// <returns></returns>
		public bool isActive() {
			return active && camera && !isBusy();
		}

		#endregion

		#region 实体管理

		/// <summary>
		/// 添加实体
		/// </summary>
		/// <param name="entity"></param>
		public void addEntity(MapEntity entity) {
			debugLog("addEntity: " + entity);
			var player = entity as MapPlayer;
			if (player != null) this.player = player;
			debugLog("addEntity.player: " + player);

			entities.Add(entity);
		}

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        public void removeEntity(MapEntity entity) {
			debugLog("removeEntity: " + entity);
			if (entity == player) player = null;
			debugLog("removeEntity.player: " + player);

			entities.Remove(entity);
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
		/// 地图上的所有战斗者
		/// </summary>
		/// <returns></returns>
		public List<MapBattler> battlers() {
			return filterEntities<MapBattler>();
		}
		public List<MapBattler> battlers(MapCharacter.Type type) {
			return filterEntities<MapBattler>(b => b.type == type);
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

		#region 传送操作

		/// <summary>
		/// 转移
		/// </summary>
		/// <param name="player"></param>
		public void travel(Map newMap) {
			player.changeMap(newMap);
		}

		#endregion
	}
}
