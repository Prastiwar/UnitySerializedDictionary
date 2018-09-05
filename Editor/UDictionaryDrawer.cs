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

    private bool showErrorMessage;
    protected string errorMessage;
    private readonly Vector2 redBoxOffset = new Vector2(35, 0);
    protected readonly Vector2 redBoxSize = new Vector2(5, 20);
    protected Texture2D redBoxTexture;
    protected GUIStyle redBoxStyle;

    private float elementHeight;
    private bool foldoutRList;

    protected int SelectedIndex { get; private set; }
    protected ReorderableList RList { get; private set; }

    protected SerializedProperty KeysProperty { get; private set; }
    protected SerializedProperty ValuesProperty { get; private set; }

    protected bool IsDragging { get { return (bool)RList.GetType().GetField("m_Dragging", privateInstanceFlags).GetValue(RList); } }

    public sealed override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        if (!isEnabled)
        {
            OnEnable(property);
        }
        EditorPrefs.SetBool(property.name, foldoutRList);
        return base.CanCacheInspectorGUI(property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return foldoutRList ? (RList != null ? RList.GetHeight() : 0) + space : space;
    }

    private void OnEnable(SerializedProperty property)
    {
        isEnabled = true;
        OnEnabled(property);
    }

    protected virtual void OnEnabled(SerializedProperty property)
    {
        foldoutRList = EditorPrefs.GetBool(property.name);
        errorMessage = "You have duplicated keys, some changes can be lost!";
        InitializeList(property);
        InitializeRedBoxVariables();
    }

    protected virtual void InitializeRedBoxVariables()
    {
        redBoxTexture = new Texture2D(1, 1);
        redBoxTexture.SetPixel(0, 0, Color.red);
        redBoxTexture.Apply();

        redBoxStyle = new GUIStyle(GUI.skin.box);
        redBoxStyle.normal.background = redBoxTexture;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        foldoutRList = EditorGUI.Foldout(new Rect(rect.position, new Vector2(rect.size.x, space)), foldoutRList, label);
        if (showErrorMessage)
        {
            DrawErrorMessage(rect, property.name.Length, errorMessage);
        }
        if (foldoutRList && RList != null)
        {
            showErrorMessage = false;
            rect.y += space;
            RList.DoList(rect);
        }
    }

    protected void InitializeList(SerializedProperty prop)
    {
        KeysProperty = prop.FindPropertyRelative("m_keys");
        ValuesProperty = prop.FindPropertyRelative("m_values");
        RList = CreateList(prop.serializedObject, KeysProperty);
        RList.onSelectCallback += OnSelect;
    }

    protected virtual ReorderableList CreateList(SerializedObject serializedObj, SerializedProperty elements)
    {
        return new ReorderableList(serializedObj, elements, true, true, true, true) {
            drawHeaderCallback = DrawHeader,
            onAddCallback = OnAdd,
            onRemoveCallback = OnRemove,
            onReorderCallback = OnReorder,
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
        int index = KeysProperty.arraySize;
        KeysProperty.InsertArrayElementAtIndex(index);
        ValuesProperty.InsertArrayElementAtIndex(index);
    }

    protected void OnRemove(ReorderableList list)
    {
        KeysProperty.DeleteArrayElementAtIndex(SelectedIndex);
        ValuesProperty.DeleteArrayElementAtIndex(SelectedIndex);
    }

    protected void OnReorder(ReorderableList list)
    {
        ValuesProperty.MoveArrayElement(SelectedIndex, list.index);
    }

    private void OnSelect(ReorderableList list)
    {
        SelectedIndex = list.index;
    }

    protected float GetReorderableElementHeight(int index)
    {
        float keyHeight = EditorGUI.GetPropertyHeight(KeysProperty.GetArrayElementAtIndex(index));
        float valueHeight = EditorGUI.GetPropertyHeight(ValuesProperty.GetArrayElementAtIndex(index));
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
        SerializedProperty key = KeysProperty.GetArrayElementAtIndex(index);
        SerializedProperty value = ValuesProperty.GetArrayElementAtIndex(index);
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

    protected virtual void CheckRedBoxForElement(Rect rect, SerializedProperty keyProp, int index)
    {
        if (KeysProperty.HasAnyElementSameValue(keyProp, index))
        {
            DrawRedBox(rect, redBoxSize, redBoxStyle, redBoxOffset);
            showErrorMessage = true;
        }
    }

    protected void DrawErrorMessage(Rect position, int nameLength, string message)
    {
        Vector2 offsetByName = new Vector2(nameLength * 8.25f, 0);
        Vector2 size = new Vector2(position.size.x, space);
        EditorGUI.HelpBox(new Rect(position.position + offsetByName, size), message, MessageType.Error);
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
