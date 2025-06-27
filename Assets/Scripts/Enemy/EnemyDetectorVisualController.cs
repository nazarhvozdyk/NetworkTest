using UnityEngine;

public class EnemyDetectorVisualController : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private EnemyBehaviour enemyBehaviour;

    private void LateUpdate()
    {
        meshRenderer.material.SetFloat("_FillAmount", enemyBehaviour.triggerValue);
    }
}
