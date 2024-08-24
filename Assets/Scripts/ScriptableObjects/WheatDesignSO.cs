using UnityEngine;

[CreateAssetMenu(fileName = "WheatDesignSO", menuName = "ScriptableObjects/WheatDesignSO")]
public class WheatDesignSO : ScriptableObject
{
    public GameObject _particlesPrefab;
    public float _particlesDestroyDuration;
    public float _resetBoostDuration;
    
}
