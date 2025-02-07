using UnityEditor.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

namespace UnityEditor.Experimental.Rendering.HighDefinition
{
    [CanEditMultipleObjects]
    [VolumeComponentEditor(typeof(PathTracing))]
    class PathTracingEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_Enable;
        SerializedDataParameter m_LayerMask;
        SerializedDataParameter m_MaxSamples;
        SerializedDataParameter m_MinDepth;
        SerializedDataParameter m_MaxDepth;
        SerializedDataParameter m_MaxIntensity;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<PathTracing>(serializedObject);

            m_Enable = Unpack(o.Find(x => x.enable));
            m_LayerMask = Unpack(o.Find(x => x.layerMask));
            m_MaxSamples = Unpack(o.Find(x => x.maximumSamples));
            m_MinDepth = Unpack(o.Find(x => x.minimumDepth));
            m_MaxDepth = Unpack(o.Find(x => x.maximumDepth));
            m_MaxIntensity = Unpack(o.Find(x => x.maximumIntensity));
        }

        public override void OnInspectorGUI()
        {
            HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
            if (!currentAsset?.currentPlatformRenderPipelineSettings.supportRayTracing ?? false)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The current HDRP Asset does not support Ray Tracing.", MessageType.Error, wide: true);
                return;
            }
#if ENABLE_RAYTRACING
            if (currentAsset.currentPlatformRenderPipelineSettings.supportedRaytracingTier != RenderPipelineSettings.RaytracingTier.Tier2)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The current HDRP Asset does not support Path Tracing.", MessageType.Error, wide: true);
                return;
            }

            PropertyField(m_Enable);

            if (m_Enable.overrideState.boolValue && m_Enable.value.boolValue)
            {
                EditorGUI.indentLevel++;
                PropertyField(m_LayerMask);
                PropertyField(m_MaxSamples);
                PropertyField(m_MinDepth);
                PropertyField(m_MaxDepth);
                PropertyField(m_MaxIntensity);
                EditorGUI.indentLevel--;
            }
#endif
        }
    }
}
