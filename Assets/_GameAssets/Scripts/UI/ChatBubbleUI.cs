using System;
using DG.Tweening;
using TextAnimation;
using TMPro;
using UnityEngine;

public class ChatBubbleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _bubbleImageTransform;
    [SerializeField] private TMP_Text _speechText;
    [SerializeField] private Typewriter _typewriter;

    [Header("Settings")]
    [SerializeField] private float _scaleDuration = 0.5f;
    [SerializeField] private float _endDelay = 0.5f;

    private bool _isTyping;

    private void Start() 
    {
        ResetChatBubble();

        _typewriter.OnTypewriteEnd += TypeWriter_OnTypewriteEnd;        
    }

    private void TypeWriter_OnTypewriteEnd()
    {
        Invoke(nameof(EndChatBubbleAnimation), _endDelay);
    }

    public void PlayChatBubbleAnimation(string text)
    {
        if(_isTyping) { return; }

        _isTyping = true;

        _bubbleImageTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _speechText.text = text;
            _speechText.gameObject.SetActive(true);
            _typewriter.StartTypewriter();
        });
    }

    private void EndChatBubbleAnimation()
    {
        _bubbleImageTransform.DOScale(0f, _scaleDuration).SetEase(Ease.InBack).OnComplete(() 
            => ResetChatBubble());
    }

    private void ResetChatBubble()
    {
        _isTyping = false;
        _speechText.gameObject.SetActive(false);
        _bubbleImageTransform.localScale = Vector3.zero;
        _speechText.text = String.Empty;
    }
}