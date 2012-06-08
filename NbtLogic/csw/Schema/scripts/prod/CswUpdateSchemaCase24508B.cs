using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24508B
    /// </summary>
    public class CswUpdateSchemaCase24508B : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp DispenseProp = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerObjClass,
                new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.DispensePropertyName
                }
            );

            //Set Container Quantity on Add
            CswNbtMetaDataObjectClassProp QuantityNodeTypeProp = ContainerObjClass.getObjectClassProp( CswNbtObjClassContainer.QuantityPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityNodeTypeProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityNodeTypeProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            foreach( CswNbtMetaDataNodeType ContainerNodeType in ContainerObjClass.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab NodeTypeTab = ContainerNodeType.getFirstNodeTypeTab();
                Int32 FirstTabId = NodeTypeTab.TabId;
                CswNbtMetaDataNodeTypeProp DispenseNodeTypeProp = ContainerNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.DispensePropertyName );
                DispenseNodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirstTabId );
                DispenseNodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirstTabId );
            }

            //Add ContainerDispenseTransaction NodeType

            CswNbtMetaDataNodeType ContDispTransNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType(
                CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass.ToString(),
                "Container Dispense Transaction",
                "Materials"
                );

        }//Update()

    }//class CswUpdateSchemaCase24508B

}//namespace ChemSW.Nbt.Schema