using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActUpdatePropertyValue
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtActUpdatePropertyValue( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }


        public void UpdateNode( CswNbtNode Node, bool ForceUpdate )
        {
            // BZ 10240
            if( ( Node.PendingUpdate || ForceUpdate ) && Node.getObjectClass().ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.EquipmentClass )
            {
                ( (CswNbtObjClassEquipment) Node ).SyncEquipmentToAssembly();
            }


            // Update all out of date values for a given node
            foreach( CswNbtNodePropWrapper PropWrapper in Node.Properties )
            {
                if( PropWrapper.PendingUpdate || Node.PendingUpdate || ForceUpdate )
                {
                    switch( PropWrapper.getFieldType().FieldType )
                    {
                        case CswNbtMetaDataFieldType.NbtFieldType.Composite:
                            CswNbtNodePropComposite CompositeProp = PropWrapper.AsComposite;
                            CompositeProp.RecalculateCompositeValue();
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.Location:
                            CswNbtNodePropLocation LocationProp = PropWrapper.AsLocation;
                            LocationProp.RefreshNodeName();
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.LogicalSet:
                            CswNbtNodePropLogicalSet LogicalSetProp = PropWrapper.AsLogicalSet;
                            LogicalSetProp.RefreshStringValue();
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                            CswNbtNodePropNodeTypeSelect NodeTypeSelectProp = PropWrapper.AsNodeTypeSelect;
                            NodeTypeSelectProp.RefreshSelectedNodeTypeNames();
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
                            CswNbtNodePropPropertyReference PropertyReferenceProp = PropWrapper.AsPropertyReference;
                            PropertyReferenceProp.RecalculateReferenceValue();
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                            CswNbtNodePropRelationship RelationshipProp = PropWrapper.AsRelationship;
                            RelationshipProp.RefreshNodeName();
                            break;
                        //case CswNbtMetaDataFieldType.NbtFieldType.MultiRelationship:
                        //    CswNbtNodePropMultiRelationship MultiRelationshipProp = PropWrapper.AsMultiRelationship;
                        //    RelationshipProp.RefreshNodeNames();
                        //    break;
                        case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                            CswNbtNodePropViewPickList ViewPickListProp = PropWrapper.AsViewPickList;
                            ViewPickListProp.RefreshViewName();
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.ViewReference:
                            CswNbtNodePropViewReference ViewReferenceProp = PropWrapper.AsViewReference;
                            ViewReferenceProp.RefreshViewName();
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                            CswNbtNodePropMTBF MTBFProp = PropWrapper.AsMTBF;
                            MTBFProp.RefreshCachedValue();
                            break;
                        default:
                            //throw new CswDniException("Invalid property type to update", "CswNbtActUpdatePropertyValues encountered a property type (" + PropWrapper.FieldType.FieldType.ToString() + ") that it did not know how to update");
                            //_CswNbtResources.logError( new CswDniException( "Invalid property type to update", "CswNbtActUpdatePropertyValues encountered a property type (" + PropWrapper.FieldType.FieldType.ToString() + ") that it did not know how to update" ) );
                            PropWrapper.PendingUpdate = false;
                            break;
                    } // switch (PropWrapper.FieldType.FieldType)

                    // Each prop class should handle this on its own
                    // PropWrapper.PendingUpdate = false;


                } // if(PropWrapper.PendingUpdate)
            } // foreach (CswNbtNodePropWrapper PropWrapper in Node.Properties)

            Node.PendingUpdate = false;

        }// UpdateNode()
    } // public class CswNbtActUpdatePropertyValue
} // namespace ChemSW.Nbt.Actions
