using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Events;

namespace TextAnimation {
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteInEditMode]
    public class Typewriter : MonoBehaviour {
        #region Public Variables and Events
        public TypewriterSettings animation;
        public UnityEvent onEnd;
        public event Action OnTypewriteEnd;
        public Action<int> onCharacterAppear;
        #endregion

        #region Private Variables
        [SerializeField] bool runFromStart = false;
        [SerializeField] bool runFromEnable = false;

        [SerializeField] float speedModifier = 1;
        [SerializeField] float charactersModifier = 1;

        [SerializeField] float skippingSpeed = 4;
        [SerializeField] int skippingChars = 4;


        List<TagContent> tags = new List<TagContent>();
        List<CharacterToAnimate> charactersToAnimate = new List<CharacterToAnimate>();

        TMP_Text textMesh;
        TMP_TextInfo textInfo;

        public bool isHidden = false;
        public bool isStarted = false;
        public bool isPaused = false;
        public bool isSkipping = false;

        Coroutine writerRoutine;
        int prevTextLength = 0;
        #endregion

        #region Initialization
        private void Start () {
            if (!Application.isPlaying)
                return;
            Hide();
            if (runFromStart)
                StartTypewriter();
        }

        private void OnEnable () {
            textMesh = GetComponent<TMP_Text>();
            textMesh.textPreprocessor = new CustomTextPreprocessor();
            textMesh.OnPreRenderText += TextMesh_OnPreRenderText;
            textInfo = textMesh.textInfo;
            TextAnimatorManager.AddTypewriterToDict(this, textMesh, textInfo);
            if (!Application.isPlaying)
                return;
            Hide();
            if (runFromEnable)
                StartTypewriter();
        }

        private void OnDisable () {
            textMesh.OnPreRenderText -= TextMesh_OnPreRenderText;
        }

        private void TextMesh_OnPreRenderText (TMP_TextInfo obj) {
            ProcessNestedTags(obj.textComponent.text);
        }
        #endregion

        #region Main Logic
        /// <summary>
        /// Process nested tags and store them in a list
        /// </summary>
        /// <param name="text"></param>
        void ProcessNestedTags (string text) {
            string visibleString = "";
            for (int i = 0; i < textInfo.characterCount; i++) {
                var charInfo = textInfo.characterInfo[i];
                visibleString += charInfo.character;
            }

            tags = new List<TagContent>();

            var tagRegex = new Regex(@"<(/)?(\w+)\s*([^>]*)>([^<]*)");
            var matches = tagRegex.Matches(text);

            var tagList = new List<(string Tag, int Position, string parameters)>();

            foreach (Match match in matches) {
                string parameters = match.Groups[3].Value;
                string tag = match.Groups[2].Value;
                if (!TA_Extensions.IsAllowedTag(tag)) {
                    continue;
                }
                bool isClosingTag = match.Groups[1].Value == "/";
                if (!isClosingTag) {
                    tagList.Add((tag, match.Groups[4].Index, parameters));
                } else {
                    if (tagList.Count > 0 && TA_Extensions.HasTag(tagList, tag)) {
                        var openingTag = TA_Extensions.PopLastTag(tagList, tag);
                        tags.Add(new TagContent {
                            tag = tag,
                            parameters = openingTag.parameters,
                            startPos = TA_Extensions.CalculatePositionWithoutTags(text, visibleString, openingTag.Position),
                            endPos = TA_Extensions.CalculatePositionWithoutTags(text, visibleString, match.Groups[0].Index)
                        });
                    }
                }
            }

            while (tagList.Count > 0) {
                var remainingTag = TA_Extensions.PopLastTag(tagList);
                tags.Add(new TagContent {
                    tag = remainingTag.Tag,
                    parameters = remainingTag.parameters,
                    startPos = TA_Extensions.CalculatePositionWithoutTags(text, visibleString, remainingTag.Position),
                    endPos = TA_Extensions.CalculatePositionWithoutTags(text, visibleString, text.Length)
                });
            }

            if (prevTextLength != text.Length) {
                if (!isStarted && !isHidden)
                    textMesh.maxVisibleCharacters = int.MaxValue;
                prevTextLength = text.Length;
            }
        }

        /// <summary>
        /// Process animations with and without time
        /// </summary>
        /// <param name="processTime"></param>
        public void ProcessAnimations (bool processTime = true) {
            for (int i = 0; i < charactersToAnimate.Count; i++) {
                var character = charactersToAnimate[i];
                var charInfo = textInfo.characterInfo[character.characterIndex];
                if (!charInfo.isVisible)
                    continue;

                var timeline = character.time;

                int materialIndex = charInfo.materialReferenceIndex;

                var newVertexPositions = textInfo.meshInfo[materialIndex].vertices;
                var newColors = textInfo.meshInfo[materialIndex].colors32;

                // Movement X
                if (animation.movement.moveXEnabled) {
                    var moveX = animation.movement.moveX;

                    float currentCurveTime = Mathf.Clamp01(timeline * moveX.multiplierSpeed);

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

                    float currentCurveTime = Mathf.Clamp01(timeline * moveY.multiplierSpeed);

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

                    float currentCurveTime = Mathf.Clamp01(timeline * moveZ.multiplierSpeed);

                    float currentValue = moveZ.curve.Evaluate(currentCurveTime) * moveZ.multiplierY;

                    for (int j = 0; j < 4; j++) {
                        newVertexPositions[charInfo.vertexIndex + j].z += currentValue;
                    }
                }

                // Rotation X 
                if (animation.rotation.rotateXEnabled) {
                    var rotateX = animation.rotation.rotateX;

                    float currentCurveTime = Mathf.Clamp01(timeline * rotateX.multiplierSpeed);

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

                    float currentCurveTime = Mathf.Clamp01(timeline * rotateY.multiplierSpeed);

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

                    float currentCurveTime = Mathf.Clamp01(timeline * rotateZ.multiplierSpeed);

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

                    float currentCurveTime = Mathf.Clamp01(timeline * scaleX.multiplierSpeed);

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

                    float currentCurveTime = Mathf.Clamp01(timeline * scaleY.multiplierSpeed);

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

                    float currentCurveTime = Mathf.Clamp01(timeline * scaleZ.multiplierSpeed);

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

                    float currentCurveTime = Mathf.Clamp01(timeline * color.multiplierSpeed);

                    var currentColor = color.colorCurve.Evaluate(currentCurveTime);

                    for (int j = 0; j < 4; j++) {
                        if (color.useOnlyAlpha) {
                            newColors[charInfo.vertexIndex + j] = new Color32(
                                newColors[charInfo.vertexIndex + j].r,
                                newColors[charInfo.vertexIndex + j].g,
                                newColors[charInfo.vertexIndex + j].b,
                                (byte)(currentColor.a * 255)
                            );
                        } else {
                            newColors[charInfo.vertexIndex + j] = currentColor;
                        }
                    }
                }

                if (processTime) {
                    charactersToAnimate[i].time += Time.deltaTime * animation.baseSpeed * speedModifier * (isSkipping ? skippingSpeed : 1);
                }
            }
        }

        /// <summary>
        /// Show text with animations by character
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowText () {
            if (!Application.isPlaying)
                yield break;
            isStarted = true;
            isPaused = false;

            charactersToAnimate.Clear();
            
            while(textInfo.characterCount == 0) {
                yield return null;
            }

            int counter = 0;
            while (counter < textInfo.characterCount) {
                if (isPaused) {
                    yield return null;
                    continue;
                }

                for (int i = 0; i < (isSkipping ? skippingChars : 1); i++) {
                    if (counter >= textInfo.characterCount)
                        continue;

                    charactersToAnimate.Add(new CharacterToAnimate(counter));
                    textMesh.maxVisibleCharacters = counter + 1;
                    TextAnimatorManager.ProcessAnimationsWithoutTime();

                    onCharacterAppear?.Invoke(counter);

                    counter += 1;

                    foreach (var tag in tags) {
                        if (tag.startPos == counter) {
                            switch (tag.tag) {
                                case "wait":
                                    if (!isSkipping) {
                                        yield return new WaitForSecondsRealtime(GetWaitTimeFromString(tag.parameters));
                                    }
                                    break;
                                case "call":
                                    this.SendMessage(GetActionNameFromString(tag.parameters), SendMessageOptions.DontRequireReceiver);
                                    break;
                            }
                        }
                    }
                }

                float waitTime = (animation.charactersPerSecond * charactersModifier * (isSkipping ? skippingSpeed : 1));
                if (waitTime > 0) {
                    yield return new WaitForSecondsRealtime(1 / waitTime); 
                }
            }

            isSkipping = false;
            isStarted = false;
            onEnd?.Invoke();
            OnTypewriteEnd?.Invoke();
        }

        /// <summary>
        /// Get wait time from tag parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        float GetWaitTimeFromString (string parameters) {
            float wait = 1;
            var regex = new Regex(@"\s*=\s*(-?[0-9]*\.?[0-9]+)");

            var match = regex.Match(parameters);

            if (match.Success) {
                wait = float.Parse(match.Groups[1].Value);
            }

            return wait;
        }

        /// <summary>
        /// Get action name from tag parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string GetActionNameFromString (string parameters) {
            string actionName = "";
            var regex = new Regex(@"=\s*([^\s]+)");

            var match = regex.Match(parameters);

            if (match.Success) {
                actionName = match.Groups[1].Value;
            }

            return actionName;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start typewriter effect
        /// </summary>
        public void StartTypewriter () {
            isHidden = false;
            if (writerRoutine != null)
                StopCoroutine(writerRoutine);
            writerRoutine = StartCoroutine(ShowText());
        }

        /// <summary>
        /// Stop typewriter effect
        /// </summary>
        public void StopTypewriter () {
            isStarted = false;
            StopCoroutine(writerRoutine);
            textMesh.maxVisibleCharacters = int.MaxValue;
            charactersToAnimate.Clear();
        }

        /// <summary>
        /// Pause typewriter effect
        /// </summary>
        public void Pause () {
            isPaused = true;
        }

        /// <summary>
        /// Resume typewriter effect
        /// </summary>
        public void Resume () {
            isPaused = false;
        }

        /// <summary>
        /// Skip typewriter effect
        /// </summary>
        public void Skip () {
            isSkipping = true;
        }

        /// <summary>
        /// Stop skipping typewriter effect
        /// </summary>
        public void StopSkipping () {
            isSkipping = false;
        }

        public void Hide () {
            isHidden = true;
            textMesh.maxVisibleCharacters = 0;
        }

        public void Show () {
            isHidden = false;
            textMesh.maxVisibleCharacters = int.MaxValue;
        }
        #endregion
    }

    [Serializable]
    class CharacterToAnimate {
        public int characterIndex;
        public float time;

        public CharacterToAnimate (int characterIndex) {
            this.characterIndex = characterIndex;
            this.time = 0;
        }
    }
}