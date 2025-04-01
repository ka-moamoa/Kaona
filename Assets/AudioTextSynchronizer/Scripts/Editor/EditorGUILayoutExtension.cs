using UnityEditor;
using UnityEngine;

namespace AudioTextSynchronizer.Editor
{
    public class EditorGUILayoutExtension
    {
        private static GUIStyle linkStyle;
        private static GUIStyle LinkStyle
        {
            get
            {
                if (linkStyle == null)
                {
                    linkStyle = new GUIStyle(EditorStyles.label);
                    linkStyle.wordWrap = false;
                    linkStyle.fontSize = 12;
                    linkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xEA / 255f, 1f);
                    linkStyle.hover.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xEA / 255f, 1f);
                    linkStyle.stretchWidth = false;
                }
                return linkStyle;
            }
        }

        public static void UrlLabelField(string label, string url, params GUILayoutOption[] options)
        {
            var uiRect = LinkLabelField(new GUIContent(label), options);
            if (GUI.Button(uiRect, label, LinkStyle))
            {
                Application.OpenURL(url);
            }
        }

        private static Rect LinkLabelField(GUIContent label, params GUILayoutOption[] options)
        {
            var rect = GUILayoutUtility.GetRect(label, LinkStyle, options);
            Handles.BeginGUI();
            Handles.color = LinkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(rect.xMin, rect.yMax), new Vector3(rect.xMax, rect.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            return rect;
        }
    }
}