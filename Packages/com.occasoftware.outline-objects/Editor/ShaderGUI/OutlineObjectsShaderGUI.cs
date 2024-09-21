using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using System;

namespace OccaSoftware.OutlineObjects.Editor
{
    public class OutlineObjectsShaderGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor e, MaterialProperty[] properties)
        {
            MaterialProperty _OutlineColor = FindProperty("_OutlineColor", properties);
            MaterialProperty _OutlineThickness = FindProperty("_OutlineThickness", properties);
            MaterialProperty _CompleteFalloffDistance = FindProperty("_CompleteFalloffDistance", properties);
            MaterialProperty _NoiseTexture = FindProperty("_NoiseTexture", properties);
            MaterialProperty _NoiseFrequency = FindProperty("_NoiseFrequency", properties);
            MaterialProperty _NoiseFramerate = FindProperty("_NoiseFramerate", properties);

            MaterialProperty _USE_VERTEX_COLOR_ENABLED = FindProperty("_USE_VERTEX_COLOR_ENABLED", properties);
            MaterialProperty _ATTENUATE_BY_DISTANCE_ENABLED = FindProperty("_ATTENUATE_BY_DISTANCE_ENABLED", properties);
            MaterialProperty _RANDOM_OFFSETS_ENABLED = FindProperty("_RANDOM_OFFSETS_ENABLED", properties);
            MaterialProperty _USE_SMOOTHED_NORMALS_ENABLED = FindProperty("_USE_SMOOTHED_NORMALS_ENABLED", properties);

            MaterialProperty _Surface = FindProperty("_Surface", properties);

            Material material = e.target as Material;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Basic Configuration", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();
            _Surface.floatValue = e.PopupShaderProperty(_Surface, new GUIContent("Surface"), Enum.GetNames(typeof(SurfaceType)));
            if (EditorGUI.EndChangeCheck())
            {
                material.SetOverrideTag("RenderType", ""); // clear override tag
                bool zwrite = false;
                SurfaceType surfaceType = (SurfaceType)_Surface.floatValue;
                if (surfaceType == SurfaceType.Opaque)
                {
                    material.renderQueue = (int)RenderQueue.Geometry;
                    material.SetOverrideTag("RenderType", "Opaque");
                    material.SetOverrideTag("Queue", "Opaque");
                    SetSrcDestProperties(material, BlendMode.One, BlendMode.Zero);
                    zwrite = true;
                    material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                }
                else
                {
                    material.renderQueue = (int)RenderQueue.Transparent;
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetOverrideTag("Queue", "Transparent");
                    SetSrcDestProperties(material, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
                    zwrite = false;
                    material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                }
                SetupDepthWriting(material, zwrite);
            }

            e.ShaderProperty(_OutlineColor, new GUIContent("Outline Color"));
            DrawFloatWithMinValue(new GUIContent("Outline Thickness"), _OutlineThickness, 0);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Vertex Color Configuration", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            e.ShaderProperty(
                _USE_VERTEX_COLOR_ENABLED,
                new GUIContent(
                    "Use Vertex Color",
                    "Uses the Vertex Color (R Channel) to reduce the size of the outline. If the Vertex Color (R) is 0, the outline will have a width of 0."
                )
            );
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Distance-based Attenuation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            e.ShaderProperty(_ATTENUATE_BY_DISTANCE_ENABLED, new GUIContent("Attenuate by Distance"));
            if (_ATTENUATE_BY_DISTANCE_ENABLED.intValue == 1)
            {
                e.ShaderProperty(_CompleteFalloffDistance, new GUIContent("Complete Falloff Distance"));
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Noise", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            DrawTextureProperty(_NoiseTexture, new GUIContent("Noise Texture", "Distance-based noise field"));

            if (_NoiseTexture.textureValue != null)
            {
                EditorGUI.indentLevel++;
                e.ShaderProperty(_NoiseFrequency, new GUIContent("Noise Frequency"));
                e.ShaderProperty(_NoiseFramerate, new GUIContent("Noise Framerate"));
                e.ShaderProperty(_RANDOM_OFFSETS_ENABLED, new GUIContent("Jitter"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Normals", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            e.ShaderProperty(
                _USE_SMOOTHED_NORMALS_ENABLED,
                new GUIContent(
                    "Use Smoothed Normals",
                    "Use the Generate Smooth Normals tool to bake smoothed normals to the UV3 channel of your mesh."
                )
            );
            EditorGUI.indentLevel--;

            void DrawTextureProperty(MaterialProperty p, GUIContent c)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = p.hasMixedValue;
                Texture2D t = (Texture2D)
                    EditorGUILayout.ObjectField(c, p.textureValue, typeof(Texture2D), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                if (EditorGUI.EndChangeCheck())
                {
                    p.textureValue = t;
                }
                EditorGUI.showMixedValue = false;
            }

            void DrawFloatWithMinValue(GUIContent content, MaterialProperty a, float min)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = a.hasMixedValue;
                float b = EditorGUILayout.FloatField(content, a.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    a.floatValue = Mathf.Max(min, b);
                }

                EditorGUI.showMixedValue = false;
            }
        }

        private static class ShaderParams
        {
            public static int outlineColor = Shader.PropertyToID("_OutlineColor");
            public static int outlineThickness = Shader.PropertyToID("_OutlineThickness");
            public static int completeFalloffDistance = Shader.PropertyToID("_CompleteFalloffDistance");
            public static int noiseTexture = Shader.PropertyToID("_NoiseTexture");
            public static int noiseFrequency = Shader.PropertyToID("_NoiseFrequency");
            public static int noiseFramerate = Shader.PropertyToID("_NoiseFramerate");

            public static string useVertexColors = "USE_VERTEX_COLOR_ENABLED";
            public static string attenuateByDistance = "ATTENUATE_BY_DISTANCE_ENABLED";
            public static string randomOffset = "RANDOM_OFFSETS_ENABLED";
        }

        private static void SetSrcDestProperties(Material t, BlendMode src, BlendMode dst)
        {
            t.SetFloat("_SrcBlend", (float)src);
            t.SetFloat("_DstBlend", (float)dst);
        }

        private static void SetupDepthWriting(Material t, bool depthWrite)
        {
            t.SetFloat("_ZWrite", depthWrite ? 1.0f : 0.0f);
            t.SetShaderPassEnabled("DepthOnly", depthWrite);
            t.SetShaderPassEnabled("DepthNormalsOnly", depthWrite);
        }

        public enum SurfaceType
        {
            Opaque,
            Transparent
        }
    }
}
