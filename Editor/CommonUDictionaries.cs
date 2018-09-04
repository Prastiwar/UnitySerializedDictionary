using UnityEditor;

/// <summary> You need to add CustomPropertyDrawer attribute to draw custom dictionary with my default drawer </summary>
[CustomPropertyDrawer(typeof(UDictionaryStringString))]
[CustomPropertyDrawer(typeof(UDictionaryStringInt))]
[CustomPropertyDrawer(typeof(UDictionaryStringBool))]
[CustomPropertyDrawer(typeof(UDictionaryStringFloat))]
[CustomPropertyDrawer(typeof(UDictionaryStringDouble))]
[CustomPropertyDrawer(typeof(UDictionaryStringColor))]
[CustomPropertyDrawer(typeof(UDictionaryStringObject))]
[CustomPropertyDrawer(typeof(UDictionaryIntInt))]
[CustomPropertyDrawer(typeof(UDictionaryIntBool))]
[CustomPropertyDrawer(typeof(UDictionaryIntString))]
[CustomPropertyDrawer(typeof(UDictionaryIntFloat))]
[CustomPropertyDrawer(typeof(UDictionaryIntDouble))]
[CustomPropertyDrawer(typeof(UDictionaryIntColor))]
[CustomPropertyDrawer(typeof(UDictionaryIntObject))]
internal class UDictionaryDrawers : UDictionaryDrawer { }

/// <summary> You need to add CustomPropertyDrawer attribute to draw custom dictionary with my default readonly drawer </summary>
//[CustomPropertyDrawer(typeof(UDictionaryIntString))]
internal class UDictionaryReadOnlyDrawers : UDictionaryReadOnlyDrawer { }

/// <summary> You need to add CustomPropertyDrawer attribute to draw custom dictionary with my default read key drawer </summary>
//[CustomPropertyDrawer(typeof(UDictionaryIntString))]
internal class UDictionaryReadKeyDrawers : UDictionaryReadKeyDrawer { }
