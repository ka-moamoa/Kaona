using System;
using System.Collections.Generic;
using AudioTextSynchronizer.TextEffects.MeshAnimations.Base;
using UnityEngine;
using UnityEngine.UI;

namespace AudioTextSynchronizer.TextEffects.Components.Base
{
    public abstract class TextMeshProcessorBase : MonoBehaviour, IDisposable
    {
        public event Action OnMeshAnimationFinished;
        
        public MeshAnimationBase MeshAnimation;
        public Graphic Graphic { get; protected set; }
        public bool FillInstantly { get; set; }
        public abstract bool SkipIndents { get; }

        protected readonly List<float> CharsProgress = new List<float>();
        protected MeshAnimationBase Animation;
        protected bool IsStarted;
        protected bool IsFinishedInternal;
        private bool IsAnimating { get; set; } = true;
        private bool isFinished;
        private bool isDirty;

        public virtual void PreInit()
        {
        }
        
        public virtual void Init()
        {
            CharsProgress.Clear();
            Dispose();
            Animation = Instantiate(MeshAnimation);
            IsStarted = true;
        }
        
        public virtual void Dispose()
        {
            IsStarted = false;
            isFinished = false;
            IsFinishedInternal = false;
            if (Animation != null)
            {
                Destroy(Animation);
                Animation = null;
            }
        }
        
        private void OnDestroy()
        {
            Dispose();
        }

        public void ForceUpdate()
        {
            UpdateInternal(true);
        }
        
        public void Finish()
        {
            isFinished = true;
        }

        public virtual void ResetProgress()
        {
            CharsProgress.Clear();
        }

        public virtual void OnCharProgress(int charIndex)
        {
        }

        public virtual void OnPreSetNewText()
        {
        }

        public virtual void OnSetNewText()
        {
        }
        
        private void Update()
        {
            if (!IsStarted || IsFinishedInternal || Animation == null)
                return;

            UpdateInternal();
        }

        protected void UpdateInternal(bool setDirty = false)
        {
            IsAnimating = false;
            isDirty = setDirty;
            for (var i = 0; i < CharsProgress.Count; i++)
            {
                if (CharsProgress[i] < 1f)
                {
                    CharsProgress[i] += Time.deltaTime * Animation.Speed;
                    isDirty = true;
                }

                CharsProgress[i] = Mathf.Clamp(CharsProgress[i], 0f, 1f);

                if (isFinished && !IsAnimating && 1f - CharsProgress[i] > 0f)
                {
                    IsAnimating = true;
                }
            }

            if (isFinished && !IsAnimating && CharsProgress.Count > 0)
            {
                OnMeshAnimationFinished?.Invoke();
                IsFinishedInternal = true;
            }

            if (isDirty && Graphic != null)
            {
                UpdateMesh();
            }
        }

        protected virtual void UpdateMesh()
        {
        }
    }
}