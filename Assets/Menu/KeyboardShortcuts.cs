using UnityEngine;
using TMPro;
using WebGLCopyAndPaste;

public class KeyboardShortcuts : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    void Update()
    {
        // Check if InputField is focused
        if (inputField.isFocused)
        {
            // Ctrl+C or Cmd+C - Copy
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) 
                && Input.GetKeyDown(KeyCode.C))
            {
                OnCopy();
            }

            // Ctrl+V or Cmd+V - Paste
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) 
                && Input.GetKeyDown(KeyCode.V))
            {
                OnPaste();
            }

            // Ctrl+X or Cmd+X - Cut
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) 
                && Input.GetKeyDown(KeyCode.X))
            {
                OnCut();
            }
        }
    }

    void OnCopy()
    {
        WebGLCopyAndPasteAPI.CopyToClipboard(inputField.text);
        GUIUtility.systemCopyBuffer = inputField.text;
        Debug.Log("Copied!");
    }

    void OnPaste()
    {
        inputField.text = GUIUtility.systemCopyBuffer;
        Debug.Log("Pasted!");
    }

    void OnCut()
    {
        inputField.text = "";
        Debug.Log("Cut!");
    }
}