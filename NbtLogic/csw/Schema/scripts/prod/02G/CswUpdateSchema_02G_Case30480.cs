using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30480 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Update Location Inventory Group Data Again"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30480; }
        }

        public override string ScriptName
        {
            get { return "IGUpdate Take 2"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClass InventoryGroupPermissisonOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass );

            //1: Get Default Inventory Group
            CswNbtObjClassInventoryGroup DefaultInventoryGroup = null;
            foreach( CswNbtObjClassInventoryGroup InventoryGroup in InventoryGroupOc.getNodes( true, false, false, true ) )
            {
                if( InventoryGroup.Name.Text == "Default Inventory Group" )
                {
                    DefaultInventoryGroup = InventoryGroup;
                    break;
                }
            }
            if( null == DefaultInventoryGroup )
            {
                DefaultInventoryGroup = InventoryGroupOc.getNodes( true, false, false, true ).FirstOrDefault();
            }
            if( null == DefaultInventoryGroup )
            {
                CswNbtMetaDataNodeType InvGrpNT = InventoryGroupOc.FirstNodeType;
                if( null != InvGrpNT )
                {
                    DefaultInventoryGroup = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( InvGrpNT.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassInventoryGroup DefInvGrp = NewNode;
                        DefInvGrp.Name.Text = "Default Inventory Group";
                    } );
                }
            }

            if( null != DefaultInventoryGroup )
            {
                //2: Set Default Value to Default Inventory Group, if no DefaultValue is present
                CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                foreach( CswNbtMetaDataNodeType LocationNt in LocationOc.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp InventoryGroupNtp = LocationNt.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                    if( false == InventoryGroupNtp.HasDefaultValue() )
                    {
                        InventoryGroupNtp.DefaultValue.AsRelationship.RelatedNodeId = DefaultInventoryGroup.NodeId;
                    }
                }

                //3: Update any null Inventory Group values with Default
                foreach( CswNbtObjClassLocation Location in LocationOc.getNodes( true, false, false, true ) )
                {
                    if( false == CswTools.IsPrimaryKey( Location.InventoryGroup.RelatedNodeId ) )
                    {
                        Location.InventoryGroup.RelatedNodeId = DefaultInventoryGroup.NodeId;
                        Location.postChanges( ForceUpdate: false );
                    }
                }

                //4: Ensure at least one Inventory Group Permission exists
                CswNbtView IgPermitView = _CswNbtSchemaModTrnsctn.makeView();
                IgPermitView.AddViewRelationship( InventoryGroupPermissisonOc, IncludeDefaultFilters: false );
                ICswNbtTree Tree = _CswNbtSchemaModTrnsctn.getTreeFromView( IgPermitView, IncludeSystemNodes: false );
                if( Tree.getChildNodeCount() == 0 )
                {
                    CswNbtMetaDataNodeType IgPermNt = InventoryGroupPermissisonOc.getLatestVersionNodeTypes().FirstOrDefault();
                    if( null != IgPermNt )
                    {
                        CswNbtObjClassInventoryGroupPermission DefaultPermission = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( IgPermNt.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            CswNbtObjClassInventoryGroupPermission WildCardPerm = NewNode;
                            WildCardPerm.PermissionGroup.RelatedNodeId = DefaultInventoryGroup.NodeId;
                            WildCardPerm.ApplyToAllRoles.Checked = CswEnumTristate.True;
                            WildCardPerm.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                            WildCardPerm.Dispense.Checked = CswEnumTristate.True;
                            WildCardPerm.Dispose.Checked = CswEnumTristate.True;
                            WildCardPerm.Edit.Checked = CswEnumTristate.True;
                            WildCardPerm.Request.Checked = CswEnumTristate.True;
                            WildCardPerm.Undispose.Checked = CswEnumTristate.True;
                        } );
                    }
                }
            }
        }
    }
}