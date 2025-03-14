using UnityEngine;

public class ShaderTestManager : MonoBehaviour
{
    public Color[] oldColors;
    public Color[] newColors;

    [SerializeField] private Shader shader;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private SpriteRenderer pigRenderer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Texture2D texture = TextureUtils.CreateColorTexture(oldColors, newColors);
            texture.filterMode = FilterMode.Point;
            SetColorShaderInfo(texture);
            renderer.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    public void SetColorShaderInfo(Texture2D texture)
    {
        Material mat = new Material(shader);
        texture.filterMode = FilterMode.Point;
        mat.SetTexture("_ColorTex", texture);
        
        pigRenderer.material = mat;
    }
}
