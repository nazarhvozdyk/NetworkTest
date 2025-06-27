using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private NetworkVariable<Vector2> _position = new NetworkVariable<Vector2>();

    [SerializeField] private float speed = 7;
    private Vector3 _direction;

    public void SetUp(Vector3 direction)
    {
        _direction = direction;
        _direction.y = 0;

        SetStartPosClientRpc(transform.position);
        SetUpClientRpc();
    }

    [ClientRpc]
    private void SetUpClientRpc()
    {
        if (NetworkManager.Singleton.IsServer) return;

        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Collider>());
    }

    [ClientRpc]
    private void SetStartPosClientRpc(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer)

        {
            var delta = _direction * speed * Time.deltaTime;
            transform.Translate(delta);

            var position2D = new Vector2(transform.position.x, transform.position.z);
            _position.Value = position2D;
        }
        else
        {
            var pos = new Vector3(_position.Value.x, transform.position.y, _position.Value.y);
            transform.position = pos;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        var health = collision.collider.GetComponent<Health>();
        var damage = 20;

        if (health)
            health.TakeDamage(damage);

        Destroy(gameObject);

        ColisionHandlerClientRpc();
    }

    [ClientRpc]
    private void ColisionHandlerClientRpc()
    {
        Destroy(gameObject);
    }
}
