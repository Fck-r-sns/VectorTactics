using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSwitcher : MonoBehaviour
{

    [SerializeField]
    private Texture2D crosshair;

	void Start () {

        if (crosshair != null)
        {
            Cursor.SetCursor(
                crosshair,
                new Vector2(crosshair.width / 2.0f, crosshair.height / 2.0f),
                CursorMode.Auto
                );
        }
	}
}
