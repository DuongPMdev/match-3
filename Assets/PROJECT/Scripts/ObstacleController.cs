using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObstacleTypes {

    public const string NULL = "null";
    public const string LOCKER = "locker";
    public const string WOODBOX = "woodbox";
    public const string WOODBOX_HARD = "woodbox_hard";
    public const string BUSH = "bush";
    public const string BLUE_CRYSTAL = "blue_crystal";
    public const string RED_CRYSTAL = "red_crystal";

}

public class ObstacleController : MonoBehaviour {

    #region Prefabs
    [Header("Prefabs")]
    [SerializeField]
    private GameObject s_goPrefabBreakLocker;
    [SerializeField]
    private GameObject s_goPrefabBreakWoodbox;
    [SerializeField]
    private GameObject s_goPrefabBreakBush;
    [SerializeField]
    private GameObject s_goPrefabBreakBlueCrystal;
    [SerializeField]
    private GameObject s_goPrefabBreakRedCrystal;
    #endregion

    #region Views
    [Header("Views")]
    [SerializeField]
    private GameObject s_goCrack;
    [SerializeField]
    private GameObject s_goLocker;
    [SerializeField]
    private GameObject s_goWoodbox;
    [SerializeField]
    private GameObject s_goWoodboxHard;
    [SerializeField]
    private GameObject s_goBush;
    [SerializeField]
    private GameObject s_goBlueCrystal;
    [SerializeField]
    private GameObject s_goRedCrystal;
    #endregion

    #region Variables
    private ObstacleModel m_oObstacleModel;
    private TileController m_oTile;

    private float m_fStartSpeed;
    private float m_fAcceleration;
    private float m_fMaxSpeed;
    public float m_fCurrentSpeed;

    private bool m_bIsMoving;
    private int m_nHealth;
    #endregion

    #region Functions
    private void Awake() {
        m_fStartSpeed = GameSceneController.Instance.GetTileStartSpeed();
        m_fAcceleration = GameSceneController.Instance.GetTileAcceleration();
        m_fMaxSpeed = GameSceneController.Instance.GetTileMaxSpeed();
        m_fCurrentSpeed = m_fStartSpeed;
        m_bIsMoving = false;
    }

    public void SetObstacleModel(ObstacleModel p_oObstacleModel) {
        m_oObstacleModel = p_oObstacleModel;

        if (m_oObstacleModel.type.Equals(ObstacleTypes.LOCKER) == true) {
            s_goLocker.gameObject.SetActive(true);
            m_nHealth = 1;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX) == true) {
            s_goWoodbox.gameObject.SetActive(true);
            m_nHealth = 1;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX_HARD) == true) {
            s_goWoodboxHard.gameObject.SetActive(true);
            m_nHealth = 2;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.BUSH) == true) {
            s_goBush.gameObject.SetActive(true);
            m_nHealth = 1;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.BLUE_CRYSTAL) == true) {
            s_goBlueCrystal.gameObject.SetActive(true);
            m_nHealth = 2;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.RED_CRYSTAL) == true) {
            s_goRedCrystal.gameObject.SetActive(true);
            m_nHealth = 2;
        }
    }

    private GameObject GetPrefabBreak() {
        if (m_oObstacleModel.type.Equals(ObstacleTypes.LOCKER) == true) {
            return s_goPrefabBreakLocker;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX) == true) {
            return s_goPrefabBreakWoodbox;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX_HARD) == true) {
            return s_goPrefabBreakWoodbox;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.BUSH) == true) {
            return s_goPrefabBreakBush;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.BLUE_CRYSTAL) == true) {
            return s_goPrefabBreakBlueCrystal;
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.RED_CRYSTAL) == true) {
            return s_goPrefabBreakRedCrystal;
        }
        return null;
    }

    private AudioClip GetBreakSound() {
        if (m_oObstacleModel.type.Equals(ObstacleTypes.LOCKER) == true) {

        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX) == true) {
            return SoundController.Instance.GetSoundBreakWoodbox();
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX_HARD) == true) {
            return SoundController.Instance.GetSoundBreakWoodbox();
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.BUSH) == true) {

        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.BLUE_CRYSTAL) == true) {
            return SoundController.Instance.GetSoundBreakCrystal();
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.RED_CRYSTAL) == true) {
            return SoundController.Instance.GetSoundBreakCrystal();
        }
        return null;
    }

    public ObstacleModel GetObstacleModel() {
        return m_oObstacleModel;
    }

    public void SetTile(TileController p_oTile) {
        m_oTile = p_oTile;
        if (m_oObstacleModel.type.Equals(ObstacleTypes.NULL) == true) {
            m_oTile.ClearFooter();
        }
        m_oObstacleModel.position = m_oTile.GetPosition();
        if (transform.localPosition.magnitude > 0.1f) {
            StartCoroutine(MoveToZeroLocalPositionIE());
        }
        else {
            transform.localPosition = Vector3.zero;
        }
    }

    public void ResetSpeed() {
        m_fCurrentSpeed = m_fStartSpeed;
    }

    public float GetCurrentSpeed() {
        return m_fCurrentSpeed;
    }

    private IEnumerator MoveToZeroLocalPositionIE() {
        m_bIsMoving = true;
        LevelController.Instance.OnMoveObstacleStart();

        Vector3 _v3TargetPosition = Vector3.zero;
        Vector3 _v3CurrentPosition = transform.localPosition;
        
        float _fDistance = Vector3.Distance(_v3CurrentPosition, _v3TargetPosition);
        while (_fDistance > 0.001f) {
            m_fCurrentSpeed = Mathf.Min(m_fCurrentSpeed + m_fAcceleration * Time.deltaTime, m_fMaxSpeed);

            Vector3 _v3NextPosition = Vector3.MoveTowards(_v3CurrentPosition, _v3TargetPosition, Mathf.Min(_fDistance, m_fCurrentSpeed * Time.deltaTime));
            transform.localPosition = _v3NextPosition;
            _v3CurrentPosition = _v3NextPosition;

            _fDistance = Vector3.Distance(_v3CurrentPosition, _v3TargetPosition);

            yield return null;
        }

        transform.localPosition = Vector3.zero;

        LevelController.Instance.OnMoveObstacleDone();
        m_bIsMoving = false;
    }

    public bool IsMoving() {
        return m_bIsMoving;
    }

    public bool IsNull() {
        if (m_oObstacleModel.type.Equals(ObstacleTypes.NULL) == true) {
            return true;
        }
        return false;
    }

    public void Affected() {
        if (m_oObstacleModel.type.Equals(ObstacleTypes.LOCKER) == true) {
            TakeDamage(1);
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.BUSH) == true) {
            TakeDamage(1);
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX) == true) {
            TakeDamage(1);
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.WOODBOX_HARD) == true) {
            TakeDamage(1);
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.BLUE_CRYSTAL) == true) {
            TakeDamage(1);
        }
        else if (m_oObstacleModel.type.Equals(ObstacleTypes.RED_CRYSTAL) == true) {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int p_nDamage) {
        if (IsNull() == true) {
            return;
        }
        m_nHealth -= p_nDamage;
        AudioClip _oSoundBreak = GetBreakSound();
        if (_oSoundBreak != null) {
            SettingsManager.Instance.PlaySound(_oSoundBreak);
        }
        if (m_nHealth <= 0) {
            Break();
        }
        else {
            s_goCrack.SetActive(true);
            GameObject _goPrefabBreak = GetPrefabBreak();
            if (_goPrefabBreak != null) {
                GameObject _goBreak = Instantiate(_goPrefabBreak, transform.position, Quaternion.identity, transform);
                Destroy(_goBreak, 1.0f);
            }
        }
    }

    private void Break() {
        StartCoroutine(BreakIE());
    }

    private IEnumerator BreakIE() {
        //LevelController.Instance.OnBreakObstacleStart();
        m_oTile.RemoveObstacle();
        LevelController.Instance.OnCollectTarget(m_oObstacleModel.type, 0);

        s_goCrack.SetActive(false);

        GameObject _goPrefabBreak = GetPrefabBreak();
        if (_goPrefabBreak != null) {
            s_goCrack.SetActive(false);
            s_goLocker.SetActive(false);
            s_goWoodbox.SetActive(false);
            s_goWoodboxHard.SetActive(false);
            s_goBush.SetActive(false);
            s_goBlueCrystal.SetActive(false);
            s_goRedCrystal.SetActive(false);
            GameObject _goBreak = Instantiate(_goPrefabBreak, transform.position, Quaternion.identity, transform);
            Destroy(_goBreak, 1.0f);
            yield return new WaitForSeconds(1.0f);
        }
        else {
            float _fDuration = 0.2f;
            float _fElapsedTime = 0.0f;
            while (_fElapsedTime < _fDuration) {
                float _fProceed = _fElapsedTime / _fDuration;
                transform.localScale = Vector3.one * (1.0f - _fProceed);

                _fElapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
        //LevelController.Instance.OnBreakObstacleDone();
    }
    #endregion

}
