using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardAccumulationInput :
    MonoBehaviour,
    IAccumulationInput
{
    public Vector2 Movement
    {
        get
        {
            if (Keyboard.current == null)
                return Vector2.zero;

            float horizontal = 0f;
            float vertical = 0f;

            if (Keyboard.current.dKey.isPressed)
                horizontal += 1f;

            if (Keyboard.current.aKey.isPressed)
                horizontal -= 1f;

            if (Keyboard.current.wKey.isPressed)
                vertical += 1f;

            if (Keyboard.current.sKey.isPressed)
                vertical -= 1f;

            Vector2 direction =
                new Vector2(horizontal, vertical);

            return Vector2.ClampMagnitude(direction, 1f);
        }
    }

    public bool AccumulationHeld
    {
        get
        {
            return Keyboard.current != null &&
                   Keyboard.current.spaceKey.isPressed;
        }
    }

    public bool ResetPressed
    {
        get
        {
            return Keyboard.current != null &&
                   Keyboard.current.rKey.wasPressedThisFrame;
        }
    }
}