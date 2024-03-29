﻿using System.Collections.Generic;

using UnityEngine;

using Core.Data;
using Core.UI;
using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

using BattleModule.Data;

namespace UI.MapSystem.Controls {

	using BattleSystem.Controls;

	/// <summary>
	/// 主角远程攻击
	/// </summary>
	public class ActorLongRangeAttack : SkillProcessor {// ItemDisplay<Skill> {

		/// <summary>
		/// 预制件设置
		/// </summary>
		public GameObject bulletPerfab;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		List<BulletProcessor> bullets = new List<BulletProcessor>();

		/// <summary>
		/// 世界变换
		/// </summary>
		Transform world => battler.transform.parent;


		#region	更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			udpateBullet();
		}

		/// <summary>
		/// 更新子弹
		/// </summary>
		void udpateBullet() {
			var bullets = new List<BulletProcessor>(this.bullets);

			foreach (var bullet in bullets) {
                if (!bullet) return;
                if (battler.runtimeBattler.isDead()) bullet.destroy();
                if (bullet.destroyFlag) {
                    bullet.destroy(true);
                    this.bullets.Remove(bullet);
                    //bullet = null;
                }
            }
		}

		#endregion

		#region 执行&使用

		/// <summary>
		/// 是否结束
		/// </summary>
		/// <returns></returns>
		public override bool isTerminated() {
			return base.isTerminated();
			//return !bullet && base.isTerminated();
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		public override void onUse() {
			base.onUse();

            if (battler.runtimeBattler.isDead())
                return;

            var bullet = createBullet();
            bullets.Add(bullet);
		}

        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        public override bool isApplyValid() {
            return bullets.Count >= 1;
        }

        /// <summary>
        /// 发射子弹（剑气）
        /// </summary>
        /// <returns></returns>
        BulletProcessor createBullet() {
			var obj = Instantiate(bulletPerfab, world);
			var bullet = SceneUtils.get<BulletProcessor>(obj);
			bullet.activate(this, battler.direction);
			return bullet;
		}

		#endregion

	}
}