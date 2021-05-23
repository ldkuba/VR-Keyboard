using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class TypeInput : MonoBehaviour
{
    private const int LETTERS_PER_SQUARE = 9;

    public GameObject controllerLeft;
    public GameObject controllerRight;
    public InputField textEditor;

    public float joyThreshold;

    public enum JoyStick{
        LEFT,
        RIGHT
    }

    public enum LetterPosition
    {
        LEFT_TOP = 0, CENTER_TOP, RIGHT_TOP,
        LEFT_CENTER, CENTER_CENTER, RIGHT_CENTER,
        LEFT_BOTTOM, CENTER_BOTTOM, RIGHT_BOTTOM
    }

    public JoyStick primaryInput;

    private Dictionary<ValueTuple<LetterPosition, LetterPosition>, string> m_letters;
    // Config var for setting layout
    public KeyboardLayout layout;

    private Vector2 m_primaryJoy;
    private Vector2 m_secondaryJoy;

    private LetterPosition m_primaryPosition;
    private LetterPosition m_secondaryPosition;

    private bool m_isInputing;
    private LetterPosition m_activeSquare;

    void Start()
    {
        textEditor.ActivateInputField();
        textEditor.text = "";
        m_isInputing = false;

        m_letters = new Dictionary<ValueTuple<LetterPosition, LetterPosition>, string>();

        // Load layout into dictionary
        if(layout.squares.Count != LETTERS_PER_SQUARE)
        {
            Debug.LogError("Incorrect amount of square layouts!");
            return;
        }

        for(int j = 0; j < LETTERS_PER_SQUARE; j++)
        {
            if(layout.squares.Count != LETTERS_PER_SQUARE)
            {
                Debug.LogError("Incorrect amount of inputs in square: " + j);
                m_letters.Clear();
                return; 
            }

            for(int i = 0; i < LETTERS_PER_SQUARE; i++)
            {
                m_letters.Add(((LetterPosition)j, (LetterPosition)i), layout.GetInput(j, i));
            }
        }
    }

    void Update()
    {
        if(primaryInput == JoyStick.LEFT)
        {
            m_primaryJoy = SteamVR_Input.GetVector2("default", "JoyLeft", SteamVR_Input_Sources.LeftHand);
            m_secondaryJoy = SteamVR_Input.GetVector2("default", "JoyRight", SteamVR_Input_Sources.RightHand);
        }else if(primaryInput == JoyStick.RIGHT)
        {
            m_secondaryJoy = SteamVR_Input.GetVector2("default", "JoyLeft", SteamVR_Input_Sources.LeftHand);
            m_primaryJoy = SteamVR_Input.GetVector2("default", "JoyRight", SteamVR_Input_Sources.RightHand);
        }

        // Debug.Log("Left: " + primaryJoy);
        Debug.Log("Right: " + m_secondaryJoy);

        // Debug.Log("Caret pos: " + textEditor.caretPosition);
        // Debug.Log("Text: " + textEditor.text);

        m_primaryPosition = GetJoyPosition(m_primaryJoy);
        m_secondaryPosition = GetJoyPosition(m_secondaryJoy);

        //Debug.Log("Secondary Pos: " + secondaryPosition);

        if(m_isInputing)
        {
            if(m_primaryPosition == LetterPosition.CENTER_CENTER)
            {
                // submit selected input
                SubmitText(m_letters[(m_activeSquare, m_secondaryPosition)]);
                // Hide square
                ShowSecondarySquare(LetterPosition.CENTER_CENTER);
                m_isInputing = false;
            }
        }else
        {
            if(m_primaryPosition != LetterPosition.CENTER_CENTER)
            {
                m_isInputing = true;
                ShowSecondarySquare(m_primaryPosition);
            }
        }
    }

    LetterPosition GetJoyPosition(Vector2 input)
    {
        float angle = Vector2.Angle(Vector2.right, input);
        float length = input.magnitude;

        if(length > joyThreshold)
        {
            if(angle <= 22.5f)
            {
                return LetterPosition.RIGHT_CENTER;
            }else if(angle > 22.5f && angle <= 67.5f)
            {
                if(input.y > 0)
                {
                    return LetterPosition.RIGHT_TOP;
                }else
                {
                    return LetterPosition.RIGHT_BOTTOM;
                }
            }else if(angle > 67.5f && angle <= 112.5f)
            {
                if(input.y > 0)
                {
                    return LetterPosition.CENTER_TOP;
                }else
                {
                    return LetterPosition.CENTER_BOTTOM;
                }
            }else if(angle > 112.5f && angle <= 157.5f)
            {
                if(input.y > 0)
                {
                    return LetterPosition.LEFT_TOP;
                }else
                {
                    return LetterPosition.LEFT_BOTTOM;
                }
            }else
            {
                return LetterPosition.LEFT_CENTER;
            }
        }else
        {
            return LetterPosition.CENTER_CENTER;
        }
    }

    void ShowSecondarySquare(LetterPosition pos)
    {
        // Center-center means "no selection" -> hide secondary square
        m_activeSquare = pos;

        // TODO: show visual representaiton of selected square
    }

    void SubmitText(string text)
    {
        textEditor.text = textEditor.text.Insert(textEditor.caretPosition, text);
    }
}
