using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

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
            CswNbtMetaDataObjectClassProp RequestOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerObjClass,
                new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.RequestPropertyName,
                    Extended = CswNbtNodePropButton.ButtonMode.menu,
                    ListOptions = CswNbtObjClassContainer.RequestMenu.Options.ToString(),
                    StaticText = CswNbtObjClassContainer.RequestMenu.Dispense
                }
            );


            foreach( CswNbtMetaDataNodeType ContainerNodeType in ContainerObjClass.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab NodeTypeTab = ContainerNodeType.getFirstNodeTypeTab();
                Int32 FirstTabId = NodeTypeTab.TabId;
                CswNbtMetaDataNodeTypeProp DispenseNodeTypeProp = ContainerNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.RequestPropertyName );
                DispenseNodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FirstTabId );
                DispenseNodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true, FirstTabId );
            }

            //Add ContainerDispenseTransaction NodeType

            CswNbtMetaDataNodeType ContDispTransNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType(
                CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass.ToString(),
                "Container Dispense Transaction",
                "Materials"
                );

            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.DispenseContainer, false, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtResources.CswNbtModule.CISPro, CswNbtActionName.DispenseContainer );

        }//Update()

    }//class CswUpdateSchemaCase24508B

}//namespace ChemSW.Nbt.Schema