using System;
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
            CswPrimaryKey HMISTotalsReportId = null;
            CswPrimaryKey HMISMaterialsReportId = null;

            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );

            CswNbtSearch FindReport = _CswNbtSchemaModTrnsctn.CswNbtSearch;
            FindReport.addFilter( ReportOC, false );
            FindReport.SearchType = CswEnumSqlLikeMode.Begins;
            FindReport.SearchTerm = "HMIS";
            ICswNbtTree SearchResults = FindReport.Results();
            for( Int32 r = 0; r < SearchResults.getChildNodeCount(); r++ )
            {
                SearchResults.goToNthChild( r );
                string ReportName = SearchResults.getNodeNameForCurrentPosition();
                if( ReportName == "HMIS Totals" )
                {
                    HMISTotalsReportId = SearchResults.getNodeIdForCurrentPosition();
                }
                if( ReportName == "HMIS Materials" )
                {
                    HMISMaterialsReportId = SearchResults.getNodeIdForCurrentPosition();
                }
                SearchResults.goToParentNode();
            } // for( Int32 r = 0; r < SearchResults.getChildNodeCount(); r++ )

            CswNbtMetaDataObjectClass ControlZoneOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            foreach( CswNbtMetaDataNodeType ControlZoneNT in ControlZoneOC.getNodeTypes() )
            {
                if( CswTools.IsPrimaryKey( HMISTotalsReportId ) )
                {
                    CswNbtMetaDataNodeTypeProp HMISTotalsNTP = ControlZoneNT.getNodeTypePropByObjectClassProp( CswNbtObjClassControlZone.PropertyName.HMISTotals );
                    HMISTotalsNTP.SetFK( "nodeid", HMISTotalsReportId.PrimaryKey );
                    HMISTotalsNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                }
                if( CswTools.IsPrimaryKey( HMISMaterialsReportId ) )
                {
                    CswNbtMetaDataNodeTypeProp HMISMaterialsNTP = ControlZoneNT.getNodeTypePropByObjectClassProp( CswNbtObjClassControlZone.PropertyName.HMISMaterials );
                    HMISMaterialsNTP.SetFK( "nodeid", HMISMaterialsReportId.PrimaryKey );
                    HMISMaterialsNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                }
            } // foreach( CswNbtMetaDataNodeType ControlZoneNT in ControlZoneOC.getNodeTypes() )

        } // update()

    } // class CswUpdateMetaData_02H_Case30914

}//namespace ChemSW.Nbt.Schema