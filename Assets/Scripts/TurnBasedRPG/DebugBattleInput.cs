using UnityEngine;
using UnityEngine.InputSystem;

public class DebugBattleInput : MonoBehaviour
{
    public InputActionAsset inputActions;
    public float speed = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move2d = inputActions.FindAction("Movement").ReadValue<Vector2>();
        Vector3 move = new(move2d.x, 0f, move2d.y);

        transform.position += speed * Time.deltaTime * move;
            
    }
}
