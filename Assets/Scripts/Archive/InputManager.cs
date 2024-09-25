// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class InputManager : MonoBehaviour
// {
//     public static InputManager Instance { get; private set; }

//     public bool IsTerminalOpen { get; set; }  // This flag can be set from the terminal class

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     public bool CanProcessGameInput()
//     {
//         return !IsTerminalOpen;  // If the terminal is open, block game input
//     }
// }
