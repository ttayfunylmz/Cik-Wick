using UnityEngine;
using Zenject;

public class HealthManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Settings")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    private PlayerHealthUI _playerHealthUI;
    private GameManager _gameManager;

    [Inject]
    private void ZenjectSetup(PlayerHealthUI playerHealthUI, GameManager gameManager)
    {
        _playerHealthUI = playerHealthUI;
        _gameManager = gameManager;
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
                _gameManager.PlayGameOver(_playerTransform, false);
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
