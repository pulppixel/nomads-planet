using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    public class ColorButton : MonoBehaviour
    {
        public Outline outline;
        public Button button;

        [SerializeField]
        public Image colorOneImage;
        
        [SerializeField]
        public Image colorTwoImage;
        
        [SerializeField]
        public Image colorThreeImageOne;
        
        [SerializeField]
        public Image colorThreeImageTwo;

        public void SetColors(List<Color> colors)
        {
            colorOneImage.color = colors[0];
            if (colors.Count == 2)
            {
                colorTwoImage.color = colors[1];
                colorTwoImage.gameObject.SetActive(true);
            }
            else if (colors.Count == 3)
            {
                colorThreeImageOne.color = colors[1];
                colorThreeImageOne.gameObject.SetActive(true);
                colorThreeImageTwo.color = colors[2];
                colorThreeImageTwo.gameObject.SetActive(true);
            }
        }
    }
}
