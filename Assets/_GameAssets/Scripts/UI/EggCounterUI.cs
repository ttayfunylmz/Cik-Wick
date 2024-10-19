using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class EggCounterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _eggCounterText;

    [Header("Settings")]
    [SerializeField] private Color _eggCounterColor;
    [SerializeField] private float _colorDuration;
    [SerializeField] private float _scaleDuration;

    private RectTransform _eggCounterRectTransform;

    private void Awake() 
    {
        _eggCounterRectTransform = _eggCounterText.gameObject.GetComponent<RectTransform>();    
    }

    public void SetEggCounterText(int counter, int max)
    {
        _eggCounterText.text = counter.ToString() +"/" + max.ToString();
    }

    public void SetEggCompleted()
    {
        _eggCounterText.DOColor(_eggCounterColor, _colorDuration);

        _eggCounterRectTransform.DOScale(1.2f, _scaleDuration).SetEase(Ease.OutBack);
    }
}
