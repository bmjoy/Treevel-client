namespace Treevel.Common.Patterns.Singleton
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
        private static T _instance;

        private static readonly object _lock = new object();

        public static T Instance {
            get {
                lock (_lock) {
                    return _instance ?? (_instance = new T());
                }
            }
        }
    }
}
