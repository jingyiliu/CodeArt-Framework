using System;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.Log;
using CodeArt.DTO;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// Ӧ�ó���Ự��ָ������Ӧ�ó���ִ���ڼ䣬��ͬ���û���ӵ���Լ���appSession���ö�����Ե�ǰ�û�����
    /// �����в�����ͻ���ö����ڲ��������ǵ�ǰ�û������
    /// </summary>
    [AppSessionAccess]
    public static class AppSession
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Using(Action action,bool useSymbiosis)
        {
            try
            {
                Initialize();
                if (useSymbiosis) Symbiosis.Open();
                action();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (useSymbiosis) Symbiosis.Close();
                Dispose();
            }
        }

        public static void AsyncUsing(Action action, bool useSymbiosis)
        {
            Task.Run(() =>
            {
                try
                {
                    Initialize();
                    if (useSymbiosis) Symbiosis.Open();
                    action();
                }
                catch (Exception ex)
                {
                    LogWrapper.Default.Fatal(ex);
                }
                finally
                {
                    if (useSymbiosis) Symbiosis.Close();
                    Dispose();
                }
            });
        }


        /// <summary>
        /// ��ʼ���ػ�
        /// </summary>
        public static void Initialize()
        {
            Current.Initialize();
        }


        /// <summary>
        /// ����ǰ�ػ�����
        /// </summary>
        public static void Dispose()
        {
            Current.Dispose();
        }

        public static T GetOrAddItem<T>(string name, Func<T> factory)
        {
            var appSession = Current;
            object item = appSession.GetItem(name);
            if (item == null)
            {
                item = factory();
                appSession.SetItem(name, item);
            }
            return (T)item;
        }

        public static void SetItem<T>(string name, T value)
        {
            Current.SetItem(name, value);
        }

        public static object GetItem(string name)
        {
            return Current.GetItem(name);
        }

        public static T GetItem<T>(string name)
        {
            return (T)GetItem(name);
        }

        public static bool ContainsItem(string name)
        {
            return Current.ContainsItem(name);
        }

        private static IAppSession _current;

        private static IAppSession Current
        {
            get
            {
                if (_current == null)
                {
                    _current = _sessionByConfig ?? _sessionByRegister ?? ThreadSession.Instance;
                }
                return _current;
            }
        }


        /// <summary>
        /// �Ƿ���ڻػ�
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
        {
            return Current != null && Current.Initialized;
        }


        private static IAppSession _sessionByConfig;

        static AppSession()
        {
            var imp = Configuration.Current.AppSetting.AppSessionImplementer;
            if (imp != null)
            {
                var appSession = imp.GetInstance<IAppSession>();
                AppSessionAccessAttribute.CheckUp(appSession);
                _sessionByConfig = appSession;
            }
        }



        private static IAppSession _sessionByRegister;

        /// <summary>
        /// ע��һ��Ӧ�ó���Ự�����뱣֤<paramref name="appSession"/>���̰߳�ȫ��
        /// </summary>
        /// <param name="appSession"></param>
        public static void Register(IAppSession appSession)
        {
            AppSessionAccessAttribute.CheckUp(appSession);
            _sessionByRegister = appSession;
        }

        #region �Ự�������ݺ�����

        /// <summary>
        /// ��ǰӦ�ó���Ựʹ�õ����
        /// </summary>
        public static DTObject Identity
        {
            get
            {
                return AppSession.GetItem("SessionIdentity") as DTObject;
            }
            set
            {
                AppSession.SetItem("SessionIdentity", value);
                CodeArt.Language.Init(); //��ʼ������ѡ���Ϊ��ݿ��ܺ���������Ϣ��������Ҫ��ʼ��
                if (IdentityChanged != null)
                    IdentityChanged(value);
            }
        }

        /// <summary>
        /// ���Ự����ݷ����ı�ʱ����
        /// </summary>
        public static event Action<DTObject> IdentityChanged;


        /// <summary>
        /// ��ǰӦ�ó���Ựʹ�õ�����
        /// </summary>
        public static string Language
        {
            get
            {
                return Identity == null ? string.Empty : Identity.GetValue<string>("name", string.Empty);
            }
        }


        #endregion
    }
}
