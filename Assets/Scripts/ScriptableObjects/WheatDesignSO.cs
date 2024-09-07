using UnityEngine;

[CreateAssetMenu(fileName = "WheatDesignSO", menuName = "ScriptableObjects/WheatDesignSO")]
public class WheatDesignSO : ScriptableObject
{
    public GameObject _particlesPrefab;
    public float _particlesDestroyDuration;
    public float _resetBoostDuration;
    public Sprite _activeSprite;
    public Sprite _passiveSprite;
    public Sprite _activeWheatSprite;
    public Sprite _passiveWheatSprite;
}
