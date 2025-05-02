using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Events;
using PrimeTween;
using UnityEngine.TextCore.Text;

namespace CharacterControllerPlayerInput {
    public struct Inputs {
        public Vector3 MovementInput;
        public Quaternion CameraRotation;
        public bool DashRequested;
        public bool JumpRequested;
        public bool TeleportRequested;
        public bool CrouchDown;
    }
}

public enum MovementStates {
    stable = 0,
    sprinting = 1,
    sliding = 2,
    inAir = 3,
    groundDashing = 4,
    airDashing = 5,
    crouching = 6
}

public class MyCharacterController : MonoBehaviour, ICharacterController {
    [Header("References")]
    [SerializeField] private KinematicCharacterMotor _motor;
    [SerializeField] private GameObject _meshRoot;
    [SerializeField] private StaminaSystem staminaSystem;
    

    [Header("Stable Movement")]
    [Tooltip("The player's base movespeed")]
    [SerializeField] private float _baseMovespeed = 8f;
    [Tooltip("How fast the player accelerates while on stable ground")]
    [SerializeField] private float _stableAcceleration = 10f;
    [Tooltip("How fast the player deaccelerates while on stable ground")]
    [SerializeField] private float _stableDeacceleration = 15f;


    [Header("Sprinting")]
    [Tooltip("How fast the player moves while sprinting relative to their base movespeed")]
    [SerializeField] private float _sprintSpeedMult = 2f;
    [Tooltip("How fast the player accelerates to sprint speed")]
    [SerializeField] private float _sprintAcceleration = 1.8f;
    [Tooltip("Time it takes for a player to start sprinting after being above the running treshold")]
    [SerializeField] private float _timeTilSprint = 0.9f;
    [Tooltip("Time it takes for the sprint to expire after being below the running treshold")]
    [SerializeField] private float _sprintExpireTime = 0.1f;
    [Tooltip("Velocity threshold that the player has to surpass to start sprinting")]
    [SerializeField] private float _runningTreshold = 5f;

    
    [Header("Sliding")]
    [Tooltip("How much the player's current speed is multiplied when they slide")]
    [SerializeField] private float _slideInitalSpeedMult = 1.25f;
    [Tooltip("Upper limit to how much speed the slide can give")]
    [SerializeField] private float _slideForceMax = 7f;
    [Tooltip("The max speed a slide can have when going down a hill relatiev to the base speed")]
    [SerializeField] private float _slideSpeedMult = 4f;
    [Tooltip("How long the player is forced to slide for after initiating a slide")]
    [SerializeField] private float _slideStunDuration = 0.75f;
    [Tooltip("Velocity threshold needed to enter a slide")]
    [SerializeField] private float _slideSpeedThreshold = 8.1f;
    [Tooltip("Velocity threshold needed to exit a slide")]
    [SerializeField] private float _slideSpeedExitThreshold = 6f;
    [Tooltip("How much the player is allowed to redirect their movement while sliding")]
    [SerializeField] private float _slidingRotationSmoothing = 2f;
    [Tooltip("How smooth the slide slow down is")]
    [SerializeField] private float _slidingDragSmoothing = 3f;
    [Tooltip("How fast the slide increases in speed down a slope")]
    [SerializeField] private float _slideBuildUpSmoothing = 3f;
    [Tooltip("The percentage of velocity slowdown on the slide")]
    [SerializeField] private float _slideSlowDown = 0.85f;
    

    [Header("Air Movement")]
    [Tooltip("Should turning around decrease player momentum in the air")]
    [SerializeField] private bool _momentumAirMovementToggle;
    [Tooltip("How fast the player moves while in the air relative to their base movespeed")]
    [SerializeField] private float _airBaseSpeedMult = 1f;
    [Tooltip("How fast the player accelerates up to air base speed")]
    [SerializeField] private float _airAcceleration = 1f;
    [Tooltip("How easily the player is able to change direction in the air")]
    [SerializeField] private float _airRotationSmoothing = 1f;
    [Tooltip("Air drag")]
    [SerializeField] private float _drag = 0.00001f;


    [Header("Dashes")]
    [Tooltip("How fast to dash")]
    [SerializeField] private float _dashForce = 7f;
    [Tooltip("How long the player is stuck moving in a certain direction")]
    [SerializeField] private float _dashStun = 0.8f;
    [Tooltip("A buffer for the dash input")]
    [SerializeField] private float _dashBuffer = 0.1f;

    
    [Header("Jumps")]
    [Tooltip("Allow jumps on steep slopes")]
    [SerializeField] private bool _allowJumpWhileSliding = true;
    [Tooltip("How high the player jumps")]
    [SerializeField] private float _jumpForce = 15f;
    [Tooltip("A buffer for the jump input")]
    [SerializeField] private float _jumpBuffer = 0.1f;
    [Tooltip("Time it takes to jump again")]
    [SerializeField] private float _jumpCooldown = 0.05f;
    [Tooltip("Time after leaving stable ground where the player can still jump")]
    [SerializeField] private float _coyoteTime = 0.1f; 
    [Tooltip("Time at the apex of a player's jump where gravity is reduced")]
    [SerializeField] private float _jumpHangInterval = 0.5f; 
    [Tooltip("The number of jumps the player has")]
    [SerializeField] private int _maxJumps = 2;


    [Header("Crounching")]
    [Tooltip("How fast the player moves while in crouching relative to their base movespeed")]
    [SerializeField] private float _crouchSpeedMult = 0.5f;


    [Header("Gravity")]
    [Tooltip("The player's base gravity")]
    [SerializeField] private Vector3 _gravity = new Vector3(0f, -30f, 0f);
    [Tooltip("Gravity while falling")]
    [SerializeField] private float _gravityFallingMult = 1.15f;
    [Tooltip("Gravity while at the apex of a player's jump")]
    [SerializeField] private float _gravityJumpHangMult = 0.9f;

    
    [Header("Misc")]
    [Tooltip("Layer masks that indicates what layers the character collider to ignore")]
    [SerializeField] private LayerMask _ignoredLayers;
    [Tooltip("Position where the player teleports back to")]
    [SerializeField] private Transform _teleportTransform;

    // Gravity
    // Look & Input Vectors
    private Vector3 _lookVector = new Vector3(1,0,0);
    private Vector3 _inputVector = Vector3.zero;
    // Sprinting
    private float _sprintTimer = 0f;
    private float _sprintExpireTimer = 0f;
    private bool _canSprint => _sprintTimer >= _timeTilSprint;
    // Sliding
    private float _slideStunTimer = 0f;
    // Dash
    private float _timeSinceDashRequested = 0f;
    private float _dashStunTimer = 0f;
    // Jumping
    private int _jumps = 0;
    private bool _canJump = true;
    private bool _jumpedThisFrame = false;
    private float _timeSinceJumpRequested = 25f;
    private float _timeSinceJumpAllowed = 0f;
    private float _jumpTimer = 0f;
    // Crouching
    private bool _crouchDown;
    private Collider[] _probedColliders = new Collider[8];
    // Misc.
    private bool _teleportRequested = false; 
    private float _movementMult = 1f; 
    private float _movementAcceleration = 15f; 
    private float _movementDeacceleration = 20f; 
    private bool _zeroMovement = false; 
    private Vector3 _internalVelocityAdd = Vector3.zero;
    public MovementStates _state {get; private set;}
    
    // Getters && Setters
    [HideInInspector] public Vector3 CurrentVelocity {get; private set;}
    [HideInInspector] public Vector3 CurrentHorVelocity => Vector3.ProjectOnPlane(CurrentVelocity, _motor.CharacterUp);

    // Events
    [HideInInspector] public UnityEvent OnJump;

    private void Start() {
        _motor.CharacterController = this;
        _state = MovementStates.stable;
    }



    /** State Machine **/



    private void TransitionState(MovementStates newState) {
        // Dont transition if the states are the same
        if (newState == _state) {
            return;
        }
        if (ExitState(_state, newState)) {
            EnterState(_state, newState);
            _state = newState;
        }
    }

    // Returns true if exit conditions can be met, else false
    private bool ExitState(MovementStates oldState, MovementStates newState) {
        switch(oldState) {
            case MovementStates.stable: {
                break;
            }
            case MovementStates.crouching: {
                if (!UncrouchPlayer()) {
                    return false;
                }
                break;
            }
            case MovementStates.sliding: {
                // If the stun timer is active and the player is grounded, do not allow the slide state to change
                if (_slideStunTimer > 0f) {
                    return false;
                }
                // If the player can't uncrouch, transition to crouch state
                if (!CanUncrouch()) {
                    EnterState(_state, MovementStates.crouching);
                    _state = MovementStates.crouching;
                    return false;
                }
                else {
                    UncrouchPlayer();
                }
                // Allow jumping 
                _canJump = true;
                break;
            }
            case MovementStates.groundDashing: {
                if (_dashStunTimer > 0f) {
                    return false;
                }
                break;
            }
            case MovementStates.airDashing: {
                if (_dashStunTimer > 0f) {
                    return false;
                }
                break;
            }
            default: {
                break;
            }
        }
        return true;
    }

    private void EnterState(MovementStates oldState, MovementStates newState) {
        switch(newState) {
            case MovementStates.stable: {
                _movementMult = 1f;
                _movementAcceleration = _stableAcceleration;
                _movementDeacceleration = _stableDeacceleration;
                break;
            }
            case MovementStates.crouching: {
                _movementMult = _crouchSpeedMult;
                _movementAcceleration = _stableAcceleration;
                _movementDeacceleration = _stableDeacceleration;
                // Enter crouching positon
                _motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                _meshRoot.transform.localScale = new Vector3(1f, 0.5f, 1f);
                // Stop sprinting if crouching
                StopSprint();
                break;
            }
            case MovementStates.sprinting: {
                _movementMult = _sprintSpeedMult;
                _movementAcceleration = _sprintAcceleration;
                _movementDeacceleration = _stableDeacceleration;
                break;
            }
            case MovementStates.airDashing: {
                _movementMult = 1f;
                _movementAcceleration = _stableAcceleration;
                _movementDeacceleration = _stableDeacceleration;
                if (staminaSystem.ConsumeStamina()) {
                    AddVelocityInPlayerInputDirection(_dashForce);
                    _dashStunTimer = _dashStun;
                }
                break;
            }
            case MovementStates.groundDashing: {
                _movementMult = 1f;
                _movementAcceleration = _stableAcceleration;
                _movementDeacceleration = _stableDeacceleration;
                if (staminaSystem.ConsumeStamina()) {
                    AddVelocityInPlayerInputDirection(_dashForce);
                    _dashStunTimer = _dashStun;
                }
                break;
            }
            case MovementStates.sliding: {
                _movementMult = _slideSlowDown;
                _movementAcceleration = _slidingDragSmoothing;
                _movementDeacceleration = _slidingDragSmoothing;
                // Force the player to slide for some amount of time
                _slideStunTimer = _slideStunDuration;
                // Disallow jumping during stun duration
                _canJump = false;
                // Enter crouching positon
                _motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                _meshRoot.transform.localScale = new Vector3(1f, 0.5f, 1f);
                // Apply the slide force
                float speedToAdd = Mathf.Clamp(CurrentVelocity.magnitude * _slideInitalSpeedMult, 0f, _slideForceMax);
                AddVelocity(CurrentVelocity.normalized * speedToAdd);
                break;
            }
            default: {
                _movementMult = 1f;
                _movementAcceleration= _stableAcceleration;
                _movementDeacceleration = _stableDeacceleration;
                break;
            }
        }
    }

    private void HandleStates() {
        // Slide, if the player is pressing the crouch button, the player is grounded, and they are moving past a certain threshold or are moving down a slope
        if (_crouchDown && 
            _motor.GroundingStatus.FoundAnyGround &&
            ((_state == MovementStates.sliding ? CurrentHorVelocity.magnitude >= _slideSpeedExitThreshold : CurrentHorVelocity.magnitude >= _slideSpeedThreshold) || 
                MovingDownASlope)) {
            TransitionState(MovementStates.sliding);
        }
        // Stable Grounded States 
        else if (_motor.GroundingStatus.IsStableOnGround) {
            if (_crouchDown && _motor.GroundingStatus.IsStableOnGround) {
                TransitionState(MovementStates.crouching);
            }
            // Ground Dash, if the player has stamina to consume, the player has pressed the dash button, are inputting in a direction, and not sliding/crouching/dashing
            else if (staminaSystem.CanConsumeStamina() && 
                _timeSinceDashRequested < _dashBuffer && 
                _inputVector != Vector3.zero && 
                (_state != MovementStates.sliding || _state != MovementStates.crouching || _state != MovementStates.groundDashing || _state != MovementStates.airDashing)) {
                TransitionState(MovementStates.groundDashing);
            }
            else if (_canSprint && _motor.GroundingStatus.IsStableOnGround) {
                TransitionState(MovementStates.sprinting);
            }
            else {
                TransitionState(MovementStates.stable);
            }
        }
        // Air States
        else {
            // Air Dash, if the player has stamina to consume, the player has pressed the dash button, are inputting in a direction, and not sliding/crouching/dashing
            if (staminaSystem.CanConsumeStamina() && 
                _timeSinceDashRequested < _dashBuffer && 
                _inputVector != Vector3.zero && 
                (_state != MovementStates.sliding || _state != MovementStates.crouching || _state != MovementStates.groundDashing || _state != MovementStates.airDashing)) {
                TransitionState(MovementStates.airDashing);
            }
            else {
                TransitionState(MovementStates.inAir);
            }
        }
    }



    /** Private Methods **/



    private Vector3 HandleGravity(Vector3 currentVelocity, float deltaTime) {
        float yVelocity = currentVelocity.y;
        Vector3 gravityToApply = _gravity * deltaTime;
        if (yVelocity < -_jumpHangInterval) {
            gravityToApply *= _gravityFallingMult;
        }
        else if (yVelocity < _jumpHangInterval) {
            gravityToApply *= _gravityJumpHangMult;
        }
        return gravityToApply;
    }

    private void HandleSprinting(Vector3 currentVelocity, float deltaTime) {
        // The player can start sprinting only if they are on the ground and above a certain velocity
        if (!_canSprint && (_state != MovementStates.sliding) && _motor.GroundingStatus.IsStableOnGround && Vector3.ProjectOnPlane(currentVelocity, _motor.CharacterUp).magnitude > _runningTreshold) {
            _sprintTimer += deltaTime;
        }
        // If the player is sprinting and above the speed threshold, keep sprinting
        else if (_canSprint && Vector3.ProjectOnPlane(currentVelocity, _motor.CharacterUp).magnitude > _runningTreshold) {
            _sprintExpireTimer = _sprintExpireTime;
        }
        // If the player is sprinting and below the speed threshold, start the sprint expire timer
        else if (_canSprint && _sprintExpireTimer > 0) {
            _sprintExpireTimer -= deltaTime;
        }
        // Reset sprinting
        else {
            _sprintExpireTimer = 0f;
            _sprintTimer = 0f;
        }
    }

    // Returns true if the player can uncrouch otherwise false, uncrouches the player if possible
    private bool UncrouchPlayer() {
        // Detects a collider above the player, if there is one, don't uncrouch
        if (!CanUncrouch()) {
            return false;
        }
        else {
            _motor.SetCapsuleDimensions(0.5f, 2f, 1f);
            _meshRoot.transform.localScale = new Vector3(1f, 1f, 1f);
            return true;
        }
    }

    // Returns true if the player can uncrouch otherwise false, does not uncrouch the player if possible
    private bool CanUncrouch() {
        _motor.SetCapsuleDimensions(0.5f, 2f, 1f);
        if (_motor.CharacterCollisionsOverlap(_motor.TransientPosition, _motor.TransientRotation, _probedColliders) > 0) {
            _motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
            return false;
        }
        else {
            _motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
            return true;
        }
    }

    private void StopSprint() {
        _sprintTimer = 0f;
    }

    private Vector3 HandleSliding(Vector3 currentVelocity, Vector3 targetVelocityVector, float deltaTime) {
        // If the player is sliding down a slope, increase their velocity
        if (MovingDownASlope) {
            _movementMult = _slideSpeedMult;
            _movementAcceleration = _slideBuildUpSmoothing;
            _movementDeacceleration = _slideBuildUpSmoothing;
            if (_inputVector != Vector3.zero) {
                targetVelocityVector = Vector3.Lerp(currentVelocity, targetVelocityVector, 1 - Mathf.Exp(-_slidingRotationSmoothing * deltaTime)).normalized * targetVelocityVector.magnitude;
            }
            else {
                targetVelocityVector = currentVelocity.normalized * _baseMovespeed;
            }
        }
        // If the player is not sliding down a slope, decrease their velocity
        else {
            _movementMult = _slideSlowDown;
            _movementAcceleration = _slidingDragSmoothing;
            _movementDeacceleration = _slidingDragSmoothing;
            if (_inputVector != Vector3.zero) {
                targetVelocityVector = Vector3.Lerp(currentVelocity, targetVelocityVector, 1 - Mathf.Exp(-_slidingRotationSmoothing * deltaTime)).normalized * currentVelocity.magnitude;
            }
            else {
                targetVelocityVector = currentVelocity;
            }
        }

        return targetVelocityVector;
    }

    private void HandleTeleport() {
        if (_teleportRequested) {
            _motor.MoveCharacter(_teleportTransform.position);
            _teleportRequested = false;
        }
    }



    /** ICharacterController Implementations **/



    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
        currentRotation = Quaternion.LookRotation(_lookVector, _motor.CharacterUp);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
        // Handle movement
        {
            Vector3 targetVelocityVector = Vector3.zero;
            // Check if the player is on stable ground i.e. not on a slope or in the air
            if (_motor.GroundingStatus.IsStableOnGround) {
                // Calculate max movement vector
                Vector3 inputRight = Vector3.Cross(_inputVector, _motor.CharacterUp);
                Vector3 reorientedInputVector = Vector3.Cross(_motor.GroundingStatus.GroundNormal, inputRight).normalized * _inputVector.magnitude;
                targetVelocityVector = reorientedInputVector * _baseMovespeed;

                if (_state == MovementStates.sliding) {
                    targetVelocityVector = HandleSliding(currentVelocity, targetVelocityVector, deltaTime);
                }

                // Turn the player towards the input direction
                if (_inputVector != Vector3.zero) {
                    Vector3 turnVector = Quaternion.FromToRotation(currentVelocity, targetVelocityVector) * currentVelocity;
                    currentVelocity = Vector3.Lerp(turnVector, targetVelocityVector * _movementMult, 1 - Mathf.Exp(-_movementAcceleration * deltaTime));
                }
                // If not inputting and not sliding, apply deacceleration 
                else {
                    currentVelocity = Vector3.Lerp(currentVelocity, targetVelocityVector * _movementMult, 1 - Mathf.Exp(-_movementDeacceleration * deltaTime));
                }
            }   
            // If the player is in the air or on a slope
            else {
                // Add air movement if the player is inputing in the air
                if (_inputVector.sqrMagnitude > 0f) {
                    // Find the target velocity of the player in the air
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(currentVelocity, _motor.CharacterUp);
                    if ((_baseMovespeed * _airBaseSpeedMult) >= horizontalVelocity.magnitude) {
                        // If the player's max air movespeed is more then their current horizontal velocity, set their target horizontal velocity to that
                        targetVelocityVector = _inputVector * _baseMovespeed * _airBaseSpeedMult;
                    }
                    else {
                        targetVelocityVector = _inputVector * horizontalVelocity.magnitude;
                    }

                    // Preventing climbing on unstable ground while in the air
                    if (_motor.GroundingStatus.FoundAnyGround) {
                        Vector3 perpendicularObstructionNormal = Vector3.Cross(Vector3.Cross(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal), _motor.CharacterUp).normalized;
                        targetVelocityVector = Vector3.ProjectOnPlane(targetVelocityVector, perpendicularObstructionNormal);
                    }

                    // Calculate how hard it is to redirect movement in air
                    // Vector3 velocityDiff = Vector3.ProjectOnPlane(targetVelocityVector - currentVelocity, _gravity);
                    // currentVelocity += velocityDiff * _airAcceleration * deltaTime;

                    // Turn the player in the air without decreasing velocity
                    Vector3 horizontalCurrentVector = Vector3.ProjectOnPlane(currentVelocity, _gravity);
                    Vector3 horizontalTurnVector = Vector3.Lerp(horizontalCurrentVector.normalized, targetVelocityVector.normalized, 1 - Mathf.Exp(-_airRotationSmoothing * deltaTime)).normalized * horizontalCurrentVector.magnitude;
                    currentVelocity = Vector3.Lerp(horizontalTurnVector, horizontalTurnVector.normalized * targetVelocityVector.magnitude, 1 - Mathf.Exp(-_airAcceleration * deltaTime)) + Vector3.Project(currentVelocity, _gravity);
                }   
                
                // Add gravity to the gravity
                currentVelocity += HandleGravity(currentVelocity, deltaTime);

                // Add drag to the player
                currentVelocity *= 1f / (1f + (_drag * deltaTime));
            }
        }

        // Handle Jumps
        _timeSinceJumpRequested += deltaTime;
        _jumpedThisFrame = false;
        if (_timeSinceJumpRequested <= _jumpBuffer && _canJump) {
            // Check if we are allowed to jump on the ground
            if (_jumps > 0 && _jumpTimer <= 0 && ((_allowJumpWhileSliding ? _motor.GroundingStatus.FoundAnyGround : _motor.GroundingStatus.IsStableOnGround) || _timeSinceJumpAllowed <= _coyoteTime)) {
                // Calculate jump direction
                Vector3 jumpDirection = _motor.CharacterUp;
                if (_motor.GroundingStatus.FoundAnyGround && !_motor.GroundingStatus.IsStableOnGround) {
                    jumpDirection = _motor.GroundingStatus.GroundNormal;
                }

                _motor.ForceUnground(0.1f);
                
                currentVelocity += jumpDirection * _jumpForce - Vector3.Project(currentVelocity, _motor.CharacterUp);
                _timeSinceJumpRequested = _jumpBuffer + 1f;
                _jumps--;
                _jumpTimer = _jumpCooldown;
                _jumpedThisFrame = true;
                OnJump?.Invoke();
            }
            // Handle Air Jumps
            else if (_jumps > 0 && _jumpTimer <= 0 && (_allowJumpWhileSliding ? !_motor.GroundingStatus.FoundAnyGround : !_motor.GroundingStatus.IsStableOnGround)) {
                _motor.ForceUnground(0.1f);

                currentVelocity += _motor.CharacterUp * _jumpForce - Vector3.Project(currentVelocity, _motor.CharacterUp);
                _timeSinceJumpRequested = _jumpBuffer + 1f;
                _jumps--;
                _jumpTimer = _jumpCooldown;
                _jumpedThisFrame = true;
                OnJump?.Invoke();
            }
            _jumpTimer -= deltaTime;
        }

        // Handle Additive Velocity
        if (_internalVelocityAdd.sqrMagnitude > 0f) {
            _motor.ForceUnground(0.1f);
            currentVelocity += _internalVelocityAdd;
            _internalVelocityAdd = Vector3.zero;
        }

        // Handle Sprinting
        HandleSprinting(currentVelocity, deltaTime);

        // Set vertical velocity to 0 
        if (_state == MovementStates.airDashing) {
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, _motor.CharacterUp);
        }

        // Set velocity to zero
        if (_zeroMovement) {
            currentVelocity = Vector3.zero;
        }

        // Set local varaible _currentVelocity to currentVelocity
        CurrentVelocity = currentVelocity;
    }

    public void AfterCharacterUpdate(float deltaTime) {
        // Check if the player is grounded and reset their jumps if so
        if (_allowJumpWhileSliding ? _motor.GroundingStatus.FoundAnyGround : _motor.GroundingStatus.IsStableOnGround) {
            if (!_jumpedThisFrame) {
                _jumps = _maxJumps;
            }
            _timeSinceJumpAllowed = 0f;
        }
        else {
            // Time kept for coyote time
            _timeSinceJumpAllowed += deltaTime;
        }
    }

    private void HandleSlideDashDuration(float deltaTime) {
        if (_state == MovementStates.sliding) {
            // Allow jumping once stun duration is over or player is in the air
            if (_slideStunTimer <= 0 || !_motor.GroundingStatus.FoundAnyGround) {
                _canJump = true;
            }
        }
        _slideStunTimer -= deltaTime;
    }

    private void HandleDash() {
        if (_dashStunTimer > 0) {
            _dashStunTimer -= Time.deltaTime;
        }
        _timeSinceDashRequested += Time.deltaTime;
    }

    public void BeforeCharacterUpdate(float deltaTime) {
        // Handle character states
        HandleStates();

        // Handle Slide Stun Duration
        HandleSlideDashDuration(deltaTime);

        // Handle Dashing
        HandleDash();

        // Handle Teleporting
        HandleTeleport();
    }

    public bool IsColliderValidForCollisions(Collider coll) {
        if (((1 << coll.gameObject.layer) & _ignoredLayers) < 0) {
            return false;
        }
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider) {
        
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
        
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) {
        
    }

    public void PostGroundingUpdate(float deltaTime) {
        
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        
    }



    /** Public Methods **/



    /// <summary>
    /// Takes in the player inputs
    /// </summary>
    public void FeedPlayerInput(ref CharacterControllerPlayerInput.Inputs inputs) {
        // Find the vector that the camera is pointing on the character's horizontal plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, _motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, _motor.CharacterUp).normalized;
        }
        // From camera's planar direction, find it's rotation
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, _motor.CharacterUp);

        _inputVector = (cameraPlanarRotation * inputs.MovementInput).normalized;
        _lookVector = cameraPlanarDirection;

        // Fill in the last time jump was inputted
        if (inputs.JumpRequested) {
            _timeSinceJumpRequested = 0f;
            inputs.JumpRequested = false;
        }
        // Fill in the last time dash was inputted
        if (inputs.DashRequested) {
            _timeSinceDashRequested = 0f;
            inputs.DashRequested = false;
        }
        if (inputs.TeleportRequested) {
            _teleportRequested = true;
            inputs.TeleportRequested = false;
        }
        _crouchDown = inputs.CrouchDown;
    }

    public void AddVelocity(Vector3 velocity) {
        _internalVelocityAdd += velocity;
    }

    public void AddVelocityInPlayerInputDirection(float force) {
        _internalVelocityAdd += _inputVector.normalized * force;
    }

    public void ZeroMovement() {
        _zeroMovement = true;
    }

    public bool IsOnASlope => _motor.GroundingStatus.FoundAnyGround && Vector3.Angle(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal) > 0.1f;

    public bool MovingDownASlope => _motor.GroundingStatus.FoundAnyGround && Vector3.Angle(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal) > 0.1f && Vector3.ProjectOnPlane(CurrentVelocity, _motor.GroundingStatus.GroundNormal).y < -0.01f;
}