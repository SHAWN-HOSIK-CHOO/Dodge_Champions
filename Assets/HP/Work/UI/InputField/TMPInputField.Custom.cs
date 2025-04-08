using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static InputSystemNaming;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace HP
{
    public partial class TMPInputField
    {
        public interface IInputMode
        {
            void ChangeModeNext();
            string GetName();
        }

        private IInputMode _inputMode;
        public IInputMode InputMode
        {
            get => _inputMode;
            set
            {
                _inputMode = value;
                _inputModeText.text = _inputMode.GetName();
            }
        }
        [SerializeField]
        TMP_Text _inputModeText;
        [SerializeField]
        public bool _useAutoFocus = false;
        [SerializeField]
        public bool _resetOnSubmit = true;
        bool initialized; //아니 왜 start랑 destroy 두번 불림? 
        protected override void Start()
        {
            if (!Application.isPlaying) return;
            if (initialized == false)
            {
                onFocusSelectAll = true;
                initialized = true;
            }
        }
        protected EditState KeyPressed(Event evt)
        {
            var currentEventModifiers = evt.modifiers;
            bool ctrl = m_IsApplePlatform ? (currentEventModifiers & EventModifiers.Command) != 0 : (currentEventModifiers & EventModifiers.Control) != 0;
            bool shift = (currentEventModifiers & EventModifiers.Shift) != 0;
            bool alt = (currentEventModifiers & EventModifiers.Alt) != 0;
            bool ctrlOnly = ctrl && !alt && !shift;
            m_LastKeyCode = evt.keyCode;

            switch (evt.keyCode)
            {
                case KeyCode.Backspace:
                    {
                        Backspace();
                        return EditState.Continue;
                    }

                case KeyCode.Delete:
                    {
                        DeleteKey();
                        return EditState.Continue;
                    }

                case KeyCode.Home:
                    {
                        MoveToStartOfLine(shift, ctrl);
                        return EditState.Continue;
                    }

                case KeyCode.End:
                    {
                        MoveToEndOfLine(shift, ctrl);
                        return EditState.Continue;
                    }

                // Select All
                case KeyCode.A:
                    {
                        if (ctrlOnly)
                        {
                            SelectAll();
                            return EditState.Continue;
                        }
                        break;
                    }

                // Copy
                case KeyCode.C:
                    {
                        if (ctrlOnly)
                        {
                            if (inputType != InputType.Password)
                                clipboard = GetSelectedString();
                            else
                                clipboard = "";
                            return EditState.Continue;
                        }
                        break;
                    }

                // Paste
                case KeyCode.V:
                    {
                        if (ctrlOnly)
                        {
                            Append(clipboard);
                            return EditState.Continue;
                        }
                        break;
                    }

                // Cut
                case KeyCode.X:
                    {
                        if (ctrlOnly)
                        {
                            if (inputType != InputType.Password)
                                clipboard = GetSelectedString();
                            else
                                clipboard = "";
                            Delete();
                            UpdateTouchKeyboardFromEditChanges();
                            SendOnValueChangedAndUpdateLabel();
                            return EditState.Continue;
                        }
                        break;
                    }

                case KeyCode.LeftArrow:
                    {
                        MoveLeft(shift, ctrl);
                        return EditState.Continue;
                    }

                case KeyCode.RightArrow:
                    {
                        MoveRight(shift, ctrl);
                        return EditState.Continue;
                    }

                case KeyCode.UpArrow:
                    {
                        MoveUp(shift);
                        return EditState.Continue;
                    }

                case KeyCode.DownArrow:
                    {
                        MoveDown(shift);
                        return EditState.Continue;
                    }

                case KeyCode.PageUp:
                    {
                        MovePageUp(shift);
                        return EditState.Continue;
                    }

                case KeyCode.PageDown:
                    {
                        MovePageDown(shift);
                        return EditState.Continue;
                    }

                // Submit
                case KeyCode.Return:
                    break;
                case KeyCode.KeypadEnter:
                    {
                        if (lineType != LineType.MultiLineNewline)
                        {
                            m_ReleaseSelection = true;
                            return EditState.Finish;
                        }
                        else
                        {
                            TMP_TextInfo textInfo = m_TextComponent.textInfo;

                            if (m_LineLimit > 0 && textInfo != null && textInfo.lineCount >= m_LineLimit)
                            {
                                m_ReleaseSelection = true;
                                return EditState.Finish;
                            }
                        }
                        break;
                    }

                case KeyCode.Escape:
                    {
                        m_ReleaseSelection = true;
                        m_WasCanceled = true;
                        return EditState.Finish;
                    }
            }

            char c = evt.character;
            if (!multiLine && (evt.character == '\r' || evt.character == '\n'))
            {
                return EditState.Finish;
            }
            else if (multiLine)
            {
                if (InputMode != null && !shift && c == '\t')
                {
                    InputMode.ChangeModeNext();
                    _inputModeText.text = InputMode.GetName();
                    return EditState.Continue;
                }
                // Convert carriage return and end-of-text characters to newline.
                if (c == '\r' || c == 3)
                    c = '\n';
                // Convert Shift Enter to Vertical tab
                if (shift && c == '\n')
                    c = '\v';
                if (IsValidChar(c))
                {
                    if (multiLine && (c == '\n'))
                        return EditState.Finish;
                    Append(c);
                }
            }
            return EditState.Continue;
        }
        public virtual void OnSubmit(BaseEventData eventData)
        {
            eventData?.Use();
        }
        protected void SendOnSubmit()
        {
            if (onSubmit != null)
                onSubmit.Invoke(m_Text);
            if (m_Text != string.Empty)
            {
                ActivateInputField();
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            if (_resetOnSubmit)
                text = string.Empty;
        }
        protected override void OnDestroy()
        {
            if (!Application.isPlaying) return;
            initialized = false;
        }
        protected virtual void LateUpdate()
        {
            // Only activate if we are not already activated.
            if (m_ShouldActivateNextUpdate)
            {
                if (!isFocused)
                {
                    ActivateInputFieldInternal();
                    m_ShouldActivateNextUpdate = false;
                    return;
                }

                // Reset as we are already activated.
                m_ShouldActivateNextUpdate = false;
            }

            // If the device's state changed in a way that affects whether we should use a touchscreen keyboard or not,
            // then deactivate the input field.
            if (isFocused && InPlaceEditingChanged())
                DeactivateInputField();

            // Handle double click to reset / deselect Input Field when ResetOnActivation is false.
            if (!isFocused && m_SelectionStillActive)
            {
                GameObject selectedObject = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;

                if (selectedObject == null && m_ResetOnDeActivation)
                {
                    ReleaseSelection();
                    return;
                }

                if (selectedObject != null && selectedObject != this.gameObject)
                {
                    if (selectedObject == m_PreviouslySelectedObject)
                        return;

                    m_PreviouslySelectedObject = selectedObject;

                    // Special handling for Vertical Scrollbar
                    if (m_VerticalScrollbar && selectedObject == m_VerticalScrollbar.gameObject)
                    {
                        // Do not release selection
                        return;
                    }

                    // Release selection for all objects when ResetOnDeActivation is true
                    if (m_ResetOnDeActivation)
                    {
                        ReleaseSelection();
                        return;
                    }

                    // Release current selection of selected object is another Input Field
                    if (m_KeepTextSelectionVisible == false && selectedObject.GetComponent<TMP_InputField>() != null)
                        ReleaseSelection();

                    return;
                }

#if ENABLE_INPUT_SYSTEM
                if (m_ProcessingEvent != null && m_ProcessingEvent.rawType == EventType.MouseDown && m_ProcessingEvent.button == 0)
                {
                    // Check for Double Click
                    bool isDoubleClick = false;
                    float timeStamp = Time.unscaledTime;

                    if (m_KeyDownStartTime + m_DoubleClickDelay > timeStamp)
                        isDoubleClick = true;

                    m_KeyDownStartTime = timeStamp;

                    if (isDoubleClick)
                    {
                        //m_StringPosition = m_StringSelectPosition = 0;
                        //m_CaretPosition = m_CaretSelectPosition = 0;
                        //m_TextComponent.rectTransform.localPosition = m_DefaultTransformPosition;

                        //if (caretRectTrans != null)
                        //    caretRectTrans.localPosition = Vector3.zero;

                        ReleaseSelection();

                        return;
                    }
                }
#else
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // Check for Double Click
                    bool isDoubleClick = false;
                    float timeStamp = Time.unscaledTime;

                    if (m_KeyDownStartTime + m_DoubleClickDelay > timeStamp)
                        isDoubleClick = true;

                    m_KeyDownStartTime = timeStamp;

                    if (isDoubleClick)
                    {
                        //m_StringPosition = m_StringSelectPosition = 0;
                        //m_CaretPosition = m_CaretSelectPosition = 0;
                        //m_TextComponent.rectTransform.localPosition = m_DefaultTransformPosition;

                        //if (caretRectTrans != null)
                        //    caretRectTrans.localPosition = Vector3.zero;

                        ReleaseSelection();

                        return;
                    }
                }
#endif
            }

            UpdateMaskRegions();

            if (InPlaceEditing() && isKeyboardUsingEvents() || !isFocused)
            {
                return;
            }

            AssignPositioningIfNeeded();

            if (m_SoftKeyboard == null || m_SoftKeyboard.status != TouchScreenKeyboard.Status.Visible)
            {
                if (m_SoftKeyboard != null)
                {
                    if (!m_ReadOnly)
                        text = m_SoftKeyboard.text;

                    TouchScreenKeyboard.Status status = m_SoftKeyboard.status;

                    // Special handling for UWP - Hololens which does not support Canceled status
                    if (m_LastKeyCode != KeyCode.Return && status == TouchScreenKeyboard.Status.Done && isUWP())
                    {
                        status = TouchScreenKeyboard.Status.Canceled;
                        // The HoloLen's X button will not be acting as an ESC Key (TMBP-98)
                        m_IsKeyboardBeingClosedInHoloLens = true;
                    }

                    switch (status)
                    {
                        case TouchScreenKeyboard.Status.LostFocus:
                            SendTouchScreenKeyboardStatusChanged();
                            break;
                        case TouchScreenKeyboard.Status.Canceled:
                            m_ReleaseSelection = true;
                            m_WasCanceled = true;
                            SendTouchScreenKeyboardStatusChanged();
                            break;
                        case TouchScreenKeyboard.Status.Done:
                            m_ReleaseSelection = true;
                            SendTouchScreenKeyboardStatusChanged();
                            OnSubmit(null);
                            break;
                    }
                }

                OnDeselect(null);
                return;
            }

            string val = m_SoftKeyboard.text;

            if (m_Text != val)
            {
                if (m_ReadOnly)
                {
                    m_SoftKeyboard.text = m_Text;
                }
                else
                {
                    m_Text = "";

                    for (int i = 0; i < val.Length; ++i)
                    {
                        char c = val[i];
                        bool hasValidateUpdatedText = false;

                        if (c == '\r' || c == 3)
                            c = '\n';

                        if (onValidateInput != null)
                            c = onValidateInput(m_Text, m_Text.Length, c);
                        else if (characterValidation != CharacterValidation.None)
                        {
                            string textBeforeValidate = m_Text;
                            c = Validate(m_Text, m_Text.Length, c);
                            hasValidateUpdatedText = textBeforeValidate != m_Text;
                        }

                        if (lineType != LineType.MultiLineNewline && c == '\n')
                        {
                            UpdateLabel();

                            OnSubmit(null);
                            OnDeselect(null);
                            return;
                        }

                        // In the case of a Custom Validator, the user is expected to modify the m_Text where as such we do not append c.
                        // However we will append c if the user did not modify the m_Text (UUM-42147)
                        if (c != 0 && (characterValidation != CharacterValidation.CustomValidator || !hasValidateUpdatedText))
                            m_Text += c;
                    }

                    if (characterLimit > 0 && m_Text.Length > characterLimit)
                        m_Text = m_Text.Substring(0, characterLimit);

                    UpdateStringPositionFromKeyboard();

                    // Set keyboard text before updating label, as we might have changed it with validation
                    // and update label will take the old value from keyboard if we don't change it here
                    if (m_Text != val)
                        m_SoftKeyboard.text = m_Text;

                    SendOnValueChangedAndUpdateLabel();
                }
            }
            // On iOS/tvOS we always have TouchScreenKeyboard instance even when using external keyboard
            // so we keep track of the caret position there
            else if (m_HideMobileInput && m_SoftKeyboard != null && m_SoftKeyboard.canSetSelection &&
                     Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.tvOS)
            {
                var selectionStart = Mathf.Min(stringSelectPositionInternal, stringPositionInternal);
                var selectionLength = Mathf.Abs(stringSelectPositionInternal - stringPositionInternal);
                m_SoftKeyboard.selection = new RangeInt(selectionStart, selectionLength);
            }
            else if (m_HideMobileInput && Application.platform == RuntimePlatform.Android ||
                     m_SoftKeyboard.canSetSelection && (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS))
            {
                UpdateStringPositionFromKeyboard();
            }

            //else if (m_HideMobileInput) // m_Keyboard.canSetSelection
            //{
            //    int length = stringPositionInternal < stringSelectPositionInternal ? stringSelectPositionInternal - stringPositionInternal : stringPositionInternal - stringSelectPositionInternal;
            //    m_SoftKeyboard.selection = new RangeInt(stringPositionInternal < stringSelectPositionInternal ? stringPositionInternal : stringSelectPositionInternal, length);
            //}
            //else if (!m_HideMobileInput) // m_Keyboard.canGetSelection)
            //{
            //    UpdateStringPositionFromKeyboard();
            //}

            if (m_SoftKeyboard != null && m_SoftKeyboard.status != TouchScreenKeyboard.Status.Visible)
            {
                if (m_SoftKeyboard.status == TouchScreenKeyboard.Status.Canceled)
                    m_WasCanceled = true;

                OnDeselect(null);
            }
        }
    }
}