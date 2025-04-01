using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AudioTextSynchronizer.Editor.Timings
{
	[Serializable]
	public class AudioManager
	{
		public AudioSource Source;
		[SerializeField] private GameObject hiddenGameObject;
		[SerializeField] private float time;

		public void InitSource(AudioClip clip)
		{
			if (clip == null) 
				return;
			
			if (hiddenGameObject == null || Source == null)
			{
				Destroy();
				hiddenGameObject = new GameObject("_hiddenGameObject")
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				Source = hiddenGameObject.AddComponent<AudioSource>();
				Source.clip = clip;
			}
		}

		public bool IsStartPlaying()
		{
			return Source != null && Source.clip != null && Source.time > 0f;
		}
		
		public void RestorePosition()
		{
			if (Source != null && Source.clip != null)
			{
				Source.time = time;
				Source.Play();
				Source.Pause();
			}
		}

		public void SetPosition(float newTime)
		{
			if (Source != null && Source.clip != null)
			{
				time = newTime;
				Source.time = newTime;
			}
		}

		public void Destroy()
		{
			if (hiddenGameObject != null)
			{
				Object.DestroyImmediate(hiddenGameObject);
			}
		}
	}
}