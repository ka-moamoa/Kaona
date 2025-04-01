using System.Collections.Generic;
using AudioTextSynchronizer.TextEffects.Components.Base;
using UnityEngine;
using UnityEngine.UI;

namespace AudioTextSynchronizer.TextEffects.Components
{
    public class TextMeshProcessorUIText : TextMeshProcessorBase, IMeshModifier
    {
        public override bool SkipIndents => true;

        private readonly List<UIVertex> vertices = new List<UIVertex>();

        public override void PreInit()
        {
            if (Graphic == null)
            {
                TryGetComponent(out Graphic graphic);
                Graphic = graphic;
            }
        }

        public override void Init()
        {
            base.Init();
            UpdateInternal(true);
        }

        public override void ResetProgress()
        {
            base.ResetProgress();
            vertices.Clear();
            IsFinishedInternal = false;
            Animation.ResetProgress();
        }

        protected override void UpdateMesh()
        {
            Graphic.SetVerticesDirty();
        }

        public void ModifyMesh(Mesh mesh)
        {
            using (var vh = new VertexHelper())
            {
                ModifyMesh(vh);
                vh.FillMesh(mesh);
            }
        }

        public void ModifyMesh(VertexHelper vh)
        {
            if (!Application.isPlaying || !isActiveAndEnabled || vh.currentVertCount == 0 || !IsStarted || IsFinishedInternal)
                return;
            
            vh.GetUIVertexStream(vertices);
            if (Animation != null)
            {
                for (var i = 0; i < vertices.Count; i++)
                {
                    vertices[i] = Animation.ProcessVertexBeforeEffect(vertices, vertices[i]);
                }

                var verticesCount = CharsProgress.Count;
                for (var i = 0; i < verticesCount; i++)
                {
                    var index = i * 6;
                    var progress = CharsProgress[i];
                    for (var j = 0; j < 6; j++)
                    {
                        var currentVertexIndex = index + j;
                        if (currentVertexIndex < vertices.Count)
                        {
                            vertices[currentVertexIndex] = Animation.ProcessVertexEffect(vertices[currentVertexIndex], currentVertexIndex, progress);
                        }
                    }
                }
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertices);
        }
        
        public override void OnCharProgress(int charIndex)
        {
            if (FillInstantly)
            {
                if (CharsProgress.Count < vertices.Count / 6)
                {
                    var count = vertices.Count / 6;
                    for (var i = CharsProgress.Count; i < count; i++)
                    {
                        CharsProgress.Add(0f);
                    }
                }
            }
            else if (CharsProgress.Count < charIndex && CharsProgress.Count < vertices.Count / 6)
            {
                var count = charIndex - CharsProgress.Count;
                for (var i = 0; i < count; i++)
                {
                    CharsProgress.Add(0f);
                }
            }
        }
    }
}