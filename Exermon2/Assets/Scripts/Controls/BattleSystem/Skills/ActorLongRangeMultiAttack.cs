using System.Collections.Generic;

using UnityEngine;

using Core.Data;
using Core.UI;
using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

using BattleModule.Data;

namespace UI.MapSystem.Controls {

    using BattleSystem.Controls;
    using MapModule.Data;
    using System.Collections;

    /// <summary>
    /// 主角远程攻击
    /// </summary>
    public class ActorLongRangeMultiAttack : SkillProcessor {// ItemDisplay<Skill> {

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
            foreach (var bullet in bullets) {
                if (!bullet) return;
                if (bullet.destroyFlag) {
                    bullets.Remove(bullet);
                    bullet.destroy(true);
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

            doRoutine("test");
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

            return bullet;
        }

        IEnumerator test() {

            for (int i = 0; i < 5; i++) {
                yield return new WaitForSeconds(0.8f);
                var bullet = createBullet();
                bullets.Add(bullet);

                bullet.activate(this, battler.direction);

            }

        }

        #endregion

    }
}