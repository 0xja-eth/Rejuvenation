using System;
using UnityEngine;

namespace UI.MapSystem.Controls {

    /// <summary>
    /// 技能处理器
    /// </summary>
    public class BigBulletProcessor : BulletProcessor {


        /// <summary>
        /// 配置位置
        /// </summary>
        protected override void setupPosition() {
            Vector3 position = skillProcessor.transform.position + new Vector3(0, UnityEngine.Random.Range(-0.5f, 1f), 0);
            transform.position = position;

        }

    }

}
