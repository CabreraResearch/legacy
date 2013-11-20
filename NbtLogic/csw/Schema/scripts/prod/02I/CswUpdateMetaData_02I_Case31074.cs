using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31074 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31074; }
        }

        public override string Title
        {
            get { return "Fix GHS Classifications: Object classes"; }
        }

        public override void update()
        {
            // Add new 'GHS Classification' object class
            CswNbtMetaDataObjectClass GHSClassOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.GHSClassificationClass, "warning.png", false );

            // TODO: Add object class to property set 'Phrase'

            _CswNbtSchemaModTrnsctn.createObjectClassProp( GHSClassOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    FieldType = CswEnumNbtFieldType.List,
                    PropName = CswNbtObjClassGHSClassification.PropertyName.Category,
                    ListOptions = "Physical,Health,Environmental"
                } );

            CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );

            // Delete old GHS properties
            string[] doomedPropNames = new string[]
                {
                    "Classification",
                    "Class Codes Grid",
                    "Class Codes",
                    "Add Class Codes"
                };
            foreach( string doomedPropName in doomedPropNames )
            {
                CswNbtMetaDataObjectClassProp doomedOCP = GhsOC.getObjectClassProp( doomedPropName );
                if( null != doomedOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( doomedOCP, true );
                }
            }

            // Add new GHS properties
            _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                FieldType = CswEnumNbtFieldType.Grid,
                PropName = CswNbtObjClassGHS.PropertyName.ClassificationsGrid
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                FieldType = CswEnumNbtFieldType.MultiList,
                PropName = CswNbtObjClassGHS.PropertyName.Classifications
            } );


        } // update()
    }

}//namespace ChemSW.Nbt.Schema