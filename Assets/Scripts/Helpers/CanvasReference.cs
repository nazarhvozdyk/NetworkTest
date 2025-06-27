using UnityEngine;

public class CanvasReference : MonoBehaviour
{
    public static CanvasReference instance { get; private set; }
    [SerializeField] private Canvas canvas;
    public Canvas Canvas { get => canvas; }

    private void Awake()
    {
        instance = this;

        if (canvas == null)
            canvas = GetComponent<Canvas>();
    }
}
