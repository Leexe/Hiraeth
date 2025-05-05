using UnityEngine;
using FMOD.Studio;

public class PlayerSounds : MonoBehaviour {
    [Header("References")]
    [SerializeField] private MyCharacterController _characterController;
    [SerializeField] private GameObject _playerGameObject;

    private EventInstance _slideStartInstance;
    private EventInstance _slideLoopInstance;

    private void Start() {
        // Create Instances
        _slideStartInstance = AudioManager.Instance.CreateInstanceAndAttach(FMODEvents.Instance.Slide_sfx, _playerGameObject);
    }

    private void OnEnable() {
        // Subscribe to Events
        _characterController.OnGroundJump.AddListener(PlayGroundJumpSound); 
        _characterController.OnAirJump.AddListener(PlayAirJumpSound); 
        _characterController.OnGroundDash.AddListener(PlayGroundDashSound); 
        _characterController.OnAirDash.AddListener(PlayAirDashSound); 
        _characterController.OnLanding.AddListener(PlayLandSound); 
        _characterController.OnSlideStart.AddListener(PlaySlideStartSounds); 
        _characterController.OnSlideEnd.AddListener(HandleSlideEndSounds); 
    }

    private void OnDisable() {
        // Unsubscribe to Events
        _characterController.OnGroundJump.RemoveListener(PlayGroundJumpSound); 
        _characterController.OnAirJump.RemoveListener(PlayAirJumpSound); 
        _characterController.OnGroundDash.RemoveListener(PlayGroundDashSound); 
        _characterController.OnAirDash.RemoveListener(PlayAirDashSound); 
        _characterController.OnLanding.RemoveListener(PlayLandSound); 
        _characterController.OnSlideStart.RemoveListener(PlaySlideStartSounds); 
        _characterController.OnSlideEnd.AddListener(HandleSlideEndSounds); 
    }

    private void PlayGroundJumpSound() {
        AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.Jump_sfx, _playerGameObject);
    }

    private void PlayAirJumpSound() {
        AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.AirJump_sfx, _playerGameObject);
    }

    private void PlayGroundDashSound() {
        AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.GroundedDash_sfx, _playerGameObject);
    }

    private void PlayAirDashSound() {
        AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.AirDash_sfx, _playerGameObject);
    }

    private void PlayLandSound() {
        AudioManager.Instance.PlayOneShotAttached(FMODEvents.Instance.Land_sfx, _playerGameObject);
    }

    private void PlaySlideStartSounds() {
        Debug.Log("Slide Start");
        _slideStartInstance.start();
    }

    private void HandleSlideEndSounds() {
        Debug.Log("Slide End");
        _slideStartInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
