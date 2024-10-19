#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TextAnimation {
    [CustomEditor(typeof(Typewriter)), CanEditMultipleObjects]
    public class TypewriterDrawer : Editor {
        /// <summary>
        /// On inspector GUI
        /// </summary>
        public override void OnInspectorGUI () {
            serializedObject.Update();

            Typewriter typewriter = (Typewriter) target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("animation"));
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speedModifier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("charactersModifier"));
            GUILayout.Space(20);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("runFromStart"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("runFromEnable"));

            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = Application.isPlaying && !typewriter.isStarted;
            if (GUILayout.Button("Start", GUILayout.Width(100), GUILayout.Height(30))) {
                typewriter.StartTypewriter();
            }
            GUI.enabled = Application.isPlaying && typewriter.isStarted;
            if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.Height(30))) {
                typewriter.StopTypewriter();
            }
            GUI.enabled = Application.isPlaying && !typewriter.isStarted;
            if (GUILayout.Button("Hide", GUILayout.Width(100), GUILayout.Height(30))) {
                typewriter.Hide();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = Application.isPlaying && typewriter.isPaused && typewriter.isStarted;
            if (GUILayout.Button("Resume", GUILayout.Width(100), GUILayout.Height(30))) {
                typewriter.Resume();
            }
            GUI.enabled = Application.isPlaying && !typewriter.isPaused && typewriter.isStarted;
            if (GUILayout.Button("Pause", GUILayout.Width(100), GUILayout.Height(30))) {
                typewriter.Pause();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUI.enabled = true;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skippingSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skippingChars"));
            GUILayout.Space(10);
            GUI.enabled = Application.isPlaying && typewriter.isStarted && !typewriter.isSkipping;
            if (GUILayout.Button("Skip", GUILayout.Width(100), GUILayout.Height(30))) {
                typewriter.Skip();
            }
            GUI.enabled = true;

            GUILayout.Space(20);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onEnd"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif