using System;
using UnityEngine;
using GameCreator.Runtime.Common;
using UnityEngine.EventSystems;

namespace GameCreator.Runtime.VisualScripting
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/visual-scripting/hotspots")]
    [AddComponentMenu("Game Creator/Visual Scripting/Hotspot")]
    [DefaultExecutionOrder(ApplicationManager.EXECUTION_ORDER_DEFAULT_LATER)]
    
    [Icon(RuntimePaths.GIZMOS + "GizmoHotspot.png")]
    public class Hotspot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly Color GIZMOS_COLOR = Color.red;

        private const float TRANSITION_SMOOTH_TIME = 0.25f;
        
        private const float GIZMOS_ALPHA_ON = 0.25f;
        private const float GIZMOS_ALPHA_OFF = 0.1f;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField]
        protected PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();
        
        [SerializeField] protected float m_Radius = 10f;
        [SerializeField] protected Vector3 m_Offset = Vector3.zero;

        [SerializeField]
        protected SpotList m_Spots = new SpotList();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private float m_Velocity;
        private Args m_Args;

        // PROPERTIES: ----------------------------------------------------------------------------

        public GameObject Target => this.m_Target.Get(this.m_Args);
        
        public float Radius
        {
            get => this.m_Radius;
            set => this.m_Radius = value;
        }

        public Vector3 Position => this.transform.TransformPoint(this.m_Offset);
        public Quaternion Rotation => this.transform.rotation;

        [field: NonSerialized] public bool IsActive { get; private set; }
        [field: NonSerialized] public float Transition { get; private set; }
        
        [field: NonSerialized] public float Distance { get; private set; }
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventOnActivate;
        public event Action EventOnDeactivate;

        // MAIN METHODS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.m_Args = new Args(this);
            this.m_Spots.OnAwake(this);
        }

        private void Start()
        {
            this.m_Spots.OnStart(this);
        }

        private void Update()
        {
            bool wasActive = this.IsActive;

            if (this.Target == null)
            {
                this.IsActive = false;
                this.Distance = float.MaxValue;
            }
            else
            {
                this.Distance = Vector3.Distance(
                    this.Target.transform.position,
                    this.Position
                );

                this.IsActive = this.Distance <= this.Radius;
            }

            this.Transition = Mathf.SmoothDamp(
                this.Transition,
                this.IsActive ? 1f : 0f,
                ref this.m_Velocity,
                TRANSITION_SMOOTH_TIME
            );

            this.m_Spots.OnUpdate(this);
            
            switch (wasActive)
            {
                case false when this.IsActive: this.EventOnActivate?.Invoke(); break;
                case true when !this.IsActive: this.EventOnDeactivate?.Invoke(); break;
            }
        }

        private void OnEnable()
        {
            this.m_Velocity = 0f;
            this.Transition = 0f;
            this.m_Spots.OnEnable(this);
        }

        private void OnDisable()
        {
            this.m_Velocity = 0f;
            this.Transition = 0f;
            this.m_Spots.OnDisable(this);
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            this.m_Spots.OnPointerEnter(this);
        }
        
        public void OnPointerExit(PointerEventData pointerEventData)
        {
            this.m_Spots.OnPointerExit(this);
        }

        // GIZMOS: --------------------------------------------------------------------------------

        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this.gameObject)) return;
            #endif
            
            float alpha = Mathf.Lerp(
                GIZMOS_ALPHA_OFF,
                GIZMOS_ALPHA_ON,
                this.IsActive ? 1f : 0f
            );

            Gizmos.color = new Color(
                GIZMOS_COLOR.r,
                GIZMOS_COLOR.g,
                GIZMOS_COLOR.b,
                alpha
            );

            GizmosExtension.Octahedron(
                this.Position,
                this.Rotation,
                this.Radius
            );

            this.m_Spots.OnGizmos(this);
            
            if (!Application.isPlaying) return;
            
            if (this.Target != null)
            {
                Gizmos.DrawLine(
                    this.Target.transform.position,
                    this.Position
                );
            }
        }
    }
}