using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

using Core.UI;

using MapModule.Data;
using BattleModule.Data;

namespace UI.Common.Controls.MapSystem {

	using BattleSystem;
	using SystemExtend.PhysicsExtend;

	/// <summary>
	/// 技能处理器
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class BulletProcessor : Collider2DExtend {//, ISkillApplication { 

		/// <summary>
		/// 最大范围
		/// </summary>
		const float DefaultMaxRange = 100;

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public SkillProcessor skillProcessor;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		new Rigidbody2D rigidbody;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public float speed = 1; // 移动速度
		public float range = 4; // 最大范围（-1为无穷大）
		public int throughEntities = 0; // 可穿透实体个数（-1为无穷大）
		public int throughWalls = 0; // 可穿透墙壁个数（-1为无穷大）

		/// <summary>
		/// 内部变量定义
		/// </summary>
		int entityCnt = 0, wallCnt = 0;
		Vector2 oriPos;

		/// <summary>
		/// 技能
		/// </summary>
		public Skill skill => skillProcessor.skill;

		#region 初始化

		/// <summary>
		/// 初始化碰撞函数
		/// </summary>
		protected override void initializeCollFuncs() {
			registerOnEnterFunc<Tilemap>(t => apply(t));
			registerOnEnterFunc<MapEntity>(e => apply(e));
		}

		/// <summary>
		/// 激活
		/// </summary>
		public override void activate() {
			base.activate();

			if (range == -1) range = DefaultMaxRange;
			oriPos = transform.position;
		}
		public void activate(SkillProcessor skill, 
			RuntimeCharacter.Direction d) {
			activate();
			debugLog("activate: " + d);

			skillProcessor = skill;
			setupRotation(d);
			setupVelocity(d);
		}

		/// <summary>
		/// 配置旋转
		/// </summary>
		void setupRotation(RuntimeCharacter.Direction d) {
			var rot = transform.rotation;
			rot.z = RuntimeCharacter.dir82Angle(d);
			transform.rotation = rot;
		}

		/// <summary>
		/// 配置速度
		/// </summary>
		void setupVelocity(RuntimeCharacter.Direction d) {
			var vec = RuntimeCharacter.dir82Vec(d);
			rigidbody.velocity = vec * speed;
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateCollider();
			updateRange();
		}

		/// <summary>
		/// 更新碰撞体
		/// </summary>
		void updateCollider() {
			collider.enabled = !skillProcessor.isTerminated();
		}
		
		/// <summary>
		/// 更新最大范围
		/// </summary>
		void updateRange() {
			var deltaPos = (Vector2)transform.position - oriPos;
			var dist = deltaPos.magnitude;
			if (dist >= range) destroy();
		}

		#endregion
		
		#region 作用

		/// <summary>
		/// 是否有效
		/// </summary>
		/// <returns></returns>
		public virtual bool isApplyValid() {
			return skillProcessor.isApplyValid();
		}

		/// <summary>
		/// 作用到不同物体
		/// </summary>
		public virtual bool apply(Tilemap map) {
			if (!skillProcessor.apply(map)) return false;

			// 不可无限穿透同时穿透个数超出最大个数
			if (throughWalls != -1 &&
				wallCnt++ >= throughWalls) destroy();
			return true;
		}
		public virtual bool apply(MapEntity entity) {
			if (!skillProcessor.apply(entity)) return false;

			// 不可无限穿透同时穿透个数超出最大个数
			if (throughEntities != -1 &&
				entityCnt++ >= throughEntities) destroy();
			return true;
		}

		#endregion
	}
}