
using UnityEngine;

using Core.UI;

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

		/// <summary>
		/// 内部控件设置
		/// </summary>
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

		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected override void initializeCollFuncs() {
			//registerOnStayFunc<ISkillApplication>(onSkillHit);
		}

		#endregion

		#region 技能控制

		///// <summary>
		///// 技能命中回调
		///// </summary>
		///// <param name="skill"></param>
		//void onSkillHit(ISkillApplication skill) {
		//	if (isHittable() && isValidSkill(skill)) apply(skill);
		//}

		///// <summary>
		///// 技能应用
		///// </summary>
		///// <param name="skill"></param>
		//protected virtual void apply(ISkillApplication skill) {
		//	skill.applyEntity(this);
		//}

		///// <summary>
		///// 能否被击中
		///// </summary>
		///// <returns></returns>
		//public virtual bool isApplyable() {
		//	return true;
		//}

		///// <summary>
		///// 是否有效的技能
		///// </summary>
		///// <returns></returns>
		//protected virtual bool isValidSkill(ISkillApplication skill) {
		//	return skill.isApplyValid();
		//}

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
	}
}