namespace UnityEngine.Animations.Rigging
{
    /// <summary>
    /// The ChainIK constraint data.
    /// </summary>
    [System.Serializable]
    public struct ChainIKConstraintData : IAnimationJobData, IChainIKConstraintData
    {
        internal const int k_MinIterations = 1;
        internal const int k_MaxIterations = 50;
        internal const float k_MinTolerance = 0f;
        internal const float k_MaxTolerance = 0.01f;

        [SerializeField] Transform m_Root;
        [SerializeField] Transform m_Tip;

        [SyncSceneToStream, SerializeField] Transform m_Target;
        [SyncSceneToStream, SerializeField, Range(0f, 1f)] float m_ChainRotationWeight;
        [SyncSceneToStream, SerializeField, Range(0f, 1f)] float m_TipRotationWeight;

        [NotKeyable, SerializeField, Range(k_MinIterations, k_MaxIterations)] int m_MaxIterations;
        [NotKeyable, SerializeField, Range(k_MinTolerance, k_MaxTolerance)] float m_Tolerance;
        [NotKeyable, SerializeField] bool m_MaintainTargetPositionOffset;
        [NotKeyable, SerializeField] bool m_MaintainTargetRotationOffset;

        /// <inheritdoc />
        public Transform root { get => m_Root; set => m_Root = value; }
        /// <inheritdoc />
        public Transform tip { get => m_Tip; set => m_Tip = value; }
        /// <inheritdoc />
        public Transform target { get => m_Target; set => m_Target = value; }
        /// <summary>The weight for which ChainIK target has an effect on chain (up to tip Transform). This is a value in between 0 and 1.</summary>
        public float chainRotationWeight { get => m_ChainRotationWeight; set => m_ChainRotationWeight = Mathf.Clamp01(value); }
        /// <summary>The weight for which ChainIK target has and effect on tip Transform. This is a value in between 0 and 1.</summary>
        public float tipRotationWeight { get => m_TipRotationWeight; set => m_TipRotationWeight = Mathf.Clamp01(value); }
        /// <inheritdoc />
        public int maxIterations { get => m_MaxIterations; set => m_MaxIterations = Mathf.Clamp(value, k_MinIterations, k_MaxIterations); }
        /// <inheritdoc />
        public float tolerance { get => m_Tolerance; set => m_Tolerance = Mathf.Clamp(value, k_MinTolerance, k_MaxTolerance); }
        /// <inheritdoc />
        public bool maintainTargetPositionOffset { get => m_MaintainTargetPositionOffset; set => m_MaintainTargetPositionOffset = value; }
        /// <inheritdoc />
        public bool maintainTargetRotationOffset { get => m_MaintainTargetRotationOffset; set => m_MaintainTargetRotationOffset = value; }

        /// <inheritdoc />
        string IChainIKConstraintData.chainRotationWeightFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_ChainRotationWeight));
        /// <inheritdoc />
        string IChainIKConstraintData.tipRotationWeightFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_TipRotationWeight));

        /// <inheritdoc />
        bool IAnimationJobData.IsValid()
        {
            if (m_Root == null || m_Tip == null || m_Target == null)
                return false;

            int count = 1;
            Transform tmp = m_Tip;
            while (tmp != null && tmp != m_Root)
            {
                tmp = tmp.parent;
                ++count;
            }

            return (tmp == m_Root && count > 2);
        }

        /// <inheritdoc />
        void IAnimationJobData.SetDefaultValues()
        {
            m_Root = null;
            m_Tip = null;
            m_Target = null;
            m_ChainRotationWeight = 1f;
            m_TipRotationWeight = 1f;
            m_MaxIterations = 15;
            m_Tolerance = 0.0001f;
            m_MaintainTargetPositionOffset = false;
            m_MaintainTargetRotationOffset = false;
        }
    }

    /// <summary>
    /// ChainIK constraint
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Chain IK Constraint")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/ChainIKConstraint.html")]
    public class ChainIKConstraint : RigConstraint<
        ChainIKConstraintJob,
        ChainIKConstraintData,
        ChainIKConstraintJobBinder<ChainIKConstraintData>
        >
    {
        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Data.chainRotationWeight = Mathf.Clamp01(m_Data.chainRotationWeight);
            m_Data.tipRotationWeight = Mathf.Clamp01(m_Data.tipRotationWeight);
            m_Data.maxIterations = Mathf.Clamp(
                m_Data.maxIterations, ChainIKConstraintData.k_MinIterations, ChainIKConstraintData.k_MaxIterations
            );
            m_Data.tolerance = Mathf.Clamp(
                m_Data.tolerance, ChainIKConstraintData.k_MinTolerance, ChainIKConstraintData.k_MaxTolerance
            );
        }
    }
}
