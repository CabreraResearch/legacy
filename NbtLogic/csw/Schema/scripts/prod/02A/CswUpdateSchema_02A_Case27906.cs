using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27906
    /// </summary>
    public class CswUpdateSchema_02A_Case27906 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27906; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Login_Data, false, String.Empty, "System" );

            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RoleClass );
            foreach( CswNbtNode RoleNode in RoleOC.getNodes( false, true ) )
            {
                bool CanViewLoginData = ( RoleNode.NodeName == "Administrator" || RoleNode.NodeName == "chemsw_admin_role" );
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Material_Approval, RoleNode, CanViewLoginData );
            }
        } // update()
    }//class CswUpdateSchema_02A_Case27906
}//namespace ChemSW.Nbt.Schema