using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class UDictionaryDrawer : PropertyDrawer
{
    private readonly GUIContent cachedContent = new GUIContent();
    private readonly float space = 17;

    private readonly BindingFlags privateInstance = BindingFlags.NonPublic | BindingFlags.Instance;
    private readonly Vector2 errLineOffset = new Vector2(35, 0);
    private readonly Vector2 errSize = new Vector2(5, 20);
    private Texture2D errorTexture;
    private GUIStyle errBoxStyle;
    private bool showError;

    protected bool foldoutRList;
    protected int selectedIndex;
    protected float elementHeight;
    protected ReorderableList rList;

    protected SerializedProperty keys;
    protected SerializedProperty values;

    protected bool IsDragging { get { return (bool)rList.GetType().GetField("m_Dragging", privateInstance).GetValue(rList); } }

    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        if (rList == null)
        {
            InitializeList(property);

            foldoutRList = EditorPrefs.GetBool(property.name);

            errorTexture = new Texture2D(1, 1);
            errorTexture.SetPixel(0, 0, Color.red);
            errorTexture.Apply();

            errBoxStyle = new GUIStyle(GUI.skin.box);
            errBoxStyle.normal.background = errorTexture;
        }
        EditorPrefs.SetBool(property.name, foldoutRList);
        return base.CanCacheInspectorGUI(property);
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        foldoutRList = EditorGUI.Foldout(new Rect(rect.position, new Vector2(rect.size.x, space)), foldoutRList, label);
        if (showError)
        {
            DrawErrorMessage(rect, property.name.Length);
        }
        if (foldoutRList)
        {
            showError = false;
            rect.y += space;
            rList.DoList(rect);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return foldoutRList ? rList.GetHeight() + space : space;
    }

    protected GUIContent GUIContent(string text)
    {
        cachedContent.text = text;
        return cachedContent;
    }

    protected void InitializeList(SerializedProperty prop)
    {
        keys = prop.FindPropertyRelative("m_keys");
        values = prop.FindPropertyRelative("m_values");

        rList = new ReorderableList(prop.serializedObject, keys, true, true, true, true) {
            drawHeaderCallback = DrawHeader,
            onAddCallback = OnAdd,
            onRemoveCallback = OnRemove,
            onReorderCallback = OnReorder,
            onSelectCallback = OnSelect,
            elementHeightCallback = GetReorderableElementHeight,
            drawElementCallback = DrawElement,
        };
    }

    private void DrawErrorMessage(Rect rect, int nameLength)
    {
        Vector2 addPos = new Vector2(nameLength * 7.5f, 0);
        Vector2 size = new Vector2(rect.size.x, space);
        EditorGUI.HelpBox(new Rect(rect.position + addPos, size), "You have duplicated keys, some changes can be lost!", MessageType.Error);
    }

    private void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, fieldInfo.Name);
    }

    private void OnAdd(ReorderableList list)
    {
        int index = keys.arraySize;
        keys.InsertArrayElementAtIndex(index);
        values.InsertArrayElementAtIndex(index);
    }

    private void OnRemove(ReorderableList list)
    {
        keys.DeleteArrayElementAtIndex(selectedIndex);
        values.DeleteArrayElementAtIndex(selectedIndex);
    }

    private void OnSelect(ReorderableList list)
    {
        selectedIndex = list.index;
    }

    private void OnReorder(ReorderableList list)
    {
        values.MoveArrayElement(selectedIndex, list.index);
    }

    private float GetReorderableElementHeight(int index)
    {
        float keyHeight = EditorGUI.GetPropertyHeight(keys.GetArrayElementAtIndex(index));
        float valueHeight = EditorGUI.GetPropertyHeight(values.GetArrayElementAtIndex(index));
        float height = 8 + Math.Max(keyHeight, valueHeight);
        if (!IsDragging || (IsDragging && elementHeight < height))
        {
            elementHeight = height;
        }
        return elementHeight;
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.position = new Vector2(rect.position.x + 10, rect.position.y);
        SerializedProperty key = keys.GetArrayElementAtIndex(index);
        SerializedProperty value = values.GetArrayElementAtIndex(index);

        float oldWidth = EditorGUIUtility.labelWidth;
        float halfSizeX = rect.size.x / 2;
        float resize = 100;
        Vector2 sizeKey = new Vector2(halfSizeX - resize, rect.size.y);
        Vector2 sizeValue = new Vector2(halfSizeX + resize, rect.size.y);
        Vector2 valuePosition = rect.position + new Vector2(sizeKey.x + 25, 0);

        EditorGUIUtility.labelWidth = 50;
        EditorGUI.PropertyField(new Rect(rect.position, sizeKey), key, GUIContent(key.type), true);
        EditorGUI.PropertyField(new Rect(valuePosition, sizeValue), value, GUIContent(value.type), true);
        EditorGUIUtility.labelWidth = oldWidth;

        if (HasAnySameKeyValue(key, index))
        {
            Rect iconRect = new Rect(rect.position - errLineOffset, errSize);
            GUI.Label(iconRect, UnityEngine.GUIContent.none, errBoxStyle); // hack for drawing red error box without flickering or losing focus
            showError = true;
        }
    }

    private bool HasAnySameKeyValue(SerializedProperty key1, int actualIndex)
    {
        int length = keys.arraySize;
        for (int i = 0; i < length; i++)
        {
            if (i == actualIndex)
            {
                continue;
            }

            SerializedProperty key2 = keys.GetArrayElementAtIndex(i);
            if (GetPropertyValue(key1).Equals(GetPropertyValue(key2)))
            {
                return true;
            }
        }
        return false;
    }

    private object GetPropertyValue(SerializedProperty key)
    {
        switch (key.propertyType)
        {
            case SerializedPropertyType.Integer:
                return key.intValue;
            case SerializedPropertyType.Float:
                return key.floatValue;
            case SerializedPropertyType.String:
                return key.stringValue;
            case SerializedPropertyType.Enum:
                return key.enumValueIndex;
            case SerializedPropertyType.Boolean:
                return key.boolValue;
            case SerializedPropertyType.Color:
                return key.colorValue;
            case SerializedPropertyType.ObjectReference:
                return key.objectReferenceValue;
            case SerializedPropertyType.Vector2:
                return key.vector2Value;
            case SerializedPropertyType.Vector3:
                return key.vector3Value;
            case SerializedPropertyType.Vector4:
                return key.vector4Value;
            case SerializedPropertyType.Quaternion:
                return key.quaternionValue;
            case SerializedPropertyType.Vector2Int:
                return key.vector2IntValue;
            case SerializedPropertyType.Vector3Int:
                return key.vector3IntValue;
            case SerializedPropertyType.ExposedReference:
                return key.exposedReferenceValue;
            case SerializedPropertyType.ArraySize:
                return key.arraySize;
            case SerializedPropertyType.Rect:
                return key.rectValue;
            case SerializedPropertyType.RectInt:
                return key.rectIntValue;
            case SerializedPropertyType.Bounds:
                return key.boundsValue;
            case SerializedPropertyType.BoundsInt:
                return key.boundsIntValue;
            case SerializedPropertyType.FixedBufferSize:
                return key.fixedBufferSize;
            case SerializedPropertyType.AnimationCurve:
                return key.animationCurveValue;
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

        string typ = key.type;
        if (typ == "double")
        {
            return key.doubleValue;
        }
        else if (typ == "long")
        {
            return key.longValue;
        }
        return GetTargetObjectOfProperty(key);
    }

    private object GetTargetObjectOfProperty(SerializedProperty prop)
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

    private object GetValue(object source, string name)
    {
        if (source == null)
            return null;
        Type type = source.GetType();
        while (type != null)
        {
            FieldInfo f = type.GetField(name, BindingFlags.Public | privateInstance);
            if (f != null)
                return f.GetValue(source);

            PropertyInfo p = type.GetProperty(name, BindingFlags.Public | privateInstance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private object GetValue(object source, string name, int index)
    {
        IEnumerable enumerable = GetValue(source, name) as IEnumerable;
        if (enumerable == null)
            return null;
        IEnumerator enm = enumerable.GetEnumerator();

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext())
                return null;
        }
        return enm.Current;
    }
}
