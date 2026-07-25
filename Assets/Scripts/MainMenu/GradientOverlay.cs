using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GradientOverlay : MonoBehaviour
{
    [Header("Gradient Settings")]
    [Tooltip("How far across the screen the solid dark area extends (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float solidCoverage = 0.4f;

    [Tooltip("How far across the screen the entire gradient extends (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float totalCoverage = 0.75f;

    [Tooltip("Overall opacity of the darkest part")]
    [Range(0f, 1f)]
    [SerializeField] private float alpha = 0.9f;

    [Tooltip("Fade curve power (1 = linear, higher = sharper cutoff)")]
    [Range(0.5f, 5f)]
    [SerializeField] private float fadeCurve = 1.5f;

    private Image _image;
    private Texture2D _texture;
    private float _lastSolid, _lastTotal, _lastAlpha, _lastCurve;

    private void OnEnable()
    {
        _image = GetComponent<Image>();
        RegenerateGradient();
    }

    private void Update()
    {
        if (solidCoverage != _lastSolid || totalCoverage != _lastTotal ||
            alpha != _lastAlpha || fadeCurve != _lastCurve)
        {
            RegenerateGradient();
        }
    }

    private void RegenerateGradient()
    {
        _lastSolid = solidCoverage;
        _lastTotal = totalCoverage;
        _lastAlpha = alpha;
        _lastCurve = fadeCurve;

        if (_texture == null)
        {
            _texture = new Texture2D(256, 1, TextureFormat.RGBA32, false);
            _texture.wrapMode = TextureWrapMode.Clamp;
            _texture.filterMode = FilterMode.Bilinear;
        }

        for (int x = 0; x < 256; x++)
        {
            float t = (float)x / 255f;
            float a;

            if (totalCoverage <= solidCoverage || totalCoverage <= 0f)
            {
                a = t <= solidCoverage ? alpha : 0f;
            }
            else if (t <= solidCoverage)
            {
                a = alpha;
            }
            else if (t <= totalCoverage)
            {
                float fadeT = (t - solidCoverage) / (totalCoverage - solidCoverage);
                a = alpha * (1f - Mathf.Pow(fadeT, fadeCurve));
            }
            else
            {
                a = 0f;
            }

            _texture.SetPixel(x, 0, new Color(0f, 0f, 0f, a));
        }

        _texture.Apply();

        if (_image != null)
        {
            _image.sprite = Sprite.Create(_texture, new Rect(0, 0, 256, 1),
                new Vector2(0.5f, 0.5f));
            _image.type = Image.Type.Simple;
            _image.color = Color.white;
        }

        // Update RectTransform to match totalCoverage
        var rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMax = new Vector2(totalCoverage, 1f);
        }
    }

    private void OnDestroy()
    {
        if (_texture != null)
        {
            if (Application.isPlaying)
                Destroy(_texture);
            else
                DestroyImmediate(_texture);
        }
    }
}
