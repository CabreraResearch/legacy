using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateSchema_02K_Case31847: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31847; }
        }

        public override string Title
        {
            get { return "Rename Kiosk Mode report to Kiosk Mode Barcodes"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtSearch FindReport = _CswNbtSchemaModTrnsctn.CswNbtSearch;
            FindReport.addFilter( ReportOC, false );
            FindReport.SearchType = CswEnumSqlLikeMode.Exact;
            FindReport.SearchTerm = "Kiosk Mode";
            ICswNbtTree SearchResults = FindReport.Results();
            for( Int32 r = 0; r < SearchResults.getChildNodeCount(); r++ )
            {
                SearchResults.goToNthChild( r );
                CswNbtObjClassReport ReportNode = SearchResults.getNodeForCurrentPosition();
                ReportNode.ReportName.Text = "Kiosk Mode Barcodes";
                ReportNode.postChanges( false );
                SearchResults.goToParentNode();
            }
        } // update()

    }//class CswUpdateMetaData_02K_Case31847

}//namespace ChemSW.Nbt.Schema