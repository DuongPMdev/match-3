using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NGenAssets {

	public class ScaleOnPointer : MonoBehaviour {

		#region Variables
		[SerializeField] private string s_sEventCommand;
		private enum STATE { IDLE_UP, DOWNING, IDLE_DOWN, UPING }
		private STATE m_oState;
		#endregion

		#region Functions
		private void Start () {
			LoadVariables();
		}

		private void LoadVariables() {
			m_oState = STATE.IDLE_UP;
			if (GetComponent<EventTrigger> () == null) {
				gameObject.AddComponent<EventTrigger> ();
			}
			EventTrigger _oEventTrigger = GetComponent<EventTrigger> ();

			EventTrigger.Entry _oPointerDownEntry = new();
			_oPointerDownEntry.eventID = EventTriggerType.PointerDown;
			_oPointerDownEntry.callback.AddListener ((p_oPointerEventData) => {
				OnPointerDownDelegate ((PointerEventData)p_oPointerEventData);
			});
			_oEventTrigger.triggers.Add (_oPointerDownEntry);

			EventTrigger.Entry _oPointerUpEntry = new();
			_oPointerUpEntry.eventID = EventTriggerType.PointerUp;
			_oPointerUpEntry.callback.AddListener ((p_oPointerEventData) => {
				OnPointerUpDelegate ((PointerEventData)p_oPointerEventData);
			});
			_oEventTrigger.triggers.Add (_oPointerUpEntry);
		}

		private void FixedUpdate() {
			FixedUpdateScale();
		}

		private void FixedUpdateScale() {
			if (m_oState == STATE.IDLE_UP) {

			}
			else if (m_oState == STATE.DOWNING) {
				Vector3 _v3LocalScale = transform.localScale;
				if (_v3LocalScale.x > 0.95f) {
					_v3LocalScale -= Vector3.one * 0.01f;
					transform.localScale = _v3LocalScale;
				}
				else {
					transform.localScale = Vector3.one * 0.95f;
					m_oState = STATE.IDLE_DOWN;
                }
			}
			else if (m_oState == STATE.IDLE_DOWN) {

			}
			else if (m_oState == STATE.UPING) {
				Vector3 _v3LocalScale = transform.localScale;
				if (_v3LocalScale.x < 1.0f) {
					_v3LocalScale += Vector3.one * 0.01f;
					transform.localScale = _v3LocalScale;
				}
				else {
					transform.localScale = Vector3.one;
					m_oState = STATE.IDLE_UP;
                }
			}
		}

		private void OnPointerDownDelegate(PointerEventData p_oPointerEventData) {
			if (GetComponent<Button>() != null) {
				if (GetComponent<Button>().interactable == false) {
					return;
				}
            }
			m_oState = STATE.DOWNING;
		}

		private void OnPointerUpDelegate (PointerEventData p_oPointerEventData) {
			if (GetComponent<Button>() != null) {
				if (GetComponent<Button>().interactable == false) {
					return;
				}
            }
			if (string.IsNullOrEmpty(s_sEventCommand) == false) {

			}
			m_oState = STATE.UPING;
		}
		#endregion

	}
}