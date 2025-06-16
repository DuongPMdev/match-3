using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObstacleTypes {

    public const string NULL = "null";
    public const string LOCKER = "locker";

}

public class ObstacleController : MonoBehaviour {
    
    #region Views
    [Header("Views")]
    [SerializeField]
    private SpriteRenderer s_uiObstacleSpriteRenderer;
    #endregion

    #region Variables
    private ObstacleModel m_oObstacleModel;
    private TileController m_oTile;

    private float m_fStartSpeed;
    private float m_fAcceleration;
    private float m_fMaxSpeed;
    public float m_fCurrentSpeed;

    private bool m_bIsMoving;
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

        Sprite _oObstacleSprite = ThemeController.Instance.GetObstacleSprite(p_oObstacleModel.type);
        s_uiObstacleSpriteRenderer.sprite = _oObstacleSprite;
    }

    public ObstacleModel GetObstacleModel() {
        return m_oObstacleModel;
    }

    public void SetTile(TileController p_oTile) {
        m_oTile = p_oTile;
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

    public void TakeDamage(int p_nDamage) {
        Break();
    }

    public void Break() {
        StartCoroutine(BreakIE());
    }

    private IEnumerator BreakIE() {
        //LevelController.Instance.OnBreakObstacleStart();
        m_oTile.RemoveObstacle();

        float _fDuration = 0.2f;
        float _fElapsedTime = 0.0f;
        while (_fElapsedTime < _fDuration) {
            float _fProceed = _fElapsedTime / _fDuration;
            transform.localScale = Vector3.one * (1.0f - _fProceed);

            _fElapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
        //LevelController.Instance.OnBreakObstacleDone();
    }
    #endregion

}
