using UnityEditor;

namespace INab.ToonDetailer.URP
{ 
    [CustomEditor(typeof(ToonDetailer))]
    public class ToonDetailerFeatureEditor : Editor
    {
        #region Serialized Properties
        private SerializedProperty _DetailerType;
        private SerializedProperty _MaskUse;
        private SerializedProperty _MaskLayer;
        private SerializedProperty _ControlViaVolumes;

        private SerializedProperty _ColorHue;
        private SerializedProperty _UseFade;
        private SerializedProperty _FadeAffectsOnlyContours;
        private SerializedProperty _FadeStart;
        private SerializedProperty _FadeEnd;
        private SerializedProperty _BlackOffset;

        private SerializedProperty _ContoursIntensity;
        private SerializedProperty _ContoursThickness;
        private SerializedProperty _ContoursElevationStrength;
        private SerializedProperty _ContoursElevationSmoothness;
        private SerializedProperty _ContoursDepressionStrength;
        private SerializedProperty _ContoursDepressionSmoothness;

        private SerializedProperty _CavityIntensity;
        private SerializedProperty _CavityRadius;
        private SerializedProperty _CavityStrength;
        private SerializedProperty _CavitySamples;
        #endregion


        private void OnEnable()
        {
            SerializedProperty settings = serializedObject.FindProperty("m_Settings");

            _ColorHue = settings.FindPropertyRelative("_ColorHue");
            _UseFade = settings.FindPropertyRelative("_UseFade");
            _ControlViaVolumes = settings.FindPropertyRelative("_ControlViaVolumes");
            _FadeAffectsOnlyContours = settings.FindPropertyRelative("_FadeAffectsOnlyContours");
            _FadeStart = settings.FindPropertyRelative("_FadeStart");
            _FadeEnd = settings.FindPropertyRelative("_FadeEnd");
            _BlackOffset = settings.FindPropertyRelative("_BlackOffset");

            _DetailerType = settings.FindPropertyRelative("_DetailerType");
            _MaskUse = settings.FindPropertyRelative("_MaskUse");
            _MaskLayer = settings.FindPropertyRelative("_MaskLayer");

            _ContoursIntensity = settings.FindPropertyRelative("_ContoursIntensity");
            _ContoursThickness = settings.FindPropertyRelative("_ContoursThickness");
            _ContoursElevationStrength = settings.FindPropertyRelative("_ContoursElevationStrength");
            _ContoursElevationSmoothness = settings.FindPropertyRelative("_ContoursElevationSmoothness");
            _ContoursDepressionStrength = settings.FindPropertyRelative("_ContoursDepressionStrength");
            _ContoursDepressionSmoothness = settings.FindPropertyRelative("_ContoursDepressionSmoothness");

            _CavityIntensity = settings.FindPropertyRelative("_CavityIntensity");
            _CavityRadius = settings.FindPropertyRelative("_CavityRadius");
            _CavityStrength = settings.FindPropertyRelative("_CavityStrength");
            _CavitySamples = settings.FindPropertyRelative("_CavitySamples");
        }


        private void DrawContours()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Contours", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_ContoursIntensity);
                EditorGUILayout.PropertyField(_ContoursThickness);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_ContoursElevationStrength);
                EditorGUILayout.PropertyField(_ContoursElevationSmoothness);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_ContoursDepressionStrength);
                EditorGUILayout.PropertyField(_ContoursDepressionSmoothness);
            }
        }
        private void DrawCavity()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Cavity", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_CavityIntensity);
                EditorGUILayout.PropertyField(_CavityRadius);
                EditorGUILayout.PropertyField(_CavityStrength);
                EditorGUILayout.PropertyField(_CavitySamples);
            }
        }

        private void DrawAdjustments()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Adjustments", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_ColorHue);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(_UseFade);
                if (_UseFade.boolValue)
                {
                    EditorGUILayout.PropertyField(_FadeAffectsOnlyContours);
                    EditorGUILayout.PropertyField(_FadeStart);
                    EditorGUILayout.PropertyField(_FadeEnd);
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_BlackOffset);
            }
        }

        private void DrawGeneral()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(_DetailerType);
                EditorGUILayout.PropertyField(_MaskUse);
                if(_MaskUse.enumValueIndex != 0) EditorGUILayout.PropertyField(_MaskLayer);
                EditorGUILayout.PropertyField(_ControlViaVolumes);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGeneral();
            DrawAdjustments();

            switch (_DetailerType.enumValueIndex)
            {
                case 0:
                    DrawContours();
                    DrawCavity();
                    break;
                case 1:
                    DrawContours();
                    break;
                case 2:
                    DrawCavity();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}
