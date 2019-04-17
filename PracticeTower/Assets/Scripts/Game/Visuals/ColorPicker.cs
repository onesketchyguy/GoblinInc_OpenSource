using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
namespace LowEngine
{
    public class ColorPicker : MonoBehaviour
    {
        public Texture2D colorsPallete;

        public int numRows;
        public int numColumns;

        public Vector2 textureSize;

        private List<Color32> allColors = new List<Color32>() { };
        private int totalNumColors;
        public Color32 SelectedColor { get; private set; }
        private int currentIndex;

        private float ColorStrength = 1f;

        public Slider StrengthSlider;

        public Image selectedColorImage;

        public Sprite buttonImage;

        public GameObject ColorPickingParent;

        public Transform contentArea;

        private void OnEnable()
        {
            if (allColors == null || allColors.Count < 1)
            {
                allColors.Add(Color.white);
                allColors.Add(Color.gray);
                allColors.Add(Color.black);

                CreatColorArray();

                for (int i = 0; i < allColors.Count; i++)
                {
                    Color32 color = allColors[i];
                    SpawnButton(color, i);
                }

                if (StrengthSlider != null)
                    StrengthSlider.onValueChanged.AddListener(delegate { ChangeColor(currentIndex); });
            }

            SelectedColor = allColors[0];

            ChangeColor(0);
        }

        private void SpawnButton(Color color, int no)
        {
            GameObject stored = new GameObject(color.ToString());

            GameObject go = Instantiate(stored, contentArea);

            Destroy(stored);

            Image img = go.AddComponent<Image>();
            img.sprite = buttonImage;
            img.color = color;

            Button button = go.AddComponent<Button>();
            button.targetGraphic = img;
            button.onClick.AddListener(() => ChangeColor(no));
        }

        private Color32 SamplePaletteTexture(float xOffset, float yOffset)
        {
            Color32 color = colorsPallete.GetPixel((int)(xOffset * textureSize.x) - 5, (int)(yOffset * textureSize.y) - 5);

            return color;
        }

        private void CreatColorArray()
        {
            totalNumColors = numRows * numColumns;

            int totalCount = 0;
            for (int row = 0; row < numRows; row++)
            {
                for (int column = 0; column < numColumns; column++)
                {
                    float sptX = (row + 1f) / numRows;
                    float sptY = (column + 1f) / numColumns;

                    Color n_Color = SamplePaletteTexture(sptX, sptY);

                    if (allColors.Contains(n_Color) == false)
                        allColors.Add(n_Color);

                    totalCount++;
                }
            }
        }

        public void ChangeColor(int num)
        {
            if (StrengthSlider != null)
            {
                ColorStrength = StrengthSlider.value;
            }

            currentIndex = num;
            if (currentIndex < 0) currentIndex = 0;
            if (currentIndex > totalNumColors - 1) currentIndex = totalNumColors - 1;
            Color newColor = Color.Lerp(Color.white, allColors[currentIndex], ColorStrength);

            SelectedColor = newColor;
            selectedColorImage.color = SelectedColor;
            ColorPickingParent.SetActive(false);
        }

        public void ToggleColorBox()
        {
            ColorPickingParent.SetActive(!ColorPickingParent.activeSelf);
        }
    }
}