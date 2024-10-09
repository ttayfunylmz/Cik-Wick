using UnityEngine;

[CreateAssetMenu(fileName = "WheatDesignSO", menuName = "ScriptableObjects/WheatDesignSO")]
public class WheatDesignSO : ScriptableObject
{
    [SerializeField] private GameObject particlesPrefab;
    [SerializeField] private GameObject hitParticlesPrefab;
    [SerializeField] private float particlesDestroyDuration;
    [SerializeField] private float resetBoostDuration;
    [SerializeField] private float hitParticlesDestroyDuration;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite passiveSprite;
    [SerializeField] private Sprite activeWheatSprite;
    [SerializeField] private Sprite passiveWheatSprite;

    public GameObject ParticlesPrefab => particlesPrefab;
    public GameObject HitParticlesPrefab => hitParticlesPrefab;
    public float ParticlesDestroyDuration => particlesDestroyDuration;
    public float ResetBoostDuration => resetBoostDuration;
    public float HitParticlesDestroyDuration => hitParticlesDestroyDuration;
    public Sprite ActiveSprite => activeSprite;
    public Sprite PassiveSprite => passiveSprite;
    public Sprite ActiveWheatSprite => activeWheatSprite;
    public Sprite PassiveWheatSprite => passiveWheatSprite;
}
