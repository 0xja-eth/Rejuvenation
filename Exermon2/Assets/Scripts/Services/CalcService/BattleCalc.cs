using System;

using Core.Services;

using BattleModule.Data;

/// <summary>
/// 游戏模块服务
/// </summary>
namespace GameModule.Services {

	/// <summary>
	/// 计算服务
	/// </summary>
	public partial class CalcService : BaseService<CalcService> {

		/// <summary>
		/// 特训行动结果生成器
		/// </summary>
		public class ActionResultGenerator {
			
			/// <summary>
			/// 结果
			/// </summary>
			RuntimeActionResult result;

			/// <summary>
			/// 属性
			/// </summary>
			RuntimeBattler subject => result.action.subject;
			RuntimeBattler object_ => result.object_;

			Skill skill => result.action.skill;

			/// <summary>
			/// 生成
			/// </summary>
			/// <param name="action">行动</param>
			/// <returns>返回结果</returns>
			public static void generate(RuntimeActionResult result) {
				var generator = new ActionResultGenerator(result);
				generator.processEffect();
			}

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="action">行动</param>
			ActionResultGenerator(RuntimeActionResult result) {
				this.result = result; 
			}

			/// <summary>
			/// 处理行动效果
			/// </summary>
			void processEffect() {
				// TODO: 添加更多效果
				processDamage(skill.power);
				processHitFreeze(skill.hitting, skill.freezing);
			}

			#region 处理函数

			/// <summary>
			/// 计算伤害值
			/// </summary>
			/// <param name="a">伤害点数</param>
			/// <param name="h">伤害类型</param>
			void processDamage(float a) {
				var val = calcDamage(a);
				result.hpDamage = val;
				//if (h == HPRecoverType)
				//	result.hpRecover = calcRecover(a);
				//else {
				//	var val = calcDamage(a);
				//	if (h == HPDamageType) result.hpDamage = val;
				//	if (h == HPDrainType) result.hpDrain = val;
				//}
			}

			/// <summary>
			/// 计算伤害
			/// </summary>
			/// <param name="a"></param>
			/// <returns></returns>
			int calcDamage(float a) {
				double res = a + subject.attack - object_.defense;

				res += subject.damagePlusVal();
				res += object_.hurtPlusVal();

				res *= subject.damagePlusRate();
				res *= object_.hurtPlusRate();

				return (int)Math.Round(Math.Max(res, 1));
			}

			/// <summary>
			/// 计算回复
			/// </summary>
			/// <param name="a"></param>
			/// <returns></returns>
			int calcRecover(float a) {
				double res = a;

				res += object_.recoverPlusVal();
				res *= object_.recoverPlusRate();

				return (int)Math.Round(Math.Max(res, 1));
			}

			/// <summary>
			/// 处理受击硬直
			/// </summary>
			/// <param name="f">硬直</param>
			void processHitFreeze(float h, float f) {
				result.hitting = h;
				result.freezing = f;
			}

			/// <summary>
			/// 处理Buff
			/// </summary>
			/// <param name="p">属性ID</param>
			/// <param name="a">点数</param>
			/// <param name="b">比率</param>
			/// <param name="n">回合数（为-1则为永久）</param>
			void processBuff(int p, float a = 0, float b = 100, int n = 0) {
				addBuff(p, a, b / 100.0, n);
			}

			/// <summary>
			/// 添加Buff
			/// </summary>
			/// <param name="paramId">状态ID</param>
			/// <param name="value">状态ID</param>
			/// <param name="rate">状态ID</param>
			/// <param name="turns">持续回合</param>
			void addBuff(int paramId, float value = 0, double rate = 1, int turns = 0) {
				result.addBuffs.Add(new RuntimeBuff(paramId, value, rate, turns));
			}

			#endregion
		}

		/// <summary>
		/// 结果应用计算类
		/// </summary>
		public class ResultApplyCalc {

			/// <summary>
			/// 属性
			/// </summary>
			RuntimeBattler battler;
			RuntimeActionResult result;

			RuntimeBattler subject; // 发起者
			RuntimeActor actor; // 关联的玩家

			/// <summary>
			/// 应用
			/// </summary>
			/// <param name="battler">对战者</param>
			/// <param name="result">结果</param>
			public static void apply(RuntimeBattler battler, RuntimeActionResult result) {
				var calc = new ResultApplyCalc(battler, result);
				calc.processHP();
				calc.processHitFreeze();
				calc.processAddBuffs();
			}

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="battler">对战者</param>
			/// <param name="result">结果</param>
			ResultApplyCalc(RuntimeBattler battler, RuntimeActionResult result) {
				this.battler = battler;
				this.result = result;

				subject = result.action?.subject;
				actor = (battler as RuntimeActor) ?? 
					(subject as RuntimeActor);
			}

			/// <summary>
			/// 处理HP
			/// </summary>
			void processHP() {
				battler.addHP(result.hpRecover);
				battler.addHP(-result.hpDamage);
				battler.addHP(-result.hpDrain);

				subject?.addHP(result.hpDrain);
			}

			/// <summary>
			/// 处理硬直
			/// </summary>
			void processHitFreeze() {
				battler.setHitting(result.hitting);
				battler.setFreezing(result.freezing);
			}

			/// <summary>
			/// 处理Buff增加
			/// </summary>
			void processAddBuffs() {
				foreach (var buff in result.addBuffs)
					battler.addBuff(buff);
			}
			
		}

	}

}
