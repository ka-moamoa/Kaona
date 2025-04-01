using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
#if TEXTMESHPRO_3_0_OR_NEWER
using TMPro;
#endif

namespace AudioTextSynchronizer.Editor
{
	[CustomEditor(typeof(TextSynchronizer))]
	public class TextSynchronizerInspector : UnityEditor.Editor
	{
		private TextSynchronizer synchronizer;
		private Component[] components;
		private string[] componentNames;
		private int componentIndex;
		private string[] propertyNames;
		private int propertyIndex;

		private SerializedProperty gameObjectProperty;
		private SerializedProperty timingsProperty;
		private SerializedProperty sourceProperty;
		private SerializedProperty textEffectProperty;
		private SerializedProperty isRunningProperty;
		
		private void OnEnable()
		{
			Init();
			gameObjectProperty = serializedObject.FindProperty("gameObjectWithTextComponent");
			sourceProperty = serializedObject.FindProperty("source");
			timingsProperty = serializedObject.FindProperty("timings");
			textEffectProperty = serializedObject.FindProperty("textEffect");
			isRunningProperty = serializedObject.FindProperty("isRunning");
			Undo.undoRedoPerformed += UndoRedoPerformed;
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= UndoRedoPerformed;
		}

		private void UndoRedoPerformed()
		{
			Init();
		}

		private void Init()
		{
			synchronizer = (TextSynchronizer) target;
			if (synchronizer.GameObjectWithTextComponent == null) 
				return;
			
			components = synchronizer.GameObjectWithTextComponent.GetComponents<Component>().Where(
				x => GetPropertyNames(x).Length > 0).ToArray();
			if (components.Length == 0) 
				return;
			
			componentNames = components.Select(y => y.GetType().Name).ToArray();
			componentIndex = components.ToList().IndexOf(synchronizer.TextComponent);
			if (componentIndex < 0)
			{
				componentIndex = 0;
				propertyIndex = 0;
				synchronizer.TextComponent = components[componentIndex];
				propertyNames = GetPropertyNames(synchronizer.TextComponent);
			}
			else
			{
				synchronizer.TextComponent = components[componentIndex];
				propertyNames = GetPropertyNames(synchronizer.TextComponent);
				if (!string.IsNullOrEmpty(synchronizer.Property))
				{
					propertyIndex = propertyNames.ToList().IndexOf(synchronizer.Property);
					if (propertyIndex < 0)
					{
						propertyIndex = 0;
					}
				}
			}
		}

		private string[] GetPropertyNames(object component)
		{
			return component.GetType().GetProperties()
				.Where(x => x.PropertyType == typeof(System.String) && x.Name != "tag" && x.Name != "name")
				.Select(x => x.Name).ToArray();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			synchronizer = (TextSynchronizer) target;
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(gameObjectProperty, new GUIContent("GameObject"));
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				Init();
			}
			
			if (gameObjectProperty.objectReferenceValue != null && (components.Length == 0 || propertyNames.Length == 0))
			{
				Init();
			}

			if (synchronizer.GameObjectWithTextComponent != null)
			{
				EditorGUI.BeginChangeCheck();
				componentIndex = EditorGUILayout.Popup("Component", componentIndex, componentNames);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(synchronizer, "Change TextSynchronizer.Component field");
					MarkAsDirty();
					synchronizer.TextComponent = components[componentIndex];
					propertyIndex = 0;
					propertyNames = GetPropertyNames(synchronizer.TextComponent);
				}

				if (propertyNames.Length > 0)
				{
					EditorGUI.BeginChangeCheck();
					propertyIndex = EditorGUILayout.Popup("Property", propertyIndex, propertyNames);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(synchronizer, "Change TextSynchronizer.Property field");
						MarkAsDirty();
					}
					synchronizer.Property = propertyNames[propertyIndex];
				}
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(sourceProperty, new GUIContent("Audio Source"));
			EditorGUILayout.PropertyField(timingsProperty, new GUIContent("Phrases Asset"));
			EditorGUILayout.PropertyField(textEffectProperty, new GUIContent("Text Effect"));

			EditorGUILayout.PropertyField(isRunningProperty, new GUIContent("Is Running"));
			if (EditorGUI.EndChangeCheck())
			{
				MarkAsDirty();
			}
			
			serializedObject.ApplyModifiedProperties();
		}

		private void MarkAsDirty()
		{
			if (!Application.isPlaying && synchronizer != null)
			{
				EditorUtility.SetDirty(synchronizer);
				EditorSceneManager.MarkSceneDirty(synchronizer.gameObject.scene);
			}
		}
	}
}