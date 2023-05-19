using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class FieldGetLocalList : TFieldGetVariable
    {
        [SerializeField]
        protected LocalListVariables m_Variable;

        [SerializeReference]
        protected TListGetPick m_Select = new GetPickFirst();

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public FieldGetLocalList(IdString typeID)
        {
            this.m_TypeID = typeID;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override object Get(Args args)
        {
            return this.m_Variable != null ? m_Variable.Get(this.m_Select, args) : null;
        }

        public override string ToString() => this.m_Variable != null
            ? $"{m_Variable.name}[{this.m_Select}]"
            : "(none)";
    }
}