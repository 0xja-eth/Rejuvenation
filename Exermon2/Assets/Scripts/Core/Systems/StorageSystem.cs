﻿
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using GameModule.Services;

namespace Core.Systems {

    /// <summary>
    /// 存储管理器
    /// </summary>
    /// <remarks>
    /// 负责对文件访问API的封装，数据加密以及用于处理游戏中缓存的储存
    /// 名词定义：
    ///     缓存：缓存的题目数据
    ///     缓存文件：储存缓存的文件
    /// </remarks>
    public class StorageSystem : BaseSystem<StorageSystem> {

        /// <summary>
        /// 储存项
        /// </summary>
        /// <remarks>
        /// 缓存项为一项需要缓存的数据，由一个 BaseData（要缓存的数据）和一个字符串（储存到的文件名）组成
        /// 要注意要缓存的数据必须要事先初始化（保证传输的是指针）
        /// 加入缓存项中的对象会在游戏开始时自动尝试加载，并在退出游戏时自动保存
        /// </remarks>
        public class StorageItem {

            /// <summary>
            /// 属性
            /// </summary>
            public BaseData data;
            public string filename;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="filename">储存文件名</param>
            public StorageItem(BaseData data, string filename) {
                this.data = data; this.filename = filename;
            }

            /// <summary>
            /// 转化为JSON
            /// </summary>
            /// <returns>JSON数据</returns>
            public JsonData toJson() { return data.toJson(); }

            /// <summary>
            /// 从JSON中读取
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="data">读取的数据</param>
            public void load(JsonData json) {
                data = DataLoader.load(data, json);
            }

        }

        /// <summary>
        /// 是否需要加密
        /// </summary>
        const bool NeedEncode = true;

        /// <summary>
        /// 路径常量定义
        /// </summary>
        public static readonly string SaveRootPath = Application.persistentDataPath + "/";

        /// <summary>
        /// 缓存文件路径
        /// </summary>
        public const string StaticDataFilename = ".static";
        public const string ConfigDataFilename = ".config";

        /// <summary>
        /// 加密盐
        /// </summary>
        const string DefaultSalt = "aZrY5R0cDc97oCEv3vdDcMz34gwPf9hL8wL3TaAE2Lm1DaxpZAlcgMMALa1EMA";
        const string LastSalt = "R0cDovMzgwLTE2Lm1pZAl";

        /// <summary>
        /// 储存项
        /// </summary>
        List<StorageItem> storageItems;

        /// <summary>
        /// 外部系统
        /// </summary>
        DataService dataSer;
        GameSystem gameSys;
        GameService gameSer;

        #region 初始化

        ///// <summary>
        ///// 初始化外部系统
        ///// </summary>
        //protected override void initializeSystems() {
        //    base.initializeSystems();
        //    gameSys = GameSystem.get();
        //    dataSer = DataService.get();
        //    gameSer = GameService.get();
        //}

        /// <summary>
        /// 初始化其他
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            storageItems = new List<StorageItem>();
            storageItems.Add(new StorageItem(dataSer.staticData, StaticDataFilename));
            storageItems.Add(new StorageItem(gameSer.configure, ConfigDataFilename));
        }

		#endregion

		#region 文件操作

		/// <summary>
		/// 储存数据到文件（JSON数据）
		/// </summary>
		/// <param name="json">JSON数据</param>
		/// <param name="filePath">文件路径和文件名</param>
		public static void saveObjectIntoFile(BaseData obj, string filePath) {
			saveJsonIntoFile(obj.toJson(), filePath);
		}
		/// <param name="path">文件路径</param>
		/// <param name="name">文件名</param>
		public static void saveObjectIntoFile(BaseData obj, string path, string name) {
			Debug.Log("Saving " + obj + " into " + path + name);
			saveJsonIntoFile(obj.toJson(), path, name);
		}

		/// <summary>
		/// 储存数据到文件（JSON数据）
		/// </summary>
		/// <param name="json">JSON数据</param>
		/// <param name="filePath">文件路径和文件名</param>
		public static void saveJsonIntoFile(JsonData json, string filePath) {
			saveDataIntoFile(json.ToJson(), filePath);
		}
		/// <param name="path">文件路径</param>
		/// <param name="name">文件名</param>
		public static void saveJsonIntoFile(JsonData json, string path, string name) {
			saveDataIntoFile(json.ToJson(), path, name);
		}

		/// <summary>
		/// 存储数据到指定文件
		/// </summary>
		/// <param name="data">数据（任意字符串）</param>
		/// <param name="filePath">文件路径和文件名</param>
		public static void saveDataIntoFile(string data, string filePath) {
			var index = filePath.LastIndexOf('/') + 1;
			var path = filePath.Substring(0, index);
			var name = filePath.Substring(index);

			saveDataIntoFile(data, path, name);
		}
		/// <param name="path">文件路径</param>
		/// <param name="name">文件名</param>
		public static void saveDataIntoFile(string data, string path, string name) {
			path = SaveRootPath + path; var filePath = path + name;
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			StreamWriter streamWriter = new StreamWriter(filePath, false);
			Debug.Log("saveToFile: " + filePath + ": " + data);
			streamWriter.Write(data);
			streamWriter.Close();
			streamWriter.Dispose();
		}

		/// <summary>
		/// 从指定文件读取数据
		/// </summary>
		/// <param name="path">文件路径</param>
		/// <param name="name">文件名</param>
		/// <returns>读取的数据（字符串）</returns>
		public static void loadObjectFromFile<T>(
			ref T data, string path, string name) where T : BaseData {
			loadObjectFromFile(ref data, path + name);
		}
		/// <param name="filePath">文件路径（包括文件名）</param>
		public static void loadObjectFromFile<T>(
			ref T data, string filePath) where T : BaseData {
			var json = loadJsonFromFile(filePath);
			data = DataLoader.load(data, json);
		}

		/// <summary>
		/// 从指定文件读取数据
		/// </summary>
		/// <param name="path">文件路径</param>
		/// <param name="name">文件名</param>
		/// <returns>读取的数据（字符串）</returns>
		public static JsonData loadJsonFromFile(string path, string name) {
			return loadJsonFromFile(path + name);
		}
		/// <param name="filePath">文件路径（包括文件名）</param>
		public static JsonData loadJsonFromFile(string filePath) {
			var data = loadDataFromFile(filePath);
			return JsonMapper.ToObject(data);
		}

		/// <summary>
		/// 从指定文件读取数据
		/// </summary>
		/// <param name="path">文件路径</param>
		/// <param name="name">文件名</param>
		/// <returns>读取的数据（字符串）</returns>
		public static string loadDataFromFile(string path, string name) {
			return loadDataFromFile(path + name);
		}
		/// <param name="filePath">文件路径（包括文件名）</param>
		public static string loadDataFromFile(string filePath) {
			filePath = SaveRootPath + filePath;
			if (!File.Exists(filePath)) return "";
			StreamReader streamReader = new StreamReader(filePath);
			string data = streamReader.ReadToEnd();
			Debug.Log("loadFromFile: " + filePath + ": " + data);
			streamReader.Close();
			streamReader.Dispose();
			return data;
		}

		/// <summary>
		/// 是否存在指定文件
		/// </summary>
		/// <param name="path">文件路径</param>
		/// <param name="name">文件名</param>
		public static bool hasFile(string path, string name) {
			return hasFile(path + name);
		}
		/// <param name="filePath">文件路径（包括文件名）</param>
		public static bool hasFile(string filePath) {
			filePath = SaveRootPath + filePath;
			return File.Exists(filePath);
		}

		/// <summary>
		/// 删除指定文件
		/// </summary>
		/// <param name="path">文件路径</param>
		/// <param name="name">文件名</param>
		public static void deleteFile(string path, string name) {
			deleteFile(path + name);
		}
		/// <param name="filePath">文件路径（包括文件名）</param>
		public static void deleteFile(string filePath) {
			filePath = SaveRootPath + filePath;
			if (File.Exists(filePath)) File.Delete(filePath);
		}

		#endregion

		#region 储存项

		/// <summary>
		/// 储存储存项
		/// </summary>
		public void save() {
            foreach (var item in storageItems)
                saveObjectIntoFile(item.data, SaveRootPath, item.filename);
        }

        /// <summary>
        /// 储存储存项
        /// </summary>
        public void saveItem(string filename) {
            foreach (var item in storageItems)
                if (item.filename == filename)
                    saveObjectIntoFile(item.data, SaveRootPath, item.filename);
        }

        /// <summary>
        /// 读取储存项
        /// </summary>
        public void load() {
            foreach (var item in storageItems)
                loadObjectFromFile(ref item.data, SaveRootPath + item.filename);
        }

        #endregion

        #region 编码解码

        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="ori">源字符串</param>
        /// <param name="salt">盐</param>
        /// <returns>编码后字符串</returns>
        public string base64Encode(string ori, string salt = DefaultSalt) {
            byte[] bytes = Encoding.UTF8.GetBytes(ori);
            string code = Convert.ToBase64String(bytes, 0, bytes.Length);
            float pos = UnityEngine.Random.Range(0.1f, 0.9f);
            code = code.Insert((int)(code.Length * pos), salt);
            code = randString(DefaultSalt.Length) + code;
            return code;
        }

        /// <summary>
        /// base64解码
        /// </summary>
        /// <param name="code">编码后字符串</param>
        /// <param name="salt">盐</param>
        /// <returns>源字符串</returns>
        public string base64Decode(string code, string salt = DefaultSalt,
            string lastSalt = LastSalt) {
            // 如果存在上一个盐，用上一个盐来解码
            if (lastSalt != "" && code.Contains(LastSalt))
                return base64Decode(code, lastSalt, "");

            code = code.Substring(salt.Length);
            code = code.Replace(salt, "");
            byte[] bytes = Convert.FromBase64String(code);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 随机字符串生成
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns>随机字符串</returns>
        string randString(int len) {
            string s = "";
            for (int i = 0; i < len; i++) {
                char c = (char)UnityEngine.Random.Range('A', 'Z');
                s += (UnityEngine.Random.Range(0, 2) >= 1) ? Char.ToLower(c) : c;
            }
            return s;
        }

        #endregion
    }

}
