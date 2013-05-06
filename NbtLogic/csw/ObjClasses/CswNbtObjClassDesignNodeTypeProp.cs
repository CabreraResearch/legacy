using System;
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignNodeTypeProp : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string AuditLevel = "Audit Level";
            public const string CompoundUnique = "Compound Unique";
            public const string DisplayConditionFilter = "Display Condition Filter";
            public const string DisplayConditionProperty = "Display Condition Property";
            public const string DisplayConditionSubfield = "Display Condition Subfield";
            public const string DisplayConditionValue = "Display Condition Value";
            public const string FieldType = "Field Type";
            public const string HelpText = "Help Text";
            public const string NodeTypeValue = "NodeType";
            public const string ObjectClassPropName = "Original Name";
            public const string PropName = "Prop Name";
            public const string ReadOnly = "Read Only";
            public const string Required = "Required";
            public const string Unique = "Unique";
            public const string UseNumbering = "Use Numbering";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignNodeTypeProp( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDesignNodeTypeProp
        /// </summary>
        public static implicit operator CswNbtObjClassDesignNodeTypeProp( CswNbtNode Node )
        {
            CswNbtObjClassDesignNodeTypeProp ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DesignNodeTypePropClass ) )
            {
                ret = (CswNbtObjClassDesignNodeTypeProp) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            // Options for Field Type property
            SortedList<string, CswNbtNodeTypePropListOption> FieldTypeOptions = new SortedList<string, CswNbtNodeTypePropListOption>();
            Dictionary<Int32, CswEnumNbtFieldType> FieldTypeIds = _CswNbtResources.MetaData.getFieldTypeIds();
            foreach( Int32 FieldTypeId in FieldTypeIds.Keys )
            {
                CswEnumNbtFieldType thisFieldtypeName = FieldTypeIds[FieldTypeId];
                FieldTypeOptions.Add( thisFieldtypeName.ToString(), new CswNbtNodeTypePropListOption( thisFieldtypeName.ToString(), FieldTypeId.ToString() ) );
            }
            FieldType.Options.Override( FieldTypeOptions.Values );

            if( _CswNbtResources.EditMode != CswEnumNbtNodeEditMode.Add )
            {
                NodeTypeValue.ServerManaged = true;
            }

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropList AuditLevel { get { return ( _CswNbtNode.Properties[PropertyName.AuditLevel] ); } }
        public CswNbtNodePropLogical CompoundUnique { get { return ( _CswNbtNode.Properties[PropertyName.CompoundUnique] ); } }
        public CswNbtNodePropList DisplayConditionFilter { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionFilter] ); } }
        public CswNbtNodePropRelationship DisplayConditionProperty { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionProperty] ); } }
        public CswNbtNodePropList DisplayConditionSubfield { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionSubfield] ); } }
        public CswNbtNodePropText DisplayConditionValue { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionValue] ); } }
        public CswNbtNodePropList FieldType { get { return ( _CswNbtNode.Properties[PropertyName.FieldType] ); } }
        public CswNbtNodePropMemo HelpText { get { return ( _CswNbtNode.Properties[PropertyName.HelpText] ); } }
        public CswNbtNodePropRelationship NodeTypeValue { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeValue] ); } }
        public CswNbtNodePropText ObjectClassPropName { get { return ( _CswNbtNode.Properties[PropertyName.ObjectClassPropName] ); } }
        public CswNbtNodePropText PropName { get { return ( _CswNbtNode.Properties[PropertyName.PropName] ); } }
        public CswNbtNodePropLogical ReadOnly { get { return ( _CswNbtNode.Properties[PropertyName.ReadOnly] ); } }
        public CswNbtNodePropLogical Required { get { return ( _CswNbtNode.Properties[PropertyName.Required] ); } }
        public CswNbtNodePropLogical Unique { get { return ( _CswNbtNode.Properties[PropertyName.Unique] ); } }
        public CswNbtNodePropLogical UseNumbering { get { return ( _CswNbtNode.Properties[PropertyName.UseNumbering] ); } }

        #endregion

        public bool DerivesFromObjectClassProp
        {
            get { return false == string.IsNullOrEmpty( ObjectClassPropName.Text ); }
        }

    }//CswNbtObjClassDesignNodeTypeProp

}//namespace ChemSW.Nbt.ObjClasses
