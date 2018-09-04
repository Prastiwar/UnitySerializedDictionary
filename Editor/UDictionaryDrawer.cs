/**
*   Authored by Tomasz Piowczyk
*   MIT LICENSE: https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/LICENSE
*   Repository: https://github.com/Prastiwar/UnitySerializedDictionary
*/

using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class UDictionaryDrawer : PropertyDrawer
{
    private bool isEnabled = false;
    private readonly float space = 17;
    private readonly GUIContent cachedContent = new GUIContent();
    protected readonly BindingFlags privateInstanceFlags = BindingFlags.NonPublic | BindingFlags.Instance;

    private readonly Vector2 redBoxOffset = new Vector2(35, 0);
    protected readonly Vector2 redBoxSize = new Vector2(5, 20);
    protected Texture2D redBoxTexture;
    protected GUIStyle redBoxStyle;
    protected bool showErrorMessage;

    protected bool foldoutRList;
    protected int selectedIndex;
    protected float elementHeight;
    protected ReorderableList rList;

    protected SerializedProperty keysProperty;
    protected SerializedProperty valuesProperty;

    protected bool IsDragging { get { return (bool)rList.GetType().GetField("m_Dragging", privateInstanceFlags).GetValue(rList); } }

    private void OnEnable(SerializedProperty property)
    {
        isEnabled = true;
        OnEnabled(property);
    }

    protected virtual void OnEnabled(SerializedProperty property)
    {
        foldoutRList = EditorPrefs.GetBool(property.name);
        InitializeList(property);
        InitializeRedBoxVariables();
    }

    protected void InitializeRedBoxVariables()
    {
        redBoxTexture = new Texture2D(1, 1);
        redBoxTexture.SetPixel(0, 0, Color.red);
        redBoxTexture.Apply();

        redBoxStyle = new GUIStyle(GUI.skin.box);
        redBoxStyle.normal.background = redBoxTexture;
    }

    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        if (!isEnabled)
        {
            OnEnable(property);
        }
        EditorPrefs.SetBool(property.name, foldoutRList);
        return base.CanCacheInspectorGUI(property);
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        foldoutRList = EditorGUI.Foldout(new Rect(rect.position, new Vector2(rect.size.x, space)), foldoutRList, label);
        if (showErrorMessage)
        {
            DrawErrorMessage(rect, property.name.Length);
        }
        if (foldoutRList)
        {
            showErrorMessage = false;
            rect.y += space;
            rList.DoList(rect);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return foldoutRList ? rList.GetHeight() + space : space;
    }

    protected virtual void InitializeList(SerializedProperty prop)
    {
        keysProperty = prop.FindPropertyRelative("m_keys");
        valuesProperty = prop.FindPropertyRelative("m_values");

        rList = new ReorderableList(prop.serializedObject, keysProperty, true, true, true, true) {
            drawHeaderCallback = DrawHeader,
            onAddCallback = OnAdd,
            onRemoveCallback = OnRemove,
            onReorderCallback = OnReorder,
            onSelectCallback = OnSelect,
            elementHeightCallback = GetReorderableElementHeight,
            drawElementCallback = DrawElement,
        };
    }

    protected void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, fieldInfo.Name);
    }

    protected void OnAdd(ReorderableList list)
    {
        int index = keysProperty.arraySize;
        keysProperty.InsertArrayElementAtIndex(index);
        valuesProperty.InsertArrayElementAtIndex(index);
    }

    protected void OnRemove(ReorderableList list)
    {
        keysProperty.DeleteArrayElementAtIndex(selectedIndex);
        valuesProperty.DeleteArrayElementAtIndex(selectedIndex);
    }

    protected void OnSelect(ReorderableList list)
    {
        selectedIndex = list.index;
    }

    protected void OnReorder(ReorderableList list)
    {
        valuesProperty.MoveArrayElement(selectedIndex, list.index);
    }

    protected float GetReorderableElementHeight(int index)
    {
        float keyHeight = EditorGUI.GetPropertyHeight(keysProperty.GetArrayElementAtIndex(index));
        float valueHeight = EditorGUI.GetPropertyHeight(valuesProperty.GetArrayElementAtIndex(index));
        float height = 8 + Math.Max(keyHeight, valueHeight);
        if (!IsDragging || (IsDragging && elementHeight < height))
        {
            elementHeight = height;
        }
        return elementHeight;
    }

    protected virtual void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.position = new Vector2(rect.position.x + 10, rect.position.y);
        SerializedProperty key = keysProperty.GetArrayElementAtIndex(index);
        SerializedProperty value = valuesProperty.GetArrayElementAtIndex(index);
        float halfSizeX = rect.size.x / 2;
        float leftOffset = 100;
        float rightOffset = 58;
        Vector2 sizeKey = new Vector2(halfSizeX - leftOffset, rect.size.y);
        Vector2 sizeValue = new Vector2(halfSizeX + rightOffset, rect.size.y);
        Vector2 positionValue = rect.position + new Vector2(sizeKey.x + 25, 0);

        float oldWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 50;
        OnBeforeDrawProperties();
        rect = DrawPropertiesForElement(new Rect(rect.position, sizeKey), new Rect(positionValue, sizeValue), key, value);
        OnAfterDrawProperties();
        EditorGUIUtility.labelWidth = oldWidth;

        CheckRedBoxForElement(rect, key, index);
    }

    protected virtual Rect DrawPropertiesForElement(Rect keyRect, Rect valueRect, SerializedProperty keyProp, SerializedProperty valueProp)
    {
        EditorGUI.PropertyField(keyRect, keyProp, GUIContent(keyProp.type), true);
        EditorGUI.PropertyField(valueRect, valueProp, GUIContent(valueProp.type), true);
        return keyRect;
    }

    protected void CheckRedBoxForElement(Rect rect, SerializedProperty keyProp, int index)
    {
        if (keysProperty.HasAnyElementSameValue(keyProp, index))
        {
            DrawRedBox(rect, redBoxSize, redBoxStyle, redBoxOffset);
            showErrorMessage = true;
        }
    }

    protected void DrawErrorMessage(Rect position, int nameLength)
    {
        Vector2 offsetByName = new Vector2(nameLength * 8.25f, 0);
        Vector2 size = new Vector2(position.size.x, space);
        EditorGUI.HelpBox(new Rect(position.position + offsetByName, size), "You have duplicated keys, some changes can be lost!", MessageType.Error);
    }

    protected void DrawRedBox(Rect position, Vector2 size, GUIStyle style, Vector2 offset = new Vector2())
    {
        Rect iconRect = new Rect(position.position - offset, size);
        GUI.Label(iconRect, UnityEngine.GUIContent.none, redBoxStyle); // hack for drawing red error box without flickering or losing focus
    }

    protected GUIContent GUIContent(string text)
    {
        cachedContent.text = text;
        return cachedContent;
    }

    protected virtual void OnBeforeDrawProperties() { }
    protected virtual void OnAfterDrawProperties() { }
}
