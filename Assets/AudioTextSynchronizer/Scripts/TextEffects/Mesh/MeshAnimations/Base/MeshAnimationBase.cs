using System.Collections.Generic;
using UnityEngine;
#if TEXTMESHPRO_3_0_OR_NEWER
using TMPro;
#endif

namespace AudioTextSynchronizer.TextEffects.MeshAnimations.Base
{
    public abstract class MeshAnimationBase : ScriptableObject
    {
        [Range(0.01f, 100f)] public float Speed = 1f;

        public virtual void ResetProgress()
        {
        }
        
#if TEXTMESHPRO_3_0_OR_NEWER
        public abstract void ProcessVertexBeforeEffect(TMP_TextInfo textInfo, int vertexIndex);
        public abstract void ProcessVertexEffect(TMP_TextInfo textInfo, int startVertexIndex, int vertexIndex, float progress);
#endif
        
        public abstract UIVertex ProcessVertexBeforeEffect(List<UIVertex> vertices, UIVertex vertex);
        public abstract UIVertex ProcessVertexEffect(UIVertex vertex, int vertexIndex, float progress);
    }
}