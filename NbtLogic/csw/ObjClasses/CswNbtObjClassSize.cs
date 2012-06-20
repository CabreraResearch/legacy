using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSize : CswNbtObjClass
    {
        public static string MaterialPropertyName { get { return "Material"; } }
        public static string CapacityPropertyName { get { return "Capacity"; } }
        public static string QuantityEditablePropertyName { get { return "Quantity Editable"; } }
        public static string DispensablePropertyName { get { return "Dispensable"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassSize( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassSize
        /// </summary>
        public static implicit operator CswNbtObjClassSize( CswNbtNode Node )
        {
            CswNbtObjClassSize ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass ) )
            {
                ret = (CswNbtObjClassSize) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

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

        public override void afterPopulateProps()
        {
            //case 25759 - set capacity unittype view based on related material physical state
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( this.Material.RelatedNodeId );
            if( MaterialNode != null )
            {
                CswNbtObjClassMaterial MaterialNodeAsMaterial = (CswNbtObjClassMaterial) MaterialNode;
                if( false == String.IsNullOrEmpty( MaterialNodeAsMaterial.PhysicalState.Value ) )
                {
                    CswNbtMetaDataNodeType SizeNodeType = this.Node.getNodeType();
                    CswNbtMetaDataNodeTypeProp Capacity = SizeNodeType.getNodeTypeProp( "Capacity" );
                    string UniqueNodeViewName = "CswNbtNodeTypePropQuantity_" + Capacity.NodeTypeId.ToString() + "_" + this.NodeId.ToString();

                    CswNbtView StateSpecificUnitTypeView = new CswNbtView( _CswNbtResources );
                    StateSpecificUnitTypeView.ViewName = UniqueNodeViewName;

                    if( MaterialNodeAsMaterial.PhysicalState.Value.ToLower() == "n/a" )
                    {
                        CswNbtMetaDataNodeType EachNT = _CswNbtResources.MetaData.getNodeType( "Each Unit" );
                        if( EachNT != null )
                        {
                            StateSpecificUnitTypeView.AddViewRelationship( EachNT, true );
                        }
                    }
                    else
                    {
                        CswNbtMetaDataNodeType WeightNT = _CswNbtResources.MetaData.getNodeType( "Weight Unit" );
                        if( WeightNT != null )
                        {
                            StateSpecificUnitTypeView.AddViewRelationship( WeightNT, true );
                        }
                        if( MaterialNodeAsMaterial.PhysicalState.Value.ToLower() != "solid" )
                        {
                            CswNbtMetaDataNodeType VolumeNT = _CswNbtResources.MetaData.getNodeType( "Volume Unit" );
                            if( VolumeNT != null )
                            {
                                StateSpecificUnitTypeView.AddViewRelationship( VolumeNT, true );
                            }
                        }
                    }
                    this.Capacity.View = StateSpecificUnitTypeView;
                }
            }

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
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

        public CswNbtNodePropRelationship Material
        {
            get { return _CswNbtNode.Properties[MaterialPropertyName].AsRelationship; }
        }
        public CswNbtNodePropQuantity Capacity
        {
            get { return _CswNbtNode.Properties[CapacityPropertyName].AsQuantity; }
        }
        public CswNbtNodePropLogical QuantityEditable
        {
            get { return _CswNbtNode.Properties[QuantityEditablePropertyName].AsLogical; }
        }
        public CswNbtNodePropLogical Dispensable
        {
            get { return _CswNbtNode.Properties[DispensablePropertyName].AsLogical; }
        }
        #endregion


    }//CswNbtObjClassSize

}//namespace ChemSW.Nbt.ObjClasses
