
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;



namespace ChemSW.Nbt.Schema
{

    /// <summary>
    /// Schema Update for case 26609
    /// </summary>
    public class CswUpdateSchemaCase26609ReportUserName : CswUpdateSchemaTo
    {
        public override void update()
        {



            CswNbtMetaDataObjectClass RptOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp ReportUserNameProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RptOC,
                                                 new CswNbtWcfMetaDataModel.ObjectClassProp
                                                 {
                                                     PropName = CswNbtObjClassReport.ReportUserNamePropertyName.ToString(),
                                                     FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                                                     SetValOnAdd = false,
                                                     IsRequired = false,
                                                 }
                      );

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();




        }//Update()

    }//class CswUpdateSchemaCase26609ReportUserName

}//namespace ChemSW.Nbt.Schema