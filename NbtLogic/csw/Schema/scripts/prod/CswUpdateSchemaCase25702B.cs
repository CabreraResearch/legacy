using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25702B
    /// </summary>
    public class CswUpdateSchemaCase25702B : CswUpdateSchemaTo
    {
        public override void update()
        {
            //We need two scripts because the _CswNbtSchemaModTrnsctn.MetaData was trying to create the new ObjectClassProp before the existing OCP was renamed
            string OldPropName = "Run Status_OLD";

            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            //Create new RunStatus OCP    
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass,
                CswNbtObjClassGenerator.RunStatusPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Comments, 
                ServerManaged: true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass,
                CswNbtObjClassMailReport.RunStatusPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Comments,
                ServerManaged: true );

            //migrate data
            IEnumerable<CswNbtNode> GeneratorNodes = GeneratorOC.getNodes( false, true );
            foreach( CswNbtNode Node in GeneratorNodes )
            {
                string message = Node.Properties[OldPropName].AsStatic.StaticText;
                if( false == string.IsNullOrEmpty( message ) )
                {
                    Node.Properties[CswNbtObjClassGenerator.RunStatusPropertyName].AsComments.AddComment( message );
                    Node.postChanges( true );
                }
            }
            IEnumerable<CswNbtNode> MailReportNodes = MailReportOC.getNodes( false, true );
            foreach( CswNbtNode Node in MailReportNodes )
            {
                string message = Node.Properties[OldPropName].AsStatic.StaticText;
                if( false == string.IsNullOrEmpty( message ) )
                {
                    Node.Properties[CswNbtObjClassMailReport.RunStatusPropertyName].AsComments.AddComment( message );
                    Node.postChanges( true );
                }
            }

            //delete runStatus_old
            CswNbtMetaDataObjectClassProp GeneratorRunStatusOCP = GeneratorOC.getObjectClassProp( OldPropName );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( GeneratorRunStatusOCP, true );
            CswNbtMetaDataObjectClassProp MailReportRunStatusOCP = MailReportOC.getObjectClassProp( OldPropName );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( MailReportRunStatusOCP, true );
        }//Update()

    }//class CswUpdateSchemaCase25702B

}//namespace ChemSW.Nbt.Schema