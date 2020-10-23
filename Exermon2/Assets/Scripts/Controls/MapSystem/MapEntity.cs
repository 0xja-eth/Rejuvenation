
using UnityEngine;

using Core.UI.Utils;

using GameModule.Services;

using UI.Common.Controls.SystemExtend.PhysicsExtend;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;
	
	/// <summary>
	/// 地图上的实体
	/// </summary>
	public class MapEntity : Collider2DExtend {

		/// <summary>
		/// YZ转换系数
		/// </summary>
		const float YZConvert = 0.0001f;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public bool autoZ = true; // 自动调整Z坐标

		/// <summary>
		/// 位置
		/// </summary>
		public float x => transform.position.x;
		public float y => transform.position.y;

		public Vector2 pos {
			get => transform.position;
			set { transform.position = value; }
		}
		public Vector2 mapPos {
			get => transform.localPosition;
			set { transform.localPosition = value; }
			//get => transform.position - (Vector3)map.position;
			//set { transform.position = map.position + value; }
		}

		/// <summary>
		/// 场景组件
		/// </summary>
		public BaseMapScene scene => SceneUtils.getCurrentScene<BaseMapScene>();

		/// <summary>
		/// 内部控件设置
		/// </summary>
		[HideInInspector]
		public Map map;
		//Map _map = null;
		//public Map map {
		//	get => _map;
		//	set {
		//		if (_map == value) return;
		//		if (_map == null || value == null) _map = value;
		//		else {
		//			var lastPos = mapPos;
		//			_map = value; mapPos = lastPos;
		//		}
		//		onMapChanged();
		//	}
		//}

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			//initializeMap();
		}

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			base.start();
			setupPosition();
		}

		/// <summary>
		/// 配置（初始）
		/// </summary>
		void setupPosition() {
			changeMap(findParent<Map>(), true);
			mapPos = startPosition();
		}

		/// <summary>
		/// 初始位置
		/// </summary>
		/// <returns></returns>
		protected virtual Vector2 startPosition() {
			return mapPos;
		}

		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected override void initializeCollFuncs() {
			//registerOnStayFunc<ISkillApplication>(onSkillHit);
		}

		/// <summary>
		/// 销毁回调（同OnDestroy）
		/// </summary>
		protected override void onDestroy() {
			base.onDestroy();
			map?.removeEntity(this);
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
			if (!autoZ) return;
			var pos = transform.position;
			pos.z = calcZCoord(pos);
			transform.position = pos;
		}

		/// <summary>
		/// 计算新坐标
		/// </summary>
		protected virtual float calcZCoord(Vector3 pos) {
			if (!map?.camera) return 0;

			var cz = map.camera.transform.position.z;
			return mapY2Z(pos.y, cz);
		}

		/// <summary>
		/// 地图上Y坐标转化为Z坐标
		/// </summary>
		/// <returns></returns>
		public static float mapY2Z(float y, float cz = -1) {
			return (float)CalcService.Common.sigmoid(-y * YZConvert) * cz;
		}

		#endregion

		#region 地图控制

		/// <summary>
		/// 更改所处地图
		/// </summary>
		/// <param name="map">新地图</param>
		/// <param name="origin">是否初始地图</param>
		public virtual void changeMap(Map map, bool origin = false) {
			if (this.map == map) return; // 无变化
			this.map?.removeEntity(this);

			if ((this.map = map) == null) return;
			map.addEntity(this);

			if (!origin) moveToMapChildren();

			onMapChanged();
		}

		/// <summary>
		/// 移动到地图组件的Children
		/// </summary>
		void moveToMapChildren() {
			var mapPos = this.mapPos;
			transform.parent = map.transform;
			this.mapPos = mapPos;
		}

		/// <summary>
		/// 地图改变回调
		/// </summary>
		protected virtual void onMapChanged() {

		}

		#endregion
	}
}