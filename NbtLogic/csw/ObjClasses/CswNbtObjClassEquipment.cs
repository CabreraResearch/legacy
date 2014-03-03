using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions.KioskModeRules.OperationClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEquipment: CswNbtObjClass, ICswNbtKioskModeMoveable
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Assembly = "Assembly";
            public const string Type = "Type";
            public const string Parts = "Parts";
            public const string Status = "Status";
            public const string EquipmentId = "Equipment Id";
            public const string Location = "Location";
            public const string Condition = "Condition";
            public const string ContractNo = "Contract No";
            public const string Department = "Department";
            public const string Description = "Description";
            public const string Documents = "Documents";
            public const string HasServiceContract = "Has Service Contract";
            public const string MTBF = "MTBF";
            public const string ManualStoredAt = "Manual Stored At";
            public const string Manufacturer = "Manufacturer";
            public const string Model = "Model";
            public const string Notes = "Notes";
            public const string OutOn = "Out On";
            public const string Picture = "Picture";
            public const string Problem = "Problem";
            public const string PropertyNo = "Property No";
            public const string Purchased = "Purchased";
            public const string Received = "Received";
            public const string Responsible = "Responsible";
            public const string Schedule = "Schedule";
            public const string SerialNo = "Serial No";
            public const string ServiceCost = "Service Cost";
            public const string ServiceEndsOn = "Service Ends On";
            public const string ServicePhone = "Service Phone";
            public const string ServiceVendor = "Service Vendor";
            public const string StartingCost = "Starting Cost";
            public const string Task = "Task";
            public const string User = "User";
            public const string UserPhone = "User Phone";
            public const string Vendor = "Vendor";
        }

        public static string PartsXValueName { get { return "Uses"; } }

        private readonly CswNbtKioskModeMoveableImpl _Mover;

        public CswNbtObjClassEquipment( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _Mover = new CswNbtKioskModeMoveableImpl( CswNbtResources, this );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEquipment
        /// </summary>
        public static implicit operator CswNbtObjClassEquipment( CswNbtNode Node )
        {
            CswNbtObjClassEquipment ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.EquipmentClass ) )
            {
                ret = (CswNbtObjClassEquipment) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void afterPopulateProps()
        {
            if( Type.RelatedNodeId != null )
            {
                CswNbtNode TypeNode = _CswNbtResources.Nodes[Type.RelatedNodeId];
                if( TypeNode != null )
                {
                    CswNbtObjClassEquipmentType TypeNodeAsType = (CswNbtObjClassEquipmentType) TypeNode;
                    CswDelimitedString PartsString = new CswDelimitedString( '\n' );
                    PartsString.FromString( TypeNodeAsType.Parts.Text.Replace( "\r", "" ) );
                    this.Parts.YValues = PartsString;
                }
            }
            if( false == CswTools.IsDate( _Date ) )
            {
                SyncEquipmentToAssembly();
            }
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            // BZ 10454
            // Filter out Retired Equipment by default
            CswNbtMetaDataObjectClassProp StatusOCP = this.ObjectClass.getObjectClassProp( PropertyName.Status );
            CswNbtViewProperty StatusViewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, StatusOCP );
            CswNbtViewPropertyFilter StatusViewPropFilter = ParentRelationship.View.AddViewPropertyFilter( StatusViewProp,
                                                                                                           StatusOCP.getFieldTypeRule().SubFields.Default.Name,
                                                                                                           CswEnumNbtFilterMode.NotEquals,
                                                                                                           "Retired", //StatusOptionToDisplayString( StatusOption.Retired ),
                                                                                                           false );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override CswNbtNode CopyNode( bool IsNodeTemp = false, Action<CswNbtNode> OnCopy = null )
        {
            CswNbtNode CopiedEquipmentNode = base.CopyNodeImpl( IsNodeTemp, OnCopy );

            // Copy all Generators
            CswNbtMetaDataObjectClass GeneratorObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GeneratorClass );
            CswNbtView GeneratorView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship GeneratorRelationship = GeneratorView.AddViewRelationship( GeneratorObjectClass, false );
            CswNbtViewProperty OwnerProperty = GeneratorView.AddViewProperty( GeneratorRelationship, GeneratorObjectClass.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner ) );
            CswNbtViewPropertyFilter OwnerIsEquipmentFilter = GeneratorView.AddViewPropertyFilter(
                OwnerProperty,
                CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                CswEnumNbtFilterMode.Equals,
                NodeId.PrimaryKey.ToString() );

            ICswNbtTree GeneratorTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, GeneratorView, true, false, false );
            GeneratorTree.goToRoot();
            Int32 c = 0;
            while( c < GeneratorTree.getChildNodeCount() )
            {
                GeneratorTree.goToNthChild( c );
                CswNbtNode OriginalGeneratorNode = GeneratorTree.getNodeForCurrentPosition();
                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalGeneratorNode.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        NewNode.copyPropertyValues( OriginalGeneratorNode );
                        ( (CswNbtObjClassGenerator) NewNode ).Owner.RelatedNodeId = CopiedEquipmentNode.NodeId;
                    } );
                GeneratorTree.goToParentNode();
                c++;
            }

            return CopiedEquipmentNode;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Assembly { get { return ( _CswNbtNode.Properties[PropertyName.Assembly] ); } }
        public CswNbtNodePropRelationship Type { get { return ( _CswNbtNode.Properties[PropertyName.Type] ); } }
        public CswNbtNodePropLogicalSet Parts { get { return ( _CswNbtNode.Properties[PropertyName.Parts] ); } }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[PropertyName.Status] ); } }
        public CswNbtNodePropBarcode EquipmentId { get { return ( _CswNbtNode.Properties[PropertyName.EquipmentId] ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        public CswNbtNodePropList Condition { get { return ( _CswNbtNode.Properties[PropertyName.Condition] ); } }
        public CswNbtNodePropText ContractNo { get { return ( _CswNbtNode.Properties[PropertyName.ContractNo] ); } }
        public CswNbtNodePropRelationship Department { get { return ( _CswNbtNode.Properties[PropertyName.Department] ); } }
        public CswNbtNodePropMemo Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropGrid Documents { get { return ( _CswNbtNode.Properties[PropertyName.Documents] ); } }
        public CswNbtNodePropLogical HasServiceContract { get { return ( _CswNbtNode.Properties[PropertyName.HasServiceContract] ); } }
        public CswNbtNodePropMTBF MTBF { get { return ( _CswNbtNode.Properties[PropertyName.MTBF] ); } }
        public CswNbtNodePropText ManualStoredAt { get { return ( _CswNbtNode.Properties[PropertyName.ManualStoredAt] ); } }
        public CswNbtNodePropText Manufacturer { get { return ( _CswNbtNode.Properties[PropertyName.Manufacturer] ); } }
        public CswNbtNodePropText Model { get { return ( _CswNbtNode.Properties[PropertyName.Model] ); } }
        public CswNbtNodePropMemo Notes { get { return ( _CswNbtNode.Properties[PropertyName.Notes] ); } }
        public CswNbtNodePropDateTime OutOn { get { return ( _CswNbtNode.Properties[PropertyName.OutOn] ); } }
        public CswNbtNodePropText Picture { get { return ( _CswNbtNode.Properties[PropertyName.Picture] ); } }
        public CswNbtNodePropGrid Problem { get { return ( _CswNbtNode.Properties[PropertyName.Problem] ); } }
        public CswNbtNodePropText PropertyNo { get { return ( _CswNbtNode.Properties[PropertyName.PropertyNo] ); } }
        public CswNbtNodePropDateTime Purchased { get { return ( _CswNbtNode.Properties[PropertyName.Purchased] ); } }
        public CswNbtNodePropDateTime Received { get { return ( _CswNbtNode.Properties[PropertyName.Received] ); } }
        public CswNbtNodePropText Responsible { get { return ( _CswNbtNode.Properties[PropertyName.Responsible] ); } }
        public CswNbtNodePropGrid Schedule { get { return ( _CswNbtNode.Properties[PropertyName.Schedule] ); } }
        public CswNbtNodePropText SerialNo { get { return ( _CswNbtNode.Properties[PropertyName.SerialNo] ); } }
        public CswNbtNodePropText ServiceCost { get { return ( _CswNbtNode.Properties[PropertyName.ServiceCost] ); } }
        public CswNbtNodePropDateTime ServiceEndsOn { get { return ( _CswNbtNode.Properties[PropertyName.ServiceEndsOn] ); } }
        public CswNbtNodePropText ServicePhone { get { return ( _CswNbtNode.Properties[PropertyName.ServicePhone] ); } }
        public CswNbtNodePropRelationship ServiceVendor { get { return ( _CswNbtNode.Properties[PropertyName.ServiceVendor] ); } }
        public CswNbtNodePropText StartingCost { get { return ( _CswNbtNode.Properties[PropertyName.StartingCost] ); } }
        public CswNbtNodePropGrid Task { get { return ( _CswNbtNode.Properties[PropertyName.Task] ); } }
        public CswNbtNodePropRelationship User { get { return ( _CswNbtNode.Properties[PropertyName.User] ); } }
        public CswNbtNodePropPropertyReference UserPhone { get { return ( _CswNbtNode.Properties[PropertyName.UserPhone] ); } }
        public CswNbtNodePropRelationship Vendor { get { return ( _CswNbtNode.Properties[PropertyName.Vendor] ); } }

        #endregion

        public void SyncEquipmentToAssembly()
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
                            if( EquipProp.getFieldTypeValue() != CswEnumNbtFieldType.Grid ) // case 27270
                            {
                                if( EquipProp.PropName.ToLower() == AssemblyProp.PropName.ToLower() && EquipProp.getFieldType() == AssemblyProp.getFieldType() )
                                {
                                    // Found a match -- copy the value and set readonly
                                    EquipProp.copy( AssemblyProp );
                                    EquipProp.setReadOnly( value : true, SaveToDb : true );
                                    FoundMatch = true;
                                    // case 21809
                                    EquipProp[CswEnumNbtPropertyAttributeName.HelpText] = EquipProp.PropName + " is set on the Assembly, and must be modified there.";
                                }
                            }
                        }
                        if( !FoundMatch )
                        {
                            // if other things set these properties to readonly, this might be an issue.
                            // but it must be conditional - see BZ 7084
                            if( EquipProp.ReadOnly )
                            {
                                EquipProp.setReadOnly( value : false, SaveToDb : true );
                            }
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
                    {
                        EquipProp.setReadOnly( value : false, SaveToDb : true );
                    }
                }
            }
        } // SynchEquipmentToAssembly()

        public void TransferEquipment( CswNbtObjClassUser NewUser )
        {
            Location.SelectedNodeId = NewUser.DefaultLocationId;
            Location.SyncGestalt();
            Location.RefreshNodeName();

            UpdateOwner( NewUser );
        }

        public void UpdateOwner( CswNbtObjClassUser NewUser )
        {
            User.RelatedNodeId = NewUser.NodeId;
            User.RefreshNodeName();
            User.SyncGestalt();
        }

        #region ICswNbtKioskModeMoveable

        /// <summary>
        /// Returns false if this equipment is in an assembly
        /// </summary>
        public bool CanMove( out string Error )
        {
            //Intentionally dont use default Move
            bool ret = true;
            Error = string.Empty;
            if( null != Assembly.RelatedNodeId )
            {
                CswNbtObjClassEquipmentAssembly assembly = _CswNbtResources.Nodes[Assembly.RelatedNodeId];
                if( null != assembly )
                {
                    ret = false;
                    Error = "Cannot perform MOVE operation on Equipment (" + EquipmentId.Barcode + ") when it belongs to Assembly (" + assembly.Barcode.Barcode + ")";
                }
            }
            return ret;
        }

        public void Move( CswNbtObjClassLocation LocationToMoveTo )
        {
            _Mover.Move(LocationToMoveTo);
        }

        #endregion

    }//CswNbtObjClassEquipment

}//namespace ChemSW.Nbt.ObjClasses
