using System.Collections.Generic;

using UnityEngine;

using Core.Data;
using Core.UI;
using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

using BattleModule.Data;

namespace UI.Common.Controls.MapSystem {

	using BattleSystem;

	/// <summary>
	/// 主角远程攻击
	/// </summary>
	public class ActorLongRangeAttack : SkillProcessor {// ItemDisplay<Skill> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Transform shotPos;

		/// <summary>
		/// 预制件设置
		/// </summary>
		public GameObject bulletPerfab;

		/// <summary>
		/// 世界变换
		/// </summary>
		Transform world => battler.transform.parent;

		#region 执行&使用

		/// <summary>
		/// 使用技能
		/// </summary>
		protected override void onUse() {
			base.onUse();
			createBullet();
		}

		/// <summary>
		/// 发射子弹（剑气）
		/// </summary>
		/// <returns></returns>
		BulletProcessor createBullet() {
			var obj = Instantiate(bulletPerfab, world);
			var bullet = SceneUtils.get<BulletProcessor>(obj);

			setupTransform(obj.transform);
			bullet.activate(this, battler.direction);
			return bullet;
		}

		/// <summary>
		/// 配置子弹初始变换
		/// </summary>
		/// <param name="bullet"></param>
		void setupTransform(Transform bullet) {
			bullet.position = shotPos.position;
		}

		#endregion

	}
}