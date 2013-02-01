using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28534
    /// </summary>
    public class CswUpdateSchema_01W_Case28534 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28534; }
        }

        public override void update()
        {
            // Set up label printing nodetypes

            CswNbtMetaDataObjectClass PrinterOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrinterClass );
            CswNbtMetaDataObjectClass JobOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrintJobClass );

            CswNbtMetaDataObjectClassProp JobCreatedDateOCP = JobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.CreatedDate );
            CswNbtMetaDataObjectClassProp JobJobNoOCP = JobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.JobNo );
            CswNbtMetaDataObjectClassProp JobJobStateOCP = JobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.JobState );
            CswNbtMetaDataObjectClassProp JobPrinterOCP = JobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.Printer );
            CswNbtMetaDataObjectClassProp JobRequestedByOCP = JobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.RequestedBy );
            CswNbtMetaDataObjectClassProp JobErrorOCP = JobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.ErrorInfo );
            CswNbtMetaDataObjectClassProp JobLabelOCP = JobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.Label );
            CswNbtMetaDataObjectClassProp JobEndedDateOCP = JobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.EndedDate );

            // Printer NodeType
            {
                CswNbtMetaDataNodeType PrinterNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( PrinterOC )
                    {
                        NodeTypeName = "Printer",
                        Category = "System"
                    } );
                PrinterNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassPrinter.PropertyName.Name ) );

                // Add a job grid
                CswNbtMetaDataNodeTypeTab JobGridTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( PrinterNT, "Jobs", 2 );
                CswNbtMetaDataNodeTypeProp JobGridNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp(
                    new CswNbtWcfMetaDataModel.NodeTypeProp( PrinterNT, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid ), "Jobs" ) );
                JobGridNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, JobGridTab.TabId );

                CswNbtView JobGridView = _CswNbtSchemaModTrnsctn.restoreView( JobGridNTP.ViewId );
                JobGridView.Root.ChildRelationships.Clear();

                CswNbtViewRelationship PrinterRel = JobGridView.AddViewRelationship( PrinterOC, true );
                CswNbtViewRelationship JobRel = JobGridView.AddViewRelationship( PrinterRel, NbtViewPropOwnerType.Second, JobPrinterOCP, true );

                CswNbtViewProperty JobNoVP = JobGridView.AddViewProperty( JobRel, JobJobNoOCP );
                CswNbtViewProperty RequestedByVP = JobGridView.AddViewProperty( JobRel, JobRequestedByOCP );
                CswNbtViewProperty CreatedDateVP = JobGridView.AddViewProperty( JobRel, JobCreatedDateOCP );
                CswNbtViewProperty JobStateVP = JobGridView.AddViewProperty( JobRel, JobJobStateOCP );
                CswNbtViewProperty JobLabelVP = JobGridView.AddViewProperty( JobRel, JobLabelOCP );

                JobNoVP.Order = 1;
                RequestedByVP.Order = 2;
                CreatedDateVP.Order = 3;
                JobStateVP.Order = 4;
                JobLabelVP.Order = 5;

                JobGridView.save();
            }


            // Print Job NodeType
            {
                CswNbtMetaDataNodeType JobNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( JobOC )
                    {
                        NodeTypeName = "Print Job",
                        Category = "System"
                    } );
                JobNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassPrintJob.PropertyName.JobNo ) );
                CswNbtMetaDataNodeTypeProp JobJobNoNTP = JobNT.getNodeTypePropByObjectClassProp( CswNbtObjClassPrintJob.PropertyName.JobNo );
                JobJobNoNTP.ReadOnly = true;
            }

            // Printers View
            {
                CswNbtView PrintersView = _CswNbtSchemaModTrnsctn.makeView();
                PrintersView.saveNew( "Printers", NbtViewVisibility.Global );
                PrintersView.Category = "System";
                PrintersView.ViewMode = NbtViewRenderingMode.Tree;
                PrintersView.AddViewRelationship( PrinterOC, true );
                PrintersView.save();
            }

            // Print Job Errors View
            {
                CswNbtView ErrorsView = _CswNbtSchemaModTrnsctn.makeView();
                ErrorsView.saveNew( "Print Job Errors", NbtViewVisibility.Global );
                ErrorsView.Category = "System";
                ErrorsView.ViewMode = NbtViewRenderingMode.Grid;

                CswNbtViewRelationship JobRel = ErrorsView.AddViewRelationship( JobOC, true );

                CswNbtViewProperty JobNoVP = ErrorsView.AddViewProperty( JobRel, JobJobNoOCP );
                CswNbtViewProperty RequestedByVP = ErrorsView.AddViewProperty( JobRel, JobRequestedByOCP );
                //CswNbtViewProperty CreatedDateVP = ErrorsView.AddViewProperty( JobRel, JobCreatedDateOCP );
                CswNbtViewProperty JobStateVP = ErrorsView.AddViewProperty( JobRel, JobJobStateOCP );
                CswNbtViewProperty JobLabelVP = ErrorsView.AddViewProperty( JobRel, JobLabelOCP );
                CswNbtViewProperty ErrorVP = ErrorsView.AddViewProperty( JobRel, JobErrorOCP );
                CswNbtViewProperty EndedDateVP = ErrorsView.AddViewProperty( JobRel, JobEndedDateOCP );

                JobNoVP.Order = 1;
                RequestedByVP.Order = 2;
                //CreatedDateVP.Order = 3;
                EndedDateVP.Order = 4;
                JobStateVP.Order = 5;
                JobLabelVP.Order = 6;
                ErrorVP.Order = 7;

                ErrorsView.AddViewPropertyFilter( JobStateVP,
                                                  Value: CswNbtObjClassPrintJob.StateOption.Error,
                                                  FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                ErrorsView.AddViewPropertyFilter( EndedDateVP,
                                                  Value: "today-7",
                                                  FilterMode: CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals );

                ErrorsView.save();
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28534

}//namespace ChemSW.Nbt.Schema