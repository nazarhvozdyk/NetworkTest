using TMPro;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    [SerializeField] private Transform healthPoint;
    [SerializeField] private Health health;
    [SerializeField] private GameObject UIPrefab;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        CreateIndicator();
        health.onHealthChanged += OnHealthChanged;
    }

    private void LateUpdate()
    {
        _text.transform.position = healthPoint.position;
    }

    private void CreateIndicator()
    {
        var canvas = CanvasReference.instance.Canvas;
        var indicator = Instantiate(UIPrefab, canvas.transform);

        indicator.transform.position = healthPoint.position;
        indicator.transform.localEulerAngles = Vector3.zero;

        _text = indicator.GetComponent<TextMeshProUGUI>();
        _text.text = health.currentHealth.ToString();
    }

    private void OnHealthChanged(int currentValue)
    {
        _text.text = currentValue.ToString();
    }

    private void OnDestroy()
    {
        Destroy(_text.gameObject);
    }
}
