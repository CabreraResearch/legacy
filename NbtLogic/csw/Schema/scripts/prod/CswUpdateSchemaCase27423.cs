using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27423
    /// </summary>
    public class CswUpdateSchemaCase27423 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );

            CswNbtObjClassInventoryGroup DefaultNodeAsInventoryGroup = null;
            foreach( CswNbtNode Group in InventoryGroupOc.getNodes( true, false ) )
            {
                CswNbtObjClassInventoryGroup NodeAsInventoryGroup = Group;
                if( null != NodeAsInventoryGroup )
                {
                    if( NodeAsInventoryGroup.Name.Text.ToLower() == "cispro" )
                    {
                        DefaultNodeAsInventoryGroup = NodeAsInventoryGroup;
                        DefaultNodeAsInventoryGroup.Name.Text = "Default Inventory Group";
                        DefaultNodeAsInventoryGroup.IsDemo = true;
                        DefaultNodeAsInventoryGroup.postChanges( ForceUpdate: false );
                    }
                }
            }
            if( null == DefaultNodeAsInventoryGroup )
            {
                CswNbtMetaDataNodeType InventoryGroupNt = InventoryGroupOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null != InventoryGroupNt )
                {
                    DefaultNodeAsInventoryGroup = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( InventoryGroupNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                    DefaultNodeAsInventoryGroup.Name.Text = "Default Inventory Group";
                    DefaultNodeAsInventoryGroup.postChanges( ForceUpdate: false );
                }
            }
            CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtObjClassLocation DefaultLocation = null;
            if( null != DefaultNodeAsInventoryGroup )
            {
                foreach( CswNbtObjClassLocation Location in LocationOc.getNodes( true, false ) )
                {
                    if( null != Location )
                    {
                        if( Location.Name.Text.ToLower() == "center hall" )
                        {
                            DefaultLocation = Location;
                        }
                        if( null == Location.InventoryGroup.RelatedNodeId || Int32.MinValue == Location.InventoryGroup.RelatedNodeId.PrimaryKey )
                        {
                            Location.InventoryGroup.RelatedNodeId = DefaultNodeAsInventoryGroup.NodeId;
                            Location.postChanges( ForceUpdate: false );
                        }
                    }
                }
            }

            CswNbtMetaDataObjectClass WorkUnitOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass );
            CswNbtMetaDataNodeType WorkUnitNt = WorkUnitOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != WorkUnitNt )
            {
                WorkUnitNt.NameTemplateValue = "";
                WorkUnitNt.addNameTemplateText( CswNbtObjClassWorkUnit.NamePropertyName );
                CswNbtObjClassWorkUnit DefaultWorkUnit = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( WorkUnitNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                DefaultWorkUnit.Name.Text = "Default Work Unit";
                DefaultWorkUnit.IsDemo = true;
                DefaultWorkUnit.postChanges( ForceUpdate: false );

                CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                Collection<Int32> RoleIdsWithUsers = new Collection<Int32>();
                CswNbtMetaDataNodeType UserNt = null;
                foreach( CswNbtObjClassUser User in UserOc.getNodes( true, false ) )
                {
                    if( null != User )
                    {
                        UserNt = UserNt ?? User.NodeType;
                        RoleIdsWithUsers.Add( User.RoleId.PrimaryKey );
                        if( null == User.WorkUnitProperty.RelatedNodeId || Int32.MinValue == User.WorkUnitProperty.RelatedNodeId.PrimaryKey )
                        {
                            User.WorkUnitProperty.RelatedNodeId = DefaultWorkUnit.NodeId;
                        }
                        if( null != DefaultLocation && ( null == User.DefaultLocationProperty.SelectedNodeId || Int32.MinValue == User.DefaultLocationProperty.SelectedNodeId.PrimaryKey ) )
                        {
                            User.DefaultLocationProperty.SelectedNodeId = DefaultLocation.NodeId;
                        }
                        User.postChanges( ForceUpdate: false );
                    }
                }

                CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
                if( null == UserNt )
                {
                    UserNt = UserOc.getLatestVersionNodeTypes().FirstOrDefault();
                }
                if( null != UserNt )
                {
                    foreach( CswNbtObjClassRole Role in RoleOc.getNodes( true, false ) )
                    {
                        if( null != Role && false == RoleIdsWithUsers.Contains( Role.NodeId.PrimaryKey ) )
                        {
                            string ValidUserName = CswNbtObjClassUser.getValidUserName( Role.Name.Text.ToLower() );
                            if( ValidUserName != CswNbtObjClassUser.ChemSWAdminUsername )
                            {
                                if( CswNbtObjClassUser.IsUserNameUnique( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources, ValidUserName ) )
                                {
                                    CswNbtObjClassUser NewUser = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UserNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                                    NewUser.IsDemo = false;
                                    NewUser.Role.RelatedNodeId = Role.NodeId;
                                    NewUser.UsernameProperty.Text = ValidUserName;
                                    NewUser.WorkUnitProperty.RelatedNodeId = DefaultWorkUnit.NodeId;
                                    if( null != DefaultLocation )
                                    {
                                        NewUser.DefaultLocationProperty.SelectedNodeId = DefaultLocation.NodeId;
                                    }
                                    NewUser.PasswordProperty.Password = Role.Name.Text.ToLower();
                                    NewUser.postChanges( ForceUpdate: false );
                                }
                            }
                        }
                    }
                }
                CswNbtMetaDataObjectClass IgPermissionOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass );
                foreach( CswNbtObjClassInventoryGroupPermission Permission in IgPermissionOc.getNodes( true, false ) )
                {
                    if( null != Permission )
                    {
                        if( null != DefaultNodeAsInventoryGroup && ( null == Permission.InventoryGroup.RelatedNodeId || Int32.MinValue == Permission.InventoryGroup.RelatedNodeId.PrimaryKey ) )
                        {
                            Permission.InventoryGroup.RelatedNodeId = DefaultNodeAsInventoryGroup.NodeId;
                        }
                        if( null == Permission.WorkUnit.RelatedNodeId || Int32.MinValue == Permission.WorkUnit.RelatedNodeId.PrimaryKey )
                        {
                            Permission.WorkUnit.RelatedNodeId = DefaultWorkUnit.NodeId;
                        }
                        Permission.postChanges( ForceUpdate: false );
                    }
                }
            }

            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClass SynonymOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass );
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClass VendorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass );
            CswNbtMetaDataObjectClass UnitOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );

            CswNbtObjClassVendor DefaultVendor = null;
            CswNbtMetaDataNodeType VendorNt = VendorOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != VendorNt )
            {
                DefaultVendor = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( VendorNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                DefaultVendor.VendorName.Text = "Default Vendor";
                DefaultVendor.IsDemo = true;
                DefaultVendor.postChanges( ForceUpdate: false );
            }
            CswNbtMetaDataNodeType SizeNt = SizeOc.getLatestVersionNodeTypes().FirstOrDefault();
            CswNbtMetaDataNodeType SynonymNt = SynonymOc.getLatestVersionNodeTypes().FirstOrDefault();
            CswNbtObjClassUnitOfMeasure DefaultSizeUnit = null;
            CswNbtObjClassUnitOfMeasure DefaultTimeUnit = null;
            foreach( CswNbtObjClassUnitOfMeasure Unit in UnitOc.getNodes( true, false ) )
            {
                if( null != Unit )
                {
                    if( Unit.Name.Text.ToLower() == "g" )
                    {
                        DefaultSizeUnit = Unit;
                    }
                    else if( Unit.Name.Text.ToLower() == "years" )
                    {
                        DefaultTimeUnit = Unit;
                    }
                }
            }

            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getLatestVersionNodeTypes() )
            {
                CswNbtObjClassMaterial Material = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MaterialNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                Material.IsDemo = true;
                Material.TradeName.Text = "Default " + MaterialNt.NodeTypeName;
                if( null != DefaultVendor )
                {
                    Material.Supplier.RelatedNodeId = DefaultVendor.NodeId;
                }
                Material.PartNumber.Text = "658-35AB";
                Material.PhysicalState.Value = CswNbtObjClassMaterial.PhysicalStates.Solid;
                Material.ExpirationInterval.Quantity = 1;
                if( null != DefaultTimeUnit )
                {
                    Material.ExpirationInterval.UnitId = DefaultTimeUnit.NodeId;
                }
                Material.postChanges( ForceUpdate: false );

                if( null != SizeNt )
                {
                    CswNbtObjClassSize Size = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SizeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                    Size.IsDemo = true;
                    Size.Material.RelatedNodeId = Material.NodeId;
                    Size.CatalogNo.Text = "NE-H5/3";
                    Size.InitialQuantity.Quantity = 1;
                    if( null != DefaultSizeUnit )
                    {
                        Size.InitialQuantity.UnitId = DefaultSizeUnit.NodeId;
                    }
                    Size.postChanges( ForceUpdate: false );
                }

                if( null != SynonymNt )
                {
                    CswNbtObjClassMaterialSynonym Synonym = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SynonymNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                    Synonym.IsDemo = true;
                    Synonym.Material.RelatedNodeId = Material.NodeId;
                    Synonym.Name.Text = Material.TradeName.Text + " Synonym";
                    Synonym.postChanges( ForceUpdate: false );
                }
            }


        }//Update()

    }//class CswUpdateSchemaCase27423

}//namespace ChemSW.Nbt.Schema