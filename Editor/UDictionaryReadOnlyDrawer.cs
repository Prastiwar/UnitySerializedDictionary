/**
*   Authored by Tomasz Piowczyk
*   MIT LICENSE: https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/LICENSE
*   Repository: https://github.com/Prastiwar/UnitySerializedDictionary
*/

using UnityEngine;

public class UDictionaryReadOnlyDrawer : UDictionaryDrawer
{
    private bool wasEnabled;

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
