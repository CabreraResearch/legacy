using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29142
    /// </summary>
    public class CswUpdateSchema_01Y_Case29142 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29142; }
        }

        public override void update()
        {

            CswNbtObjClassRole CISPro_Request_Fulfiller = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "cispro_request_fulfiller" );
            CswNbtMetaDataObjectClass requestContainerUpdateOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestContainerUpdateClass );
            if( null != CISPro_Request_Fulfiller )
            {
                foreach( CswNbtMetaDataNodeType requestContainerUpdateNT in requestContainerUpdateOC.getNodeTypes() )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.View, requestContainerUpdateNT, CISPro_Request_Fulfiller, true );
                }
            }


        } //Update()

    }//class CswUpdateSchema_01Y_Case29142

}//namespace ChemSW.Nbt.Schema