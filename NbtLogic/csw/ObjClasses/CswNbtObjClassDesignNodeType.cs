using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignNodeType : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string AuditLevel = "Audit Level";
            public const string Category = "Category";
            public const string DeferSearchTo = "Defer Search To";
            public const string IconFileName = "Icon File Name";
            public const string Locked = "Locked";
            public const string NameTemplate = "Name Template";
            public const string NameTemplateAdd = "Add to Name Template";
            public const string NodeTypeName = "NodeType Name";
            public const string ObjectClassName = "Object Class Name";
            public const string ObjectClassValue = "Object Class";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignNodeType( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDesignNodeType
        /// </summary>
        public static implicit operator CswNbtObjClassDesignNodeType( CswNbtNode Node )
        {
            CswNbtObjClassDesignNodeType ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DesignNodeTypeClass ) )
            {
                ret = (CswNbtObjClassDesignNodeType) Node.ObjClass;
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
            NameTemplateAdd.SetOnPropChange( _NameTemplateAdd_Change );
            //if( CswTools.IsPrimaryKey( this.RelationalId ) )
            //{
            //    CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( this.RelationalId.PrimaryKey );
            //}

            // Options for Object Class Name property
            SortedList<string,CswNbtNodeTypePropListOption> ObjectClassOptions = new SortedList<string, CswNbtNodeTypePropListOption>();
            Dictionary<Int32, CswEnumNbtObjectClass> ObjectClassIds = _CswNbtResources.MetaData.getObjectClassIds();
            foreach( Int32 ObjectClassId in ObjectClassIds.Keys )
            {
                string thisObjectClassName = ObjectClassIds[ObjectClassId];
                ObjectClassOptions.Add( thisObjectClassName, new CswNbtNodeTypePropListOption( thisObjectClassName, ObjectClassId.ToString() ) );
            }
            ObjectClassValue.Options.Override( ObjectClassOptions.Values );


            // Options for Icon File Name property
            Dictionary<string, string> IconOptions = new Dictionary<string, string>();
            if( null != HttpContext.Current )
            {
                DirectoryInfo d = new DirectoryInfo( HttpContext.Current.Request.PhysicalApplicationPath + CswNbtMetaDataObjectClass.IconPrefix16 );
                FileInfo[] IconFiles = d.GetFiles();
                foreach( FileInfo IconFile in IconFiles )
                {
                    IconOptions.Add( IconFile.Name, IconFile.Name );
                }
                IconFileName.ImagePrefix = CswNbtMetaDataObjectClass.IconPrefix16; 
                IconFileName.Options = IconOptions;
            }

            // Options for Defer Search property
            //SortedList<string, CswNbtNodeTypePropListOption> DeferOptions = new SortedList<string, CswNbtNodeTypePropListOption>();
            //DeferOptions.Add( "", new CswNbtNodeTypePropListOption( "", "" ) );
            //DeferOptions.Add( "Not Searchable", new CswNbtNodeTypePropListOption( "Not Searchable", CswNbtMetaDataObjectClass.NotSearchableValue.ToString() ) );
            //foreach( CswNbtMetaDataNodeTypeProp Prop in ThisNodeType.getNodeTypeProps()
            //            .Where( Prop => Prop.getFieldTypeValue() == CswEnumNbtFieldType.Relationship ||
            //                    Prop.getFieldTypeValue() == CswEnumNbtFieldType.Location ) )
            //{
            //    DeferOptions.Add( Prop.PropName, new CswNbtNodeTypePropListOption( Prop.PropName, Prop.PropId.ToString() ) );
            //}
            //DeferSearchTo.Options


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
        public CswNbtNodePropText Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }
        public CswNbtNodePropRelationship DeferSearchTo { get { return ( _CswNbtNode.Properties[PropertyName.DeferSearchTo] ); } }
        public CswNbtNodePropImageList IconFileName { get { return ( _CswNbtNode.Properties[PropertyName.IconFileName] ); } }
        public CswNbtNodePropLogical Locked { get { return ( _CswNbtNode.Properties[PropertyName.Locked] ); } }
        public CswNbtNodePropText NameTemplate { get { return ( _CswNbtNode.Properties[PropertyName.NameTemplate] ); } }
        public CswNbtNodePropRelationship NameTemplateAdd { get { return ( _CswNbtNode.Properties[PropertyName.NameTemplateAdd] ); } }
        private void _NameTemplateAdd_Change(CswNbtNodeProp Prop)
        {
            // Add the selected value to the name template
            CswNbtObjClassDesignNodeTypeProp SelectedProp = _CswNbtResources.Nodes[NameTemplateAdd.RelatedNodeId];
            NameTemplate.Text += CswNbtMetaData.MakeTemplateEntry( SelectedProp.PropName.Text );
            // Clear the selected value
            NameTemplateAdd.RelatedNodeId = null;
        }
        public CswNbtNodePropText NodeTypeName { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeName] ); } }
        public CswNbtNodePropText ObjectClassName { get { return ( _CswNbtNode.Properties[PropertyName.ObjectClassName] ); } }
        public CswNbtNodePropList ObjectClassValue { get { return ( _CswNbtNode.Properties[PropertyName.ObjectClassValue] ); } }

        #endregion


    }//CswNbtObjClassDesignNodeType

}//namespace ChemSW.Nbt.ObjClasses
