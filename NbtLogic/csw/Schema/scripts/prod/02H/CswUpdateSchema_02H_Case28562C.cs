using System.IO;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case28562C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 28562; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "C"; }
        }

        public override string Title
        {
            get { return "Add new HMIS crystal reports"; }
        }

        public override void update()
        {
            // Create CISPro Report Group
            CswNbtMetaDataObjectClass ReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupClass );
            if( null != ReportGroupOC.FirstNodeType )
            {
                CswNbtObjClassReportGroup CISProGroup = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportGroupOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassReportGroup NewReportGroup = NewNode;
                        NewReportGroup.Name.Text = "CISPro Report Group";
                    } );


                CswNbtMetaDataNodeType ReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );
                if( null != ReportNT )
                {
                    // Add HMIS reports
                    _createReport( "HMIS Totals", "Materials", CISProGroup, "", "RegulatoryReporting/getHMISDataTable?ControlZone={ControlZone}", "HMISTotals.rpt" );
                    _createReport( "HMIS Materials", "Materials", CISProGroup, "", "RegulatoryReporting/getHMISDataTable?ControlZone={ControlZone}&Class={Class}", "HMISMaterials.rpt" );
                }
            }

        } // update()

        private void _createReport( string ReportName, string Category, CswNbtObjClassReportGroup Group, string Query, string WebService, string Filename )
        {
            CswNbtMetaDataNodeType ReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );

            if( null != ReportNT )
            {
                CswNbtObjClassReport ReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId(
                    ReportNT.NodeTypeId,
                    OnAfterMakeNode: delegate( CswNbtNode NewNode )
                        {
                            CswNbtObjClassReport NewReport = NewNode;
                            NewReport.ReportName.Text = ReportName;
                            NewReport.Category.Text = Category;
                            NewReport.ReportGroup.RelatedNodeId = Group.NodeId;
                            NewReport.SQL.Text = Query;
                            NewReport.WebService.Text = WebService;
                        } );

                _uploadBlobData( ReportNode, Filename );

            }
        }


        private void _uploadBlobData( CswNbtObjClassReport ReportNode, string Filename )
        {
            CswPropIdAttr PropId = new CswPropIdAttr( ReportNode.Node, ReportNode.RPTFile.NodeTypeProp );
            string Filepath = CswFilePath.getConfigurationFilePath( CswEnumSetupMode.NbtExe ) + "\\" + Filename;
            byte[] ReportFile = File.ReadAllBytes( Filepath );
            const string ContentType = "application/octet-stream";
            string Href;

            CswNbtSdBlobData SdBlobData = _CswNbtSchemaModTrnsctn.CswNbtSdBlobData;
            SdBlobData.saveFile( PropId.ToString(), ReportFile, ContentType, Filename, out Href );

        }

    }

}//namespace ChemSW.Nbt.Schema