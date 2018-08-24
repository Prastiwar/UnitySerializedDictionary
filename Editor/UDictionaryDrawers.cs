using UnityEditor;

/// <summary>  You need to add CustomPropertyDrawer attribute to draw custom dictionary with my default drawer </summary>
[CustomPropertyDrawer(typeof(UDictionaryDoubleClass))]
[CustomPropertyDrawer(typeof(UDictionaryStructStruct))]
[CustomPropertyDrawer(typeof(UDictionaryIntColor))]
[CustomPropertyDrawer(typeof(UDictionaryIntStruct))]
[CustomPropertyDrawer(typeof(UDictionaryStringInt))]
internal class UDictionaryDrawers : UDictionaryDrawer { }
