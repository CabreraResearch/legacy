using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;



namespace ChemSW.Nbt.Schema
{

    /// <summary>
    /// Schema Update for case 26609
    /// </summary>
    public class CswUpdateSchemaCase26609 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //_CswNbtSchemaModTrnsctn.addObjectClassPropRow
            CswCommaDelimitedString FormatOptionString = new CswCommaDelimitedString();
            FormatOptionString.Add( MailRptFormatOptions.Link.ToString() );
            FormatOptionString.Add( MailRptFormatOptions.CSV.ToString() );


            CswNbtMetaDataObjectClass MailRptOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp MailRptFormatProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( MailRptOC,
                                                 new CswNbtWcfMetaDataModel.ObjectClassProp
                                                 {
                                                     PropName = CswNbtObjClassMailReport.OutputFormatPropertyName.ToString(),
                                                     FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                                                     ListOptions = FormatOptionString.ToString(),
                                                     SetValOnAdd = false,
                                                     IsRequired = false
                                                 }
                      );

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( MailRptFormatProp, CswNbtSubField.SubFieldName.Value, MailRptFormatOptions.Link.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

        }//Update()

    }//class CswUpdateSchemaCase26609

}//namespace ChemSW.Nbt.Schema