
namespace Project.Scripts.Utils
{
    /// <summary>
    /// Singleton Class
    /// Use <code>var.Instance</code> to get the instance.
    /// 
    /// usage: 
    /// <code>
    /// public class MySingleton : Singleton&lt;MySingleton&gt;
    /// {
    ///     public MySingleton() {}
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="T">シングルトンにしたいクラス</typeparam>
    public class Singleton<T> where T : class, new()
    {
        static T _instance;

        static object _lock = new object();

        public static T Instance
        {
            get
            {

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }

                    return _instance;
                }
            }
        }
    }
}
