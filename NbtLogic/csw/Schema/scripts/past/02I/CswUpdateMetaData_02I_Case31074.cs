using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
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

            _CswNbtSchemaModTrnsctn.createObjectClassProp( GHSClassOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    FieldType = CswEnumNbtFieldType.List,
                    PropName = CswNbtObjClassGHSClassification.PropertyName.Category,
                    ListOptions = "Physical,Health,Environmental"
                } );

            // Language properties
            foreach( string Language in CswNbtPropertySetPhrase.SupportedLanguages.All )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GHSClassOC, new CswNbtWcfMetaDataModel.ObjectClassProp( GHSClassOC )
                {
                    PropName = Language,
                    FieldType = CswEnumNbtFieldType.Text
                } );
            }

            // Add new object class to 'Phrase' Property Set
            CswNbtMetaDataPropertySet PhrasePS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.PhraseSet );
            CswTableUpdate jctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "31074_jctPropSets_update", "jct_propertyset_objectclass" );
            DataTable jctTable = jctUpdate.getEmptyTable();
            DataRow row = jctTable.NewRow();
            row["propertysetid"] = PhrasePS.PropertySetId;
            row["objectclassid"] = GHSClassOC.ObjectClassId;
            jctTable.Rows.Add( row );
            jctUpdate.update( jctTable );


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
                PropName = CswNbtObjClassGHS.PropertyName.ClassificationsGrid,
                Extended = CswEnumNbtGridPropMode.Small.ToString(),
                NumberMaxValue = 10
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
            {
                FieldType = CswEnumNbtFieldType.MultiList,
                PropName = CswNbtObjClassGHS.PropertyName.Classifications
            } );


        } // update()
    }

}//namespace ChemSW.Nbt.Schema