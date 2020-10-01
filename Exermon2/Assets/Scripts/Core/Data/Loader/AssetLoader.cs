using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Config;

/// <summary>
/// 核心框架代码
/// </summary>
namespace Core { }

/// <summary>
/// 核心数据
/// </summary>
namespace Core.Data { }

/// <summary>
/// 数据读取器
/// </summary>
namespace Core.Data.Loaders {

	/// <summary>
	/// 资源设置
	/// </summary>
	public class AssetSetting {

		public Asset.Type name; // 资源类型名
		public string path; // 资源路径
		public string format; // 资源文件名格式

		/// <summary>
		/// 构造函数
		/// </summary>
		public AssetSetting(Asset.Type name, string path, string format) {
			this.name = name; this.path = path; this.format = format;
		}
		public AssetSetting(Asset.Type name, params string[] paths) {
			var last = paths.Length - 1;
			format = paths[last];

			paths = paths.Take(last).ToArray();
			path = Path.Combine(paths);

			this.name = name;
		}
	}

	/// <summary>
	/// 资源管理器
	/// </summary>>
	public static class AssetLoader {

		/// <summary>
		/// 预设资源路径/名称
		/// </summary>
		public const string SystemPath = "System/";

		#region 内置资源类型

		public const string AnimationName = "Animation";

		#endregion

		//      /// <summary>
		//      /// 资源路径定义
		//      /// </summary>
		//      public const string CharacterBustPath = "Character/Bust/";
		//      public const string CharacterFacePath = "Character/Face/";
		//      public const string CharacterBattlePath = "Character/Battle/";
		//      public const string ItemIconPath = "Item/";
		//      public const string ExermonFullPath = "Exermon/Full/";
		//      public const string ExermonIconPath = "Exermon/Icon/";
		//      public const string ExermonBattlePath = "Exermon/Battle/";
		//      public const string ExerGiftIconPath = "ExerGift/Icon/";
		//      public const string ExerSkillIconPath = "Exermon/Skill/Icon";
		//      public const string ExerSkillAniPath = "Exermon/Skill/Ani";
		//      public const string ExerSkillTargetPath = "Exermon/Skill/Target";

		//public const string SystemPath = "System/";

		//public const string ExerProEnemyPath = "ExerPro/Enemy/";
		//public const string ExerProItemPath = "ExerPro/Item/";
		//public const string ExerProCardPath = "ExerPro/Card/";
		//public const string ExerProStatePath = "ExerPro/State/";

		//public const string ExerProSystemPath = "ExerPro/System/";

		////public const string ExerProListeningAudioPath = "ExerPro/ListeningAudio/";

		///// <summary>
		///// 文件主体名称定义
		///// </summary>
		//public const string IconsFileName = "Icons";
		//public const string IconFileName = "Icon";

		//public const string CharacterFileName = "Character";
		//public const string ExermonFileName = "Exermon";
		//      public const string ExerGiftFileName = "BigExerGift";
		//      public const string BigExerGiftFileName = "BigExerGift";
		//      public const string ExerSkillFileName = "Skill";
		//      public const string RankIconsFileName = "RankIcons";
		//      public const string SmallRankIconsFileName = "SmallRankIcons";

		//public const string ExerProNodeIconFileName = "Node/Type";

		//public const string ExerProEnemyBattleFileName = "Battle/Enemy";
		//public const string ExerProEnemyThinkIconFileName = "Think/Icon";

		//public const string ExerProCardSkinFileName = "Skin/Skin";
		//public const string ExerProCardCharFrameFileName = "Skin/Character";
		//public const string ExerProCardTypeIconFileName = "Skin/Type";

		/// <summary>
		/// 其他常量定义
		/// </summary>
		////public const int ItemIconCols = 10; // 物品图标列数
		//public const int ItemIconSize = 96; // 物品尺寸（正方形）
		//public const int RankIconCnt = 6; // 段位数量
		//      public const int MaxSubRank = 5; // 最大子段位数目

		//public static readonly Vector2 CardIconSize = new Vector2(96, 144); // 卡牌尺寸
		//public const int StateIconSize = 32; // 状态图标尺寸（正方形）

		/// <summary>
		/// 缓存池对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static class AssetCachePool<T> where T: class {

			/// <summary>
			/// 缓存池
			/// </summary>
			static Dictionary<string, T> cache = new Dictionary<string, T>();

			/// <summary>
			/// 获取
			/// </summary>
			/// <param name="key">路径</param>
			/// <returns></returns>
			public static T get(string key) {
				if (cache.ContainsKey(key)) return cache[key];
				return null;
			}

			/// <summary>
			/// 设置
			/// </summary>
			/// <param name="key">路径</param>
			/// <param name="obj">资源</param>
			/// <returns></returns>
			public static T set(string key, T obj) {
				return cache[key] = obj;
			}

			/// <summary>
			/// 是否包含
			/// </summary>
			/// <param name="key">路径</param>
			/// <returns></returns>
			public static bool contains(string key) {
				return cache.ContainsKey(key);
			}
		}

		#region 加载资源封装

		/// <summary>
		/// 读取资源
		/// </summary>
		/// <typeparam name="T">资源类型</typeparam>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns></returns>
		public static T loadAsset<T>(string path, string fileName) where T : Object {
			var key = path + fileName;
			Debug.Log("LoadAsset<" + typeof(T) + "> from " + key);

			if (!AssetCachePool<T>.contains(key)) {
				var obj = Resources.Load<T>(key);
				return AssetCachePool<T>.set(key, obj);
			}
			return AssetCachePool<T>.get(key);
		}

		/// <summary>
		/// 读取资源（多个，组合资源）
		/// </summary>
		/// <typeparam name="T">资源类型</typeparam>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns></returns>
		public static T[] loadAssets<T>(string path, string fileName) where T : Object {
			var key = path + fileName;
			Debug.Log("LoadAssets<" + typeof(T) + "> from " + key);

			if (!AssetCachePool<T[]>.contains(key)) {
				var obj = Resources.LoadAll<T>(key);
				return AssetCachePool<T[]>.set(key, obj);
			}
			return AssetCachePool<T[]>.get(key);
		}

		/// <summary>
		/// 读取2D纹理
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>2D纹理</returns>
		public static Texture2D loadTexture2D(string path, string fileName) {
			return loadAsset<Texture2D>(path, fileName);
		}
		/// <param name="name">文件主体名称</param>
		/// <param name="id">序号</param>
		static Texture2D loadTexture2D(string path, string name, int id) {
			return loadTexture2D(path, name + "_" + id);
		}

		/// <summary>
		/// 读取预制件
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>2D纹理</returns>
		public static GameObject loadPerfab(string path, string fileName) {
			return loadAsset<GameObject>(path, fileName);
		}
		/// <param name="name">文件主体名称</param>
		/// <param name="id">序号</param>
		static GameObject loadPerfab(string path, string name, int id) {
			return loadPerfab(path, name + "_" + id);
		}

		/// <summary>
		/// 读取音频
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>音频文件</returns>
		public static AudioClip loadAudio(string path, string fileName) {
			return loadAsset<AudioClip>(path, fileName);
		}
		/// <param name="id">文件id</param>
		/// <returns>音频文件</returns>
		public static AudioClip loadAudio(string path, string name, int id) {
			return loadAudio(path, name + "_" + id);
		}

		/// <summary>
		/// 读取动画
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>音频文件</returns>
		public static AnimationClip loadAnimation(string path, string fileName) {
			return loadAsset<AnimationClip>(path, fileName);
		}
		/// <param name="id">文件id</param>
		/// <returns>音频文件</returns>
		public static AnimationClip loadAnimation(string path, string name, int id) {
			return loadAnimation(path, name + "_" + id);
		}
		public static AnimationClip loadAnimation(int id) {
			return loadAsset<AnimationClip>(
				Asset.Type.Animation, id);
		}

		#region 纹理资源处理

		/// <summary>
		/// 根据尺寸获取纹理截取区域 
		/// </summary>
		/// <param name="texture">纹理</param>
		/// <param name="xSize">单个图标宽度</param>
		/// <param name="ySize">单个图标高度</param>
		/// <param name="xIndex">图标X索引</param>
		/// <param name="yIndex">图标Y索引</param>
		/// <returns>返回要截取的区域</returns>
		public static Rect getRectBySize(Texture2D texture, 
            int xSize, int ySize, int xIndex, int yIndex) {
            int h = texture.height;
            int x = xIndex * xSize, y = h - (yIndex+1) * ySize;

            return new Rect(x, y, xSize, ySize);
        }
        /// <param name="index">图标索引</param>
        public static Rect getRectBySize(Texture2D texture, 
			int xSize, int ySize, int index) {
            int w = texture.width; int cols = w / xSize;
            int xIndex = index % cols, yIndex = index / cols;

            return getRectBySize(texture, xSize, ySize, xIndex, yIndex);
        }

		/// <summary>
		/// 根据个数获取纹理截取区域 
		/// </summary>
		/// <param name="texture">纹理</param>
		/// <param name="xCnt">X坐标图标数</param>
		/// <param name="yCnt">Y坐标图标数</param>
		/// <param name="xIndex">图标X索引</param>
		/// <param name="yIndex">图标Y索引</param>
		/// <returns></returns>
		public static Rect getRectByCnt(Texture2D texture,
			int xCnt, int yCnt, int xIndex, int yIndex) {
			int w = texture.width, h = texture.height;
			float xSize = w * 1f / xCnt, ySize = h * 1f / yCnt;
			float x = xIndex * xSize, y = h - (yIndex + 1) * ySize;

			return new Rect(x, y, xSize, ySize);
		}
		/// <param name="index">图标索引</param>
		public static Rect getRectByCnt(Texture2D texture,
			int xCnt, int yCnt, int index) {
			int xIndex = index % xCnt, yIndex = index / xCnt;

			return getRectBySize(texture, xCnt, yCnt, xIndex, yIndex);
		}

		/// <summary>
		/// 生成精灵
		/// </summary>
		/// <param name="texture">纹理</param>
		/// <param name="rect">截取矩形</param>
		/// <returns></returns>
		public static Sprite genSprite(Texture2D texture, Rect rect = default) {
			if (rect == default)
				rect = new Rect(0, 0, texture.width, texture.height);
			return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
		}
		public static Sprite genSpriteBySize(Texture2D texture,
			int xSize, int ySize, int xIndex, int yIndex) {

			var rect = getRectBySize(texture, xSize, ySize, xIndex, yIndex);
			return genSprite(texture, rect);
		}
		public static Sprite genSpriteBySize(Texture2D texture,
			int xSize, int ySize, int index) {

			var rect = getRectBySize(texture, xSize, ySize, index);
			return genSprite(texture, rect);
		}
		public static Sprite genSpriteByCnt(Texture2D texture,
			int xCnt, int yCnt, int xIndex, int yIndex) {

			var rect = getRectByCnt(texture, xCnt, yCnt, xIndex, yIndex);
			return genSprite(texture, rect);
		}
		public static Sprite genSpriteByCnt(Texture2D texture,
			int xCnt, int yCnt, int index) {

			var rect = getRectByCnt(texture, xCnt, yCnt, index);
			return genSprite(texture, rect);
		}

		#endregion

		#endregion

		#region 加载单个资源

		/// <summary>
		/// 获取资源设定
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		static AssetSetting getAssetSetting(Asset.Type name) {
			var settings = Asset.Settings;
			foreach (var setting in settings)
				if (setting.name == name) return setting;
			return null;
		}

		/// <summary>
		/// 读取资源
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public static T loadAsset<T>(Asset.Type name, int id) where T : Object {

			var setting = getAssetSetting(name);
			var fileName = string.Format(setting.format, id);
			return loadAsset<T>(setting.path, fileName);
		}

		/// <summary>
		/// 读取资源（多个，组合资源）
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public static T[] loadAssets<T>(Asset.Type name, int id) where T : Object {

			var setting = getAssetSetting(name);
			var fileName = string.Format(setting.format, id);
			return loadAssets<T>(setting.path, fileName);
		}

		#endregion

		#region 加载组合资源

		/// <summary>
		/// 获取组合资源所占的区域（图标类资源）
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <param name="path">路径</param>
		/// <param name="name">文件名</param>
		/// <param name="xSize">X尺寸</param>
		/// <param name="ySize">Y尺寸</param>
		/// <returns>返回对应图标索引的矩形区域</returns>
		public static Rect getGroupAssetsRect(int index, 
			string path, string name, int xSize, int ySize) {
			return getRectBySize(loadTexture2D(path, name), xSize, ySize, index);
		}

		/// <summary>
		/// 读取组合资源精灵（图标类资源）
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <param name="path">路径</param>
		/// <param name="name">文件名</param>
		/// <param name="xSize">X尺寸</param>
		/// <param name="ySize">Y尺寸</param>
		/// <returns>返回对应图标索引的精灵</returns>
		public static Sprite getGroupAssetsSprite(int index,
			string path, string name, int xSize, int ySize) {
			var texture = loadTexture2D(path, name);
			var rect = getRectBySize(texture, xSize, ySize, index);
			return genSprite(texture, rect);
		}

		///// <summary>
		///// 获取物品图标精灵
		///// </summary>
		///// <param name="index">图标索引</param>
		///// <returns>返回对应图标索引的精灵</returns>
		//public static Sprite getItemIconSprite(int index) {
		//	return getGroupAssetsSprite(index,
		//		ItemIconPath, IconsFileName, 
		//		ItemIconSize, ItemIconSize);
		//}

		///// <summary>
		///// 获取特训物品图标精灵
		///// </summary>
		///// <param name="index">图标索引</param>
		///// <returns>返回对应图标索引的精灵</returns>
		//public static Sprite getExerProItemIconSprite(int index) {
		//	return getGroupAssetsSprite(index,
		//		ExerProItemPath, IconsFileName, 
		//		ItemIconSize, ItemIconSize);
		//}

		///// <summary>
		///// 获取特训物品图标精灵
		///// </summary>
		///// <param name="index">图标索引</param>
		///// <returns>返回对应图标索引的精灵</returns>
		//public static Sprite getExerProStateIconSprite(int index) {
		//	return getGroupAssetsSprite(index,
		//		ExerProStatePath, IconsFileName, 
		//		StateIconSize, StateIconSize);
		//}

		///// <summary>
		///// 读取段位图标
		///// </summary>
		///// <param name="id">物品ID</param>
		///// <returns>返回段位图标集纹理</returns>
		//public static Texture2D loadRankIcons(bool small = false) {
		//          return loadTexture2D(SystemPath, small ? 
		//              SmallRankIconsFileName : RankIconsFileName);
		//      }

		//      /// <summary>
		//      /// 获取段位图标所占的区域 
		//      /// </summary>
		//      /// <param name="id">段位ID</param>
		//      /// <param name="subRank">子段位编号</param>
		//      /// <param name="small">是否加载小图标</param>
		//      /// <returns>返回对应段位的矩形区域</returns>
		//      public static Rect getRankIconRect(int id, int subRank = 0, bool small = false) {
		//          var texture = loadRankIcons(small);
		//          int w = texture.width, h = texture.height;
		//          int xSize, ySize;
		//          if (small) {
		//              xSize = w / MaxSubRank; ySize = h / RankIconCnt;
		//              return calcRect(texture, xSize, ySize, id - 1, subRank);
		//          } else {
		//              xSize = w / RankIconCnt; ySize = h;
		//              return calcRect(texture, xSize, ySize, id - 1);
		//          }
		//      }

		//      /// <summary>
		//      /// 获取段位图标精灵
		//      /// </summary>
		//      /// <param name="id">段位ID</param>
		//      /// <param name="subRank">子段位编号</param>
		//      /// <param name="small">是否加载小图标</param>
		//      /// <returns>返回对应段位的精灵</returns>
		//      public static Sprite getRankIconSprite(int id, int subRank = 0, bool small = false) {
		//          var texture = loadRankIcons(small);
		//          var rect = getRankIconRect(id, subRank, small);
		//          return generateSprite(texture, rect);
		//      }

		#endregion

		#region 自定义读取函数

		#endregion

	}
}