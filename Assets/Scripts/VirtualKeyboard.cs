using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

/*
    DON'T FORGET TO RUN THIS THING RIGHT HERE ONCE ON YOUR APPLICATION'S SETUP
    if (InputSystem.GetDevice<VirtualKeyboardDevice>() == null)
    {
        InputSystem.AddDevice<VirtualKeyboardDevice>();
    }
*/

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(displayName = "Virtual Keyboard", stateType = typeof(KeyboardState))]
public class VirtualKeyboardDevice : InputDevice, IInputUpdateCallbackReceiver
{
    private InputDevice actualKeyboard;
    private KeyboardState keyboardState;

    static VirtualKeyboardDevice()
    {
        InputSystem.RegisterLayout<VirtualKeyboardDevice>();
    }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        actualKeyboard = InputSystem.GetDevice("Keyboard");
        keyboardState = new KeyboardState();
    }

    public void OnUpdate()
    {
        actualKeyboard.CopyState<KeyboardState>(out keyboardState);
        InputSystem.QueueStateEvent(this, keyboardState);
    }
}