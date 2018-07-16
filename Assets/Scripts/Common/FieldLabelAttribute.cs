using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;
namespace MyAttribute
{
    /// <summary>
    /// 能让字段在inspect面板显示中文字符
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldLabelAttribute : PropertyAttribute
    {
        public string label;//要显示的字符
        public FieldLabelAttribute(string label)
        {
            this.label = label;
        }

    }
    [CustomPropertyDrawer(typeof(FieldLabelAttribute))]
    public class FieldLabelDrawer : PropertyDrawer
    {
        private FieldLabelAttribute FLAttribute
        {
            get { return (FieldLabelAttribute)attribute; }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(FLAttribute.label), true);

        }
    }
}

#endif
