using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case29320_PrintJobs: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29320; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass PrinterOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrinterClass );
            foreach( CswNbtMetaDataNodeType JobNt in PrinterOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp GridNtp = JobNt.getNodeTypePropByFirstVersionName( "Jobs" );
                if( null != GridNtp && CswEnumNbtFieldType.Grid == GridNtp.getFieldType().FieldType )
                {
                    CswNbtView GridView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( GridNtp.ViewId );
                    if( null != GridView )
                    {
                        GridView.Root.ChildRelationships.Clear();

                        CswNbtViewRelationship RootVr = GridView.AddViewRelationship( PrinterOc, IncludeDefaultFilters : true );

                        CswNbtMetaDataObjectClass PrintJobOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintJobClass );
                        CswNbtMetaDataObjectClassProp PrinterOcp = PrintJobOc.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.Printer );
                        CswNbtViewRelationship PrinterVr = GridView.AddViewRelationship( RootVr, CswEnumNbtViewPropOwnerType.Second, PrinterOcp, IncludeDefaultFilters: true );

                        GridView.AddViewProperty( PrinterVr, PrintJobOc.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.JobNo ) );
                        GridView.AddViewProperty( PrinterVr, PrintJobOc.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.RequestedBy ) );
                        GridView.AddViewProperty( PrinterVr, PrintJobOc.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.CreatedDate ) );
                        GridView.AddViewProperty( PrinterVr, PrintJobOc.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.JobState ) );
                        GridView.AddViewProperty( PrinterVr, PrintJobOc.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.Label ) );

                        GridView.save();
                    }
                }
            }

        } //Update()

    }//class CswUpdateSchema_01Y_CaseXXXXX

}//namespace ChemSW.Nbt.Schema