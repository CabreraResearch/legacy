using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEquipment : CswNbtObjClass
    {
        public static string AssemblyPropertyName { get { return "Assembly"; } }
        public static string TypePropertyName { get { return "Type"; } }

        public static string PartsPropertyName { get { return "Parts"; } }
        public static string PartsXValueName { get { return "Uses"; } }

        public static string StatusPropertyName { get { return "Status"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassEquipment( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            if( Assembly.WasModified )
                //_CswNbtNode.PendingUpdate = true;
                SynchEquipmentToAssembly();

            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( Assembly.WasModified )
                //_CswNbtNode.PendingUpdate = true;
                SynchEquipmentToAssembly();

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode(bool DeleteAllRequiredRelatedNodes = false)
        {
            _CswNbtObjClassDefault.beforeDeleteNode(DeleteAllRequiredRelatedNodes);

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            if( Type.RelatedNodeId != null )
            {
                CswNbtNode TypeNode = _CswNbtResources.Nodes[Type.RelatedNodeId];
                if( TypeNode != null )
                {
                    CswNbtObjClassEquipmentType TypeNodeAsType = CswNbtNodeCaster.AsEquipmentType( TypeNode );
                    CswDelimitedString PartsString = new CswDelimitedString( '\n' );
                    PartsString.FromString( TypeNodeAsType.Parts.Text.Replace( "\r", "" ) );
                    this.Parts.YValues = PartsString;
                }
            }

            // case 21809
            SynchEquipmentToAssembly();

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            // BZ 10454
            // Filter out Retired Equipment by default
            CswNbtMetaDataObjectClassProp StatusOCP = this.ObjectClass.getObjectClassProp( StatusPropertyName );
            CswNbtViewProperty StatusViewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, StatusOCP );
            CswNbtViewPropertyFilter StatusViewPropFilter = ParentRelationship.View.AddViewPropertyFilter( StatusViewProp,
                                                                                                           StatusOCP.getFieldTypeRule().SubFields.Default.Name,
                                                                                                           CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                                                                                           "Retired", //StatusOptionToDisplayString( StatusOption.Retired ),
                                                                                                           false );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        #region Object class specific properties


        public CswNbtNodePropRelationship Assembly
        {
            get
            {
                return ( _CswNbtNode.Properties[AssemblyPropertyName].AsRelationship );
            }
        }
        public CswNbtNodePropRelationship Type
        {
            get
            {
                return ( _CswNbtNode.Properties[TypePropertyName].AsRelationship );
            }
        }
        public CswNbtNodePropLogicalSet Parts
        {
            get
            {
                return ( _CswNbtNode.Properties[PartsPropertyName].AsLogicalSet );
            }
        }
        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[StatusPropertyName].AsList );
            }
        }

        #endregion


        public void SynchEquipmentToAssembly()
        {
            // for all equipment properties that match properties on the assembly
            bool FoundAssemblyNode = false;
            if( this.Assembly.RelatedNodeId != null )
            {
                CswNbtNode AssemblyNode = _CswNbtResources.Nodes.GetNode( this.Assembly.RelatedNodeId );
                if( AssemblyNode != null )
                {
                    FoundAssemblyNode = true;
                    foreach( CswNbtNodePropWrapper EquipProp in this.Node.Properties )
                    {
                        bool FoundMatch = false;
                        foreach( CswNbtNodePropWrapper AssemblyProp in AssemblyNode.Properties )
                        {
                            if( EquipProp.PropName.ToLower() == AssemblyProp.PropName.ToLower() && EquipProp.getFieldType() == AssemblyProp.getFieldType() )
                            {
                                // Found a match -- copy the value and set readonly
                                EquipProp.copy( AssemblyProp );
                                EquipProp.ReadOnly = true;
                                FoundMatch = true;
                                // case 21809
                                EquipProp.HelpText = EquipProp.PropName + " is set on the Assembly, and must be modified there.";
                            }
                        }
                        if( !FoundMatch )
                        {
                            // if other things set these properties to readonly, this might be an issue.
                            // but it must be conditional - see BZ 7084
                            if( EquipProp.ReadOnly )
                                EquipProp.ReadOnly = false;
                        }
                    }
                }
            } //  if( this.Assembly.RelatedNodeId != null )

            if( !FoundAssemblyNode )
            {
                foreach( CswNbtNodePropWrapper EquipProp in this.Node.Properties )
                {
                    // if other things set these properties to readonly, this might be an issue.
                    // but it must be conditional - see BZ 7084
                    if( EquipProp.ReadOnly )
                        EquipProp.ReadOnly = false;
                }
            }
        } // SynchEquipmentToAssembly()

    }//CswNbtObjClassEquipment

}//namespace ChemSW.Nbt.ObjClasses
