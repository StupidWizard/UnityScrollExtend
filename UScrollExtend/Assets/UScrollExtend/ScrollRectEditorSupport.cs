#region header
//----------------------------------------------
// ScrollRectEditorSupport.cs
// This stupid code is created by StupidWizard on 2017/01/11.
//----------------------------------------------
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace StupidWizard.UI {

	[RequireComponent(typeof (ScrollRectExtend))]
	public class ScrollRectEditorSupport : MonoBehaviour {

		ScrollRectExtend _scrollRect;
		ScrollRectExtend scrollRect {
			get {
				if (_scrollRect == null) {
					_scrollRect = GetComponent<ScrollRectExtend>();
				}
				return _scrollRect;
			}
		}

		ScrollRectController _controller;
		ScrollRectController controller {
			get {
				if (_controller == null) {
					_controller = GetComponent<ScrollRectController>();
				}
				return _controller;
			}
		}

		[SerializeField]
		List<RectTransform> listChild = new List<RectTransform>();

		[SerializeField]
		Vector2 childPivot = Vector2.one * 0.5f;


		public void SetupAll() {
			LoadChildPointer();
			SetupAnchorPivot();
			Reposition();
		}



		public void LoadChildPointer() {
			listChild.Clear();
			int nChild = scrollRect.content.childCount;
			if (nChild > 0) {
				for (int i = 0; i < nChild; i++) {
					listChild.Add(scrollRect.content.GetChild(i).GetComponent<RectTransform>());
				}
			}
		}



		public void SetupAnchorPivot() {
			SetRectTransformAnchorPivot(scrollRect.GetComponent<RectTransform>(), 
				ConstPivot.Center, ConstPivot.Center, ConstPivot.Center);
			SetRectTransformAnchorPivot(scrollRect.viewport, Vector2.zero, Vector2.one, ConstPivot.TopLeft);


			if (IsHorizontal()) {
				SetupAnchorPivotHorizontal();
			} else {
				SetupAnchorPivotVertical();
			}
		}

		void SetupAnchorPivotHorizontal() {
			SetRectTransformAnchorPivot(scrollRect.content, ConstPivot.Left, ConstPivot.Left, ConstPivot.Left);

			foreach (RectTransform child in listChild) {
				SetRectTransformAnchorPivot(child, ConstPivot.Left, ConstPivot.Left, childPivot);
			}
		}

		void SetupAnchorPivotVertical() {
			SetRectTransformAnchorPivot(scrollRect.content, ConstPivot.Top, ConstPivot.Top, ConstPivot.Top);

			foreach (RectTransform child in listChild) {
				SetRectTransformAnchorPivot(child, ConstPivot.Top, ConstPivot.Top, childPivot);
			}
		}





		public void Reposition() {
			scrollRect.content.anchoredPosition = Vector2.zero;
			if (IsHorizontal()) {
				RepositionHorizontal();
			} else {
				RepositionVertical();
			}
		}

		void RepositionHorizontal() {
			if (listChild.Count > 0) {
				// content
				float sub = scrollRect.viewport.rect.width - controller.CellSize.x;
				float extend = (sub>0)? sub % controller.CellSize.x : sub;
				float width = extend + listChild.Count * controller.CellSize.x;
				scrollRect.content.sizeDelta = new Vector2(width, scrollRect.viewport.rect.height);

				// child
				extend = extend / 2.0f;
				for (int i = 0; i < listChild.Count; i++) {
					listChild[i].anchoredPosition = new Vector2(extend + controller.CellSize.x * (i + 0.5f), 0);
				}
			} else {
				scrollRect.content.sizeDelta = scrollRect.viewport.rect.size;
			}
		}

		void RepositionVertical() {
			if (listChild.Count > 0) {
				// content
				float sub = scrollRect.viewport.rect.height - controller.CellSize.y;
				float extend = (sub >0)? (sub % controller.CellSize.y) : sub;
				float height = extend + listChild.Count * controller.CellSize.y;
				scrollRect.content.sizeDelta = new Vector2(scrollRect.viewport.rect.width, height);

				extend = extend / 2.0f;
				for (int i = 0; i < listChild.Count; i++) {
					listChild[i].anchoredPosition = new Vector2(0, -extend - controller.CellSize.y * (i + 0.5f));
				}
			} else {
				scrollRect.content.sizeDelta = scrollRect.viewport.rect.size;
			}
		}





		bool IsHorizontal() {
			return (scrollRect.horizontal || !scrollRect.vertical);
		}

		void SetRectTransformAnchorPivot(RectTransform target, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot) {
			target.anchorMin = anchorMin;
			target.anchorMax = anchorMax;
			target.pivot = pivot;
		}
	}





	#if UNITY_EDITOR
	[CustomEditor(typeof(ScrollRectEditorSupport))]
	public class ScrollRectEditorSupportEditor : Editor 
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			ScrollRectEditorSupport myScript = (ScrollRectEditorSupport)target;

			var btnWidth = GUILayout.Width(100);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Load Child pointer");

			if (GUILayout.Button("Load Child", btnWidth)) {
				myScript.LoadChildPointer();
			}
			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			GUILayout.Label("Set up anchor, pivot");
			if (GUILayout.Button("Set up", btnWidth)) {
				myScript.SetupAnchorPivot();
			}
			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			GUILayout.Label("Set child position");
			if (GUILayout.Button("Reposition", btnWidth)) {
				myScript.Reposition();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Setup All");
			if (GUILayout.Button("Setup All", GUILayout.Width(150))) {
				myScript.SetupAll();
			}
			GUILayout.EndHorizontal();
		}
	}
	#endif

}