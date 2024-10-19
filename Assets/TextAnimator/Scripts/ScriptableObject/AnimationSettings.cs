using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextAnimation {
    [Serializable]
    [CreateAssetMenu(fileName = "Animation", menuName = "Animations/Create Custom Animation", order = 1)]
    public class AnimationSettings : ScriptableObject {
        public string tag;
        public float baseSpeed = 1;
        public float characterDelay = 0.05f;

        public MovementCurves movement;
        public RotationCurves rotation;
        public ScaleCurves scale;
        public ColorCurves color;
    }

    [Serializable]
    public class MovementCurves {
        public bool moveXEnabled = false;
        public FloatCurve moveX;
        public bool moveYEnabled = false;
        public FloatCurve moveY;
        public bool moveZEnabled = false;
        public FloatCurve moveZ;
    }

    [Serializable]
    public class RotationCurves {
        public bool rotateXEnabled = false;
        public AnchoredCurve rotateX;
        public bool rotateYEnabled = false;
        public AnchoredCurve rotateY;
        public bool rotateZEnabled = false;
        public AnchoredVectorCurve rotateZ;
    }

    [Serializable]
    public class ScaleCurves {
        public bool scaleXEnabled = false;
        public AnchoredCurve scaleX;
        public bool scaleYEnabled = false;
        public AnchoredCurve scaleY;
        public bool scaleZEnabled = false;
        public AnchoredCurve scaleZ;
    }

    [Serializable]
    public class ColorCurves {
        public bool colorEnabled = false;
        public ColorCurve color;
    }

    [Serializable]
    public class FloatCurve {
        public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public float multiplierY = 1;
        public float multiplierSpeed = 1;
        public float multiplierDelay = 1;
    }

    [Serializable]
    public class AnchoredVectorCurve : FloatCurve {
        public Vector2 anchor = new Vector2(0.5f, 0.5f);
    }

    [Serializable]
    public class AnchoredCurve : FloatCurve {
        public float anchor = 0.5f;
    }

    [Serializable]
    public class ColorCurve {
        public Gradient colorCurve = new Gradient();
        public float multiplierSpeed = 1;
        public float multiplierDelay = 1;
        public bool useOnlyAlpha = false;
    }
}