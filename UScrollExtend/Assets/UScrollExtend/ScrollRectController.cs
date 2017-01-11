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
		private const float mRatioV = 10;

		public enum FocusState {
			IDLE = 0,
			WAIT_V_SLOW,
			UPDATE_FOCUS
		}

		FocusState mState = FocusState.IDLE;
		ScrollRectExtend mScrollRect;

		[SerializeField] GameObject mBackArrow;
		[SerializeField] GameObject mNextArrow;

		[SerializeField] Vector2 mCellSize = new Vector2(240, 320);

		int mForceTargetId = -1;

		public Action<int> OnReachCell;

		// Use this for initialization
		void Start () {
			mScrollRect = GetComponent<ScrollRectExtend>();

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
				if (mScrollRect.horizontal) {
					UpdateFocusX();
				} else {
					UpdateFocusY();
				}
				break;

			default:	// idle - nothing to do
				break;
			}
		}
			
		void UpdateFocusX() {
			Vector3 contentPos = mContent.localPosition;
			float curPosX = (mContent.localPosition.x);

			int targetIdInt = CalculateNearestTargetId();

			float targetX = -targetIdInt * mCellSize.x;
			float deltaX = targetX - curPosX;

			if (Mathf.Abs(deltaX) < mDeltaPosThreshold) {
				Stop(targetIdInt);
				contentPos.x = targetX;
				mContent.localPosition = contentPos;
			} else {
				mScrollRect.velocity = mRatioV * new Vector2(deltaX, 0);
			}
		}

		void UpdateFocusY() {
			Vector3 contentPos = mContent.localPosition;
			float curPosY = (mContent.localPosition.y);

			int targetIdInt = CalculateNearestTargetId();

			float targetY = -targetIdInt * mCellSize.y;
			float deltaY = targetY - curPosY;

			if (Mathf.Abs(deltaY) < mDeltaPosThreshold) {
				Stop(targetIdInt);
				contentPos.y = targetY;
				mContent.localPosition = contentPos;
			} else {
				mScrollRect.velocity = mRatioV * new Vector2(0, deltaY);
			}
		}


		void Stop(int idStop) {
			mState = FocusState.IDLE;
			if (OnReachCell != null) {
				OnReachCell(idStop);
			}
			mScrollRect.velocity = Vector2.zero;
		}

		int CalculateNearestTargetId() {
			int curId = CurrentId;
			if (mForceTargetId >= 0) {
				if (curId != mForceTargetId) {
					return mForceTargetId;
				} else {
					mForceTargetId = -1;
				}
			}
			return curId;
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
			if (mBackArrow != null) {
				mBackArrow.SetActive(id > 0);
			}
			if (mNextArrow != null) {
				mNextArrow.SetActive(id < MaxId);
			}
		}


		/// <summary>
		/// Scroll to back Cell.
		/// </summary>
		public void ScrollBack() {
			int curId = CurrentId;
			if (curId > 0) {
				mForceTargetId = curId - 1;

				if (mState == FocusState.IDLE) {
					mState = FocusState.UPDATE_FOCUS;
					KickMove(false);
				}
			}
		}

		/// <summary>
		/// Scroll to next Cell.
		/// </summary>
		public void ScrollNext() {
			int curId = CurrentId;
			if (curId < MaxId) {
				mForceTargetId = curId + 1;

				if (mState == FocusState.IDLE) {
					mState = FocusState.UPDATE_FOCUS;
					KickMove(true);
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
					KickMove(mForceTargetId > oldId);
				}
			}
		} 




		/// <summary>
		/// Kick the move horizontal start.
		/// </summary>
		/// <param name="isKickToRight">If set to <c>true</c> scroll to show right side.</param>
		void KickMove(bool isKickToRight) {
			var contentPos = mContent.anchoredPosition;

			if (mScrollRect.horizontal) {
				contentPos.x += Epsinol * (isKickToRight? -1 : 1);
			} else {
				contentPos.y += Epsinol * (isKickToRight? -1 : 1);
			}

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

		float mVelocityThreshold {
			get {
				return (mScrollRect != null && mScrollRect.horizontal)? mCellSize.x : mCellSize.y;
			}
		}

		float mDeltaPosThreshold {
			get {
				return mVelocityThreshold * 0.01f;		// with fps = 100;
			}
		}
	}
}