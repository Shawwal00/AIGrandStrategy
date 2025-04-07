using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputKeys : MonoBehaviour
{
    private Actions controls;
    
    //Used in other scripts
    [HideInInspector] public Vector2 movement;
    [HideInInspector] public float cameraZoom;
    [HideInInspector] public float rightClick;
    [HideInInspector] public float leftClick;
    [HideInInspector] public float menu;
    [HideInInspector] public float playGame;
    
    private void Awake()
    {
        controls = new Actions();
        controls.Gameplay.Enable();

        //Setting up all of the inputs
        controls.Gameplay.CameraMovement.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.Gameplay.CameraMovement.canceled += ctx => movement = Vector2.zero;
        
        controls.Gameplay.CameraZoom.performed += ctx => cameraZoom = ctx.ReadValue<float>();
        controls.Gameplay.CameraZoom.canceled += ctx => cameraZoom = 0;

        controls.Gameplay.RightClick.performed += ctx => rightClick = ctx.ReadValue<float>();
        controls.Gameplay.RightClick.canceled += ctx => rightClick = 0;
        
        controls.Gameplay.LeftClick.performed += ctx => leftClick = ctx.ReadValue<float>();
        controls.Gameplay.LeftClick.canceled += ctx => leftClick = 0;
        
        controls.Gameplay.Menu.performed += ctx => menu = ctx.ReadValue<float>();
        controls.Gameplay.Menu.canceled += ctx => menu = 0;
        
        controls.Gameplay.Start.performed += ctx => playGame = ctx.ReadValue<float>();
        controls.Gameplay.Start.canceled += ctx => playGame = 0;
    }
}

