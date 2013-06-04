using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29833
    /// </summary>
    public class CswUpdateSchema_02C_Case29833B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29833; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            CswNbtMetaDataObjectClass SDSDocOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
            CswNbtMetaDataNodeType SDSDocNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SDS Document" );
            Int32 SDSId = 0;
            if( null != SDSDocNT )
            {
                SDSId = SDSDocNT.NodeTypeId;
            }
            //Change SDS's OC to SDSDocument
            CswTableUpdate NTUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29833_nt_update", "nodetypes" );
            DataTable NTTable = NTUpdate.getTable( "where objectclassid = " + DocumentOC.ObjectClassId );
            foreach( DataRow NTRow in NTTable.Rows )
            {
                if( NTRow["nodetypeid"].ToString() == SDSId.ToString())
                {
                    NTRow["objectclassid"] = SDSDocOC.ObjectClassId;
                }
            }
            NTUpdate.update( NTTable );
        } // update()

    }//class CswUpdateSchema_02B_Case29833B

}//namespace ChemSW.Nbt.Schema