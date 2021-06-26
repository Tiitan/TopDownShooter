using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Character))]
public class Player : MonoBehaviour
{
    private Character _character;

    void Awake()
    {
        _character = GetComponent<Character>();
    }

    [UsedImplicitly]
    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var direction2D = context.ReadValue<Vector2>();
            _character.Direction = new Vector3(direction2D.x, 0, direction2D.y);
        }
        else if (context.canceled)
            _character.Direction = Vector3.zero;
    }
    
    [UsedImplicitly]
    public void Action(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        // TODO: action
    }
}
