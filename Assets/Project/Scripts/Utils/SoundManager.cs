using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.PlayerPrefsUtils;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Project.Scripts.Utils
{
    public enum EBGMKey {
        BGM_Gameplay
    }

    public enum ESEKey {
        SE_Success,
        SE_Failure,
        SE_ThunderAttack,
    }

    public class SoundManager : SingletonObject<SoundManager>
    {
        /// <summary>
        /// 同時再生できるSEの数
        /// </summary>
        private const int _MAX_SE_NUM = 8;

        /// <summary>
        /// 複数SE同時再生を考慮し、SE用のAudioSourceを複数用意する
        /// </summary>
        private AudioSource[] _sePlayers;

        /// <summary>
        /// BGM再生用のAudioSource
        /// </summary>
        private AudioSource _bgmPlayer;

        /// <summary>
        /// BGMリスト
        /// </summary>
        [SerializeField] private List<SoundData> _bgmClipList;

        /// <summary>
        /// SEリスト
        /// </summary>
        [SerializeField] private List<SoundData> _seClipList;

        /// <summary>
        /// 初期BGMボリューム
        /// </summary>
        [SerializeField] private float _INITIAL_BGM_VOLUME = 0.25f;

        /// <summary>
        /// 初期SEボリューム
        /// </summary>
        [SerializeField] private float _INITIAL_SE_VOLUME = 1.0f;

        private void Awake()
        {
            // BGM再生用のAudioSourceをアタッチする
            _bgmPlayer = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            _bgmPlayer.loop = true;

            // SE再生用のAudioSourceをアタッチする
            _sePlayers = new AudioSource[_MAX_SE_NUM];
            for (var i = 0; i < _MAX_SE_NUM; i++) {
                var player = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                player.loop = false;

                _sePlayers[i] = player;
            }

            ResetVolume();
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="key"> 再生するSEのキー </param>
        public void PlaySE(ESEKey key)
        {
            var clip = GetSEClip(key);
            if (clip == null)
                return;

            if (_sePlayers.Any(src => !src.isPlaying)) {
                // 再生していないAudioSourceを探す
                var player = _sePlayers.First(source => !source.isPlaying);

                player.PlayOneShot(clip);
            } else {
                Debug.LogWarning($"Failed to Play SE: {key} because there is no available audio source");
            }
        }

        /// <summary>
        /// SEを停止する
        /// </summary>
        /// <param name="key"> 停止するSEのキー </param>
        public void StopSE(ESEKey key)
        {
            var clip = GetSEClip(key);
            if (clip == null)
                return;

            var player = _sePlayers.Where(src => src.clip != null).SingleOrDefault(src => src.clip.name == clip.name);
            player?.Stop();
        }

        /// <summary>
        /// SE再生中かどうか
        /// </summary>
        /// <param name="key"> 確認したいSEのキー </param>
        /// <returns>再生中であれば`true`、そうでなければ`false`</returns>
        public bool IsPlayingSE(ESEKey key)
        {
            var clip = GetSEClip(key);
            if (clip == null)
                return false;

            var player = _sePlayers.Where(src => src.clip != null).SingleOrDefault(src => src.clip.name == clip.name);
            return player != null && player.isPlaying;
        }

        /// <summary>
        /// BGMを再生する
        /// </summary>
        /// <param name="key"> 再生するBGMのキー </param>
        /// <param name="playback"></param>
        public void PlayBGM(EBGMKey key, float playback = 0f)
        {
            var clip = GetBGMClip(key);
            if (clip == null)
                return;

            // BGM再生中であれば停止しておく
            if (_bgmPlayer.isPlaying)
                _bgmPlayer.Stop();

            _bgmPlayer.time = playback;
            _bgmPlayer.clip = clip;
            _bgmPlayer.Play();
        }

        /// <summary>
        /// 再生中のBGMを停止する
        /// </summary>
        public void StopBGM()
        {
            _bgmPlayer.Stop();
        }

        private AudioClip GetSEClip(ESEKey key)
        {
            try {
                var soundData = _seClipList.Single(data => data.key.Equals(key.ToString()));
                return soundData.clip;
            } catch (InvalidOperationException) {
                Debug.LogError($"Cannot Find SE clip with key: {key}");
                return null;
            } catch (ArgumentNullException) {
                Debug.LogError($"SE Clip list is empty");
                return null;
            }
        }

        private AudioClip GetBGMClip(EBGMKey key)
        {
            try {
                var soundData = _bgmClipList.Single(data => data.key.Equals(key.ToString()));
                return soundData.clip;
            } catch (InvalidOperationException) {
                Debug.LogError($"Cannot Find BGM clip with key: {key}");
                return null;
            } catch (ArgumentNullException) {
                Debug.LogError($"BGM Clip list is empty");
                return null;
            }
        }

        public void ResetVolume()
        {
            _bgmPlayer.volume = _INITIAL_BGM_VOLUME * UserSettings.BGMVolume;
            foreach (var sePlayer in _sePlayers) {
                sePlayer.volume = _INITIAL_SE_VOLUME * UserSettings.SEVolume;
            }
        }

        /// <summary>
        /// プレハブにクリップリストを保存できるようにデータ化するためのクラス
        /// </summary>
        [System.Serializable]
        private class SoundData
        {
            [SerializeField] public string key;
            [SerializeField] public AudioClip clip;
        }

        #if UNITY_EDITOR
        /// <summary>
        /// 簡単にゲーム中使用するSE、BGMを登録できるためエディタを改造する
        /// </summary>
        [CustomEditor(typeof(SoundManager))]
        public class SoundManagerEditor : Editor
        {
            private SerializedProperty _bgmListProp;
            private SerializedProperty _seListProp;

            public void OnEnable()
            {
                _bgmListProp = serializedObject.FindProperty("_bgmClipList");
                _seListProp = serializedObject.FindProperty("_seClipList");

                var src = target as SoundManager;

                // 新規追加のBGMキーがある時の対応
                src._bgmClipList = Enum.GetNames(typeof(EBGMKey))
                    .ToDictionary(key => key, key => src._bgmClipList.Find(data => data.key.Equals(key)))
                    .Select(pair => pair.Value)
                    .ToList();

                // 新規追加のSEキーがある時の対応
                src._seClipList = Enum.GetNames(typeof(ESEKey))
                    .ToDictionary(key => key, key => src._seClipList.Find(data => data.key.Equals(key)))
                    .Select(pair => pair.Value)
                    .ToList();
            }

            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_INITIAL_BGM_VOLUME"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_INITIAL_SE_VOLUME"));

                EditorGUILayout.LabelField("BGM List", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                foreach (var eBgm in Enum.GetValues(typeof(EBGMKey)) as EBGMKey[]) {
                    var soundDataProp = _bgmListProp.GetArrayElementAtIndex((int)eBgm);
                    // BGMのキーを保存
                    var keyProp = soundDataProp.FindPropertyRelative("key");
                    keyProp.stringValue = eBgm.ToString();

                    EditorGUILayout.PropertyField(soundDataProp.FindPropertyRelative("clip"), new GUIContent(eBgm.ToString()));
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("SE List", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                foreach (var eSe in Enum.GetValues(typeof(ESEKey)) as ESEKey[]) {
                    var soundDataProp = _seListProp.GetArrayElementAtIndex((int)eSe);
                    var keyProp = soundDataProp.FindPropertyRelative("key");
                    keyProp.stringValue = eSe.ToString();

                    EditorGUILayout.PropertyField(soundDataProp.FindPropertyRelative("clip"), new GUIContent(eSe.ToString()));
                }
                EditorGUILayout.EndVertical();

                if (!EditorGUI.EndChangeCheck()) return;

                // Set object dirty, this will make it be saved after saving the project.
                EditorUtility.SetDirty(serializedObject.targetObject);

                serializedObject.ApplyModifiedProperties();
            }
        }
        #endif
    }
}
