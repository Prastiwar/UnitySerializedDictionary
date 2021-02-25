using System;

namespace UnityEngine.Collections.Generic
{
    /// <summary>  Common UDictionary wrappers  </summary>

    // --------------------------------------------- String... --------------------------------------------- //

    [Serializable]
    public class UDictionaryStringString : UDictionary<string, string> { }

    [Serializable]
    public class UDictionaryStringInt : UDictionary<string, int> { }

    [Serializable]
    public class UDictionaryStringBool : UDictionary<string, bool> { }

    [Serializable]
    public class UDictionaryStringFloat : UDictionary<string, float> { }

    [Serializable]
    public class UDictionaryStringDouble : UDictionary<string, double> { }

    [Serializable]
    public class UDictionaryStringColor : UDictionary<string, Color> { }

    [Serializable]
    public class UDictionaryStringObject : UDictionary<string, GameObject> { }

    // --------------------------------------------- Int... --------------------------------------------- //

    [Serializable]
    public class UDictionaryIntInt : UDictionary<int, int> { }

    [Serializable]
    public class UDictionaryIntBool : UDictionary<int, bool> { }

    [Serializable]
    public class UDictionaryIntString : UDictionary<int, string> { }

    [Serializable]
    public class UDictionaryIntFloat : UDictionary<int, float> { }

    [Serializable]
    public class UDictionaryIntDouble : UDictionary<int, double> { }

    [Serializable]
    public class UDictionaryIntColor : UDictionary<int, Color> { }

    [Serializable]
    public class UDictionaryIntObject : UDictionary<int, GameObject> { }

}