using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    public event Action<GameState> OnGameStateChanged;

    [Header("References")]
    [SerializeField] private GameObject _fightingParticles;

    [Header("Settings")]
    [SerializeField] private float _delay = 1f;

    private int _currentEggCount;
    private int _maxEggCount;
    private GameState _currentGameState;

    private EggCounterUI _eggCounterUI;
    private WinLoseUI _winLoseUI;
    private CatController _catController;
    private AudioManager _audioManager;
    private BackgroundMusic _backgroundMusic;

    [Inject]
    private void ZenjectSetup(EggCounterUI eggCounterUI, WinLoseUI winLoseUI, 
        CatController catController, AudioManager audioManager, BackgroundMusic backgroundMusic)
    {
        _eggCounterUI = eggCounterUI;
        _winLoseUI = winLoseUI;
        _catController = catController;
        _audioManager = audioManager;
        _backgroundMusic = backgroundMusic;
    }

    private void Start() 
    {
        _maxEggCount = 5;

        _catController.OnCatCatched += CatController_OnCatCatched;
    }

    private void OnEnable() 
    {
        ChangeGameState(GameState.CutScene);
        _backgroundMusic.PlayBackgroundMusic(true);
    }

    public void ChangeGameState(GameState gameState)
    {
        OnGameStateChanged?.Invoke(gameState);
        _currentGameState = gameState;
        Debug.Log($"Game State: {gameState}");
    }

    private void CatController_OnCatCatched(Transform playerTransform)
    {
        PlayGameOver(playerTransform, true);
    }

    private IEnumerator OnGameOver(Transform playerTransform, bool isCatCatched)
    {
        yield return new WaitForSeconds(_delay);
        Instantiate(_fightingParticles, playerTransform.position, _fightingParticles.transform.rotation);
        ChangeGameState(GameState.GameOver);
        _winLoseUI.OnGameOver();
        if(isCatCatched) _audioManager.Play(SoundType.CatSound);
    }

    public void PlayGameOver(Transform playerTransform, bool isCatCatched)
    {
        StartCoroutine(OnGameOver(playerTransform, isCatCatched));
    }

    public void OnEggCollected()
    {
        _currentEggCount++;
        _eggCounterUI.SetEggCounterText(_currentEggCount,  _maxEggCount);

        if(_currentEggCount == _maxEggCount)
        {
            ChangeGameState(GameState.GameOver);
            _eggCounterUI.SetEggCompleted();
            _winLoseUI.OnGameWin();
        }
    }

    public int GetCurrentEggCount()
    {
        return _currentEggCount;
    }

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }
}
