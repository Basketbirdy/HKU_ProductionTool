using UnityEngine;

public class ShaderTestManager : MonoBehaviour
{
    [SerializeField] private int select;
    public Color[] oldColors;
    public Color[] newColors;

    [SerializeField] private Shader shader;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private SpriteRenderer pigRenderer;

    [SerializeField] private SpriteRenderer finalPigRenderer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Texture2D baseTexture = TextureUtils.CreateColorTexture1D(oldColors);
            Texture2D outputTexture = TextureUtils.CreateColorTexture1D(newColors);
            baseTexture.filterMode = FilterMode.Point;
            outputTexture.filterMode = FilterMode.Point;
            SetColorShaderInfo(baseTexture, outputTexture);
            //renderer.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            finalPigRenderer.material.SetFloat("_LookupOffset", 1);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            finalPigRenderer.material.SetFloat("_LookupOffset", 0);
        }
    }

    public void SetColorShaderInfo(Texture2D baseTexture, Texture2D outputTexture)
    {
        Material mat = new Material(shader);
        baseTexture.filterMode = FilterMode.Point;
        outputTexture.filterMode = FilterMode.Point;
        mat.SetTexture("_BaseColors", baseTexture);
        mat.SetTexture("_OutputColors", outputTexture);
        
        finalPigRenderer.material = mat;
    }
}
