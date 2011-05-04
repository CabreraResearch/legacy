using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-18
    /// </summary>
    public class CswUpdateSchemaTo01H18 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 18 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H18( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // Case 20306
            // Replace .rpt file content for reports

            CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H18_Blob_Update", "jct_nodes_props" );
            JctUpdate.AllowBlobColumns = true;

            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            CswNbtView ReportView = ReportOC.CreateDefaultView();
            ICswNbtTree ReportTree = _CswNbtSchemaModTrnsctn.getTreeFromView( ReportView, false );
            for( Int32 i = 0; i < ReportTree.getChildNodeCount(); i++ )
            {
                ReportTree.goToNthChild( i );
                CswNbtNode ThisNode = ReportTree.getNodeForCurrentPosition();
                CswNbtObjClassReport ReportNode = CswNbtNodeCaster.AsReport( ThisNode );
                string ReportFileName = ReportNode.RPTFile.FileName;

                FileStream ReportFileStream = File.OpenRead( "..\\Reports\\" + ReportFileName );
                BinaryReader BReader = new BinaryReader( ReportFileStream, System.Text.Encoding.Default );
                byte[] ReportData = new byte[ReportFileStream.Length];
                BReader.Read( ReportData, 0, (Int32) ReportFileStream.Length );

                if( ReportNode.RPTFile.JctNodePropId > 0 )
                {
                    DataTable JctTable = JctUpdate.getTable( "jctnodepropid", ReportNode.RPTFile.JctNodePropId );
                    JctTable.Rows[0]["blobdata"] = ReportData;
                    JctUpdate.update( JctTable );
                }

                ReportTree.goToParentNode();
            } // for(Int32 i = 0; i < ReportTree.getChildNodeCount; i++)

        } // update()

    }//class CswUpdateSchemaTo01H18

}//namespace ChemSW.Nbt.Schema


