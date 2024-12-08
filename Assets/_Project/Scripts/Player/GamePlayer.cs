using System;
using _Project.Scripts.Player;
using DefaultNamespace.Player;
using DefaultNamespace.Player.enums;
using UnityEngine;
using UnityEngine.Serialization;

public class GamePlayer : MonoBehaviour
{
   
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float jumpForce = 5.0f;
    public float gravity = -9.81f;
    [Range(1,10)]
    public float defaultMouseSensitivity = 5;
    [Range(1,10)]
    public float aimMouseSensitivity = 2;
    private float _currentMouseSensitivity;
    
    public float cameraVerticalAngleThreshold = 90;
    public Camera playerCamera;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float verticalRotation = 0;
    private Vector3 moveDirection;

    #region Aim

    private float aimMoveSpeed = 1;
    private bool isAiming = false;
    public float defaultFOV = 60f;
    public float aimFOV = 30f; // Угол обзора камеры в режиме прицеливания
    public float fovTransitionSpeed = 10f; // Скорость перехода угла обзора
    private float targetFOV;
    private bool isTransitioningFOV = false;

    #endregion


    #region Crouching

    public float crouchSpeed = 2.0f; // Скорость движения в режиме приседания
    
    private float crouchCameraHeight; // Высота камеры в режиме приседания
    private float standCameraHeight; // Высота камеры в режиме стояния
    private float targetCameraHeight;

    private float crouchControllerHeight;
    private float standControllerHeight;
    private float targetControllerHeight;
    
    public float heightTransitionSpeed = 5f; // Скорость перехода высоты камеры
    
    
    private bool isTransitioningHeight = false;

    #endregion
   
    
    private void OnEnable()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        _currentMouseSensitivity = defaultMouseSensitivity;
        targetFOV = defaultFOV;

        standCameraHeight = playerCamera.transform.localPosition.y;
        crouchCameraHeight = standCameraHeight / 2;
        targetCameraHeight = standCameraHeight;

        standControllerHeight = controller.height;
        crouchControllerHeight = standControllerHeight / 2;
        targetControllerHeight = standControllerHeight;

        PlayerState.Transform.Value = transform;
    }
    

    // Update is called once per frame
    void Update()
    {
        HandleMoveAndJump();
        HandleMouseRotation();
        HandleAiming();
    }

    private void HandleMoveAndJump()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.C)) // Клавиша C для приседания
        {
            PlayerState.IsPlayerCrouching.Value = !PlayerState.IsPlayerCrouching.Value;
            if (PlayerState.IsPlayerCrouching.Value)
            {
                targetCameraHeight = crouchCameraHeight;
                targetControllerHeight = crouchControllerHeight;
            }
            else
            {
                targetCameraHeight = standCameraHeight;
                targetControllerHeight = standControllerHeight;
            }
            isTransitioningHeight = true;
        }
        
        // Определение текущей скорости в зависимости от состояния Shift
        var speed = Input.GetKey(KeyCode.LeftShift) && isGrounded ? runSpeed : walkSpeed;
        
        float moveForwardBack = Input.GetAxis("Vertical");
        float moveLeftRight = Input.GetAxis("Horizontal");

        var isNotMoving = moveForwardBack == 0 && moveLeftRight == 0;
        
        if (isNotMoving)
        {
            if (isGrounded)
            {
                moveDirection = Vector3.zero;
            }
          
            UpdateMovementState(PlayerMovementStateType.Stay);
        }
        else if (isGrounded)
        {
            if (PlayerState.IsPlayerAiming.Value)
            {
                speed = aimMoveSpeed;
            } 
            else if (PlayerState.IsPlayerCrouching.Value)
            {
                speed = crouchSpeed;
            }
            
            if (speed == runSpeed)
            {
                UpdateMovementState(PlayerMovementStateType.Run);
            }
            else
            {
                UpdateMovementState(PlayerMovementStateType.Moving);
            }
            
            moveDirection = (transform.right * moveLeftRight + transform.forward * moveForwardBack) * speed;
        }
        
        controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            UpdateMovementState(PlayerMovementStateType.Jump);
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        HandleCrouchTransition();
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseRotation()
    {
        // Поворот камеры
        float mouseX = Input.GetAxis("Mouse X") * _currentMouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _currentMouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -cameraVerticalAngleThreshold, cameraVerticalAngleThreshold);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleAiming()
    {
        // Переключение режима прицеливания
        if (Input.GetButtonDown("Fire2")) // Правая кнопка мыши для прицеливания
        {
            isAiming = !isAiming;
            PlayerState.IsPlayerAiming.Value = isAiming; // TODO рефактор
            isTransitioningFOV = true;
            if (isAiming)
            {
                targetFOV = aimFOV;
                _currentMouseSensitivity = aimMouseSensitivity;
            }
            else
            {
                targetFOV = defaultFOV;
                _currentMouseSensitivity = defaultMouseSensitivity;
            }
        }
        
        // Плавное изменение угла обзора камеры
        if (isTransitioningFOV)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(playerCamera.fieldOfView - targetFOV) < 0.1f)
            {
                isTransitioningFOV = false;
            }
        }
    }

    private void UpdateMovementState(PlayerMovementStateType state)
    {
        if (PlayerState.PlayerMovementState.Value != state)
        {
            PlayerState.PlayerMovementState.Value = state;
        }
    }

    private void HandleCrouchTransition()
    {
        if (!isTransitioningHeight)
            return;
        
        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition, 
            new Vector3(0, targetCameraHeight, playerCamera.transform.localPosition.z), 
            heightTransitionSpeed * Time.deltaTime
        );

        controller.height =
            Mathf.Lerp(controller.height, targetControllerHeight, heightTransitionSpeed * Time.deltaTime);

        var completeTransition = Mathf.Abs(playerCamera.transform.localPosition.y - targetCameraHeight) < 0.01f
                                 && controller.height - targetControllerHeight < 0.01f;
            
        if (completeTransition)
        {
            controller.height = targetControllerHeight;
            playerCamera.transform.localPosition =
                new Vector3(0, targetCameraHeight, playerCamera.transform.localPosition.z);
            isTransitioningHeight = false;
        }
    }
}
