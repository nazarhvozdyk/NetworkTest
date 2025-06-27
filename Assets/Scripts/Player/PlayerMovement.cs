using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float rotationSpeed = 200f;

    private NetworkVariable<Vector3> _position = new NetworkVariable<Vector3>();
    private NetworkVariable<float> _yRotation = new NetworkVariable<float>();

    private void Update()
    {
        if (IsOwner)
        {
            // if you are server handle movement yourself
            if (NetworkManager.Singleton.IsServer)
            {
                HandleServerMovement();
                return;
            }

            // if no, send input data to server
            SendInputToServer();
        }

        // update transform if you are client
        UpdateTransform();
    }

    // x = z position delta, y = rotation delta
    private Vector2 GetMovementInput()
    {
        var zMovement = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
        var yRotation = Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime;

        var input = new Vector2(zMovement, yRotation);

        return input;
    }

    private void HandleServerMovement()
    {
        var input = GetMovementInput();
        Move(input);
        StoreData();
    }

    private void StoreData()
    {
        _position.Value = transform.position;
        _yRotation.Value = transform.eulerAngles.y;
    }

    private void Move(Vector2 input)
    {
        var zMovement = input.x;
        var movement = new Vector3(0, 0, zMovement);

        transform.Translate(movement);

        var yRotation = input.y;
        var rotationAngles = new Vector3(0, yRotation, 0);
        transform.Rotate(rotationAngles);
    }

    private void SendInputToServer()
    {
        var input = GetMovementInput();
        SubmitInputServerRpc(input);
    }

    private void UpdateTransform()
    {
        transform.position = _position.Value;

        var rot = transform.eulerAngles;
        rot.y = _yRotation.Value;
        transform.eulerAngles = rot;
    }

    [ServerRpc]
    public void SubmitInputServerRpc(Vector2 input)
    {
        Move(input);
        StoreData();
    }
}
