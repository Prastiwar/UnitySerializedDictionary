/**
*   Authored by Tomasz Piowczyk
*   MIT LICENSE: https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/LICENSE
*   Repository: https://github.com/Prastiwar/UnitySerializedDictionary
*/

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

public static class UDictionaryExtensions
{
    public static readonly BindingFlags PublicOrNotInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static bool HasAnyElementSameValue(this SerializedProperty array, SerializedProperty key1, int skipIndex)
    {
        int length = array.arraySize;
        for (int i = 0; i < length; i++)
        {
            if (i == skipIndex)
            {
                continue;
            }

            SerializedProperty key2 = array.GetArrayElementAtIndex(i);
            object key1Value = key1 != null ? key1.GetValue() : null;
            object key2Value = key2 != null ? key2.GetValue() : null;
            if (key1Value == null ? key2Value == null : key1Value.Equals(key2Value))
            {
                return true;
            }
        }
        return false;
    }

    public static object GetValue(this object source, string name)
    {
        if (source == null)
        {
            return null;
        }

        Type type = source.GetType();
        while (type != null)
        {
            FieldInfo f = type.GetField(name, PublicOrNotInstance);
            if (f != null)
            {
                return f.GetValue(source);
            }

            PropertyInfo p = type.GetProperty(name, PublicOrNotInstance | BindingFlags.IgnoreCase);
            if (p != null)
            {
                return p.GetValue(source, null);
            }
            type = type.BaseType;
        }
        return null;
    }

    public static object GetValue(this object source, string name, int index)
    {
        IEnumerable enumerable = GetValue(source, name) as IEnumerable;
        if (enumerable == null)
        {
            return null;
        }

        IEnumerator enm = enumerable.GetEnumerator();
        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext())
            {
                return null;
            }
        }
        return enm.Current;
    }

    public static object GetTargetObject(this SerializedProperty prop)
    {
        object targetObj = prop.serializedObject.targetObject;
        string[] elements = prop.propertyPath.Replace(".Array.data[", "[").Split('.');
        int length = elements.Length;
        for (int i = 0; i < length; i++)
        {
            if (elements[i].Contains("["))
            {
                string elementName = elements[i].Substring(0, elements[i].IndexOf("["));
                int index = Convert.ToInt32(elements[i].Substring(elements[i].IndexOf("[")).Replace("[", "").Replace("]", ""));
                targetObj = GetValue(targetObj, elementName, index);
            }
            else
            {
                targetObj = GetValue(targetObj, elements[i]);
            }
        }
        return targetObj;
    }

    public static object GetValue(this SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                return prop.intValue;
            case SerializedPropertyType.Float:
                return prop.floatValue;
            case SerializedPropertyType.String:
                return prop.stringValue;
            case SerializedPropertyType.Enum:
                return prop.enumValueIndex;
            case SerializedPropertyType.Boolean:
                return prop.boolValue;
            case SerializedPropertyType.Color:
                return prop.colorValue;
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue;
            case SerializedPropertyType.Vector2:
                return prop.vector2Value;
            case SerializedPropertyType.Vector3:
                return prop.vector3Value;
            case SerializedPropertyType.Vector4:
                return prop.vector4Value;
            case SerializedPropertyType.Quaternion:
                return prop.quaternionValue;
            case SerializedPropertyType.Vector2Int:
                return prop.vector2IntValue;
            case SerializedPropertyType.Vector3Int:
                return prop.vector3IntValue;
            case SerializedPropertyType.ExposedReference:
                return prop.exposedReferenceValue;
            case SerializedPropertyType.ArraySize:
                return prop.arraySize;
            case SerializedPropertyType.Rect:
                return prop.rectValue;
            case SerializedPropertyType.RectInt:
                return prop.rectIntValue;
            case SerializedPropertyType.Bounds:
                return prop.boundsValue;
            case SerializedPropertyType.BoundsInt:
                return prop.boundsIntValue;
            case SerializedPropertyType.FixedBufferSize:
                return prop.fixedBufferSize;
            case SerializedPropertyType.AnimationCurve:
                return prop.animationCurveValue;
            //case SerializedPropertyType.Generic:
            //    return key.;
            //case SerializedPropertyType.LayerMask:
            //    return key.;
            //case SerializedPropertyType.Character:
            //    return key.;
            //case SerializedPropertyType.Gradient:
            //    return key.;
            default:
                break;
        }

        string typ = prop.type;
        if (typ == "double")
        {
            return prop.doubleValue;
        }
        else if (typ == "long")
        {
            return prop.longValue;
        }
        return prop.GetTargetObject();
    }
}
