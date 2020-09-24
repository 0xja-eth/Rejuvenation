using System;
using System.Reflection;

namespace Core.Utils {

	/// <summary>
	/// 反射工具类
	/// </summary>
	public static class ReflectionUtils {

		/// <summary>
		/// 默认绑定标志
		/// </summary>
		public static readonly BindingFlags DefaultFlag =
			BindingFlags.Instance | BindingFlags.Public | 
			BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		/// <summary>
		/// 快速处理成员
		/// </summary>
		/// <typeparam name="M">MemberInfo类型</typeparam>
		/// <param name="type">所属类</param>
		/// <param name="processFunc">处理函数</param>
		public static void processMember<M>(Type type, 
			Action<M> processFunc, int processCnt = 0) where M : MemberInfo {

			var memberInfos = getMemberInfos<M>(type);
			if (memberInfos == null) return;

			var cnt = 0;
			foreach (var m in memberInfos) {
				if (processCnt > 0 && cnt++ >= processCnt) return;

				var member = m as M;
				if (member == null) continue;

				processFunc(member);
			}
		}
		/// <typeparam name="T">成员类型</typeparam>
		public static void processMember<M, T>(Type type,
			Action<M> processFunc, int processCnt = 0) where M : MemberInfo {

			var tType = typeof(T); var cnt = 0;
			processMember<M>(type, m => {
				if (processCnt > 0 && cnt++ >= processCnt) return;

				var mType = getMemberType<M>(m);
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc(m);
			});
		}
		/// <typeparam name="T">成员类型</typeparam>
		public static void processMember<M>(Type type, Type tType,
			Action<M> processFunc, int processCnt = 0) where M : MemberInfo {

			var cnt = 0;
			processMember<M>(type, m => {
				if (processCnt > 0 && cnt++ >= processCnt) return;

				var mType = getMemberType<M>(m);
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc(m);
			});
		}

		/// <summary>
		/// 快速处理特性
		/// </summary>
		/// <typeparam name="M">MemberInfo类型</typeparam>
		/// <typeparam name="A">特性类型</typeparam>
		/// <param name="type">所属类</param>
		/// <param name="processFunc">处理函数</param>
		public static void processAttribute<M, A>(Type type, Action<M, A> processFunc)
			where M : MemberInfo where A : Attribute {

			processMember<M>(type, m => {
				foreach (Attribute a in m.GetCustomAttributes(false)) {
					var attr = a as A;
					if (attr == null) continue;

					processFunc(m, attr);
				}
			});
		}

		/// <summary>
		/// 处理类特性
		/// </summary>
		/// <typeparam name="A">特性类型</typeparam>
		/// <param name="type">类型</param>
		/// <param name="processFunc">处理函数</param>
		public static void processClassAttribute<A>(Type type, 
			Action<A> processFunc) where A : Attribute {
			var member = type as MemberInfo;
			if (member == null) return;

			foreach (Attribute a in member.GetCustomAttributes(true)) {
				var attr = a as A;
				if (attr == null) continue;

				processFunc(attr);
			}
		}

		/// <summary>
		/// 处理字段
		/// </summary>
		/// <typeparam name="T">字段类型</typeparam>
		/// <param name="self">实例</param>
		/// <param name="processFunc">处理函数</param>
		public static void processField<T>(object self, Action<T> processFunc) {
			var tType = typeof(T);
			var sType = self.GetType();
			processMember<FieldInfo>(sType, m => {
				var mType = m.FieldType;
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc((T)m.GetValue(self));
			});
		}
		public static void processField(object self, 
			Type tType, Action<object> processFunc) {
			var sType = self.GetType();

			processMember<FieldInfo>(sType, m => {
				var mType = m.FieldType;
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc(m.GetValue(self));
			});
		}

		/// <summary>
		/// 处理属性
		/// </summary>
		/// <typeparam name="T">属性类型</typeparam>
		/// <param name="self">实例</param>
		/// <param name="processFunc">处理函数</param>
		public static void processProperty<T>(object self, Action<T> processFunc) {
			var tType = typeof(T);
			var sType = self.GetType();
			processMember<PropertyInfo>(sType, m => {
				var mType = m.PropertyType;
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc((T)m.GetValue(self));
			});
		}
		public static void processProperty(object self, 
			Type tType, Action<object> processFunc) {
			var sType = self.GetType();

			processMember<PropertyInfo>(sType, m => {
				var mType = m.PropertyType;
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc(m.GetValue(self));
			});
		}

		/// <summary>
		/// 获取成员信息数组
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static MemberInfo[] getMemberInfos(Type type, MemberTypes memberType) {

			switch (memberType) {
				case MemberTypes.All:
					return type.GetMembers(DefaultFlag);
				case MemberTypes.Field:
					return type.GetFields(DefaultFlag);
				case MemberTypes.Property:
					return type.GetProperties(DefaultFlag);
				case MemberTypes.Event:
					return type.GetEvents(DefaultFlag);
				case MemberTypes.Method:
					return type.GetMethods(DefaultFlag);
				default: return null;
			}
		}
		public static MemberInfo[] getMemberInfos<M>(Type type) where M : MemberInfo {
			var memberType = typeof(M);

			if (memberType == typeof(MemberInfo))
				return getMemberInfos(type, MemberTypes.All);
			if (memberType == typeof(FieldInfo))
				return getMemberInfos(type, MemberTypes.Field);
			if (memberType == typeof(PropertyInfo))
				return getMemberInfos(type, MemberTypes.Property);
			if (memberType == typeof(EventInfo))
				return getMemberInfos(type, MemberTypes.Event);
			if (memberType == typeof(MethodInfo))
				return getMemberInfos(type, MemberTypes.Method);

			return null;
		}
		public static Type getMemberType<M>(MemberInfo info) where M : MemberInfo {
			var memberType = typeof(M);

			if (memberType == typeof(FieldInfo))
				return (info as FieldInfo).FieldType;
			if (memberType == typeof(PropertyInfo))
				return (info as PropertyInfo).PropertyType;
			if (memberType == typeof(EventInfo))
				return (info as EventInfo).EventHandlerType;

			return null;
		}

	}
}
