using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02G_Case30813: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30813; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Remove old LegacyID prop from all OCs"; }
        }

        public override void update()
        {
            CswNbtMetaDataFieldType TextFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Text );

            //update the FT of existing Legacy Id OC props
            CswTableUpdate legacyIdOCPTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "LegacyIdTableUpdate_OCP", "object_class_props" );
            DataTable ocpTbl = legacyIdOCPTU.getTable( "where propname = '" + CswNbtObjClass.PropertyName.LegacyId + "'" );
            foreach( DataRow row in ocpTbl.Rows )
            {
                row["fieldtypeid"] = TextFT.FieldTypeId;
            }
            legacyIdOCPTU.update( ocpTbl );

            CswTableUpdate legacyIdNTPTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "LegacyIdTableUpdate_NTP", "nodetype_props" );
            DataTable ntpTbl = legacyIdNTPTU.getTable( "where propname = '" + CswNbtObjClass.PropertyName.LegacyId + "'" );
            foreach( DataRow row in ntpTbl.Rows )
            {
                row["fieldtypeid"] = TextFT.FieldTypeId;
            }
            legacyIdNTPTU.update( ntpTbl );

            //Create new Legacy Id props if needed
            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtSchemaModTrnsctn.MetaData.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp LegacyIdOCP = ObjectClass.getObjectClassProp( CswNbtObjClass.PropertyName.LegacyId );
                if( null == LegacyIdOCP )
                {
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ObjectClass )
                        {
                            FieldType = CswEnumNbtFieldType.Text,
                            PropName = CswNbtObjClass.PropertyName.LegacyId
                        } );
                }
                
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema