using UnityEngine;

public class EnemyRotation : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;

    public void RotateTowards(Vector3 position)
    {
        var targetDirection = position - transform.position;
        targetDirection.y = 0;
        var singleStep = speed * Time.deltaTime;
        var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
