namespace MaskTransitions
{
    using DG.Tweening;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager Instance;

        private float screenWidth;
        private float screenHeight;
        [HideInInspector] public static float maxSize;
        private float individualTransitionTime;

        [Header("Transition Properties")]
        public Sprite transitionImage;
        public Color transitionColor;
        public bool rotation;
        [Tooltip("Time taken for one half of the transition to complete")]
        public float transitionTime;

        [Header("Image Components")]
        [SerializeField] private RectTransform parentMaskRect;
        [SerializeField] private RectTransform maskRect;
        [SerializeField] private RectTransform transitionCanvas;
        [SerializeField] private Image parentMaskImage;
        [SerializeField] private CutoutMaskUI cutoutMask;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Assign the transition sprite and color
            parentMaskImage.sprite = transitionImage;
            cutoutMask.sprite = transitionImage;
            cutoutMask.color = transitionColor;

            individualTransitionTime = transitionTime / 2;

            SetupMaxSize();
        }

        #region Setup
        void SetupMaxSize()
        {
            screenWidth = transitionCanvas.rect.width;
            screenHeight = transitionCanvas.rect.height;

            maxSize = Mathf.Max(screenWidth, screenHeight);
            maxSize += maxSize / 4;
        }

        void StartAnimation(float? totalTime = null)
        {
            float animationTime = totalTime ?? individualTransitionTime;

            maskRect.sizeDelta = Vector2.zero;
            parentMaskRect.sizeDelta = Vector2.zero;

            maskRect.DOSizeDelta(new Vector2(maxSize, maxSize), animationTime).SetEase(Ease.InOutQuad);
            if (rotation)
                maskRect.DORotate(new Vector3(0, 0, 180), animationTime, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad);
        }

        Tween StartAnimationForLoad(float? totalTime = null)
        {
            float animationTime = totalTime ?? individualTransitionTime;

            maskRect.sizeDelta = Vector2.zero;
            parentMaskRect.sizeDelta = Vector2.zero;
            maskRect.rotation = Quaternion.identity;

            Tween blueTweenSize = maskRect.DOSizeDelta(new Vector2(maxSize, maxSize), animationTime).SetEase(Ease.InOutQuad);

            Sequence animationSequence = DOTween.Sequence().Join(blueTweenSize);

            if (rotation)
            {
                Tween blueTweenRotate = maskRect.DORotate(new Vector3(0, 0, 180), animationTime).SetEase(Ease.InOutQuad);
                animationSequence.Join(blueTweenRotate);
            }

            return animationSequence;
        }


        void EndAnimation(float? totalTime = null)
        {
            float animationTime = totalTime ?? individualTransitionTime;

            maskRect.sizeDelta = new Vector2(maxSize, maxSize);
            parentMaskRect.sizeDelta = Vector2.zero;
            parentMaskRect.rotation = Quaternion.identity;

            parentMaskRect.DOSizeDelta(new Vector2(maxSize, maxSize), animationTime).SetEase(Ease.InOutQuad);
            if (rotation)
                parentMaskRect.DORotate(new Vector3(0, 0, 180), animationTime).SetEase(Ease.InOutQuad);
        }
        #endregion

        #region Transition Without Scene Load
        public void PlayTransition(float transitionTime, float startDelay = 0f)
        {
            StartCoroutine(PlayTransitionWithDelay(transitionTime, startDelay));
        }

        IEnumerator PlayTransitionWithDelay(float transitionTime, float startDelay)
        {
            float dividedTime = transitionTime / 3;

            //Optional Delay
            yield return new WaitForSeconds(startDelay);

            StartAnimation(dividedTime);
            yield return new WaitForSeconds(dividedTime);
            EndAnimation(dividedTime);
        }
        #endregion

        #region Transition With Scene Load 
        public void LoadLevel(string sceneName, float delay = 0f)
        {
            StartCoroutine(LoadLevelWithWait(sceneName, delay));
        }

        IEnumerator LoadLevelWithWait(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);

            Tween animationTween = StartAnimationForLoad();

            // Wait for the animation to complete
            yield return animationTween.WaitForCompletion();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            EndAnimation();
        }
        #endregion

        #region Play Partial Transitions
        public void PlayStartHalfTransition(float transitionTime, float startDelay = 0f)
        {
            StartCoroutine(PlayStartHalfTransitionWithDelay(transitionTime, startDelay));
        }
        public void PlayEndHalfTransition(float transitionTime, float startDelay = 0f)
        {
            StartCoroutine(PlayEndHalfTransitionWithDelay(transitionTime, startDelay));
        }
        IEnumerator PlayStartHalfTransitionWithDelay(float transitionTime, float startDelay)
        {
            yield return new WaitForSeconds(startDelay);
            StartAnimation(transitionTime);
        }
        IEnumerator PlayEndHalfTransitionWithDelay(float transitionTime, float startDelay)
        {
            yield return new WaitForSeconds(startDelay);
            EndAnimation(transitionTime);
        }
        #endregion
    }
}

