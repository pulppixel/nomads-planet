using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/variables/local-name-variables")]
    [AddComponentMenu("Game Creator/Variables/Local Name Variables")]
    [Icon(RuntimePaths.GIZMOS + "GizmoLocalNameVariables.png")]
    
    [Serializable]
    public class LocalNameVariables : TLocalVariables, INameVariable
    {
        // MEMBERS: -------------------------------------------------------------------------------
    
        [SerializeField] private NameVariableRuntime m_Runtime = new NameVariableRuntime();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        internal NameVariableRuntime Runtime => this.m_Runtime;
        
        // EVENTS: --------------------------------------------------------------------------------
        
        private event Action<string> EventChange;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            this.m_Runtime.OnStartup();
            this.m_Runtime.EventChange += this.OnRuntimeChange;
            
            base.Awake();
        }
        
        public static LocalNameVariables Create(GameObject target, NameVariableRuntime variables)
        {
            LocalNameVariables instance = target.Add<LocalNameVariables>();
            instance.m_Runtime = variables;
            instance.m_Runtime.OnStartup();
            
            instance.m_Runtime.EventChange += instance.OnRuntimeChange;
            return instance;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool Exists(string name)
        {
            return this.m_Runtime.Exists(name);
        }
        
        public object Get(string name)
        {
            return this.m_Runtime.Get(name);
        }

        public void Set(string name, object value)
        {
            this.m_Runtime.Set(name, value);
        }
        
        public void Register(Action<string> callback)
        {
            this.EventChange += callback;
        }
        
        public void Unregister(Action<string> callback)
        {
            this.EventChange -= callback;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnRuntimeChange(string name)
        {
            this.EventChange?.Invoke(name);
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public override Type SaveType => typeof(SaveSingleNameVariables);

        public override object SaveData => this.m_SaveUniqueID.SaveValue
            ? new SaveSingleNameVariables(this.m_Runtime)
            : null;

        public override Task OnLoad(object value)
        {
            SaveSingleNameVariables saveData = value as SaveSingleNameVariables;
            if (saveData != null && this.m_SaveUniqueID.SaveValue)
            {
                NameVariable[] candidates = saveData.Variables.ToArray();
                foreach (NameVariable candidate in candidates)
                {
                    if (!this.m_Runtime.Exists(candidate.Name)) continue;
                    this.m_Runtime.Set(candidate.Name, candidate.Value);
                }
            }
            
            return Task.FromResult(saveData != null || !this.m_SaveUniqueID.SaveValue);
        }
    }
}