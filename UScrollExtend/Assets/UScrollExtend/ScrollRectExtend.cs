﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace StupidWizard.UI {
	
	public class ScrollRectExtend : ScrollRect {

		public Action<bool> onDragEventListenerSp;

		public override void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			base.OnBeginDrag (eventData);
			//		Debug.LogError("on begin Drag");
			if (onDragEventListenerSp != null) {
				onDragEventListenerSp(true);
			}
		}

		public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			base.OnDrag (eventData);
		}

		public override void OnEndDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			base.OnEndDrag (eventData);
			if (onDragEventListenerSp != null) {
				onDragEventListenerSp(false);
			}
			//		Debug.LogError("on stop Drag, velocity = " + velocity);
		}
	}

}

