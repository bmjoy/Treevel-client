using UnityEngine;

namespace Project.Scripts.Utils
{
    /// <summary>
    /// Monobehaviourを継承したSingletonクラス
    /// </summary>
    /// <see c-ref="http://wiki.unity3d.com/index.php/Singleton"/>
    /// <typeparam name="T">継承するクラス名</typeparam>
    public abstract class SingletonObject<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        private static object @lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                // instance を作成途中に他のスレッドも作っちゃうとSingletonにならないのでロックする
                lock (@lock)
                {
                    // instance がない場合、シーンで探すか、新しく作成。
                    // instance すでに値を持ってる場合そのまま返す。
                    if (instance == null)
                    {
                        instance = (T)FindObjectOfType(typeof(T));

                        // クラスTを持つオブジェクトが二つ以上あったらおかしいのでエラーを出す
                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopenning the scene might fix it.");
                            return instance;
                        }

                        // シーンに存在しない場合新しくオブジェクトを作成し、アタッチする。
                        if (instance == null)
                        {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<T>();
                            singleton.name = "_" + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);

                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                instance.gameObject.name);
                        }
                    }

                    return instance;
                }
            }
        }

        static bool applicationIsQuitting = false;

        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}
