using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case29542 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29542; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true ) )
            {
                RoleNode.IsDemo = false;
                RoleNode.postChanges( false );
            }
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtObjClassUser UserNode in UserOC.getNodes( false, true ) )
            {
                if( UserNode.IsAdministrator() )
                {
                    UserNode.IsDemo = false;
                    UserNode.postChanges( false );
                }
            }
            CswNbtMetaDataObjectClass InvGrpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            foreach( CswNbtNode InvGrpNode in InvGrpOC.getNodes( false, true ) )
            {
                InvGrpNode.IsDemo = false;
                InvGrpNode.postChanges( false );
            }
            CswNbtMetaDataObjectClass InvGrpPermOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass );
            foreach( CswNbtNode InvGrpPermNode in InvGrpPermOC.getNodes( false, true ) )
            {
                InvGrpPermNode.IsDemo = false;
                InvGrpPermNode.postChanges( false );
            }
            CswNbtMetaDataObjectClass JurisdictionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.JurisdictionClass );
            foreach( CswNbtNode JurisdictionNode in JurisdictionOC.getNodes( false, true ) )
            {
                JurisdictionNode.IsDemo = false;
                JurisdictionNode.postChanges( false );
            }
            CswNbtMetaDataObjectClass LabelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
            foreach( CswNbtNode LabelNode in LabelOC.getNodes( false, true ) )
            {
                LabelNode.IsDemo = false;
                LabelNode.postChanges( false );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema