using System.Linq;
using ChemSW.Nbt.csw.Dev;
using System.IO;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31171 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31171; }
        }

        public override string Title
        {
            get { return "Custom Text Barcode report"; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );
            if( null != ReportNT )
            {

                CswNbtObjClassReport ReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportNT.NodeTypeId, OnAfterMakeNode: delegate( CswNbtNode newNode )
                    {
                        CswNbtObjClassReport report = newNode;
                        report.ReportName.Text = "Custom Text Barcode";
                        report.Category.Text = "System Reports";
                        report.SQL.Text = "select UPPER('{text}') as text from dual";

                        CswNbtMetaDataObjectClass ReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupClass );
                        CswNbtObjClassReportGroup SystemReportGroup = ReportGroupOC.getNodes( forceReInit: false, includeSystemNodes: true )
                                                                                   .FirstOrDefault( ReportGroup => ( (CswNbtObjClassReportGroup) ReportGroup ).Name.Text == "System Reports" );
                        if( null != SystemReportGroup )
                        {
                            report.ReportGroup.RelatedNodeId = SystemReportGroup.NodeId;
                        }
                    } );

                // upload .RPT
                string Filename = "customtextbarcode.rpt";
                CswPropIdAttr PropId = new CswPropIdAttr( ReportNode.Node, ReportNode.RPTFile.NodeTypeProp );
                string Filepath = CswFilePath.getConfigurationFilePath( CswEnumSetupMode.NbtExe ) + "\\" + Filename;
                byte[] ReportFile = File.ReadAllBytes( Filepath );

                const string ContentType = "application/octet-stream";
                string Href;
                CswNbtSdBlobData SdBlobData = _CswNbtSchemaModTrnsctn.CswNbtSdBlobData;
                SdBlobData.saveFile( PropId.ToString(), ReportFile, ContentType, Filename, out Href );

            } // if( null != ReportNT )
        } // update()


    }//class CswUpdateSchema_02I_Case31171

}//namespace ChemSW.Nbt.Schema

