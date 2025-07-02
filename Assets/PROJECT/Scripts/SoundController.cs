using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    #region Singleton
    public static SoundController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Sounds
    [Header("Sounds")]
    [SerializeField]
    private AudioClip s_oMusic;
    [SerializeField]
    private AudioClip s_oSoundButtonClick;
    [SerializeField]
    private AudioClip s_oSoundLevelCompleted;
    [SerializeField]
    private AudioClip s_oSoundAperoad;
    [SerializeField]
    private AudioClip s_oSoundExplose;
    [SerializeField]
    private AudioClip s_oSoundKongMode;
    [SerializeField]
    private AudioClip s_oSoundBarrelRoll;
    #endregion

    #region Functions
    public AudioClip GetMusic() {
        return s_oMusic;
    }

    public AudioClip GetSoundButtonClick() {
        return s_oSoundButtonClick;
    }

    public AudioClip GetSoundLevelCompleted() {
        return s_oSoundLevelCompleted;
    }

    public AudioClip GetSoundAperoad() {
        return s_oSoundAperoad;
    }

    public AudioClip GetSoundExplose() {
        return s_oSoundExplose;
    }

    public AudioClip GetSoundKongMode() {
        return s_oSoundKongMode;
    }

    public AudioClip GetSoundBarrelRoll() {
        return s_oSoundBarrelRoll;
    }
    #endregion

}
