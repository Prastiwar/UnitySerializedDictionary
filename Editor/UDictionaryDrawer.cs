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
            if (key1.GetValue().Equals(key2.GetValue()))
            {
                return true;
            }
        }
        return false;
    }
}
