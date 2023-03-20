using UnityEngine;
using System.Linq;

namespace Pathfinding {
	public class TargetClick : MonoBehaviour
    {
		public LayerMask mask;
		public Transform target;
		IAstarAI[] ais;
		public bool oneClick;
		public bool use2D;

		Camera cam;

		public void Start () {
			cam = Camera.main;
			ais = FindObjectsOfType<MonoBehaviour>().OfType<IAstarAI>().ToArray();
			useGUILayout = false;
		}

		public void OnGUI () {
			if (oneClick && cam != null && Event.current.button == 1 && Event.current.clickCount == 1) {
				UpdateTargetPosition();
			}
			
			if (oneClick && cam != null && Event.current.button == 0 && Event.current.clickCount == 2) {
				Vector3 shot = Vector3.zero;
				shot = cam.ScreenToWorldPoint(Input.mousePosition);
				shot.z = 0;
				GameBase.G.LeaveStart( shot );
			}
		}

		void Update () {
			if (!oneClick && cam != null) {
				UpdateTargetPosition();
			}
		}

		public void UpdateTargetPosition () {
			Vector3 newPosition = Vector3.zero;
			bool positionFound = false;

			if (use2D) {
				newPosition = cam.ScreenToWorldPoint(Input.mousePosition);
				newPosition.z = 0;
				positionFound = true;
			} else {
				RaycastHit hit;
				if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask)) {
					newPosition = hit.point;
					positionFound = true;
				}
			}

			if (positionFound && newPosition != target.position) {
				target.position = newPosition;

				if (oneClick) {
					for (int i = 0; i < ais.Length; i++) {
						if (ais[i] != null) ais[i].SearchPath();
					}
				}
			}
		}
	}
}
