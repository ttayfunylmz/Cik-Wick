#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextAnimation {
    [CustomEditor(typeof(TypewriterSettings))]
    public class TypewriterSettingsDrawer : Editor {
        public bool show_move = false;
        public bool showX_move = false;
        public bool showY_move = false;
        public bool showZ_move = false;

        public bool show_rot = false;
        public bool showX_rot = false;
        public bool showY_rot = false;
        public bool showZ_rot = false;

        public bool show_scale = false;
        public bool showX_scale = false;
        public bool showY_scale = false;
        public bool showZ_scale = false;

        public bool show_color = false;
        public bool showX_color = false;

        /// <summary>
        /// Override the inspector GUI to show the custom inspector for the TypewriterSettings
        /// </summary>
        public override void OnInspectorGUI () {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("charactersPerSecond"));

            GUILayout.Space(10);
            show_move = EditorGUILayout.Foldout(show_move, "Movement", true);
            if (show_move) {
                var movement = serializedObject.FindProperty("movement");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = movement.FindPropertyRelative("moveXEnabled").boolValue;
                showX_move = EditorGUILayout.Foldout(showX_move, "Movement X", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(movement.FindPropertyRelative("moveXEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = movement.FindPropertyRelative("moveXEnabled").boolValue;
                if (showX_move) {
                    var move = movement.FindPropertyRelative("moveX");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = movement.FindPropertyRelative("moveYEnabled").boolValue;
                showY_move = EditorGUILayout.Foldout(showY_move, "Movement Y", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(movement.FindPropertyRelative("moveYEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = movement.FindPropertyRelative("moveYEnabled").boolValue;
                if (showY_move) {
                    var move = movement.FindPropertyRelative("moveY");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = movement.FindPropertyRelative("moveZEnabled").boolValue;
                showZ_move = EditorGUILayout.Foldout(showZ_move, "Movement Z", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(movement.FindPropertyRelative("moveZEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = movement.FindPropertyRelative("moveZEnabled").boolValue;
                if (showZ_move) {
                    var move = movement.FindPropertyRelative("moveZ");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(move.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;
            }

            show_rot = EditorGUILayout.Foldout(show_rot, "Rotation", true);
            if (show_rot) {
                var rotation = serializedObject.FindProperty("rotation");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = rotation.FindPropertyRelative("rotateXEnabled").boolValue;
                showX_rot = EditorGUILayout.Foldout(showX_rot, "Rotation X", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(rotation.FindPropertyRelative("rotateXEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = rotation.FindPropertyRelative("rotateXEnabled").boolValue;
                if (showX_rot) {
                    var rot = rotation.FindPropertyRelative("rotateX");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("anchor"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = rotation.FindPropertyRelative("rotateYEnabled").boolValue;
                showY_rot = EditorGUILayout.Foldout(showY_rot, "Rotation Y", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(rotation.FindPropertyRelative("rotateYEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = rotation.FindPropertyRelative("rotateYEnabled").boolValue;
                if (showY_rot) {
                    var rot = rotation.FindPropertyRelative("rotateY");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("anchor"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = rotation.FindPropertyRelative("rotateZEnabled").boolValue;
                showZ_rot = EditorGUILayout.Foldout(showZ_rot, "Rotation Z", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(rotation.FindPropertyRelative("rotateZEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = rotation.FindPropertyRelative("rotateZEnabled").boolValue;
                if (showZ_rot) {
                    var rot = rotation.FindPropertyRelative("rotateZ");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("anchor"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(rot.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;
            }

            show_scale = EditorGUILayout.Foldout(show_scale, "Scale", true);
            if (show_scale) {
                var scaleParent = serializedObject.FindProperty("scale");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = scaleParent.FindPropertyRelative("scaleXEnabled").boolValue;
                showX_scale = EditorGUILayout.Foldout(showX_scale, "Scale X", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(scaleParent.FindPropertyRelative("scaleXEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = scaleParent.FindPropertyRelative("scaleXEnabled").boolValue;
                if (showX_scale) {
                    var scale = scaleParent.FindPropertyRelative("scaleX");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("anchor"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = scaleParent.FindPropertyRelative("scaleYEnabled").boolValue;
                showY_scale = EditorGUILayout.Foldout(showY_scale, "Scale Y", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(scaleParent.FindPropertyRelative("scaleYEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = scaleParent.FindPropertyRelative("scaleYEnabled").boolValue;
                if (showY_scale) {
                    var scale = scaleParent.FindPropertyRelative("scaleY");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("anchor"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = scaleParent.FindPropertyRelative("scaleZEnabled").boolValue;
                showZ_scale = EditorGUILayout.Foldout(showZ_scale, "Scale Z", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(scaleParent.FindPropertyRelative("scaleZEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = scaleParent.FindPropertyRelative("scaleZEnabled").boolValue;
                if (showZ_scale) {
                    var scale = scaleParent.FindPropertyRelative("scaleZ");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("curve"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("anchor"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("multiplierY"));
                    EditorGUILayout.PropertyField(scale.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;
            }

            show_color = EditorGUILayout.Foldout(show_color, "Color", true);
            if (show_color) {
                var colorParent = serializedObject.FindProperty("color");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.enabled = colorParent.FindPropertyRelative("colorEnabled").boolValue;
                showX_color = EditorGUILayout.Foldout(showX_color, "Color Settings", true);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(colorParent.FindPropertyRelative("colorEnabled"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = colorParent.FindPropertyRelative("colorEnabled").boolValue;
                if (showX_color) {
                    var color = colorParent.FindPropertyRelative("color");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(color.FindPropertyRelative("colorCurve"));
                    EditorGUILayout.PropertyField(color.FindPropertyRelative("multiplierSpeed"));
                    EditorGUILayout.PropertyField(color.FindPropertyRelative("useOnlyAlpha"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUI.enabled = true;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif