
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
    public abstract class Singleton<T> where T : class, new()
    {
        private static T instance;

        private static object @lock = new object();

        public static T Instance
        {
            get
            {

                lock (@lock)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }

                    return instance;
                }
            }
        }
    }
}
