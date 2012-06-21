﻿using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24517CurrentQuantity
    /// </summary>
    public class CswUpdateSchemaCase24517CurrentQuantity : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryLevelOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryLevelClass, "docs.gif", true, false );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                PropName = CswNbtObjClassInventoryLevel.PropertyName.CurrentQuantity,
                ServerManaged = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments,
                PropName = CswNbtObjClassInventoryLevel.PropertyName.CurrentQuantityLog,
                ServerManaged = true
            } );

            foreach( CswNbtMetaDataNodeType LocationLevelNt in InventoryLevelOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab LogTab = LocationLevelNt.getNodeTypeTab( "Log" );
                if( null == LogTab )
                {
                    LogTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( LocationLevelNt, "Log", Int32.MinValue );
                }
                CswNbtMetaDataNodeTypeProp QuantityLogNtp = LocationLevelNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.CurrentQuantityLog );
                QuantityLogNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LogTab.TabId );
            }


        }//Update()

    }//class CswUpdateSchemaCase24517CurrentQuantity

}//namespace ChemSW.Nbt.Schema