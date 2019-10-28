using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    [Title("Master", "Stylized")]
    class StylizedMasterNode : MasterNode<ISurfaceSubShader>, IMayRequirePosition, IMayRequireNormal, IMayRequireTangent
    {
        public const string AlphaSlotName = "Alpha";
        public const string AlphaClipThresholdSlotName = "AlphaClipThreshold";
        public const string DiffuseSlotName = "Diffuse";
        public const string EmissionSlotName = "Emission";
        public const string VertexNormalSlotName = "Vertex Normal";
        public const string VertexPositionSlotName = "Vertex Position";
        public const string VertexTangentSlotName = "Vertex Tangent";

        public const int AlphaSlotId = 5;
        public const int AlphaThresholdSlotId = 6;
        public const int DiffuseSlotId = 3;
        public const int EmissionSlotId = 4;
        public const int VertexNormalSlotId = 1;
        public const int VertexPositionSlotId = 0;
        public const int VertexTangentSlotId = 2;

        [SerializeField]
        AlphaMode m_AlphaMode;

        [SerializeField]
        SurfaceType m_SurfaceType;

        [SerializeField]
        bool m_TwoSided;

        public StylizedMasterNode()
        {
            UpdateNodeAfterDeserialization();
        }

        public AlphaMode alphaMode
        {
            get { return m_AlphaMode; }
            set
            {
                if (m_AlphaMode == value)
                    return;

                m_AlphaMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public SurfaceType surfaceType
        {
            get { return m_SurfaceType; }
            set
            {
                if (m_SurfaceType == value)
                    return;

                m_SurfaceType = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public ToggleData twoSided
        {
            get { return new ToggleData(m_TwoSided); }
            set
            {
                if (m_TwoSided == value.isOn)
                    return;
                m_TwoSided = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            name = "Surface Shader Master";
            AddSlot(new PositionMaterialSlot(VertexPositionSlotId, VertexPositionSlotName, VertexPositionSlotName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            AddSlot(new NormalMaterialSlot(VertexNormalSlotId, VertexNormalSlotName, VertexNormalSlotName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            AddSlot(new TangentMaterialSlot(VertexTangentSlotId, VertexTangentSlotName, VertexTangentSlotName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            AddSlot(new ColorRGBMaterialSlot(DiffuseSlotId, DiffuseSlotName, DiffuseSlotName, SlotType.Input, Color.grey, ColorMode.Default, ShaderStageCapability.Fragment));
            AddSlot(new ColorRGBMaterialSlot(EmissionSlotId, EmissionSlotName, EmissionSlotName, SlotType.Input, Color.black, ColorMode.HDR, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(AlphaSlotId, AlphaSlotName, AlphaSlotName, SlotType.Input, 1f, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(AlphaThresholdSlotId, AlphaClipThresholdSlotName, AlphaClipThresholdSlotName, SlotType.Input, 0.5f, ShaderStageCapability.Fragment));

            // Clear out slot names that do not match the slots we support.
            RemoveSlotsNameNotMatching(new[] {
                                        VertexPositionSlotId,
                                        VertexNormalSlotId,
                                        VertexTangentSlotId,
                                        DiffuseSlotId,
                                        EmissionSlotId,
                                        AlphaSlotId,
                                        AlphaThresholdSlotId,
                                    }, true);
        }

        protected override VisualElement CreateCommonSettingsElement()
        {
            return new StylizedSettingsView(this);
        }

        public NeededCoordinateSpace RequiresNormal(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireNormal>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresNormal(stageCapability));
        }

        public NeededCoordinateSpace RequiresPosition(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequirePosition>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresPosition(stageCapability));
        }

        public NeededCoordinateSpace RequiresTangent(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireTangent>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresTangent(stageCapability));
        }
    }
}
