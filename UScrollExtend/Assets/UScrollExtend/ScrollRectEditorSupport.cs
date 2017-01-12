#region header
//----------------------------------------------
// ScrollRectEditorSupport.cs
// This stupid code is created by StupidWizard on 2017/01/11.
//----------------------------------------------
#endregion

using UnityEngine;
using System.Collections;

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
		RectTransform[] listChild;

		[SerializeField]
		Vector2 childPivot = Vector2.one * 0.5f;

		public void LoadChildPointer() {
			int nChild = scrollRect.content.childCount;
			if (nChild > 0) {
				listChild = new RectTransform[nChild];
				for (int i = 0; i < nChild; i++) {
					listChild[i] = scrollRect.content.GetChild(i).GetComponent<RectTransform>();
				}
			} else {
				listChild = null;
			}
		}


		public void SetupAnchorPivot() {
			SetRectTransformAnchorPivot(scrollRect.GetComponent<RectTransform>(), 
				ConstPivot.Center, ConstPivot.Center, ConstPivot.Center);
			SetRectTransformAnchorPivot(scrollRect.viewport, Vector2.zero, Vector2.one, Vector2.zero);


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
			if (IsHorizontal()) {
				RepositionHorizontal();
			} else {
				RepositionVertical();
			}
		}

		void RepositionHorizontal() {
			// content
			float sub = scrollRect.viewport.rect.width - controller.CellSize.x;
			float extend = (sub>0)? sub % controller.CellSize.x : sub;
			float width = extend + listChild.Length * controller.CellSize.x;
			scrollRect.content.sizeDelta = new Vector2(width, scrollRect.content.sizeDelta.y);
			scrollRect.content.anchoredPosition = Vector2.zero;

			// child
			extend = extend / 2.0f;
			for (int i = 0; i < listChild.Length; i++) {
				listChild[i].anchoredPosition = new Vector2(extend + controller.CellSize.x * (i + 0.5f), 0);
			}
		}

		void RepositionVertical() {
			
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

			var btnWidth = GUILayout.Width(120);

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
		}
	}
	#endif

}