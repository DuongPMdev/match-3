using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    #region Views
    [Header("Views")]
    [SerializeField]
    private SpriteRenderer s_oSpriteRenderer;
    #endregion

    #region Variables
    private ItemModel m_oItemModel;
    private TileController m_oTile;

    private float m_fStartSpeed;
    private float m_fAcceleration;
    private float m_fMaxSpeed;
    private float m_fCurrentSpeed;

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

    public void SetItemModel(ItemModel p_oItemModel) {
        m_oItemModel = p_oItemModel;
    }

    public void SetTile(TileController p_oTile) {
        m_oTile = p_oTile;
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

    }
    #endregion

}
