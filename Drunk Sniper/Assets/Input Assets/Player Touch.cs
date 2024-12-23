//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/Input Assets/Player Touch.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerTouch: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerTouch()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Touch"",
    ""maps"": [
        {
            ""name"": ""Touch"",
            ""id"": ""93a3a9c6-f0a4-4284-b9cd-bfba946576e0"",
            ""actions"": [
                {
                    ""name"": ""Primary Contact"",
                    ""type"": ""PassThrough"",
                    ""id"": ""ec08b77e-3edc-4a47-8f85-df95fa130ac6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""primary Postion"",
                    ""type"": ""PassThrough"",
                    ""id"": ""b4905d58-5ed3-4aa5-81ab-4ab503144e25"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""a1dc5e20-dbd4-4cba-aff8-dd6c0350a1f7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePos"",
                    ""type"": ""Button"",
                    ""id"": ""64c1afd9-e424-4a05-8def-882116ae8fa2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2ad933b2-fdea-4fd4-a2f7-93cfd2daf694"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Primary Contact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""efa0856b-5d5b-4efd-9bea-c1a356069e66"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""primary Postion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73402f66-8d4b-452f-94c3-a406c51b6965"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""18cb64aa-2285-4250-95a2-f8a533d0189a"",
                    ""path"": ""*/{Back}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""660914cb-c4a2-4e16-a2ea-d11e50edfaf4"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Touch
        m_Touch = asset.FindActionMap("Touch", throwIfNotFound: true);
        m_Touch_PrimaryContact = m_Touch.FindAction("Primary Contact", throwIfNotFound: true);
        m_Touch_primaryPostion = m_Touch.FindAction("primary Postion", throwIfNotFound: true);
        m_Touch_Pause = m_Touch.FindAction("Pause", throwIfNotFound: true);
        m_Touch_MousePos = m_Touch.FindAction("MousePos", throwIfNotFound: true);
    }

    ~@PlayerTouch()
    {
        UnityEngine.Debug.Assert(!m_Touch.enabled, "This will cause a leak and performance issues, PlayerTouch.Touch.Disable() has not been called.");
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Touch
    private readonly InputActionMap m_Touch;
    private List<ITouchActions> m_TouchActionsCallbackInterfaces = new List<ITouchActions>();
    private readonly InputAction m_Touch_PrimaryContact;
    private readonly InputAction m_Touch_primaryPostion;
    private readonly InputAction m_Touch_Pause;
    private readonly InputAction m_Touch_MousePos;
    public struct TouchActions
    {
        private @PlayerTouch m_Wrapper;
        public TouchActions(@PlayerTouch wrapper) { m_Wrapper = wrapper; }
        public InputAction @PrimaryContact => m_Wrapper.m_Touch_PrimaryContact;
        public InputAction @primaryPostion => m_Wrapper.m_Touch_primaryPostion;
        public InputAction @Pause => m_Wrapper.m_Touch_Pause;
        public InputAction @MousePos => m_Wrapper.m_Touch_MousePos;
        public InputActionMap Get() { return m_Wrapper.m_Touch; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchActions set) { return set.Get(); }
        public void AddCallbacks(ITouchActions instance)
        {
            if (instance == null || m_Wrapper.m_TouchActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_TouchActionsCallbackInterfaces.Add(instance);
            @PrimaryContact.started += instance.OnPrimaryContact;
            @PrimaryContact.performed += instance.OnPrimaryContact;
            @PrimaryContact.canceled += instance.OnPrimaryContact;
            @primaryPostion.started += instance.OnPrimaryPostion;
            @primaryPostion.performed += instance.OnPrimaryPostion;
            @primaryPostion.canceled += instance.OnPrimaryPostion;
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @MousePos.started += instance.OnMousePos;
            @MousePos.performed += instance.OnMousePos;
            @MousePos.canceled += instance.OnMousePos;
        }

        private void UnregisterCallbacks(ITouchActions instance)
        {
            @PrimaryContact.started -= instance.OnPrimaryContact;
            @PrimaryContact.performed -= instance.OnPrimaryContact;
            @PrimaryContact.canceled -= instance.OnPrimaryContact;
            @primaryPostion.started -= instance.OnPrimaryPostion;
            @primaryPostion.performed -= instance.OnPrimaryPostion;
            @primaryPostion.canceled -= instance.OnPrimaryPostion;
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @MousePos.started -= instance.OnMousePos;
            @MousePos.performed -= instance.OnMousePos;
            @MousePos.canceled -= instance.OnMousePos;
        }

        public void RemoveCallbacks(ITouchActions instance)
        {
            if (m_Wrapper.m_TouchActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ITouchActions instance)
        {
            foreach (var item in m_Wrapper.m_TouchActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_TouchActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public TouchActions @Touch => new TouchActions(this);
    public interface ITouchActions
    {
        void OnPrimaryContact(InputAction.CallbackContext context);
        void OnPrimaryPostion(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnMousePos(InputAction.CallbackContext context);
    }
}
