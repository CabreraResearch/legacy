using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02L_Case52821 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52821; }
        }

        public override string Title
        {
            get { return "Fix control zone report link behavior"; }
        }

        // See CIS-52705 and schema script CswUpdateMetaData02K_Case31853
        public override void update()
        {
            // Change Control Zone "Name" property to "Control Zone Name"
            CswNbtMetaDataObjectClass ControlZoneOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            CswNbtMetaDataObjectClassProp CzNameOCP = ControlZoneOC.getObjectClassProp( "Name" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CzNameOCP, CswEnumNbtObjectClassPropAttributes.propname, "Control Zone Name" );


            // Update parameter name in HMIS reports to "Control Zone Name"
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtSearch FindReport = _CswNbtSchemaModTrnsctn.CswNbtSearch;
            FindReport.addFilter( ReportOC, false );
            FindReport.SearchType = CswEnumSqlLikeMode.Begins;
            FindReport.SearchTerm = "HMIS";
            ICswNbtTree SearchResults = FindReport.Results();
            for( Int32 r = 0; r < SearchResults.getChildNodeCount(); r++ )
            {
                SearchResults.goToNthChild( r );
                CswNbtObjClassReport ReportNode = SearchResults.getNodeForCurrentPosition();
                ReportNode.WebService.Text = ReportNode.WebService.Text.Replace( "{Control_Zone}", "{Control Zone Name}" );
                ReportNode.postChanges( false );
                SearchResults.goToParentNode();
            }
        } // update()

    }//class CswUpdateMetaData_02L_Case52821

}//namespace ChemSW.Nbt.Schema