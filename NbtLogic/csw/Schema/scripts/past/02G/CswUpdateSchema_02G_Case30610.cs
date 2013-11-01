using System.IO;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30610 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30610; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30610"; }
        }

        public override string Title
        {
            get { return "Add New Reports to Master"; }
        }

        public override void update()
        {

            CswNbtObjClassReportGroup SystemReportGroup = _createReportGroup("System Reports");

            if( null != SystemReportGroup )
            {
                string SqlQuery = @"
select * from (
    select barcode ucode,username,firstname,lastname,firstname || ' ' || lastname as fullname from user1
) x
where lower(fullname) like lower('%{FullNameContains}%')
order by lower(fullname)
";

                _createReport( "User Codes", "System Reports", SystemReportGroup, SqlQuery, "users.rpt" );




                SqlQuery = @"
select lscode,location,name,Location || ' > ' || name pathname, type from (
    select barcode lscode,name,location,'building' type from building
    union
    select barcode lscode,name,location,'room' type from room
    union
    select barcode lscode,name,location,'cabinet' type from cabinet
) x where name is not null and location is not null
-- and lower(location) like lower('{LocationBegins}%')
order by lower(location),lower(name)
";

                _createReport( "Location Codes", "System Reports", SystemReportGroup, SqlQuery, "locations.rpt" );




                SqlQuery = @"
select * from (
    select 'DISPOSE' as val from dual
    union 
    select 'MOVE' as val from dual
    union
    select 'TRANSFER' from dual
    union
    select 'OWNER' from dual
    union
    select 'DISPENSE' from dual
) x order by val
";

                _createReport( "Kiosk Mode", "System Reports", SystemReportGroup, SqlQuery, "kiosk.rpt" );

            }
        } // update()


        private CswNbtObjClassReportGroup _createReportGroup( string Name )
        {
            CswNbtObjClassReportGroup SystemReportGroup = null;


            CswNbtMetaDataObjectClass ReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupClass );
            if( null != ReportGroupOC )
            {
                CswNbtMetaDataNodeType ReportGroupNT = ReportGroupOC.FirstNodeType;

                if( null != ReportGroupNT )
                {
                   SystemReportGroup = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportGroupNT.NodeTypeId, 
                       OnAfterMakeNode: ( CswNbtNode ) =>
                           {
                               CswNbtObjClassReportGroup NewNode = CswNbtNode;
                               NewNode.Name.Text = Name;
                           });


                }
            }


            return SystemReportGroup;
        }



        private void _createReport(string ReportName, string Category, CswNbtObjClassReportGroup Group, string Query, string Filename)
        {
            CswNbtMetaDataNodeType ReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );

            if( null != ReportNT )
            {
                CswNbtObjClassReport ReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( 
                    ReportNT.NodeTypeId,
                    OnAfterMakeNode: ( CswNbtNode ) =>
                        {
                            CswNbtObjClassReport NewNode = CswNbtNode;
                            NewNode.ReportName.Text = ReportName;
                            NewNode.Category.Text = Category;
                            NewNode.ReportGroup.RelatedNodeId = Group.NodeId;
                            NewNode.SQL.Text = Query;
                        }
                    );

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


    }//class CswUpdateSchema_02F_Case30610

}//namespace ChemSW.Nbt.Schema
