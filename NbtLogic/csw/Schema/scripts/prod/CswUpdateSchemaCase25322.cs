using System;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25322
    /// </summary>
    public class CswUpdateSchemaCase25322 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // usenumbering on the object_class_prop
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class_props", "usenumbering" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "object_class_props", "usenumbering", "Whether the property should be numbered", false, false );
            }
            
            // case 25322 - Finished and cancelled become buttons

            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp FinishedCheckboxOCP = InspectionDesignOC.getObjectClassProp( "Finished" );
            CswNbtMetaDataObjectClassProp CancelledCheckboxOCP = InspectionDesignOC.getObjectClassProp( "Cancelled" );

            // Remove old checkboxes
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( FinishedCheckboxOCP, true );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( CancelledCheckboxOCP, true );

            // Add new buttons
            CswNbtMetaDataObjectClassProp FinishButtonOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.FinishPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Button );
            CswNbtMetaDataObjectClassProp CancelButtonOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass, CswNbtObjClassInspectionDesign.CancelPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Button );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FinishButtonOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.usenumbering, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CancelButtonOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.usenumbering, false );

            // Put buttons on Action tab, if the design has one
            foreach( CswNbtMetaDataNodeType InspectionDesignNT in InspectionDesignOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab ActionTab = InspectionDesignNT.getNodeTypeTab( "Action" );
                if( ActionTab != null )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, InspectionDesignNT.NodeTypeId, InspectionDesignNT.getNodeTypePropIdByObjectClassProp( FinishButtonOCP.PropName ), ActionTab.TabId, 1, 1 );
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, InspectionDesignNT.NodeTypeId, InspectionDesignNT.getNodeTypePropIdByObjectClassProp( CancelButtonOCP.PropName ), ActionTab.TabId, 4, 1 );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase25322

}//namespace ChemSW.Nbt.Schema