using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case30533B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30533; }
        }

        public override string AppendToScriptName()
        {
            return "BCD";
        }

        public override string Title
        {
            get { return "Delete old Request Item ObjectClasses and PropertySet"; }
        }

        public override void update()
        {
            CswNbtMetaDataPropertySet RequestItemPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( "RequestItemSet" );
            if( null != RequestItemPS )
            {
                CswTableUpdate PropSetOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "propSetRequestItemProps", "jct_propertyset_ocprop" );
                DataTable PropSetOCPTable = PropSetOCPUpdate.getTable( "where propertysetid = " + RequestItemPS.PropertySetId );
                foreach( DataRow Row in PropSetOCPTable.Rows )
                {
                    Row.Delete();
                }
                PropSetOCPUpdate.update( PropSetOCPTable );

                CswTableUpdate PropSetOCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "propSetRequestItemProps", "jct_propertyset_objectclass" );
                DataTable PropSetOCTable = PropSetOCUpdate.getTable( "where propertysetid = " + RequestItemPS.PropertySetId );
                foreach( DataRow Row in PropSetOCTable.Rows )
                {
                    Row.Delete();
                }
                PropSetOCUpdate.update( PropSetOCTable );

                CswTableUpdate PropSetUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "propSetRequestItemProps", "property_set" );
                DataTable PropSetTable = PropSetUpdate.getTable( "where propertysetid = " + RequestItemPS.PropertySetId );
                foreach( DataRow Row in PropSetTable.Rows )
                {
                    Row.Delete();
                }
                PropSetUpdate.update( PropSetTable );
            }
            CswNbtMetaDataObjectClass RequestMaterialCreateOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "RequestMaterialCreateClass" );
            if( null != RequestMaterialCreateOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( RequestMaterialCreateOC );
            }
            CswNbtMetaDataObjectClass RequestMaterialDispenseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "RequestMaterialDispenseClass" );
            if( null != RequestMaterialDispenseOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( RequestMaterialDispenseOC );
            }
            CswNbtMetaDataObjectClass RequestContainerDispenseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "RequestContainerDispenseClass" );
            if( null != RequestContainerDispenseOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( RequestContainerDispenseOC );
            }
            CswNbtMetaDataObjectClass RequestContainerUpdateOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "RequestContainerUpdateClass" );
            if( null != RequestContainerUpdateOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( RequestContainerUpdateOC );
            }
        } // update()
    }
}//namespace ChemSW.Nbt.Schema