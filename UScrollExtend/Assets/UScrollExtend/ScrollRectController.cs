#region header
//----------------------------------------------
// ScrollRectController.cs
// This stupid code is created by StupidWizard on 2017/01/10.
//----------------------------------------------
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StupidWizard.UI {
	public class ScrollRectController : MonoBehaviour {
		private const float Epsinol = 2.0f;

		public enum FocusState {
			IDLE = 0,
			WAIT_V_SLOW,
			UPDATE_FOCUS
		}

		FocusState mState = FocusState.IDLE;

		[SerializeField] ScrollRectExtend mScrollRect;
		[SerializeField] GameObject[] mScrollArrows;
		[SerializeField] Vector2 mCellSize = new Vector2(240, 320);
		[SerializeField] float mVelocityThreshold = 240;
		[SerializeField] float mDeltaPosThreshold = 1;
		[SerializeField] float mRatioV = 10;

		int mForceTargetId = -1;

		public Action<int> OnReachCell;

		// Use this for initialization
		void Start () {
			mScrollRect.onDragEventListenerSp += onDragEventListener;
			OnReachCell += OnMyReachCell;
		}
			
		// Update is called once per frame
		void Update () {
			switch (mState) {
			case FocusState.WAIT_V_SLOW:
				if (mScrollRect.velocity.magnitude < mVelocityThreshold) {
					mState = FocusState.UPDATE_FOCUS;
				}
				break;

			case FocusState.UPDATE_FOCUS:
				UpdateFocus();
				break;

			default:	// idle - nothing to do
				break;
			}
		}
			
		void UpdateFocus() {
			Vector3 contentPos = mContent.localPosition;
			float curPosX = (mContent.localPosition.x);

			int targetIdInt = CurrentId;
			if (mForceTargetId >= 0) {
				if (targetIdInt != mForceTargetId) {
					targetIdInt = mForceTargetId;
				} else {
					mForceTargetId = -1;
				}
			}

			float targetX = -targetIdInt * mCellSize.x;
			float deltaX = targetX - curPosX;

			if (Mathf.Abs(deltaX) < mDeltaPosThreshold) {
				mState = FocusState.IDLE;
				if (OnReachCell != null) {
					OnReachCell(targetIdInt);
				}
				mScrollRect.velocity = Vector2.zero;
				contentPos.x = targetX;
				mContent.localPosition = contentPos;
			} else {
				mScrollRect.velocity = mRatioV * new Vector2(deltaX, 0);
			}
		}



		/// <summary>
		/// the drag event change callback.
		/// </summary>
		/// <param name="isDrag">If set to <c>true</c> is start drag else is finish drag.</param>
		public void onDragEventListener(bool isDrag) {
			if (isDrag) {
				mState = FocusState.IDLE;
			} else {
				if (mScrollRect.velocity.magnitude > mVelocityThreshold) {
					mState = FocusState.WAIT_V_SLOW;
				} else {
					mState = FocusState.UPDATE_FOCUS;
				}
			}
		}


		/// <summary>
		/// Call back when scrollView reach any cell.
		/// </summary>
		/// <param name="id">Id of cell reached.</param>
		void OnMyReachCell(int id) {
			if (mScrollArrows != null && mScrollArrows.Length >= 2) {
				mScrollArrows[0].SetActive(id > 0);
				mScrollArrows[1].SetActive(id < MaxId);
			}
		}


		/// <summary>
		/// Scrolls to left cell (nearest).
		/// </summary>
		public void ScrollToLeftCell() {
			int curId = CurrentId;
			if (curId > 0) {
				mForceTargetId = curId - 1;

				if (mState == FocusState.IDLE) {
					mState = FocusState.UPDATE_FOCUS;
					KickMoveHorizontal(false);
				}
			}
		}

		/// <summary>
		/// Scrolls to right cell (nearest).
		/// </summary>
		public void ScrollToRightCell() {
			int curId = CurrentId;
			if (curId < MaxId) {
				mForceTargetId = curId + 1;

				if (mState == FocusState.IDLE) {
					mState = FocusState.UPDATE_FOCUS;
					KickMoveHorizontal(true);
				}
			}
		}


		/// <summary>
		/// Scrolls to any cell.
		/// </summary>
		/// <param name="targetId">Id of Cell which want to scroll to.</param>
		public void ScrollToCell(int targetId) {
			int oldId = CurrentId;
			if (targetId == oldId) {
				OnMyReachCell(targetId);
				return;
			}

			if (targetId >= 0 && targetId < MaxId) {
				mForceTargetId = targetId;
				if (mState == FocusState.IDLE) {
					mState = FocusState.UPDATE_FOCUS;
					KickMoveHorizontal(mForceTargetId > oldId);
				}
			}
		} 




		/// <summary>
		/// Kick the move horizontal start.
		/// </summary>
		/// <param name="isKickToRight">If set to <c>true</c> scroll to show right side.</param>
		void KickMoveHorizontal(bool isKickToRight) {
			var contentPos = mContent.anchoredPosition;
			contentPos.x += Epsinol * (isKickToRight? -1 : 1);
			mContent.anchoredPosition = contentPos;
		}



		/// <summary>
		/// Gets the id of current Cell.
		/// </summary>
		/// <value>The current identifier.</value>
		public int CurrentId {
			get {
				float curPosX = mContent.localPosition.x;
				float targetIdFloat = -curPosX / mCellSize.x;
				int targetIdInt = Mathf.RoundToInt(targetIdFloat);

				targetIdInt = Mathf.Min(targetIdInt, MaxId);
				targetIdInt = Mathf.Max(targetIdInt, 0);

				return targetIdInt;
			}
		}


		/// <summary>
		/// Gets the max id of all cell.
		/// </summary>
		/// <value>The max identifier.</value>
		int MaxId {
			get {
				float bound = mViewport.rect.width - mCellSize.x;
				int maxId = Mathf.Max(0, Mathf.RoundToInt((mContent.rect.width - bound)/mCellSize.x) - 1);
				return maxId;
			}
		}


		RectTransform mContent {
			get {
				return (mScrollRect != null)? mScrollRect.content : null;
			}
		}

		RectTransform mViewport {
			get {
				return (mScrollRect != null)? mScrollRect.viewport : null;
			}
		}
	}
}