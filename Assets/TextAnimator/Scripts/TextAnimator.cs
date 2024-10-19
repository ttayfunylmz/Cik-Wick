using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using System;
using UnityEngine.UIElements;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using System.IO;

namespace TextAnimation {
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteInEditMode]
    public class TextAnimator : MonoBehaviour {
        #region Local Variables
        TMP_Text textMesh;
        TMP_TextInfo textInfo;
        #endregion

        #region Initialization
        private void OnEnable () {
            textMesh = GetComponent<TMP_Text>();
            textMesh.textPreprocessor = new CustomTextPreprocessor();
            textMesh.OnPreRenderText += TextMesh_OnPreRenderText;
            textInfo = textMesh.textInfo;
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
        /// Process nested tags
        /// </summary>
        /// <param name="text"></param>
        void ProcessNestedTags (string text) {
            string visibleString = "";
            for (int i = 0; i < textInfo.characterCount; i++) {
                var charInfo = textInfo.characterInfo[i];
                visibleString += charInfo.character;
            }

            List<TagContent> tags = new List<TagContent>();

            var tagRegex = new Regex(@"<(/)?(\w+)\s*([^>]*)>([^<]*)");
            var matches = tagRegex.Matches(text);

            var tagList = new List<(string Tag, int Position, string parameters)>();

            foreach (Match match in matches) {
                string parameters = match.Groups[3].Value;
                string tag = match.Groups[2].Value;
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

            TextAnimatorManager.AddTagsToDict(this, textMesh, textInfo, tags);
        }
        #endregion
    }

    public class CustomTextPreprocessor : ITextPreprocessor {
        public string PreprocessText (string text) {
            return TextAnimatorManager.ProcessText(text);
        }
    }
}