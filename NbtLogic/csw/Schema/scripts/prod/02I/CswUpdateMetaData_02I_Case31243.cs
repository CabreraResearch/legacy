using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    class CswUpdateMetaData_02I_Case31243 : CswUpdateSchemaTo
    {

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31243; }
        }

        public override string Title
        {
            get { return "Add Country prop to SDS Document"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            // Create Country property on SDSDocumentOC
            CswNbtMetaDataObjectClass SDSDocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
            CswNbtMetaDataObjectClassProp CountryOCP = SDSDocumentOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.ChemWatch );
            if( null == CountryOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( SDSDocumentOC, new CswNbtWcfMetaDataModel.ObjectClassProp( SDSDocumentOC )
                    {
                        PropName = CswNbtObjClassSDSDocument.PropertyName.Country,
                        FieldType = CswEnumNbtFieldType.List
                    } );
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema