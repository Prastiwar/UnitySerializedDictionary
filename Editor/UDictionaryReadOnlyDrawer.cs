/**
*   Authored by Tomasz Piowczyk
*   MIT LICENSE: https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/LICENSE
*   Repository: https://github.com/Prastiwar/UnitySerializedDictionary
*/

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class UDictionaryReadOnlyDrawer : UDictionaryDrawer
{
    private bool wasEnabled;

    protected override ReorderableList CreateList(SerializedObject serializedObj, SerializedProperty elements)
    {
        return new ReorderableList(serializedObj, elements, false, true, false, false) {
            drawHeaderCallback = DrawHeader,
            elementHeightCallback = GetReorderableElementHeight,
            drawElementCallback = DrawElement,
        };
    }

    protected override void OnBeforeDrawProperties()
    {
        wasEnabled = GUI.enabled;
        GUI.enabled = false;
    }

    protected override void OnAfterDrawProperties()
    {
        GUI.enabled = wasEnabled;
    }
}
