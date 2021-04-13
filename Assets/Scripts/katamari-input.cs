// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/katamari-input.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Katamariinput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Katamariinput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""katamari-input"",
    ""maps"": [
        {
            ""name"": ""Katamari"",
            ""id"": ""054fc63f-3c3e-4154-b795-a9798c8f49c0"",
            ""actions"": [
                {
                    ""name"": ""Left Throttle"",
                    ""type"": ""Value"",
                    ""id"": ""c3a5362b-e505-4d2e-9398-4d8982928d21"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right Throttle"",
                    ""type"": ""Value"",
                    ""id"": ""911a8557-e2ed-4681-a983-fe3d01b333cf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0ea3b25e-a7b1-4665-9991-a6467e66259c"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Left Throttle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""35abe837-4780-49c3-b233-282789b503f0"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Right Throttle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard&Mouse"",
            ""bindingGroup"": ""Keyboard&Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Joystick"",
            ""bindingGroup"": ""Joystick"",
            ""devices"": [
                {
                    ""devicePath"": ""<Joystick>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""XR"",
            ""bindingGroup"": ""XR"",
            ""devices"": [
                {
                    ""devicePath"": ""<XRController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Katamari
        m_Katamari = asset.FindActionMap("Katamari", throwIfNotFound: true);
        m_Katamari_LeftThrottle = m_Katamari.FindAction("Left Throttle", throwIfNotFound: true);
        m_Katamari_RightThrottle = m_Katamari.FindAction("Right Throttle", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Katamari
    private readonly InputActionMap m_Katamari;
    private IKatamariActions m_KatamariActionsCallbackInterface;
    private readonly InputAction m_Katamari_LeftThrottle;
    private readonly InputAction m_Katamari_RightThrottle;
    public struct KatamariActions
    {
        private @Katamariinput m_Wrapper;
        public KatamariActions(@Katamariinput wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftThrottle => m_Wrapper.m_Katamari_LeftThrottle;
        public InputAction @RightThrottle => m_Wrapper.m_Katamari_RightThrottle;
        public InputActionMap Get() { return m_Wrapper.m_Katamari; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KatamariActions set) { return set.Get(); }
        public void SetCallbacks(IKatamariActions instance)
        {
            if (m_Wrapper.m_KatamariActionsCallbackInterface != null)
            {
                @LeftThrottle.started -= m_Wrapper.m_KatamariActionsCallbackInterface.OnLeftThrottle;
                @LeftThrottle.performed -= m_Wrapper.m_KatamariActionsCallbackInterface.OnLeftThrottle;
                @LeftThrottle.canceled -= m_Wrapper.m_KatamariActionsCallbackInterface.OnLeftThrottle;
                @RightThrottle.started -= m_Wrapper.m_KatamariActionsCallbackInterface.OnRightThrottle;
                @RightThrottle.performed -= m_Wrapper.m_KatamariActionsCallbackInterface.OnRightThrottle;
                @RightThrottle.canceled -= m_Wrapper.m_KatamariActionsCallbackInterface.OnRightThrottle;
            }
            m_Wrapper.m_KatamariActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftThrottle.started += instance.OnLeftThrottle;
                @LeftThrottle.performed += instance.OnLeftThrottle;
                @LeftThrottle.canceled += instance.OnLeftThrottle;
                @RightThrottle.started += instance.OnRightThrottle;
                @RightThrottle.performed += instance.OnRightThrottle;
                @RightThrottle.canceled += instance.OnRightThrottle;
            }
        }
    }
    public KatamariActions @Katamari => new KatamariActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard&Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_TouchSchemeIndex = -1;
    public InputControlScheme TouchScheme
    {
        get
        {
            if (m_TouchSchemeIndex == -1) m_TouchSchemeIndex = asset.FindControlSchemeIndex("Touch");
            return asset.controlSchemes[m_TouchSchemeIndex];
        }
    }
    private int m_JoystickSchemeIndex = -1;
    public InputControlScheme JoystickScheme
    {
        get
        {
            if (m_JoystickSchemeIndex == -1) m_JoystickSchemeIndex = asset.FindControlSchemeIndex("Joystick");
            return asset.controlSchemes[m_JoystickSchemeIndex];
        }
    }
    private int m_XRSchemeIndex = -1;
    public InputControlScheme XRScheme
    {
        get
        {
            if (m_XRSchemeIndex == -1) m_XRSchemeIndex = asset.FindControlSchemeIndex("XR");
            return asset.controlSchemes[m_XRSchemeIndex];
        }
    }
    public interface IKatamariActions
    {
        void OnLeftThrottle(InputAction.CallbackContext context);
        void OnRightThrottle(InputAction.CallbackContext context);
    }
}
