using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    #region Views
    [Header("Views")]
    [SerializeField]
    private SpriteRenderer s_oItemSpriteRenderer;
    #endregion

    #region Variables
    private ItemModel m_oItemModel;
    private TileController m_oTile;

    private float m_fStartSpeed;
    private float m_fAcceleration;
    private float m_fMaxSpeed;
    public float m_fCurrentSpeed;

    private bool m_bIsMoving;

    private int m_nActivedTime;
    #endregion

    #region Functions
    private void Awake() {
        m_fStartSpeed = GameSceneController.Instance.GetTileStartSpeed();
        m_fAcceleration = GameSceneController.Instance.GetTileAcceleration();
        m_fMaxSpeed = GameSceneController.Instance.GetTileMaxSpeed();
        m_fCurrentSpeed = m_fStartSpeed;
        m_bIsMoving = false;
        m_nActivedTime = 0;
    }

    public void SetItemModel(ItemModel p_oItemModel) {
        m_oItemModel = p_oItemModel;

        Sprite _oItemSprite = ThemeController.Instance.GetItemSprite(p_oItemModel.piece, p_oItemModel.type);
        s_oItemSpriteRenderer.sprite = _oItemSprite;

        StartCoroutine(SpawnIE());
    }

    public ItemModel GetItemModel() {
        return m_oItemModel;
    }

    private IEnumerator SpawnIE() {
        LevelController.Instance.OnCreateItemStart();

        float _fDuration = 0.2f;
        float _fElapsedTime = 0.0f;
        while (_fElapsedTime < _fDuration) {
            float _fProceed = _fElapsedTime / _fDuration;
            transform.localScale = Vector3.one * _fProceed;

            _fElapsedTime += Time.deltaTime;
            yield return null;
        }
        LevelController.Instance.OnCreateItemDone();
    }

    public void SetTile(TileController p_oTile) {
        m_oTile = p_oTile;
        m_oItemModel.position = m_oTile.GetPosition();
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
        LevelController.Instance.OnMoveItemStart();

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

        LevelController.Instance.OnMoveItemDone();
        m_bIsMoving = false;

        yield return null;
    }

    public bool IsMoving() {
        return m_bIsMoving;
    }

    public void Active() {
        LevelController.Instance.ActiveItem(m_oItemModel);
        if (m_oItemModel.type.Equals("bomb") == true) {
            if (m_nActivedTime > 0) {
                m_oTile.RemoveItem();
                Destroy(gameObject);
            }
        }
        else {
            m_oTile.RemoveItem();
            Destroy(gameObject);
        }
        m_nActivedTime++;
    }
    #endregion

}
