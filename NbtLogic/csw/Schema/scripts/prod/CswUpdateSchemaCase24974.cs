using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 24974
    /// </summary>
    public class CswUpdateSchemaCase24974: CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp EquipmentStatusOCP = EquipmentOC.getObjectClassProp( CswNbtObjClassEquipment.StatusPropertyName );
            string StatusListOptions = EquipmentStatusOCP.ListOptions;

            // Clear list options from Equipment 'Status' object class prop
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( EquipmentStatusOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, "" );

            // Set list options on Equipment 'Status' nodetype prop
            foreach( CswNbtMetaDataNodeTypeProp EquipmentStatusNTP in EquipmentStatusOCP.getNodeTypeProps() )
            {
                EquipmentStatusNTP.ListOptions = StatusListOptions;
            }

        }//Update()

    }//class CswUpdateSchemaCase24974
}//namespace ChemSW.Nbt.Schema