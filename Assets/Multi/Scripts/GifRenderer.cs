using UnityEngine;
using UnityEngine.UI;

public class GifRenderer : MonoBehaviour
{

    [SerializeField] private Sprite[] _gifFrames;
    [SerializeField] private float fps = 10.0f;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        int index = (int)(Time.time * fps);
        index = index % _gifFrames.Length;
        image.sprite = _gifFrames[index];
    }
}
