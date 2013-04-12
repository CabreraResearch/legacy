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
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27906; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.Login_Data, false, String.Empty, "System" );

            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtNode RoleNode in RoleOC.getNodes( false, true ) )
            {
                bool CanViewLoginData = ( RoleNode.NodeName == "Administrator" || RoleNode.NodeName == "chemsw_admin_role" );
                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Login_Data, RoleNode, CanViewLoginData );
            }
        } // update()
    }//class CswUpdateSchema_02A_Case27906
}//namespace ChemSW.Nbt.Schema