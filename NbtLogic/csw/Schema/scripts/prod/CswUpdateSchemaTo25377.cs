using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to case 25377
    /// </summary>
    public class CswUpdateSchemaTo25377 : CswUpdateSchemaTo
    {
        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 12 ); } }
        //public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            //remove inspectiontargetclass.lastinspectiondate property
            CswNbtMetaDataObjectClassProp aprop = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass ).ObjectClassId, "Last Inspection Date" );
            if( null != aprop )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( aprop, true );
            }

            //add props to InspectionDesignClass
            CswNbtMetaDataObjectClassProp idateProp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, "Inspection Date", CswNbtMetaDataFieldType.NbtFieldType.DateTime,false,true,false,"",Int32.MinValue,false,false,false,true);
            
            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp isprProp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, "Inspector", CswNbtMetaDataFieldType.NbtFieldType.Relationship,true,false,true,NbtViewRelatedIdType.ObjectClassId.ToString(),userOC.ObjectClassId );
            

        }//Update()

    }//class CswUpdateSchemaTo25377

}//namespace ChemSW.Nbt.Schema