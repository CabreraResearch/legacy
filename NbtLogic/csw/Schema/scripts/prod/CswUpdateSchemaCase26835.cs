using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26835
    /// </summary>
    public class CswUpdateSchemaCase26835 : CswUpdateSchemaTo
    {
        public override void update()
        {

            /*
             * I realize that most of the time you do not want to go directly to the table, but in this specific case I was %100 guarenteed that there were
             * no material nodes running wild and that I would be able to change the field type directly in the table without fear of having nodes with 
             * data filled in the Scientific property. If there were, that would mean this would result in nodes having Scientific prop type data in a 
             * Number prop type field...that's bad and would have dire consequences.
             * 
             * tldr; DON'T USE THIS AS A TEMPLATE TO SOLVE SIMILIAR PROBLEMS!
             */

            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp specificGravityOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.SpecificGravityPropertyName );

            CswNbtMetaDataFieldType numberFieldType = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Number );

            if( null != numberFieldType )
            {
                CswTableUpdate tableOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "OCP_SpecGravity_26835", "object_class_props" );
                DataTable ocpTable = tableOCPUpdate.getTable( "objectclasspropid", specificGravityOCP.ObjectClassPropId );
                foreach( DataRow row in ocpTable.Rows )
                {
                    row["fieldtypeid"] = numberFieldType.FieldTypeId;
                }
                tableOCPUpdate.update( ocpTable );

                CswTableUpdate tableNTPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "NTP_SpecGravity_26835", "nodetype_props" );
                DataTable ntpTable = tableNTPUpdate.getTable( "objectclasspropid", specificGravityOCP.ObjectClassPropId );
                foreach( DataRow row in ntpTable.Rows )
                {
                    row["fieldtypeid"] = numberFieldType.FieldTypeId;
                }
                tableNTPUpdate.update( ntpTable );
            }

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( specificGravityOCP, specificGravityOCP.getFieldTypeRule().SubFields.Default.Name, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( specificGravityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

        }//Update()

    }//class CswUpdateSchemaCase26835

}//namespace ChemSW.Nbt.Schema