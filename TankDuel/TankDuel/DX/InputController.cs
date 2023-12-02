using System;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Windows;

namespace DX
{
    class InputController : IDisposable
    {
        // Экземпляр объекта "прямого ввода"
        private DirectInput _directInput;

        private Keyboard _keyboard;
        private KeyboardState _keyboardState;
        public KeyboardState KeyboardState { get => _keyboardState; }
        private bool _keyboardUpdated;
        public bool KeyboardUpdated { get => _keyboardUpdated; }
        private bool _keyboardAcquired;
        private bool _keyQPrevios;
        private bool _keyQCurrent;
        private bool _keyQ;
        public bool KeyQ { get => _keyQ; }
        private bool _keyZPrevios;
        private bool _keyZCurrent;
        private bool _keyZ;
        public bool KeyZ { get => _keyZ; }

        private Mouse _mouse;
        private MouseState _mouseState;
        public MouseState MouseState { get => _mouseState; }
        private bool _mouseUpdated;
        public bool MouseUpdated { get => _mouseUpdated; }
        private bool _mouseAcquired;
        private bool _previousLBM;
        private bool _currentLBM;
        private bool _lmb;
        public bool LMB { get => _lmb; }

        public InputController(RenderForm renderForm)
        {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.Properties.BufferSize = 16;
            _keyboard.SetCooperativeLevel(renderForm.Handle, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
            AcquireKeyboard();
            _keyboardState = new KeyboardState();

            _mouse = new Mouse(_directInput);
            _mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            _mouse.SetCooperativeLevel(renderForm.Handle, CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
            AcquireMouse();
            _mouseState = new MouseState();
        }

        private void AcquireKeyboard()
        {
            try
            {
                _keyboard.Acquire();
                _keyboardAcquired = true;
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Failure)
                    _keyboardAcquired = false;
            }
        }

        private void AcquireMouse()
        {
            try
            {
                _mouse.Acquire();
                _mouseAcquired = true;
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Failure)
                    _mouseAcquired = false;
            }
        }
        private bool TriggerByKeyDown(Key key, ref bool previos, ref bool current)
        {
            previos = current;
            current = _keyboardState.IsPressed(key);
            return !previos && current;
        }

        private void ProcessKeyboardState()
        {
            _keyQ = TriggerByKeyDown(Key.Q, ref _keyQPrevios, ref _keyQCurrent);
            _keyZ = TriggerByKeyDown(Key.Z, ref _keyZPrevios, ref _keyZCurrent);
        }

        public void UpdateKeyboardState()
        {
            if (!_keyboardAcquired) AcquireKeyboard();
            ResultDescriptor resultCode = ResultCode.Ok;
            try
            {
                _keyboard.GetCurrentState(ref _keyboardState);
                ProcessKeyboardState();
                _keyboardUpdated = true;
            }
            catch (SharpDXException e)
            {
                resultCode = e.Descriptor;
                _keyboardUpdated = false;
            }

            if (resultCode == ResultCode.InputLost || resultCode == ResultCode.NotAcquired)
                _keyboardAcquired = false;
        }

        private bool TriggerMouseClick(int index, ref bool previous, ref bool current)
        {
            previous = current;
            current = MouseState.Buttons[index];
            return !previous && current;
        }

        private void ProccessMouseState()
        {
            _lmb = TriggerMouseClick(0, ref _previousLBM, ref _currentLBM);
        }

        public void UpdateMouseState()
        {
            if (!_mouseAcquired) AcquireMouse();
            ResultDescriptor resultCode = ResultCode.Ok;
            try
            {
                _mouse.GetCurrentState(ref _mouseState);
                ProccessMouseState();
                _mouseUpdated = true;
            }
            catch (SharpDXException e)
            {
                resultCode = e.Descriptor;
                _mouseUpdated = false;
            }
            if (resultCode == ResultCode.InputLost || resultCode == ResultCode.NotAcquired)
                _mouseAcquired = false;
        }

        public void Dispose()
        {
            Utilities.Dispose(ref _mouse);
            Utilities.Dispose(ref _keyboard);
            Utilities.Dispose(ref _directInput);
        }
    }
}