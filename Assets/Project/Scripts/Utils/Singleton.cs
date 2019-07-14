
namespace Project.Scripts.Utils
{
    /// <summary>
    /// Singleton Class
    /// Use <c>var.Instance</c> to get the insatance.
    /// </summary>
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
