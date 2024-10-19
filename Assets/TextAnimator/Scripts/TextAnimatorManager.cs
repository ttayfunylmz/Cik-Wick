using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace TextAnimation {
    public static class TA_Extensions {
        #region Work with tags
        public static string[] allowedTags = new string[] { "wait", "call" };

        /// <summary>
        /// Has tag in list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasTag (List<(string Tag, int Position, string parameters)> list, string tag) {
            return list.Exists(x => x.Tag == tag);
        }

        /// <summary>
        /// Pop last tag from list by tag name
        /// </summary>
        /// <param name="list"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static (string Tag, int Position, string parameters) PopLastTag (List<(string Tag, int Position, string parameters)> list, string tag) {
            var lastTag = list.FindLast(x => x.Tag == tag);
            list.Remove(lastTag);
            return lastTag;
        }

        /// <summary>
        /// Pop last tag from list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static (string Tag, int Position, string parameters) PopLastTag (List<(string Tag, int Position, string parameters)> list) {
            var lastTag = list[list.Count - 1];
            list.Remove(lastTag);
            return lastTag;
        }

        /// <summary>
        /// Calculate position in text without tags
        /// </summary>
        /// <param name="originalInput"></param>
        /// <param name="visibleInput"></param>
        /// <param name="originalPosition"></param>
        /// <returns></returns>
        public static int CalculatePositionWithoutTags (string originalInput, string visibleInput, int originalPosition) {
            int tagCounter = 0;
            int currentTagCounter = 0;
            bool isOpenedTag = false;
            string currentTag = "";
            for (int i = 0; i < originalPosition; i++) {
                if (originalInput[i] == '<' && !isOpenedTag) {
                    isOpenedTag = true;
                }

                if (isOpenedTag) {
                    currentTag += originalInput[i];
                    currentTagCounter++;
                }

                if (originalInput[i] == '>' && isOpenedTag) {
                    isOpenedTag = false;

                    if (!visibleInput.Contains(currentTag)) {
                        tagCounter += currentTagCounter;
                    }
                    currentTagCounter = 0;
                    currentTag = "";
                }
            }
            return originalPosition - tagCounter;
        }

        /// <summary>
        /// Is allowed tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool IsAllowedTag (string tag) {
            return System.Array.Exists(allowedTags, element => element == tag);
        }
        #endregion
    }

    public class TextAnimatorManager : MonoBehaviour {
        #region Local Variables
        float timeline = 0;
        float timeMultiplier = 1;
        AnimationsScriptable animations;
        Dictionary<string, TextAnimatorTags> textAnimatorTags = new Dictionary<string, TextAnimatorTags>();
        #endregion

        #region Singleton
        static TextAnimatorManager instance;

        static TextAnimatorManager Instance {
            get {
                if (instance == null) {
                    if (FindObjectOfType<TextAnimatorManager>() == null) {
                        instance = new GameObject().AddComponent<TextAnimatorManager>();
                        instance.gameObject.name = "TextAnimatorManager";
                    } else {
                        instance = FindObjectOfType<TextAnimatorManager>();
                    }
                    instance.animations = Resources.Load<AnimationsScriptable>("AnimationsList");
                    if (instance.animations == null) {
                        Debug.LogError("No animations found. Please place AnimationsList inside Resources folder");
                    }
                    try {
                        DontDestroyOnLoad(instance.gameObject);
                    } catch {

                    }
                }
                return instance;
            }
        }
        #endregion

        #region Processing Text
        /// <summary>
        /// Process text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ProcessText (string text) {
            foreach (string tag in TA_Extensions.allowedTags) {
                text = Regex.Replace(text, @"<\s*" + tag + @"[^>]*>", string.Empty, RegexOptions.IgnoreCase);
                text = Regex.Replace(text, @"</" + tag + @">", string.Empty, RegexOptions.IgnoreCase);
            }

            if (Instance.animations == null) {
                return text;
            }
            for (int i = 0; i < Instance.animations.animations.Length; i++) {
                var animation = Instance.animations.animations[i];
                text = Regex.Replace(text, @"<\s*" + animation.tag + @"[^>]*>", string.Empty, RegexOptions.IgnoreCase);
                text = Regex.Replace(text, @"</" + animation.tag + @">", string.Empty, RegexOptions.IgnoreCase);
            }
            return text;
        }
        #endregion

        #region Adding to dictionary
        /// <summary>
        /// Add tags of text animator to dictionary
        /// </summary>
        /// <param name="textAnimator"></param>
        /// <param name="textMesh"></param>
        /// <param name="textInfo"></param>
        /// <param name="tags"></param>
        public static void AddTagsToDict (TextAnimator textAnimator, TMP_Text textMesh, TMP_TextInfo textInfo, List<TagContent> tags) {
            string guid = textAnimator.gameObject.GetInstanceID().ToString();

            for (int i = 0; i < tags.Count; i++) {
                if (Instance.GetAnimation(tags[i].tag) == null) {
                    tags.RemoveAt(i);
                }
            }

            if (Instance.textAnimatorTags.ContainsKey(guid)) {
                Instance.textAnimatorTags[guid].tags = tags;
            } else {
                TextAnimatorTags newTags = new TextAnimatorTags();
                newTags.textMesh = textMesh;
                newTags.textInfo = textInfo;
                newTags.tags = tags;
                Instance.textAnimatorTags.Add(guid, newTags);
            }
        }

        /// <summary>
        /// Add typewriter to dictionary
        /// </summary>
        /// <param name="typewriter"></param>
        /// <param name="textMesh"></param>
        /// <param name="textInfo"></param>
        public static void AddTypewriterToDict (Typewriter typewriter, TMP_Text textMesh, TMP_TextInfo textInfo) {
            string guid = typewriter.gameObject.GetInstanceID().ToString();
            if (Instance.textAnimatorTags.ContainsKey(guid)) {
                Instance.textAnimatorTags[guid].typeWriter = typewriter;
            } else {
                TextAnimatorTags newTags = new TextAnimatorTags();
                newTags.textMesh = textMesh;
                newTags.textInfo = textInfo;
                newTags.typeWriter = typewriter;
                Instance.textAnimatorTags.Add(guid, newTags);
            }
        }
        #endregion

        #region Animation Processing
        /// <summary>
        /// Update
        /// </summary>
        private void Update () {
            timeline += Time.deltaTime * timeMultiplier;
            ProcessAnimations();
        }

        /// <summary>
        /// Process typewriter animations without time
        /// </summary>
        public static void ProcessAnimationsWithoutTime () {
            Instance.ProcessAnimations(false);
        }

        /// <summary>
        /// Process animations
        /// </summary>
        /// <param name="processTime"></param>
        void ProcessAnimations (bool processTime = true) {
            foreach (var textAnimator in textAnimatorTags) {
                var value = textAnimator.Value;

                var textMesh = value.textMesh;
                var textInfo = value.textInfo;
                var tags = value.tags;

                textMesh.ForceMeshUpdate();

                bool canUpdateVertexData = false;

                if (textInfo.characterCount > 0 && tags.Count > 0) {
                    canUpdateVertexData = true;
                    for (int i = 0; i < tags.Count; i++) {
                        var tag = tags[i];
                        var animation = GetAnimation(tag.tag);
                        if (animation == null) {
                            continue;
                        }
                        Animate(animation, textInfo, tag.parameters, tag.startPos, tag.endPos);
                    }
                }

                if (value.typeWriter != null && textInfo.characterCount > 0) {
                    canUpdateVertexData = true;
                    value.typeWriter.ProcessAnimations(processTime);
                }

                if(canUpdateVertexData)
                    textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }

        /// <summary>
        /// Animate text
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="textInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        void Animate (AnimationSettings animation, TMP_TextInfo textInfo, string parameters, int startPos, int endPos) {
            float speed = 1;
            float delay = 1;
            GetParametersFromString(parameters, out speed, out delay);

            int characterIndex = 0;
            for (int i = startPos; i < endPos; i++) {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;

                var newVertexPositions = textInfo.meshInfo[materialIndex].vertices;
                var newColors = textInfo.meshInfo[materialIndex].colors32;

                // Movement X
                if (animation.movement.moveXEnabled) {
                    var moveX = animation.movement.moveX;

                    float curveLength = moveX.curve.keys[moveX.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * moveX.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * moveX.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = moveX.curve.Evaluate(currentCurveTime) * moveX.multiplierY;

                    Vector3 minBounds = newVertexPositions[charInfo.vertexIndex];
                    Vector3 maxBounds = newVertexPositions[charInfo.vertexIndex + 2];
                    float xSize = maxBounds.x - minBounds.x;

                    for (int j = 0; j < 4; j++) {
                        newVertexPositions[charInfo.vertexIndex + j].x += currentValue * xSize;
                    }
                }

                // Movement Y
                if (animation.movement.moveYEnabled) {
                    var moveY = animation.movement.moveY;

                    float curveLength = moveY.curve.keys[moveY.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * moveY.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * moveY.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = moveY.curve.Evaluate(currentCurveTime) * moveY.multiplierY;

                    Vector3 minBounds = newVertexPositions[charInfo.vertexIndex];
                    Vector3 maxBounds = newVertexPositions[charInfo.vertexIndex + 2];
                    float ySize = maxBounds.y - minBounds.y;

                    for (int j = 0; j < 4; j++) {
                        newVertexPositions[charInfo.vertexIndex + j].y += currentValue * ySize;
                    }
                }

                // Movement Z
                if (animation.movement.moveZEnabled) {
                    var moveZ = animation.movement.moveZ;

                    float curveLength = moveZ.curve.keys[moveZ.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * moveZ.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * moveZ.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = moveZ.curve.Evaluate(currentCurveTime) * moveZ.multiplierY;

                    for (int j = 0; j < 4; j++) {
                        newVertexPositions[charInfo.vertexIndex + j].z += currentValue;
                    }
                }

                // Rotation X 
                if (animation.rotation.rotateXEnabled) {
                    var rotateX = animation.rotation.rotateX;

                    float curveLength = rotateX.curve.keys[rotateX.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * rotateX.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * rotateX.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = rotateX.curve.Evaluate(currentCurveTime) * rotateX.multiplierY;

                    Quaternion rotation = Quaternion.Euler(currentValue, 0, 0);

                    float anchorNormalized = rotateX.anchor;
                    Vector3 minBounds = newVertexPositions[charInfo.vertexIndex];
                    Vector3 maxBounds = newVertexPositions[charInfo.vertexIndex + 2];

                    Vector3 anchorReal = new Vector3(
                        0,
                        Mathf.Lerp(minBounds.y, maxBounds.y, anchorNormalized),
                        0
                    );

                    for (int j = 0; j < 4; j++) {
                        Vector3 vertex = newVertexPositions[charInfo.vertexIndex + j];
                        vertex -= anchorReal;
                        vertex = rotation * vertex;
                        vertex += anchorReal;
                        newVertexPositions[charInfo.vertexIndex + j] = vertex;
                    }
                }

                // Rotation Y
                if (animation.rotation.rotateYEnabled) {
                    var rotateY = animation.rotation.rotateY;

                    float curveLength = rotateY.curve.keys[rotateY.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * rotateY.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * rotateY.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = rotateY.curve.Evaluate(currentCurveTime) * rotateY.multiplierY;

                    Quaternion rotation = Quaternion.Euler(0, currentValue, 0);

                    float anchorNormalized = rotateY.anchor;
                    Vector3 minBounds = newVertexPositions[charInfo.vertexIndex];
                    Vector3 maxBounds = newVertexPositions[charInfo.vertexIndex + 2];

                    Vector3 anchorReal = new Vector3(
                        Mathf.Lerp(minBounds.x, maxBounds.x, anchorNormalized),
                        0,
                        0
                    );

                    for (int j = 0; j < 4; j++) {
                        Vector3 vertex = newVertexPositions[charInfo.vertexIndex + j];
                        vertex -= anchorReal;
                        vertex = rotation * vertex;
                        vertex += anchorReal;
                        newVertexPositions[charInfo.vertexIndex + j] = vertex;
                    }
                }

                // Rotation Z
                if (animation.rotation.rotateZEnabled) {
                    var rotateZ = animation.rotation.rotateZ;

                    float curveLength = rotateZ.curve.keys[rotateZ.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * rotateZ.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * rotateZ.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = rotateZ.curve.Evaluate(currentCurveTime) * rotateZ.multiplierY;

                    Quaternion rotation = Quaternion.Euler(0, 0, currentValue);

                    Vector2 anchorNormalized = rotateZ.anchor;
                    Vector3 minBounds = newVertexPositions[charInfo.vertexIndex];
                    Vector3 maxBounds = newVertexPositions[charInfo.vertexIndex + 2];

                    Vector3 anchorReal = new Vector3(
                        Mathf.Lerp(minBounds.x, maxBounds.x, anchorNormalized.x),
                        Mathf.Lerp(minBounds.y, maxBounds.y, anchorNormalized.y),
                        0
                    );

                    for (int j = 0; j < 4; j++) {
                        Vector3 vertex = newVertexPositions[charInfo.vertexIndex + j];
                        vertex -= anchorReal;
                        vertex = rotation * vertex;
                        vertex += anchorReal;
                        newVertexPositions[charInfo.vertexIndex + j] = vertex;
                    }
                }

                // Scale X
                if (animation.scale.scaleXEnabled) {
                    var scaleX = animation.scale.scaleX;

                    float curveLength = scaleX.curve.keys[scaleX.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * scaleX.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * scaleX.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = scaleX.curve.Evaluate(currentCurveTime) * scaleX.multiplierY;

                    float anchorNormalized = scaleX.anchor;
                    Vector3 minBounds = newVertexPositions[charInfo.vertexIndex];
                    Vector3 maxBounds = newVertexPositions[charInfo.vertexIndex + 2];

                    float anchorReal = Mathf.Lerp(minBounds.x, maxBounds.x, anchorNormalized);

                    for (int j = 0; j < 4; j++) {
                        Vector3 vertex = newVertexPositions[charInfo.vertexIndex + j];
                        vertex.x -= anchorReal;
                        vertex.x *= currentValue;
                        vertex.x += anchorReal;

                        newVertexPositions[charInfo.vertexIndex + j] = vertex;
                    }
                }

                // Scale Y
                if (animation.scale.scaleYEnabled) {
                    var scaleY = animation.scale.scaleY;

                    float curveLength = scaleY.curve.keys[scaleY.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * scaleY.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * scaleY.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = scaleY.curve.Evaluate(currentCurveTime) * scaleY.multiplierY;

                    float anchorNormalized = scaleY.anchor;
                    Vector3 minBounds = newVertexPositions[charInfo.vertexIndex];
                    Vector3 maxBounds = newVertexPositions[charInfo.vertexIndex + 2];

                    float anchorReal = Mathf.Lerp(minBounds.y, maxBounds.y, anchorNormalized);

                    for (int j = 0; j < 4; j++) {
                        Vector3 vertex = newVertexPositions[charInfo.vertexIndex + j];
                        vertex.y -= anchorReal;
                        vertex.y *= currentValue;
                        vertex.y += anchorReal;

                        newVertexPositions[charInfo.vertexIndex + j] = vertex;
                    }
                }

                // Scale Z
                if (animation.scale.scaleZEnabled) {
                    var scaleZ = animation.scale.scaleZ;

                    float curveLength = scaleZ.curve.keys[scaleZ.curve.length - 1].time;
                    float currentCurveTime = timeline * animation.baseSpeed * scaleZ.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * scaleZ.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % curveLength;

                    float currentValue = scaleZ.curve.Evaluate(currentCurveTime) * scaleZ.multiplierY;

                    float anchorNormalized = scaleZ.anchor;
                    Vector3 minBounds = newVertexPositions[charInfo.vertexIndex];
                    Vector3 maxBounds = newVertexPositions[charInfo.vertexIndex + 2];

                    float anchorReal = Mathf.Lerp(minBounds.z, maxBounds.z, anchorNormalized);

                    for (int j = 0; j < 4; j++) {
                        Vector3 vertex = newVertexPositions[charInfo.vertexIndex + j];
                        vertex.z -= anchorReal;
                        vertex.z *= currentValue;
                        vertex.z += anchorReal;

                        newVertexPositions[charInfo.vertexIndex + j] = vertex;
                    }
                }

                // Color
                if (animation.color.colorEnabled) {
                    var color = animation.color.color;

                    float currentCurveTime = timeline * animation.baseSpeed * color.multiplierSpeed * speed;
                    currentCurveTime += characterIndex * animation.characterDelay * color.multiplierDelay * delay;
                    currentCurveTime = currentCurveTime % 1;

                    var currentColor = color.colorCurve.Evaluate(currentCurveTime);

                    for (int j = 0; j < 4; j++) {
                        if (color.useOnlyAlpha) {
                            newColors[charInfo.vertexIndex + j] = new Color32(
                                newColors[charInfo.vertexIndex + j].r,
                                newColors[charInfo.vertexIndex + j].g,
                                newColors[charInfo.vertexIndex + j].b,
                                (byte) (currentColor.a * 255)
                            );
                        } else {
                            newColors[charInfo.vertexIndex + j] = currentColor;
                        }
                    }
                }

                characterIndex++;
            }
        }
        #endregion

        #region Extensions
        /// <summary>
        /// Get parameters from string
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="speed"></param>
        /// <param name="delay"></param>
        void GetParametersFromString (string parameters, out float speed, out float delay) {
            speed = 1;
            delay = 1;
            var speedRegex = new Regex(@"s\s*=\s*(-?[0-9]*\.?[0-9]+)");
            var delayRegex = new Regex(@"d\s*=\s*(-?[0-9]*\.?[0-9]+)");

            var speedMatch = speedRegex.Match(parameters);
            var delayMatch = delayRegex.Match(parameters);

            if (speedMatch.Success) {
                speed = float.Parse(speedMatch.Groups[1].Value);
            }

            if (delayMatch.Success) {
                delay = float.Parse(delayMatch.Groups[1].Value);
            }
        }

        /// <summary>
        /// Get animation by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        AnimationSettings GetAnimation (string tag) {
            for (int i = 0; i < animations.animations.Length; i++) {
                if (animations.animations[i].tag == tag) {
                    return animations.animations[i];
                }
            }
            return null;
        }
        #endregion
    }

    public struct TagContent {
        public string tag;
        public string parameters;
        public int startPos;
        public int endPos;
    }

    class TextAnimatorTags {
        public TMP_Text textMesh;
        public TMP_TextInfo textInfo;

        public List<TagContent> tags = new List<TagContent>();
        public Typewriter typeWriter;
    }
}