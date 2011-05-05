using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-29
    /// </summary>
    public class CswUpdateSchemaTo01H29 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 29 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H29( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {

            // case 20971
            CswNbtView AllProblems = _CswNbtSchemaModTrnsctn.restoreView( "All Problems" );
            if( null == AllProblems )
            {
				AllProblems = _CswNbtSchemaModTrnsctn.makeView();
				AllProblems.makeNew( "All Problems", NbtViewVisibility.Global, null, null, null );
            }

            AllProblems.Root.ChildRelationships.Clear();

            CswNbtMetaDataObjectClass ProblemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass );
            if( 0 < ProblemOC.NodeTypes.Count )
            {
                foreach( CswNbtMetaDataNodeType Problem in ProblemOC.NodeTypes )
                {
                    CswNbtViewRelationship ProblemRelationship = AllProblems.AddViewRelationship( Problem, false );

                    CswNbtMetaDataNodeTypeProp ReportedByNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ReportedByPropertyName );
                    CswNbtViewProperty ReportedByVp = AllProblems.AddViewProperty( ProblemRelationship, ReportedByNtp );
                    ReportedByVp.Order = 1;

                    CswNbtMetaDataNodeTypeProp ClosedNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ClosedPropertyName );
                    CswNbtViewProperty ClosedVp = AllProblems.AddViewProperty( ProblemRelationship, ClosedNtp );
                    ClosedVp.Order = 2;
                    AllProblems.AddViewPropertyFilter( ClosedVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );

                    CswNbtMetaDataNodeTypeProp DateOpenedNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.DateOpenedPropertyName );
                    CswNbtViewProperty DateOpenedVp = AllProblems.AddViewProperty( ProblemRelationship, DateOpenedNtp );
                    DateOpenedVp.Order = 3;

                    CswNbtMetaDataNodeTypeProp EquipmentNtp = Problem.getNodeTypeProp( "Equipment" );
                    if( null != EquipmentNtp )
                    {
                        CswNbtViewProperty EquipmentVp = AllProblems.AddViewProperty( ProblemRelationship, EquipmentNtp );
                        EquipmentVp.Order = 4;
                    }

                    CswNbtMetaDataNodeTypeProp AssemblyNtp = Problem.getNodeTypeProp( "Assembly" );
                    if( null != AssemblyNtp )
                    {
                        CswNbtViewProperty AssemblyVp = AllProblems.AddViewProperty( ProblemRelationship, AssemblyNtp );
                        AssemblyVp.Order = 5;
                    }

                    CswNbtMetaDataNodeTypeProp LocationNtp = Problem.getNodeTypeProp( "Location" );
                    if( null != LocationNtp )
                    {
                        CswNbtViewProperty LocationVp = AllProblems.AddViewProperty( ProblemRelationship, LocationNtp );
                        LocationVp.Order = 6;
                    }

                    CswNbtMetaDataNodeTypeProp SummaryNtp = Problem.getNodeTypeProp( "Summary" );
                    if( null != SummaryNtp )
                    {
                        CswNbtViewProperty SummaryVp = AllProblems.AddViewProperty( ProblemRelationship, SummaryNtp );
                        SummaryVp.Order = 7;
                    }

                    CswNbtMetaDataNodeTypeProp TechnicianNtp = Problem.getNodeTypeProp( "Technician" );
                    if( null != TechnicianNtp )
                    {
                        CswNbtViewProperty TechnicianVp = AllProblems.AddViewProperty( ProblemRelationship, TechnicianNtp );
                        TechnicianVp.Order = 8;
                    }

                }
                AllProblems.save();
            }
        } // update()

    }//class CswUpdateSchemaTo01H29

}//namespace ChemSW.Nbt.Schema

