using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30914C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30914; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "C"; }
        }

        public override string Title
        {
            get { return "Configure Report Link properties"; }
        }

        public override void update()
        {
            CswPrimaryKey HMISReportNodeId = null;
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtSearch FindReport = _CswNbtSchemaModTrnsctn.CswNbtSearch;
            FindReport.addFilter( ReportOC, false );
            FindReport.SearchTerm = "HMIS Totals";
            ICswNbtTree SearchResults = FindReport.Results();
            if( SearchResults.getChildNodeCount() > 0 )
            {
                SearchResults.goToNthChild( 0 );
                HMISReportNodeId = SearchResults.getNodeIdForCurrentPosition();
            }

            if( CswTools.IsPrimaryKey( HMISReportNodeId ) )
            {
                CswNbtMetaDataObjectClass ControlZoneOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
                foreach( CswNbtMetaDataNodeType ControlZoneNT in ControlZoneOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp HMISReportNTP = ControlZoneNT.getNodeTypePropByObjectClassProp( CswNbtObjClassControlZone.PropertyName.HMISReport );
                    HMISReportNTP.SetFK( "nodeid", HMISReportNodeId.PrimaryKey );
                }
            }

        } // update()

    } // class CswUpdateMetaData_02H_Case30914

}//namespace ChemSW.Nbt.Schema