using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Patterns;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;

namespace Project.Scripts.Utils
{
    public class SoundManager : SingletonObject<SoundManager>
    {
        /// <summary>
        /// 同時再生できるSEの数
        /// </summary>
        private const int _MAX_SE_NUM = 8;

        /// <summary>
        /// 複数SE同時再生を考慮し、SE用のAudioSourceを複数用意する
        /// </summary>
        private AudioSource[] _SePlayers;

        /// <summary>
        /// BGM再生用のAudioSource
        /// </summary>
        private AudioSource _BgmPlayer;

        /// <summary>
        /// BGMリスト
        /// </summary>
        private Dictionary<EBGMKey, AudioClip> _bgmDict;

        /// <summary>
        /// SEリスト
        /// </summary>
        private Dictionary<ESEKey, AudioClip> _seDict;

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
            _BgmPlayer = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            _BgmPlayer.loop = true;

            _SePlayers = new AudioSource[_MAX_SE_NUM];
            for (var i = 0; i < _MAX_SE_NUM; i++) {
                var player = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                player.loop = false;

                _SePlayers[i] = player;
            }

            AddressableAssetManager.LoadAsset<AudioSheet>("AudioClipSheet").Completed += handled => {
                var sheet = handled.Result;
                _bgmDict = sheet.bgmDict.ToDictionary(data => (EBGMKey)Enum.Parse(typeof(EBGMKey), data.key), data => data.clip);
                _seDict = sheet.seDict.ToDictionary(data => (ESEKey)Enum.Parse(typeof(ESEKey), data.key), data => data.clip);
            };

            ResetVolume();
            DontDestroyOnLoad(gameObject);
        }

        public void PlaySE(ESEKey key)
        {
            if (!ValidateKey(key))
                return;

            if (_SePlayers.Any(src => !src.isPlaying)) {
                // 再生していないAudioSourceを探す
                var player = _SePlayers.First(source => !source.isPlaying);

                var clip = _seDict[key];
                player.PlayOneShot(clip);
            } else {
                Debug.LogWarning($"Failed to Play SE: {key} because there is no available audio source");
            }
        }

        public void StopSE(ESEKey key)
        {
            if (!ValidateKey(key))
                return;

            var clip = _seDict[key];
            var player = _SePlayers.Where(src => src.clip != null).SingleOrDefault(src => src.clip.name == clip.name);
            player?.Stop();
        }

        public bool IsPlaying(ESEKey key)
        {
            if (!ValidateKey(key))
                return false;
            
            var clip = _seDict[key];
            var player = _SePlayers.Where(src => src.clip != null).SingleOrDefault(src => src.clip.name == clip.name);
            return player != null && player.isPlaying;
        }

        public void PlayBGM(EBGMKey key, float playback = 0f)
        {
            if (!ValidateKey(key))
                return;

            // BGM再生中であれば停止しておく
            if (_BgmPlayer.isPlaying)
                _BgmPlayer.Stop();

            var clip = _bgmDict[key];
            _BgmPlayer.time = playback;
            _BgmPlayer.clip = clip;
            _BgmPlayer.Play();
        }

        public void StopBGM()
        {
            _BgmPlayer.Stop();
        }

        private bool ValidateKey(ESEKey key)
        {
            if (!_seDict.ContainsKey(key)) {
                Debug.LogWarning($"Cannot Find Sound Effect clip with key: {key}");
                return false;
            }
            return true;
        }

        private bool ValidateKey(EBGMKey key)
        {
            if (!_bgmDict.ContainsKey(key)) {
                Debug.LogWarning($"Cannot Find BGM clip with key: {key}");
                return false;
            }
            return true;
        }

        public void ResetVolume()
        {
            _BgmPlayer.volume = _INITIAL_BGM_VOLUME * UserSettings.BGMVolume;
            foreach (var sePlayer in _SePlayers) {
                sePlayer.volume = _INITIAL_SE_VOLUME * UserSettings.SEVolume;
            } 
        }
    }
}
