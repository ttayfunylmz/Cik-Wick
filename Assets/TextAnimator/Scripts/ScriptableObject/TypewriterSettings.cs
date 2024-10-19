using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextAnimation {
    [Serializable]
    [CreateAssetMenu(fileName = "Typewriter Animation", menuName = "Animations/Create Typewriter Animation", order = 1)]
    public class TypewriterSettings : ScriptableObject {
        public float baseSpeed = 1;
        public float charactersPerSecond = 10;

        public MovementCurves movement;
        public RotationCurves rotation;
        public ScaleCurves scale;
        public ColorCurves color;
    }
}