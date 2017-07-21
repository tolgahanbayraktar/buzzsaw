using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour {

	public GUIStyle Style{ get; set;}

	static readonly GUISkin skin = Resources.Load<GUISkin>("GameSkin");
	GUIContent _content;
	IFloatingTextPositioner _positioner;

	public static void Show(string text, string style, IFloatingTextPositioner positioner)
	{
		// Birtane gameobject oluşturuyoruz sahnede ve ona bir companent ekliyoruz.
		var go = new GameObject ("FloatingText");
		var floatingText = go.AddComponent<FloatingText> ();

		floatingText._content = new GUIContent (text);
		floatingText.Style = skin.GetStyle (style);
		floatingText._positioner = positioner;
	}

	public void OnGUI()
	{

		var position = new Vector2 ();
		var contentSize = Style.CalcSize (_content);

		if (!_positioner.GetPosition (ref position, _content, contentSize)) {
			Destroy (gameObject);
			return;
		}

		GUI.Label (new Rect (position.x, position.y, contentSize.x, contentSize.y), _content, Style);

	}
}
