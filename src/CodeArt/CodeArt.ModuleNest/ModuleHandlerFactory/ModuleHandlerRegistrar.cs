using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.ModuleNest
{
    /// <summary>
    /// ע��ִ�Ա
    /// </summary>
    internal static class ModuleHandlerRegistrar
    {

        #region ��ȡ�ִ���ʵ��

        public static IModuleHandler<Q, S> GetHandler<Q, S>(string handlerKey)
            where Q : class
            where S : class
        {
            object handler = null;
            if (_singletons.TryGetValue(handlerKey, out handler)) return (IModuleHandler<Q, S>)handler;
            return null;
        }

        #endregion

        private static Dictionary<string, object> _singletons = new Dictionary<string, object>();

        private static object _syncObject = new object();

        /// <summary>
        /// ע�ᵥ���ִ�����ȷ��<paramref name="repository"/>�ǵ�����
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="repository"></param>
        public static void RegisterHandler<Q, S>(string handlerKey, IModuleHandler<Q, S> handler)
            where Q : class
            where S : class
        {
            if (_singletons.ContainsKey(handlerKey)) throw new ModuleException("�ظ�ע��ģ�鴦����" + handlerKey);
            lock (_syncObject)
            {
                if (_singletons.ContainsKey(handlerKey)) throw new ModuleException("�ظ�ע��ģ�鴦����" + handlerKey);
                SafeAccessAttribute.CheckUp(handler.GetType());
                _singletons.Add(handlerKey, handler);
            }
        }

        public static void RegisterHandler(string handlerKey, IModuleHandler<DTObject, DTObject> handler)
        {
            RegisterHandler<DTObject, DTObject>(handlerKey, handler);
        }
    }
}