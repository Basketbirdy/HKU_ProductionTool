using UnityEngine;
using UnityEngine.UIElements;

namespace TextureTesting
{
    public class SpriteSheetTest : MonoBehaviour
    {
        [SerializeField] private Texture2D[] textures;
        [SerializeField] private UIDocument document;
        private VisualElement root;
        private VisualElement spriteSheetElement;

        private void Awake()
        {
            root = document.rootVisualElement;
            spriteSheetElement = root.Q<VisualElement>("Element_SpriteSheet");
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.P)) { OnCreateSpriteSheet(); }
        }

        public void OnCreateSpriteSheet()
        {
            Texture2D spriteSheet = TextureUtils.CreateTextureSheet(textures);
            if(spriteSheet == null) { return; }
            if(root == null) { Debug.Log("Ui root is null"); }

            if(spriteSheetElement == null) { Debug.Log("Sprite sheet element is null"); }
            spriteSheetElement.style.backgroundImage = spriteSheet;
        }
    }
}
