﻿
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Random = UnityEngine.Random;

using LitJson;

using Config;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using PlayerModule.Data;
using PlayerModule.Services;

using MapModule.Data;

using ItemModule.Data;

/// <summary>
/// 战斗模块
/// </summary>
namespace BattleModule { }

/// <summary>
/// 战斗模块数据
/// </summary>
namespace BattleModule.Data {

	/// <summary>
	/// 特训效果数据
	/// </summary>
	public class TraitData : BaseData {

		/// <summary>
		/// 效果代码枚举
		/// </summary>
		public enum Code {
			Unset = 0, // 空

			DamagePlus = 1, // 攻击伤害加成
			HurtPlus = 2, // 受到伤害加成
			RecoverPlus = 3, // 回复加成

			ParamAdd = 9, // 属性加成值
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int code { get; protected set; }
		[AutoConvert("params")]
		public JsonData params_ { get; protected set; } // 参数（数组）

		#region 数据获取

		/// <summary>
		/// 获取指定下标下的数据
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="index">下标</param>
		/// <param name="default_">默认值</param>
		/// <returns></returns>
		public T get<T>(int index, T default_ = default) {
			if (params_ == null || !params_.IsArray) return default_;
			if (index < params_.Count) return DataLoader.load<T>(params_[index]);
			return default_;
		}

		/// <summary>
		/// 特性代码枚举
		/// </summary>
		/// <returns></returns>
		public Code codeEnum() {
			return (Code)code;
		}

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public TraitData() { }
		public TraitData(Code code, JsonData params_) {
			this.code = (int)code; this.params_ = params_;
		}

	}

	/// <summary>
	/// 战斗者
	/// </summary>
	public class Battler : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		[SerializeField] string _name;
		[AutoConvert] public string name { get => _name; set { _name = value; } }

		[SerializeField] int _mhp = 4;
		[AutoConvert] public int mhp { get => _mhp; set { _mhp = value; } }

		[SerializeField] float _speed = 2;
		[AutoConvert] public float speed { get => _speed; set { _speed = value; } }

		[SerializeField] float _attack = 0;
		[AutoConvert] public float attack { get => _attack; set { _attack = value; } }

		[SerializeField] float _defense = 0;
		[AutoConvert] public float defense { get => _defense; set { _defense = value; } }

		/// <summary>
		/// 行走图属性
		/// </summary>
		[SerializeField] int _characterId = 0;
		[AutoConvert] public int characterId {
			get => _characterId;
			set { _characterId = value; clearCaches(); }
		}

		/// <summary>
		/// 获取动画实例
		/// </summary>
		/// <returns></returns>
		protected CacheAttr<Sprite[]> character_ = null;
		protected Sprite[] _character_() {
			return AssetLoader.loadAssets<Sprite>(
				Asset.Type.Character, characterId);
		}
		public Sprite[] character() {
			return character_?.value();
		}

		/// <summary>
		/// 获取动画实例
		/// </summary>
		/// <returns></returns>
		protected CacheAttr<Sprite[]> attackAni_ = null;
		protected Sprite[] _attackAni_() {
			return AssetLoader.loadAssets<Sprite>(
				Asset.Type.Battler, characterId);
		}
		public Sprite[] attackAni() {
			return attackAni_?.value();
		}

		/// <summary>
		/// 每方向攻击动画参数
		/// </summary>
		public Vector4 characterFrameCounts = new Vector4(3, 3, 3, 3); // 每方向攻击动画帧数（下，左，右，上）
		public Vector4 characterFrameStarts = new Vector4(0, 3, 6, 9); // 每方向攻击动画开始帧（下，左，右，上）
		public Vector4 characterStaticFrames = new Vector4(1, 1, 1, 1); // 每方向的静止帧（下，左，右，上）

		public bool characterFlip = false; // 行走图是否需要翻转（朝向左时候翻转）

		/// <summary>
		/// 获取指定行列的精灵
		/// </summary>
		/// <returns></returns>
		public Sprite getCharacter(int d, float rate) {
			var count = (int)characterFrameCounts[d] + 1;
			var start = (int)characterFrameStarts[d];
			int index = (int)Mathf.Floor(count * rate);
			if (index >= count - 1)
				index = (int)characterStaticFrames[d];

			return getSprite(character(), start + index);
		}

		/// <summary>
		/// 每方向攻击动画参数
		/// </summary>
		public Vector4 attackAniFrameCounts = new Vector4(6, 8, 8, 6); // 每方向攻击动画帧数（下，左，右，上）
		public Vector4 attackAniFrameStarts = new Vector4(8, 0, 0, 14); // 每方向攻击动画开始帧（下，左，右，上）

		public bool attackAniFlip = true; // 攻击动画是否需要翻转（朝向左时候翻转）

		/// <summary>
		/// 获取指定方向指定比率的战斗动画
		/// </summary>
		/// <returns></returns>
		public Sprite getAttackAni(int d, float rate) {
			var count = (int)attackAniFrameCounts[d];
			var start = (int)attackAniFrameStarts[d];
			int index = (int)Mathf.Floor(count * rate);
			if (index >= count) index = count - 1;

			return getSprite(attackAni(), start + index);
		}

		/// <summary>
		/// 获取单个精灵
		/// </summary>
		/// <returns></returns>
		Sprite getSprite(Sprite[] sprites, int index) {
			if (index < 0 || index >= sprites.Length) return null;
			return sprites[index];
		}
	}

	/// <summary>
	/// 敌人
	/// </summary>
	[Serializable]
	public class Enemy : Battler {

		/// <summary>
		/// 刷新类型
		/// </summary>
		public enum RefreshType {
			None, // 不刷新
			OnDead, // 主角死亡后重置
			Custom // 自定义
		}

		/// <summary>
		/// 行为类型
		/// </summary>
		public enum BehaviourType {
			None, // 原地不动
			Random, // 随机移动
			Close, // 靠近主角
			Far, // 远离主角
			Custom // 自定义
		}

		/// <summary>
		/// 属性
		/// </summary>
		[SerializeField] string _description;
		[AutoConvert] public string description { get => _description; set { _description = value; } }

		[SerializeField] RefreshType _refreshType = RefreshType.OnDead;
		[AutoConvert] public RefreshType refreshType { get => _refreshType; set { _refreshType = value; } }

		[SerializeField] float _frequency = 1;
		[AutoConvert] public float frequency { get => _frequency; set { _frequency = value; } }

		[SerializeField] float _criticalRange;
		[AutoConvert] public float criticalRange { get => _criticalRange; set { _criticalRange = value; } }

		[SerializeField] BehaviourType _generalBehaviour = BehaviourType.None;
		[AutoConvert] public BehaviourType generalBehaviour { get => _generalBehaviour; set { _generalBehaviour = value; } }

		[SerializeField] BehaviourType _criticalBehaviour = BehaviourType.Close;
		[AutoConvert] public BehaviourType criticalBehaviour { get => _criticalBehaviour; set { _criticalBehaviour = value; } }

	}

	/// <summary>
	/// 主角
	/// </summary>
	public class Actor : Battler {

		/// <summary>
		/// 默认属性
		/// </summary>
		public const int DefaultMHP = 10; // 初始体力值
		public const float DefaultAttack = 0; // 初始力量
        public const float DefaultDefense = 0; // 初始格挡
        public const float DefaultSpeed = 3; // 初始速度

        /// <summary>
        /// 控制模式
        /// </summary>
        public enum ControlType {
			Present, Past, Both
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public ControlType control { get; protected set; } = ControlType.Present;
		[AutoConvert]
		public RuntimeActor runtimeActor { get; protected set; }

		/// <summary>
		/// 玩家
		/// </summary>
		public Player player;

		/// <summary>
		/// ID是否可用
		/// </summary>
		/// <returns></returns>
		protected override bool idEnable() {
			return false;
		}

		/// <summary>
		/// 切换控制
		/// </summary>
		public void switchControl() {
			switch (control) {
				case ControlType.Present:
					control = ControlType.Past; break;
				case ControlType.Past:
					control = ControlType.Present; break;
			}
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public Actor() { }
		public Actor(Player player) {
			name = player.name;

			mhp = DefaultMHP;
			attack = DefaultAttack;
			defense = DefaultDefense;
            speed = DefaultSpeed;

            this.player = player;
			runtimeActor = new RuntimeActor();
		}
	}

	/// <summary>
	/// 技能
	/// </summary>
	[Serializable]
	public class Skill : BaseItem {

		/// <summary>
		/// 攻击类型
		/// </summary>
		public enum RangeType {
			ShortRange, LongRange
		}

		/// <summary>
		/// 属性
		/// </summary>
		[SerializeField] string _name;
		[AutoConvert] public override string name {
			get => _name; protected set { _name = value; }
		}

		[SerializeField] string _description;
		[AutoConvert] public override string description {
			get => _description; protected set { _description = value; }
		}

		[SerializeField] RangeType _rangeType = RangeType.ShortRange;
		[AutoConvert] public RangeType rangeType { get => _rangeType; set { _rangeType = value; } }

		[SerializeField] float _frequency;
		[AutoConvert] public float frequency { get => _frequency; set { _frequency = value; } }

		[SerializeField] float _speed;
		[AutoConvert] public float speed { get => _speed; set { _speed = value; } }

		[SerializeField] float _range;
		[AutoConvert] public float range { get => _range; set { _range = value; } }

		/// <summary>
		/// 效果属性
		/// </summary>
		[SerializeField] float _power = 1;
		[AutoConvert] public float power { get => _power; set { _power = value; } }

		[SerializeField] float _hitting = 0.4f;
		[AutoConvert] public float hitting { get => _hitting; set { _hitting = value; } }

		[SerializeField] float _freezing = 0.75f;
		[AutoConvert] public float freezing { get => _freezing; set { _freezing = value; } }

		/// <summary>
		/// 动画属性
		/// </summary>
		[AutoConvert]
		public int startAnimationId { get; set; }
		[AutoConvert]
		public int targetAnimationId { get; set; }

		/// <summary>
		/// Editor中赋值
		/// </summary>
		[SerializeField] AnimationClip _startAnimation = null;
		[SerializeField] AnimationClip _targetAnimation = null;

		/// <summary>
		/// 获取动画实例
		/// </summary>
		/// <returns></returns>
		protected CacheAttr<AnimationClip> startAnimation_ = null;
		protected AnimationClip _startAnimation_() {
			return AssetLoader.loadAnimation(startAnimationId);
		}
		public AnimationClip startAnimation() {
			return _startAnimation ?? startAnimation_?.value();
		}

		protected CacheAttr<AnimationClip> targetAnimation_ = null;
		protected AnimationClip _targetAnimation_() {
			return AssetLoader.loadAnimation(targetAnimationId);
		}
		public AnimationClip targetAnimation() {
			return _targetAnimation ?? targetAnimation_?.value();
		}

	}

	#region 运行时数据

	/// <summary>
	/// 运行时BUFF
	/// </summary>
	public class RuntimeBuff : RuntimeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int paramId { get; protected set; } // 属性ID
		[AutoConvert]
		public float value { get; protected set; } // 改变数值
		[AutoConvert]
		public double rate { get; protected set; } // 改变比率
		[AutoConvert]
		public float seconds { get; protected set; } // BUFF持续时间

		/// <summary>
		/// 是否为Debuff
		/// </summary>
		/// <returns></returns>
		public bool isDebuff() {
			return value < 0 || rate < 1;
		}

		/// <summary>
		/// BUFF是否过期
		/// </summary>
		/// <returns></returns>
		public bool isOutOfDate() {
			return seconds == 0;
		}

		/// <summary>
		/// 更新
		/// </summary>
		public void update() {
			if (seconds > 0) seconds -= Time.deltaTime;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeBuff() { }
		public RuntimeBuff(int paramId,
			float value = 0, double rate = 1, float seconds = 0) {
			this.paramId = paramId; this.value = value;
			this.rate = rate; this.seconds = seconds;
		}

	}

	/// <summary>
	/// 运行时技能
	/// </summary>
	public class RuntimeSkill : RuntimeData {

		/// <summary>
		/// 条件函数
		/// </summary>
		/// <returns></returns>
		public delegate bool Condition();

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public float rate { get; protected set; }

		/// <summary>
		/// 技能
		/// </summary>
		public Skill skill { get; protected set; }

		/// <summary>
		/// 条件
		/// </summary>
		public Condition condition { get; protected set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeSkill() { }
		public RuntimeSkill(Skill skill, float rate, 
			Condition condition = null) {
			this.skill = skill; this.rate = rate;
			this.condition = condition;
		}

		/// <summary>
		/// 是否满足条件
		/// </summary>
		/// <returns></returns>
		public bool inCondition() {
			return condition == null || condition.Invoke();
		}
	}

	/// <summary>
	/// 战斗者
	/// </summary>
	public abstract class RuntimeBattler : RuntimeCharacter {

		/// <summary>
		/// 属性ID约定
		/// </summary>
		public const int MHPParamId = 1;
		public const int AttackParamId = 2;
		public const int DefenseParamId = 3;
		public const int SpeedParamId = 4;

		public const int MaxParamCount = 4;

		/// <summary>
		/// 状态
		/// </summary>
		public new enum State {
			Idle, // 空闲
			Moving, // 移动中
			Using, // 使用物品/技能
			Hitting, // 受击
			Freezing, // 冻结（硬直）
			Dead, // 死亡
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public virtual float hp { get; protected set; }
		[AutoConvert]
		public virtual List<RuntimeBuff> buffs { get; protected set; } = new List<RuntimeBuff>();

		#region 战斗中变量标志

		[AutoConvert]
		public float hitting { get; protected set; } = 0; // 受击时间
		[AutoConvert]
		public float freezing { get; protected set; } = 0; // 硬直时间

		#endregion

		/// <summary>
		/// 是否玩家
		/// </summary>
		/// <returns></returns>
		public virtual bool isActor() { return false; }

		/// <summary>
		/// 是否敌人
		/// </summary>
		/// <returns></returns>
		public virtual bool isEnemy() { return false; }

		/// <summary>
		/// 获取战斗者
		/// </summary>
		/// <returns></returns>
		public abstract Battler battler { get; }

		/// <summary>
		/// 回调类型
		/// </summary>
		public new enum CbType {
			BattleStart, ActionStart, ActionEnd,
			BuffAdd, BuffRemove, Hit, Die
		}

		/// <summary>
		/// 回调枚举类型
		/// </summary>
		protected override Type cbType => typeof(CbType);

		#region 初始化

		/// <summary>
		/// 初始化状态
		/// </summary>
		protected override void initializeStates() {
			base.initializeStates();

			//addStateDict(State.Idle, updateIdle);
			//addStateDict(State.Moving, updateMoving);
			addStateDict(State.Using, updateUsing);
			addStateDict(State.Hitting, updateHitting);
			addStateDict(State.Freezing, updateFreezing);
			addStateDict(State.Dead, updateDead);

			//changeState(BattlerState.Idle);
		}

		#endregion

		#region 属性控制

		#region 属性快捷定义

		/// <summary>
		/// 快捷定义
		/// </summary>
		public float mhp => Mathf.Round(param(MHPParamId));
		public float attack => param(AttackParamId);
		public float defense => param(DefenseParamId);
		public float speed => param(SpeedParamId);

		/// <summary>
		/// 移动速度
		/// </summary>
		/// <returns></returns>
		public override float moveSpeed() {
			return speed;
		}

		/// <summary>
		/// 基础最大生命值
		/// </summary>
		/// <returns></returns>
		protected virtual int baseMHP() { return battler.mhp; }

		/// <summary>
		/// 基础力量
		/// </summary>
		/// <returns></returns>
		protected virtual float baseAttack() { return battler.attack; }

		/// <summary>
		/// 基础格挡
		/// </summary>
		/// <returns></returns>
		protected virtual float baseDefense() { return battler.defense; }

		/// <summary>
		/// 基础速度
		/// </summary>
		/// <returns></returns>
		protected virtual float baseSpeed() { return battler.speed; }

		#endregion

		#region HP控制

		#region HP变化显示

		/// <summary>
		/// HP该变量
		/// </summary>
		public class DeltaHP {

			public float value = 0; // 值

			public bool critical = false; // 是否暴击
			public bool miss = false; // 是否闪避

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="value"></param>
			public DeltaHP(float value = 0, bool critical = false, bool miss = false) {
				this.value = value; this.critical = critical; this.miss = miss;
			}
		}

		/// <summary>
		/// HP变化量
		/// </summary>
		DeltaHP _deltaHP = null;
		public DeltaHP deltaHP {
			get {
				var res = _deltaHP;
				_deltaHP = null; return res;
			}
		}

		/// <summary>
		/// 设置闪避标志
		/// </summary>
		public void setMissFlag() {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.miss = true;
		}

		/// <summary>
		/// 设置暴击标志
		/// </summary>
		public void setCriticalFlag() {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.critical = true;
		}

		/// <summary>
		/// 设置值变化
		/// </summary>
		/// <param name="value"></param>
		public void setHPChange(float value) {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.value += value;
		}

		#endregion

		/// <summary>
		/// 改变HP
		/// </summary>
		/// <param name="value">目标值</param>
		/// <param name="show">是否显示</param>
		public void changeHP(float value, bool show = true) {
			var oriHp = hp;
			hp = Mathf.Clamp(value, 0, mhp);
			if (show) setHPChange(hp - oriHp);
			//if (isDead()) onDie();
		}

		/// <summary>
		/// 增加HP
		/// </summary>
		/// <param name="value">增加值</param>
		/// <param name="show">是否显示</param>
		public void addHP(float value, bool show = true) {
			changeHP(hp + value, show);
		}

		/// <summary>
		/// 增加HP
		/// </summary>
		/// <param name="rate">增加率</param>
		/// <param name="show">是否显示</param>
		public void addHPRate(double rate, bool show = true) {
			changeHP((int)Math.Round(hp + mhp * rate), show);
		}

		/// <summary>
		/// 回复所有HP
		/// </summary>
		/// <param name="show">是否显示</param>
		public void recoverAll(bool show = true) {
			changeHP(mhp, show);
		}

		/// <summary>
		/// HP率
		/// </summary>
		/// <returns></returns>
		public float hpRate() {
			return Mathf.Clamp01(hp * 1f / mhp);
		}

		/// <summary>
		/// 是否死亡
		/// </summary>
		/// <returns></returns>
		public bool isDead() {
			return hp <= 0;
		}

		#endregion

		#region 属性统一接口

		/// <summary>
		/// 基本属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public virtual float baseParam(int paramId) {
			switch (paramId) {
				case MHPParamId: return baseMHP();
				case AttackParamId: return baseAttack();
				case DefenseParamId: return baseDefense();
				case SpeedParamId: return baseSpeed();
				default: return 0;
			}
		}

		/// <summary>
		/// Buff附加值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public float buffValue(int paramId) {
			float value = 0;
			foreach (var buff in buffs)
				if (buff.paramId == paramId && 
					!buff.isOutOfDate()) value += buff.value;
			return value;
		}

		/// <summary>
		/// Buff附加率
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public double buffRate(int paramId) {
			double rate = 1;
			foreach (var buff in buffs)
				if (buff.paramId == paramId && !buff.isOutOfDate()) rate *= buff.rate;
			return rate;
		}

		/// <summary>
		/// 特性属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual float traitParamVal(int paramId) {
			return sumTraits<int>(TraitData.Code.ParamAdd, paramId);
		}

		/// <summary>
		/// 特性属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual double traitParamRate(int paramId) {
			return multTraits<int>(TraitData.Code.ParamAdd, paramId);
		}

		/// <summary>
		/// 额外属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual float extraParam(int paramId) {
			return 0;
		}

		/// <summary>
		/// 属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public float param(int paramId) {
			var base_ = baseParam(paramId) + traitParamVal(paramId) + buffValue(paramId);
			var rate = buffRate(paramId) * traitParamRate(paramId);
			var extra = extraParam(paramId);
			return (int)Math.Round((base_) * rate + extra);
		}

		/// <summary>
		/// 属性相加
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="value">增加值</param>
		public void addParam(int paramId, int value) {
			addBuff(paramId, value);
		}
		public void addParam(int paramId, int value, double rate) {
			addBuff(paramId, value, rate);
		}

		/// <summary>
		/// 属性相乘
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="rate">比率</param>
		public void multParam(int paramId, double rate) {
			addBuff(paramId, 0, rate);
		}

		/// <summary>
		/// 伤害加成
		/// </summary>
		public int damagePlusVal() {
			return sumTraits(TraitData.Code.DamagePlus);
		}
		public double damagePlusRate() {
			return multTraits(TraitData.Code.DamagePlus);
		}

		/// <summary>
		/// 受伤加成
		/// </summary>
		public int hurtPlusVal() {
			return sumTraits(TraitData.Code.HurtPlus);
		}
		public double hurtPlusRate() {
			return multTraits(TraitData.Code.HurtPlus);
		}

		/// <summary>
		/// 回复加成
		/// </summary>
		public int recoverPlusVal() {
			return sumTraits(TraitData.Code.RecoverPlus);
		}
		public double recoverPlusRate() {
			return multTraits(TraitData.Code.RecoverPlus);
		}

		#endregion

		#endregion

		#region 技能控制

		/// <summary>
		/// 可用技能
		/// </summary>
		[AutoConvert]
		public List<RuntimeSkill> runtimeSkills 
			{ get; protected set; } = new List<RuntimeSkill>();

		/// <summary>
		/// 添加技能
		/// </summary>
		/// <param name="skill"></param>
		public void addSkill(RuntimeSkill skill) {
			runtimeSkills.Add(skill);
		}

		/// <summary>
		/// 获取特定运行时技能
		/// </summary>
		/// <param name="skill"></param>
		/// <returns></returns>
		public RuntimeSkill getSkill(Skill skill) {
			return runtimeSkills.Find(s => s.skill == skill);
		}

		/// <summary>
		/// 获取可用的技能列表
		/// </summary>
		/// <returns></returns>
		public List<RuntimeSkill> usableSkills() {
			return runtimeSkills.FindAll(s => s.inCondition());
		}

		#endregion

		#region 硬直控制

		/// <summary>
		/// 设置受击
		/// </summary>
		/// <param name="val"></param>
		public void setHitting(float val) {
			hitting = val; onHit();
		}

		/// <summary>
		/// 设置硬直
		/// </summary>
		/// <param name="val"></param>
		public void setFreezing(float val) {
			freezing = Math.Max(freezing, val);
		}

		#endregion

		#region 特性控制

		/// <summary>
		/// 获取所有特性
		/// </summary>
		/// <returns></returns>
		public virtual List<TraitData> traits() {
			return new List<TraitData>();
			//return statesTraits();
		}

		///// <summary>
		///// 所有状态特性
		///// </summary>
		//List<TraitData> statesTraits() {
		//	var res = new List<TraitData>();
		//	foreach (var state in states)
		//		res.AddRange(state.Value.traits());
		//	return res;
		//}

		/// <summary>
		/// 获取特定特性
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <returns>返回符合条件的特性</returns>
		public List<TraitData> filterTraits(TraitData.Code code) {
			return traits().FindAll(trait => trait.code == (int)code);
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public List<TraitData> filterTraits<T>(TraitData.Code code, T param, int id = 0) {
			return traits().FindAll(trait => trait.code == (int)code
				&& Equals(trait.get<T>(id), param));
		}

		/// <summary>
		/// 特性值求和
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <param name="index">特性参数索引</param>
		/// <param name="base_">求和基础值</param>
		/// <returns></returns>
		public int sumTraits(TraitData.Code code, int index = 0, int base_ = 0) {
			return sumTraits(filterTraits(code), index, base_);
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public int sumTraits<T>(TraitData.Code code, T param, int id = 0, int index = 1, int base_ = 0) {
			return sumTraits(filterTraits(code, param, id), index, base_);
		}
		public int sumTraits(List<TraitData> traits, int index = 0, int base_ = 0) {
			var res = base_;
			foreach (var trait in traits)
				res += trait.get(index, 0);
			return res;
		}

		/// <summary>
		/// 特性值求积
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <param name="index">特性参数索引</param>
		/// <param name="base_">概率基础值</param>
		/// <returns></returns>
		public double multTraits(TraitData.Code code, int index = 1, int base_ = 100) {
			return multTraits(filterTraits(code), index, base_);
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public double multTraits<T>(TraitData.Code code, T param, int id = 0, int index = 2, int base_ = 100) {
			return multTraits(filterTraits(code, param, id), index, base_);
		}
		public double multTraits(List<TraitData> traits, int index = 1, int base_ = 100) {
			var res = 1.0;
			foreach (var trait in traits)
				res *= (base_ + trait.get(index, 0)) / 100.0;
			return res;
		}

		#endregion

		#region Buff控制

		/// <summary>
		/// 状态是否改变
		/// </summary>
		List<RuntimeBuff> _addedBuffs = new List<RuntimeBuff>();
		public List<RuntimeBuff> addedBuffs {
			get {
				var res = _addedBuffs;
				_addedBuffs.Clear(); return res;
			}
		}

		#region Buff变更

		/// <summary>
		/// 添加Buff
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="value">变化值</param>
		/// <param name="rate">变化率</param>
		/// <param name="turns">持续回合</param>
		/// <returns>返回添加的Buff</returns>
		public RuntimeBuff addBuff(int paramId,
			int value = 0, double rate = 1, int turns = 0) {
			return addBuff(new RuntimeBuff(paramId, value, rate, turns));
		}
		public virtual RuntimeBuff addBuff(RuntimeBuff buff) {
			buffs.Add(buff); onBuffAdd(buff);
			_addedBuffs.Add(buff);

			return buff;
		}

		/// <summary>
		/// 移除Buff
		/// </summary>
		/// <param name="index">Buff索引</param>
		public void removeBuff(int index, bool force = false) {
			var buff = buffs[index];
			buffs.RemoveAt(index);
			_addedBuffs.Remove(buff);

			onBuffRemove(buff, force);
		}
		/// <param name="buff">Buff对象</param>
		public void removeBuff(RuntimeBuff buff, bool force = false) {
			buffs.Remove(buff);
			_addedBuffs.Remove(buff);

			onBuffRemove(buff, force);
		}

		/// <summary>
		/// 移除多个满足条件的Buff
		/// </summary>
		/// <param name="p">条件</param>
		public void removeBuffs(Predicate<RuntimeBuff> p, bool force = true) {
			for (int i = buffs.Count - 1; i >= 0; --i)
				if (p(buffs[i])) removeBuff(i, force);
		}

		/// <summary>
		/// 清除所有Debuff
		/// </summary>
		public void removeDebuffs(bool force = true) {
			removeBuffs(buff => buff.isDebuff(), force);
		}

		/// <summary>
		/// 清除所有Buff
		/// </summary>
		public void clearBuffs(bool force = true) {
			for (int i = buffs.Count - 1; i >= 0; --i)
				removeBuff(i, force);
		}

		/// <summary>
		/// 是否处于指定条件的Buff
		/// </summary>
		public bool containsBuff(int paramId) {
			return buffs.Exists(buff => buff.paramId == paramId);
		}
		public bool containsBuff(Predicate<RuntimeBuff> p) {
			return buffs.Exists(p);
		}

		#endregion

		#region Buff判断

		/// <summary>
		/// 是否处于指定条件的Debuff
		/// </summary>
		public bool containsDebuff(int paramId) {
			return buffs.Exists(buff => buff.isDebuff() && buff.paramId == paramId);
		}

		/// <summary>
		/// 是否存在Debuff
		/// </summary>
		public bool anyDebuff() {
			return buffs.Exists(buff => buff.isDebuff());
		}

		/// <summary>
		/// 获取指定条件的Buff
		/// </summary>
		public RuntimeBuff getBuff(int paramId) {
			return buffs.Find(buff => buff.paramId == paramId);
		}
		public RuntimeBuff getBuff(Predicate<RuntimeBuff> p) {
			return buffs.Find(p);
		}

		/// <summary>
		/// 获取指定条件的Buff（多个）
		/// </summary>
		public List<RuntimeBuff> getBuffs(Predicate<RuntimeBuff> p) {
			return buffs.FindAll(p);
		}

		#endregion
		
		/// <summary>
		/// 更新BUFF
		/// </summary>
		void updateBuffs() {
			for (int i = buffs.Count - 1; i >= 0; --i) {
				var buff = buffs[i]; buff.update();
				if (buff.isOutOfDate()) removeBuff(i);
			}
		}

		#endregion

		#region 行动控制

		/// <summary>
		/// 行动序列
		/// </summary>
		Queue<RuntimeAction> actions = new Queue<RuntimeAction>();

		/// <summary>
		/// 当前行动
		/// </summary>
		/// <returns></returns>
		public virtual RuntimeAction currentAction() {
			if (actions.Count <= 0) return null;
			return actions.Dequeue();
		}

		/// <summary>
		/// 增加行动
		/// </summary>
		/// <param name="action">行动</param>
		public void addAction(RuntimeAction action) {
			actions.Enqueue(action);
		}
		public void addAction(Skill skill) {
			addAction(new RuntimeAction(this, skill));
		}

		/// <summary>
		/// 清除所有行动
		/// </summary>
		public void clearActions() {
			actions.Clear();
		}

		/// <summary>
		/// 能否作为目标
		/// </summary>
		/// <returns></returns>
		public bool isTargetEnable() {
			return !isDead() && !isHitting();
		}

		#endregion

		#region 结果控制

		/// <summary>
		/// 当前结果
		/// </summary>
		RuntimeActionResult currentResult = null;

		/// <summary>
		/// 应用结果
		/// </summary>
		/// <param name="result"></param>
		public void applyResult(RuntimeActionResult result) {
			currentResult = result;
			// TODO: 结果应用
			CalcService.ResultApplyCalc.apply(this, result);
		}

		/// <summary>
		/// 获取当前结果（用于显示）
		/// </summary>
		/// <returns></returns>
		public RuntimeActionResult getResult() {
			var res = currentResult;
			clearResult();
			return res;
		}

		/// <summary>
		/// 清除当前结果
		/// </summary>
		public void clearResult() {
			currentResult = null;
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 回调
		/// </summary>
		/// <param name="type">回调类型枚举</param>
		public void on(CbType type) {
			callbackManager.on(type);
		}
		/// <summary>
		/// 回调（内部调用）
		/// </summary>
		/// <param name="type">回调类型枚举</param>
		protected void _on(CbType type) {
			callbackManager.on(type, false);
		}

		/// <summary>
		/// 判断回调
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool judge(CbType type) {
			return callbackManager.judge(type);
		}

		/// <summary>
		/// 战斗开始回调
		/// </summary>
		protected virtual void onBattleStart() {
			clearBuffs();
			changeState(State.Idle);
			_on(CbType.BattleStart);
		}

		/// <summary>
		/// 行动开始回调
		/// </summary>
		public virtual void onActionStart(RuntimeAction action) {
			_addedBuffs.Clear();
			_on(CbType.ActionStart);
		}

		/// <summary>
		/// 当前行动结束回调
		/// </summary>
		public virtual void onActionEnd(RuntimeAction action) {
			Debug.Log(this + " onActionEnd");
			_on(CbType.ActionEnd);
		}

		#region BUFF回调

		/// <summary>
		/// BUFF添加回调
		/// </summary>
		protected virtual void onBuffAdd(RuntimeBuff buff) {
			_on(CbType.BuffAdd);
		}

		/// <summary>
		/// BUFF移除回调
		/// </summary>
		protected virtual void onBuffRemove(RuntimeBuff buff, bool force = false) {
			_on(CbType.BuffRemove);
		}

		#endregion

		/// <summary>
		/// 受击回调
		/// </summary>
		protected virtual void onHit() {
			changeState(State.Hitting);
			_on(CbType.Hit);
		}

		/// <summary>
		/// 死亡回调
		/// </summary>
		protected virtual void onDie() {
			_on(CbType.Die);
		}

		#endregion

		#region 状态判断

		/// <summary>
		/// 是否行动中
		/// </summary>
		/// <returns></returns>
		public bool isActing() {
			return isState(State.Using);
		}

		/// <summary>
		/// 是否受击
		/// </summary>
		/// <returns></returns>
		public bool isHitting() {
			return hitting > 0;
		}

		/// <summary>
		/// 是否硬直
		/// </summary>
		/// <returns></returns>
		public bool isFreezing() {
			return freezing > 0;
		}

		/// <summary>
		/// 是否处于受击中
		/// </summary>
		/// <returns></returns>
		public bool isHittingOrFreezing() {
			return isHitting() || isFreezing();
		}

		/// <summary>
		/// 能否移动
		/// </summary>
		/// <returns></returns>
		public override bool isMoveable() {
			return base.isMoveable() && 
				!isActing() && !isHittingOrFreezing();
		}

		/// <summary>
		/// 是否空闲
		/// </summary>
		/// <returns></returns>
		public override bool isIdle() {
			return base.isIdle() && !isActing() 
				&& !isHittingOrFreezing();
		}

		#endregion

		#region 状态更新

		/// <summary>
		/// 更新空闲状态
		/// </summary>
		protected override void updateIdle() {
			base.updateIdle();
			if (judge(CbType.ActionStart))
				changeState(State.Using);
			if (isDead()) changeState(State.Dead);
		}

		/// <summary>
		/// 更新移动状态
		/// </summary>
		protected override void updateMoving() {
			base.updateMoving();
			if (isDead()) changeState(State.Dead);
		}

		/// <summary>
		/// 更新使用状态
		/// </summary>
		protected virtual void updateUsing() {
			if (judge(CbType.ActionEnd))
				changeState(State.Idle);
			if (isDead()) changeState(State.Dead);
		}

		/// <summary>
		/// 更新受击状态
		/// </summary>
		protected virtual void updateHitting() {
			hitting = Math.Max(0, hitting - Time.deltaTime);
			if (!isHitting()) changeState(State.Freezing);
			if (isDead()) changeState(State.Dead);
		}

		/// <summary>
		/// 更新硬直状态
		/// </summary>
		protected virtual void updateFreezing() {
			freezing = Math.Max(0, freezing - Time.deltaTime);
			if (!isFreezing()) changeState(State.Idle);
			if (isDead()) changeState(State.Dead);
		}

		/// <summary>
		/// 更新死亡
		/// </summary>
		protected virtual void updateDead() {
			if (!isDead()) changeState(State.Idle);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 每帧更新
		/// </summary>
		public override void update() {
			updateBuffs();
			base.update();
		}

		/// <summary>
		/// 初次更新
		/// </summary>
		protected override void firstUpdate() {
			base.firstUpdate(); hp = mhp;
		}

		#endregion

	}

	/// <summary>
	/// 战斗玩家
	/// </summary>
	public class RuntimeActor : RuntimeBattler {

		/// <summary>
		/// 常量定义
		/// </summary>
		public const int MaxEnergy = 100;

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public virtual int energy { get; protected set; } = 0;
		public float energyRate => energy * 1.0f / MaxEnergy;

		/// <summary>
		/// 战斗者
		/// </summary>
		public override Battler battler => PlayerService.Get().actor;

		/// <summary>
		/// 是否玩家
		/// </summary>
		/// <returns></returns>
		public override bool isActor() { return true; }

		#region 能量控制

		/// <summary>
		/// 更改能力值
		/// </summary>
		/// <param name="value"></param>
		public void changeEnergy(int value) {
			var oriEnergy = energy;
			energy = Mathf.Clamp(value, 0, MaxEnergy);
			setEnergyChange(energy - oriEnergy);
		}

		/// <summary>
		/// 获得能量
		/// </summary>
		/// <param name="value"></param>
		public void addEnergy(int value) {
			changeEnergy(energy + value);
		}

		#region Energy变化显示

		/// <summary>
		/// HP该变量
		/// </summary>
		public class DeltaEnergy {

			public float value = 0; // 值

			public bool critical = false; // 是否暴击
			public bool miss = false; // 是否闪避

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="value"></param>
			public DeltaEnergy(float value = 0, bool critical = false, bool miss = false) {
				this.value = value; this.critical = critical; this.miss = miss;
			}
		}

		/// <summary>
		/// 能量变化量
		/// </summary>
		DeltaEnergy _deltaEnergy = null;
		public DeltaEnergy deltaEnergy {
			get {
				var res = _deltaEnergy;
				_deltaEnergy = null; return res;
			}
		}

		/// <summary>
		/// 设置值变化
		/// </summary>
		/// <param name="value"></param>
		public void setEnergyChange(float value) {
			_deltaEnergy = _deltaEnergy ?? new DeltaEnergy();
			_deltaEnergy.value += value;
		}

		#endregion
		
		#endregion
	}

	/// <summary>
	/// 战斗分身
	/// </summary>
	public class RuntimeSeperation : RuntimeActor {

		/// <summary>
		/// 主体
		/// </summary>
		public RuntimeActor actor => PlayerService.Get().runtimeActor;

		#region 状态同步控制

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public override float hp {
			get => actor.hp;
			protected set { actor.changeHP(value); }
		}
		[AutoConvert]
		public override int energy {
			get => actor.energy;
			protected set { actor.changeEnergy(value); }
		}
		[AutoConvert]
		public override List<RuntimeBuff> buffs => actor.buffs;

		/// <summary>
		/// 基本属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public override float baseParam(int paramId) {
			return actor.baseParam(paramId);
		}

		/// <summary>
		/// 额外属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public override float extraParam(int paramId) {
			return actor.extraParam(paramId);
		}

		/// <summary>
		/// 获取所有特性
		/// </summary>
		/// <returns></returns>
		public override List<TraitData> traits() {
			return actor.traits();
		}

		#endregion
	}

	/// <summary>
	/// 战斗敌人
	/// </summary>
	public class RuntimeEnemy : RuntimeBattler {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int enemyId { get; protected set; }

		/// <summary>
		/// 警戒状态
		/// </summary>
		public bool isCritical = false;

		/// <summary>
		/// 思考时间
		/// </summary>
		float idleTime = 0;
        

        /// <summary>
        /// 自定义敌人
        /// </summary>
        public Enemy customEnemy = null;

		/// <summary>
		/// 战斗者
		/// </summary>
		public override Battler battler => customEnemy ?? enemy();

		/// <summary>
		/// 是否玩家
		/// </summary>
		/// <returns></returns>
		public override bool isEnemy() { return true; }

		/// <summary>
		/// 获取标签实例
		/// </summary>
		/// <returns></returns>
		protected CacheAttr<Enemy> enemy_ = null;
		protected Enemy _enemy_() {
			return poolGet<Enemy>(enemyId);
		}
		public Enemy enemy() {
			return customEnemy ?? enemy_?.value();
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeEnemy() { }
		public RuntimeEnemy(int enemyId) { this.enemyId = enemyId; }
		public RuntimeEnemy(int enemyId, Enemy customEnemy) : this(enemyId) {
			this.customEnemy = customEnemy; hp = mhp;
		}

		/// <summary>
		/// 更新空闲状态
		/// </summary>
		protected override void updateIdle() {
			base.updateIdle();

            if (idleTime > 0) idleTime -= Time.deltaTime;
			if (idleTime <= 0) updateAction();
		}

		/// <summary>
		/// 更新行动思考
		/// </summary>
		protected virtual void updateAction() {
			if (isCritical) updateSkill();
		}

		/// <summary>
		/// 行动开始回调
		/// </summary>
		/// <param name="action"></param>
		public override void onActionStart(RuntimeAction action) {
			base.onActionStart(action);
			idleTime = enemy().frequency;
		}

		/// <summary>
		/// 更新技能使用
		/// </summary>
		void updateSkill() {
			var skill = randomSkill();
			if (skill == null) return;

			addAction(skill.skill);
		}

		/// <summary>
		/// 随机技能
		/// </summary>
		/// <returns></returns>
		RuntimeSkill randomSkill() {
			var sum = 0f; // 总权重值
			foreach (var p in usableSkills()) sum += p.rate;

			var rand = Random.Range(0, sum);
			foreach (var p in usableSkills())
				if ((rand -= p.rate) <= 0) return p;

			return null;
		}
	}

	/// <summary>
	/// 运行时行动
	/// </summary>
	public class RuntimeAction : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public RuntimeBattler subject { get; protected set; } // 主体
		[AutoConvert]
		public Skill skill { get; protected set; } // 技能

		/// <summary>
		/// 结果
		/// </summary>
		public RuntimeActionResult[] results { get; protected set; } = null;

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeAction() { }
		public RuntimeAction(RuntimeBattler subject, Skill skill) {
			this.subject = subject; this.skill = skill;
		}

		/// <summary>
		/// 生成结果
		/// </summary>
		/// <param name="object_"></param>
		public RuntimeActionResult makeResult(RuntimeBattler object_) {
			return new RuntimeActionResult(object_, this);
		}
	}

	/// <summary>
	/// 运行时行动结果
	/// </summary>
	public class RuntimeActionResult : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public float hpDamage { get; set; }
		[AutoConvert]
		public float hpRecover { get; set; }
		[AutoConvert]
		public float hpDrain { get; set; }

		/// <summary>
		/// 受击与硬直
		/// </summary>
		[AutoConvert]
		public float hitting { get; set; }
		[AutoConvert]
		public float freezing { get; set; }

		/// <summary>
		/// 状态/Buff变更
		/// </summary>
		[AutoConvert]
		public List<RuntimeBuff> addBuffs { get; set; } = new List<RuntimeBuff>();

		/// <summary>
		/// 行动
		/// </summary>
		public RuntimeAction action { get; protected set; }

		/// <summary>
		/// 所属目标
		/// </summary>
		public RuntimeBattler object_ { get; protected set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeActionResult() { }
		public RuntimeActionResult(RuntimeBattler object_, RuntimeAction action) {
			this.object_ = object_; this.action = action;
			CalcService.ActionResultGenerator.generate(this);
		}
	}

	#endregion

}

