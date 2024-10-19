using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TextAnimation {
    public class TestCharacterEvent : MonoBehaviour {
        public Image background;

        public Color[] colors;

        public void ChangeColor0 () {
            background.color = colors[0];
        }

        public void ChangeColor1 () {
            background.color = colors[1];
        }

        public void ChangeColor2 () {
            background.color = colors[2];
        }
    }
}