using System;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

public class TimelineManager : MonoBehaviour
{
    private PlayableDirector _playableDirector;

    private GameManager _gameManager;

    [Inject]
    private void ZenjectSetup(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Awake() 
    {
        _playableDirector = GetComponent<PlayableDirector>();    
    }

    private void OnEnable() 
    {
        _playableDirector.Play();
        _playableDirector.stopped += OnTimelineFinished;   
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        _gameManager.ChangeGameState(GameState.Play);
    }

    private void OnDisable() 
    {
        _playableDirector.stopped -= OnTimelineFinished;
    }
}
