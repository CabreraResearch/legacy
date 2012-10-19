using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27742
    /// </summary>
    public class CswUpdateSchema_01S_Case27742 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Set MailReport.OutputFormat to be conditional on Type = Report

            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp TypeOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.Type );
            CswNbtMetaDataObjectClassProp OutputFormatOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.OutputFormat );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OutputFormatOCP,
                                                                    CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filterpropid,
                                                                    TypeOCP.PropId.ToString() );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OutputFormatOCP,
                                                                    CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filter,
                                                                    CswNbtMetaDataObjectClassProp.makeFilter( TypeOCP.getFieldTypeRule().SubFields.Default,
                                                                                                              CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                                                                              CswNbtObjClassMailReport.TypeOptionReport ) );

            // because of case 27922, need to apply this to nodetypes manually
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp TypeNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Type );
                CswNbtMetaDataNodeTypeProp OutputFormatNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.OutputFormat );

                OutputFormatNTP.setFilter( TypeNTP,
                                           TypeNTP.getFieldTypeRule().SubFields.Default,
                                           CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                           CswNbtObjClassMailReport.TypeOptionReport );

                // revert readonly
                foreach( CswNbtObjClassMailReport MailReportNode in MailReportNT.getNodes( false, true ) )
                {
                    MailReportNode.OutputFormat.setReadOnly( value: false, SaveToDb: true );
                }

            } // foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes() )

        }//Update()

    }//class CswUpdateSchemaCase27742

}//namespace ChemSW.Nbt.Schema