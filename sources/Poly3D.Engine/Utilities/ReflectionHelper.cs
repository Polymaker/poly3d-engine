using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Poly3D.Utilities
{
    internal static class ReflectionHelper
    {
        private static Dictionary<Type, List<MemberInfo>> Accessors;

        static ReflectionHelper()
        {
            Accessors = new Dictionary<Type, List<MemberInfo>>();

        }

        #region Fields

        public static object GetField(object target, string fieldName, BindingFlags bFlags)
        {
            if (target == null)
                throw new ArgumentNullException("object");
            return GetField(target.GetType(), target, fieldName, bFlags);
        }

        public static object GetField(Type type, object target, string fieldName, BindingFlags bFlags)
        {
            var fi = GetFieldInfo(type, fieldName, bFlags);
            if (fi == null)
                throw new MissingMemberException(fieldName);
            return fi.GetValue(target);
        }

        public static T GetField<T>(Type type, object target, string fieldName, BindingFlags bFlags)
        {
            return (T)GetField(type, target, fieldName, bFlags);
        }

        public static void SetField(Type type, string fieldName, BindingFlags bFlags, object target, object value)
        {
            var fi = GetFieldInfo(type, fieldName, bFlags);
            if (fi == null)
                throw new MissingMemberException(fieldName);
            fi.SetValue(target, value);
        }

        private static FieldInfo GetFieldInfo(Type type, string fieldName, BindingFlags bFlags)
        {
            return GetMember(type, fieldName, MemberTypes.Field, bFlags) as FieldInfo;
        }

        #endregion

        #region Properties

        public static object GetProperty(Type type, object target, string propName, BindingFlags bFlags)
        {
            var pi = GetPropertyInfo(type, propName, bFlags);
            if (pi == null)
                throw new MissingMemberException(propName);
            return pi.GetValue(target, null);
        }

        public static T GetProperty<T>(Type type, object target, string propName, BindingFlags bFlags)
        {
            return (T)GetProperty(type, target, propName, bFlags);
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propName, BindingFlags bFlags)
        {
            return GetMember(type, propName, MemberTypes.Property, bFlags) as PropertyInfo;
        }

        #endregion

        private static MemberInfo GetMember(Type owningType, string memberName, MemberTypes memberType, BindingFlags bFlags)
        {
            if (Accessors.ContainsKey(owningType))
            {
                if (Accessors[owningType].Any(m => m.Name == memberName && m.MemberType == memberType))
                    return Accessors[owningType].First(m => m.Name == memberName && m.MemberType == memberType);
            }
            else
                Accessors.Add(owningType, new List<MemberInfo>());

            var members = owningType.GetMembers(bFlags);
            if (members.Any(m => m.MemberType == memberType && m.Name == memberName))
            {
                var result = members.First(m => m.MemberType == memberType && m.Name == memberName);
                Accessors[owningType].Add(result);
                return result;
            }
            return null;
        }

        public static bool InheritsFrom(this Type type, Type otherType)
        {
            return type.IsAssignableFrom(otherType) || otherType.IsSubclassOf(type);
        }
    }
}
