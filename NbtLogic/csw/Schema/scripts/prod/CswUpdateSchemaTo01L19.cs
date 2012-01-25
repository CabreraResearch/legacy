using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-19
    /// </summary>
    public class CswUpdateSchemaTo01L19 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 19 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            #region Case 24827

            #region Case 24827

            CswNbtMetaDataObjectClass MailReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            foreach( CswNbtNode MailReportNode in MailReportOc.getNodes( true, false ) )
            {
                CswNbtObjClassMailReport NodeAsMailReport = CswNbtNodeCaster.AsMailReport( MailReportNode );
                if( CswConvert.ToInt32( NodeAsMailReport.WarningDays.Value ) < 0 )
                {
                    NodeAsMailReport.WarningDays.Value = 0;
                    MailReportNode.postChanges( true );
                }
            }

            CswNbtMetaDataObjectClassProp WarningDaysMrOcp = MailReportOc.getObjectClassProp( CswNbtObjClassMailReport.WarningDaysPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( WarningDaysMrOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( WarningDaysMrOcp, WarningDaysMrOcp.FieldTypeRule.SubFields.Default.Name, 5 );

            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            foreach( CswNbtNode GeneratorNode in GeneratorOc.getNodes( true, false ) )
            {
                CswNbtObjClassGenerator NodeAsGenerator = CswNbtNodeCaster.AsGenerator( GeneratorNode );
                if( CswConvert.ToInt32( NodeAsGenerator.WarningDays.Value ) < 0 )
                {
                    NodeAsGenerator.WarningDays.Value = 0;
                    GeneratorNode.postChanges( true );
                }
            }

            CswNbtMetaDataObjectClassProp WarningDaysGnOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.WarningDaysPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( WarningDaysGnOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( WarningDaysGnOcp, WarningDaysGnOcp.FieldTypeRule.SubFields.Default.Name, 5 );

            #endregion Case 24827

            #endregion Case 24827


        }//Update()

    }//class CswUpdateSchemaTo01L19

}//namespace ChemSW.Nbt.Schema


