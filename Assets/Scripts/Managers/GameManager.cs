using FMOD.Studio;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private GameObject _playerGameObject;

    private EventInstance _musicInstance;
    public float TimeOnLevel {get; private set;}
    public bool TimerIsCounting {get; private set;}

    private void Start() {
        TimeOnLevel = 0f;
        TimerIsCounting = true;
        PlayMusic();
    }

    private void Update() {
        if (TimerIsCounting) {
            TimeOnLevel += Time.deltaTime;
        }
    }

    private void OnDestroy() {
        StopMusic();
    }

    private void PlayMusic() {
        _musicInstance = AudioManager.Instance.CreateInstanceAndAttach(FMODEvents.Instance.MusicTrack, _playerGameObject);
        _musicInstance.start();
    }

    private void StopMusic() {
        _musicInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
