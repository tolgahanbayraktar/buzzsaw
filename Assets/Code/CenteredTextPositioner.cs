using UnityEngine;

public class CenteredTextPositioner : IFloatingTextPositioner {
	/**
	 * Büyük bir yazıyı ekranın tam ortasından yukarı doğru kaydıran script 
	 * 
	*/

	readonly float _speed;
	float _textPosition;

	public CenteredTextPositioner(float speed)
	{
		_speed = speed;
	}

	public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
	{
		_textPosition += Time.deltaTime * _speed;

		if (_textPosition > 1)
			return false;

		position = new Vector2 (Screen.width / 2f - size.x / 2, Mathf.Lerp (Screen.height / 2f + size.y, 0, _textPosition));
		return true;
	}
}
