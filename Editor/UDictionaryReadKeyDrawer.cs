/**
*   Authored by Tomasz Piowczyk
*   MIT LICENSE: https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/LICENSE
*   Repository: https://github.com/Prastiwar/UnitySerializedDictionary
*/

using UnityEditor;
using UnityEngine;

public class UDictionaryReadKeyDrawer : UDictionaryDrawer
{
    protected override Rect DrawPropertiesForElement(Rect keyRect, Rect valueRect, SerializedProperty keyProp, SerializedProperty valueProp)
    {
        bool wasEnabled = GUI.enabled;
        GUI.enabled = false;
        EditorGUI.PropertyField(keyRect, keyProp, GUIContent(keyProp.type), true);
        GUI.enabled = wasEnabled;
        EditorGUI.PropertyField(valueRect, valueProp, GUIContent(valueProp.type), true);
        return keyRect;
    }
}
