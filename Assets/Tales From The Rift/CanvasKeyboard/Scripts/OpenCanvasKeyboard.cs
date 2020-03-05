using UnityEngine;
using System.Collections;

namespace TalesFromTheRift
{
	public class OpenCanvasKeyboard : MonoBehaviour 
	{
		public static bool isOpening = false;
		// Canvas to open keyboard under
		public Canvas CanvasKeyboardObject;

		// Optional: Input Object to receive text 
		public GameObject inputObject;

		public void OpenKeyboard() 
		{
			isOpening = true;
			CanvasKeyboard.Open(CanvasKeyboardObject, inputObject != null ? inputObject : gameObject);
		}

		public void CloseKeyboard() 
		{
			isOpening = false;
			CanvasKeyboard.Close ();
		}
	}
}