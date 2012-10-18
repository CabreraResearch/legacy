using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28016
    /// </summary>
    public class CswUpdateSchema_01S_Case28016 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
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