using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29833
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case29833B: CswUpdateSchemaTo
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
            _pointSDSNTToSDSOC();
            _deleteDocOCPs();
        } // update()

        private void _pointSDSNTToSDSOC()
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
                if( NTRow["nodetypeid"].ToString() == SDSId.ToString() )
                {
                    NTRow["objectclassid"] = SDSDocOC.ObjectClassId;
                }
            }
            NTUpdate.update( NTTable );
        }

        private void _deleteDocOCPs()
        {
            CswNbtMetaDataObjectClass DocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            CswNbtMetaDataObjectClassProp DocumentClassOCP = DocumentOC.getObjectClassProp( "Document Class" );
            if( null != DocumentClassOCP )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassPropDeprecated( DocumentClassOCP, true );
            }
            CswNbtMetaDataObjectClassProp LanguageOCP = DocumentOC.getObjectClassProp( "Language" );
            if( null != LanguageOCP )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassPropDeprecated( LanguageOCP, true );
            }
            CswNbtMetaDataObjectClassProp FormatOCP = DocumentOC.getObjectClassProp( "Format" );
            if( null != FormatOCP )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassPropDeprecated( FormatOCP, true );
            }
        }

    }//class CswUpdateSchema_02C_CaseXXXXX

}//namespace ChemSW.Nbt.Schema