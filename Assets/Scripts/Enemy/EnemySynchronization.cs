using Unity.Netcode;
using UnityEngine;

public class EnemySynchronization : NetworkBehaviour
{
    private NetworkVariable<Vector3> _position = new NetworkVariable<Vector3>();
    private NetworkVariable<float> _yRotation = new NetworkVariable<float>();

    [SerializeField] private Transform rotationSource;

    private void LateUpdate()
    {
        if (IsServer)
        {
            _position.Value = transform.position;
            _yRotation.Value = rotationSource.eulerAngles.y;
            return;
        }

        transform.position = _position.Value;
        transform.rotation = Quaternion.Euler(0, _yRotation.Value, 0);
    }

}
