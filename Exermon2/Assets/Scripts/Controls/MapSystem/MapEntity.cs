
using UnityEngine;

using Core.UI;

using GameModule.Services;

using UI.Common.Controls.SystemExtend.PhysicsExtend;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;


	///// <summary>
	///// 技能应用接口
	///// </summary>
	//public interface ISkillApplication : IBaseComponent {

	//	/// <summary>
	//	/// 作用到不同物体上
	//	/// </summary>
	//	/// <param name="batler"></param>
	//	void applyBattler(MapBattler batler);
	//	void applyEntity(MapEntity entity);
	//	void applyMap(Tilemap map);

	//	/// <summary>
	//	/// 是否可应用
	//	/// </summary>
	//	/// <returns></returns>
	//	bool isApplyValid();
	//}

	/// <summary>
	/// 地图上的实体
	/// </summary>
	public class MapEntity : Collider2DExtend {

		/// <summary>
		/// YZ转换系数
		/// </summary>
		const float YZConvert = 0.0001f;

		/// <summary>
		/// 属性
		/// </summary>
		public float x => transform.position.x;
		public float y => transform.position.y;

		public Vector2 pos => transform.position;

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

		/// <summary>
		/// 能否被击中
		/// </summary>
		/// <returns></returns>
		public virtual bool isApplyable() {
			return true;
		}

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