using UnityEngine;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
	public class MyPlayerPrefs : MonoBehaviour
	{
		// 指定されたオブジェクトの情報を保存
		public static void SetObject<T>(string key, T obj)
		{
			var json = JsonUtility.ToJson(obj);
			PlayerPrefs.SetString(key, json);
			PlayerPrefs.Save();
		}

		// 指定されたオブジェクトの情報を読み込み
		public static T GetObject<T>(string key) where T : new()
		{
			var json = PlayerPrefs.GetString(key);
			if (json == "") return new T();
			var obj = JsonUtility.FromJson<T>(json);
			return obj;
		}
	}
}
