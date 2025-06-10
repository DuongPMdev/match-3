using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    #region Singleton
    public static InputController Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion

    #region Variables
    private Action<Vector3> m_aOnPointerDown;
    private Action<Vector3> m_aOnDrag;
    private Action<Vector3> m_aOnPointerUp;
    private Vector3 m_v3LastWorldPoint;
    #endregion

    #region Functions
    private void Update() {
        if (Input.touchCount > 0) {
            Touch _oTouch = Input.GetTouch(0);

            Ray _oRayTouch = Camera.main.ScreenPointToRay(_oTouch.position);
            Plane _oPlaneTouch = new Plane(Vector3.forward, Vector3.zero);

            if (_oPlaneTouch.Raycast(_oRayTouch, out float _fDistanceTouch)) {
                Vector3 _v3WorldPoint = _oRayTouch.GetPoint(_fDistanceTouch);
                if (_oTouch.phase == TouchPhase.Began) {
                    m_aOnPointerDown?.Invoke(_v3WorldPoint);
                    m_v3LastWorldPoint = _v3WorldPoint;
                }
                else if (_oTouch.phase == TouchPhase.Ended) {
                    m_aOnPointerUp?.Invoke(_v3WorldPoint);
                }
                else {
                    if ((m_v3LastWorldPoint - _v3WorldPoint).magnitude > 0) {
                        m_aOnDrag?.Invoke(_v3WorldPoint);
                        m_v3LastWorldPoint = _v3WorldPoint;
                    }
                }
            }
        }

        Ray _oRayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane _oPlaneMouse = new Plane(Vector3.forward, Vector3.zero);

        if (_oPlaneMouse.Raycast(_oRayMouse, out float _fDistanceMouse)) {
            Vector3 _v3WorldPoint = _oRayMouse.GetPoint(_fDistanceMouse);
            if (Input.GetMouseButtonDown(0) == true) {
                m_aOnPointerDown?.Invoke(_v3WorldPoint);
                m_v3LastWorldPoint = _v3WorldPoint;
            }
            else if (Input.GetMouseButtonUp(0) == true) {
                m_aOnPointerUp?.Invoke(_v3WorldPoint);
            }
            else if (Input.GetMouseButton(0) == true) {
                if ((m_v3LastWorldPoint - _v3WorldPoint).magnitude > 0) {
                    m_aOnDrag?.Invoke(_v3WorldPoint);
                    m_v3LastWorldPoint = _v3WorldPoint;
                }
            }
        }
    }

    public void SetOnPointerDown(Action<Vector3> p_aOnPointerDown) {
        m_aOnPointerDown = p_aOnPointerDown;
    }

    public void SetOnDrag(Action<Vector3> p_aOnDrag) {
        m_aOnDrag = p_aOnDrag;
    }

    public void SetOnPointerUp(Action<Vector3> p_aOnPointerUp) {
        m_aOnPointerUp = p_aOnPointerUp;
    }
    #endregion

}
