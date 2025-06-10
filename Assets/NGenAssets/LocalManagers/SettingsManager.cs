using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGenAssets {

    [Serializable]
    public class SettingsConfig {

        public bool g_bSoundOn;
        public bool g_bMusicOn;
        public bool g_bVibrateOn;

        public SettingsConfig() {
            g_bSoundOn = true;
            g_bMusicOn = true;
            g_bVibrateOn = true;
        }

    }

    public class SettingsManager : MonoBehaviour {

		#region Singleton
		public static SettingsManager Instance;
		private void Awake() {
			Instance = this;
			DontDestroyOnLoad(gameObject);
            LoadVariables();
        }
        #endregion

        #region Variables
        private SettingsConfig m_oSettingsConfig;
        private AudioSource m_oMusicSource;
        private Transform s_tfSoundContainer;
        #endregion

        #region Functions
        private void Start() {

        }

        private void LoadVariables() {
            string _sSettingsConfig = PlayerPrefs.GetString("NGenAssets_SettingsConfig", "");
            if (string.IsNullOrEmpty(_sSettingsConfig) == true) {
                m_oSettingsConfig = new SettingsConfig();
            }
            else {
                m_oSettingsConfig = JsonUtility.FromJson<SettingsConfig>(_sSettingsConfig);
            }

            GameObject _goMusicObject = new GameObject("Music Object");
            _goMusicObject.transform.parent = transform;
            m_oMusicSource = _goMusicObject.AddComponent<AudioSource>();
            m_oMusicSource.loop = true;

            GameObject _goSoundContainer = new GameObject("Sound Container");
            _goSoundContainer.transform.parent = transform;
            s_tfSoundContainer = _goSoundContainer.transform;
        }

        private void SaveSettingsConfig() {
            string _sSettingsConfig = JsonUtility.ToJson(m_oSettingsConfig);
            PlayerPrefs.SetString("NGenAssets_SettingsConfig", _sSettingsConfig);
        }

        public void TurnSound() {
            m_oSettingsConfig.g_bSoundOn = !m_oSettingsConfig.g_bSoundOn;
            if (m_oSettingsConfig.g_bSoundOn == false) {
                StopAllSound();
            }
            SaveSettingsConfig();
        }

        public bool IsSoundOn() {
            return m_oSettingsConfig.g_bSoundOn;
        }

        public void TurnMusic() {
            m_oSettingsConfig.g_bMusicOn = !m_oSettingsConfig.g_bMusicOn;
            if (m_oSettingsConfig.g_bMusicOn == true) {
                PlayMusic();
            }
            else {
                StopMusic();
            }
            SaveSettingsConfig();
        }

        public bool IsMusicOn() {
            return m_oSettingsConfig.g_bMusicOn;
        }

        public void TurnVibrate() {
            m_oSettingsConfig.g_bVibrateOn = !m_oSettingsConfig.g_bVibrateOn;
            SaveSettingsConfig();
        }

        public bool IsVibrateOn() {
            return m_oSettingsConfig.g_bVibrateOn;
        }

        public void PlayMusic() {
            if (m_oMusicSource.clip != null && m_oMusicSource.isPlaying == false) {
                m_oMusicSource.Play();
            }
        }

        public void PlayMusic(AudioClip p_acMusic) {
            if (m_oSettingsConfig.g_bMusicOn == true) {
                if (p_acMusic != m_oMusicSource.clip) {
                    m_oMusicSource.clip = p_acMusic;
                }
                if (m_oMusicSource.isPlaying == false) {
                    m_oMusicSource.Play();
                }
            }
        }

        public void StopMusic() {
            if (m_oMusicSource.isPlaying == true) {
                m_oMusicSource.Stop();
            }
        }

        public void PlaySound(AudioClip p_acSound) {
            if (m_oSettingsConfig.g_bSoundOn == true) {
                AudioSource _oAudioSource = GetSoundObject();
                _oAudioSource.PlayOneShot(p_acSound);
            }
        }

        public void StopAllSound() {
            foreach (Transform _tfSound in s_tfSoundContainer) {
                if (_tfSound.GetComponent<AudioSource>().isPlaying == true) {
                    _tfSound.GetComponent<AudioSource>().Stop();
                }
            }
        }

        private AudioSource GetSoundObject() {
            AudioSource _oAudioSource;
            for (int i = 0; i < s_tfSoundContainer.childCount; i++) {
                _oAudioSource = s_tfSoundContainer.GetChild(0).GetComponent<AudioSource>();
                if (_oAudioSource.isPlaying == false) {
                    return _oAudioSource;
                }
            }

            GameObject _goSoundObject = new GameObject();
            _goSoundObject.transform.parent = s_tfSoundContainer;
            _oAudioSource = _goSoundObject.AddComponent<AudioSource>();
            return _oAudioSource;
        }
        #endregion

    }

}
