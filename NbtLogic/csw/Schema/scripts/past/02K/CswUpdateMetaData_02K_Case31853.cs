using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case31853: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31853; }
        }

        public override string Title
        {
            get { return "Change HMIS report Name parameter to Control_Zone"; }
        }

        public override void update()
        {
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
                ReportNode.WebService.Text = ReportNode.WebService.Text.Replace( "{Name}", "{Control_Zone}" );
                ReportNode.postChanges( false );
                SearchResults.goToParentNode();
            }
        } // update()

    }//class CswUpdateMetaData_02K_Case31853

}//namespace ChemSW.Nbt.Schema