using UnityEngine;
using Zenject;

public class HealthManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    private PlayerHealthUI _playerHealthUI;

    [Inject]
    private void ZenjectSetup(PlayerHealthUI playerHealthUI)
    {
        _playerHealthUI = playerHealthUI;
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void Damage(int damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            _playerHealthUI.AnimateDamage();

            if(_currentHealth <= 0)
            {
                Debug.Log("Player Dead!");
            }
        }
    }

    public void Heal(int healAmount)
    {
        if (_currentHealth < _maxHealth)
        {
            _currentHealth = Mathf.Min(_currentHealth + healAmount, _maxHealth);
        }
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }
}
