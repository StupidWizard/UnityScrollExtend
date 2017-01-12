#region header
//----------------------------------------------
// ConstPivot.cs
// This stupid code is created by StupidWizard on 2017/01/12.
//----------------------------------------------
#endregion

using UnityEngine;
using System.Collections;

namespace StupidWizard.UI {
	public class ConstPivot {

		static Vector2 _center = new Vector2(0.5f, 0.5f);

		/// <summary>
		/// Shorthand for writing Vector2(0.5, 0.5)
		/// </summary>
		public static Vector2 Center {
			get {
				return _center;
			}
		}

		static Vector2 _top = new Vector2(0.5f, 1.0f);
		/// <summary>
		/// Shorthand for writing Vector2(0.5, 1)
		/// </summary>
		public static Vector2 Top {
			get {
				return _top;
			}
		}

		/// <summary>
		/// Shorthand for writing Vector2(0, 1)
		/// </summary>
		public static Vector2 TopLeft {
			get {
				return Vector2.up;
			}
		}

		/// <summary>
		/// Shorthand for writing Vector2(1, 1)
		/// </summary>
		public static Vector2 TopRight {
			get {
				return Vector2.one;
			}
		}

		static Vector2 _left = new Vector2(0, 0.5f);
		/// <summary>
		/// Shorthand for writing Vector2(0, 0.5)
		/// </summary>
		public static Vector2 Left {
			get {
				return _left;
			}
		}

		static Vector2 _right = new Vector2(1.0f, 0.5f);
		/// <summary>
		/// Shorthand for writing Vector2(1.0, 0.5)
		/// </summary>
		public static Vector2 Right {
			get {
				return _right;
			}
		}

		static Vector2 _bot = new Vector2(0.5f, 0);
		/// <summary>
		/// Shorthand for writing Vector2(0.5, 0)
		/// </summary>
		public static Vector2 Bot {
			get {
				return _bot;
			}
		}

		/// <summary>
		/// Shorthand for writing Vector2(0, 0)
		/// </summary>
		public static Vector2 BotLeft {
			get {
				return Vector2.zero;
			}
		}

		/// <summary>
		/// Shorthand for writing Vector2(1, 0)
		/// </summary>
		public static Vector2 BotRight {
			get {
				return Vector2.right;
			}
		}
	}

}
