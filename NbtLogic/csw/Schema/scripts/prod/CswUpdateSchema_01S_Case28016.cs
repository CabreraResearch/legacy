using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28016
    /// </summary>
    public class CswUpdateSchema_01S_Case28016 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28016; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp NextDueDateOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.NextDueDate );
            CswNbtMetaDataObjectClassProp LastProcessedOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.LastProcessed );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NextDueDateOCP,
                                                                    CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended,
                                                                    CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LastProcessedOCP,
                                                                    CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended,
                                                                    CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() );

        }//Update()

    }//class CswUpdateSchema_01S_Case28016

}//namespace ChemSW.Nbt.Schema