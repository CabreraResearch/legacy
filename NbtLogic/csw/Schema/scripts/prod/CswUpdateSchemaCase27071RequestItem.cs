using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27071RequestItem
    /// </summary>
    public class CswUpdateSchemaCase27071RequestItem : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataObjectClassProp TypeOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, false );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOcp, TypeOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtObjClassRequestItem.Types.Request );

            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            CswNbtMetaDataObjectClassProp RequestGroupOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.InventoryGroup.ToString() );
            CswNbtMetaDataObjectClassProp RequestOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestItemOc )
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.InventoryGroup,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.PropertyReference,
                    IsFk = true,
                    FkType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                    FkValue = RequestOcp.PropId,
                    ValuePropId = RequestGroupOcp.PropId,
                    ValuePropType = NbtViewPropIdType.ObjectClassPropId.ToString()
                } );
            
            foreach( CswNbtMetaDataNodeType RequestItemNt in RequestItemOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp TypeNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
                TypeNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                CswNbtMetaDataNodeTypeProp StatusNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status );
                CswNbtMetaDataNodeTypeProp MaterialNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Material );
                CswNbtMetaDataNodeTypeProp NumberNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Number );
                CswNbtMetaDataNodeTypeProp OrderNoNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
                CswNbtMetaDataNodeTypeProp FulfillNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Fulfill );
                CswNbtMetaDataNodeTypeProp RequestNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
                CswNbtMetaDataNodeTypeProp IGroupNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
                CswNbtMetaDataNodeTypeProp TotalDispNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.TotalDispensed );
                
                NumberNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 1 );
                MaterialNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 1, DisplayColumn: 2 );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 1 );
                OrderNoNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 2, DisplayColumn: 2 );
                IGroupNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 3, DisplayColumn: 1 );
                StatusNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 1 );
                TotalDispNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 4, DisplayColumn: 2 );
                FulfillNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, DisplayRow: 5, DisplayColumn: 1 );

                CswNbtMetaDataNodeTypeProp RequestByNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.RequestBy );
                CswNbtMetaDataNodeTypeProp SizeNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Size );
                CswNbtMetaDataNodeTypeProp CountNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Count );
                CswNbtMetaDataNodeTypeProp QuantityNtp = RequestItemNt.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Quantity );

                RequestByNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 1, DisplayColumn: 1 );
                CountNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 1 );
                SizeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 2, DisplayColumn: 2 );
                QuantityNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true, DisplayRow: 3, DisplayColumn: 1 );
            }

            CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataNodeType RoleNt = RoleOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != RoleNt )
            {
                CswNbtObjClassRole NodeAsRole = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( RoleNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                if( null != NodeAsRole )
                {
                    NodeAsRole.Name.Text = "CISPro_Dispenser";
                    NodeAsRole.postChanges( true );
                    
                    CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
                    foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getLatestVersionNodeTypes() )
                    {
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.View, MaterialNt, NodeAsRole, true );
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Create, MaterialNt, NodeAsRole, true );
                    }

                    CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
                    foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getLatestVersionNodeTypes() )
                    {
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.View, ContainerNt, NodeAsRole, true );
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Create, ContainerNt, NodeAsRole, true );
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Edit, ContainerNt, NodeAsRole, true );
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Delete, ContainerNt, NodeAsRole, true );
                    }

                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Receiving, NodeAsRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DispenseContainer, NodeAsRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.DisposeContainer, NodeAsRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Submit_Request, NodeAsRole, true );

                    CswNbtMetaDataNodeType UserNt = UserOc.getLatestVersionNodeTypes().FirstOrDefault();
                    if( null != UserNt )
                    {
                        CswNbtObjClassUser NodeAsUser = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UserNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                        if( null != NodeAsUser )
                        {
                            NodeAsUser.Role.RelatedNodeId = NodeAsRole.NodeId;
                            NodeAsUser.UsernameProperty.Text = "dispenser";
                            NodeAsUser.postChanges( true );
                        }
                    }
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase27071RequestItem

}//namespace ChemSW.Nbt.Schema