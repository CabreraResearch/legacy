using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25759
    /// </summary>
    public class CswUpdateSchemaCase25759 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //Set Container.Quantity to ReadOnly
            CswNbtMetaDataObjectClass ContainerObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp QuantityProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( ContainerObjClass.ObjectClassId, CswNbtObjClassContainer.QuantityPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, true );

            //Add "N/A" to Material.PhysicalState
            CswNbtMetaDataObjectClass MaterialObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp PhysicalStateProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( MaterialObjClass.ObjectClassId, CswNbtObjClassMaterial.PhysicalStatePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PhysicalStateProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, "solid,liquid,gas,n/a" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PhysicalStateProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PhysicalStateProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            //Set Supplies.PhysicalState DefaultValue to "N/A", ReadOnly = true
            CswNbtMetaDataNodeType SupplyNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );
            if( SupplyNodeType != null )
            {
                CswNbtMetaDataNodeTypeProp PhysicalStateNTProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( SupplyNodeType.NodeTypeId, PhysicalStateProp.ObjectClassPropId );
                PhysicalStateNTProp.DefaultValue.AsList.Value = "N/A";
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.removePropFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, PhysicalStateNTProp, PhysicalStateNTProp.AddLayout.TabId );
                PhysicalStateNTProp.ReadOnly = true;
            }

            //Undo Set Size.Capacity SetValOnAdd = false
            CswNbtMetaDataObjectClass SizeObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp CapacityProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( SizeObjClass.ObjectClassId, CswNbtObjClassSize.InitialQuantityPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CapacityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

        }//Update()

    }//class CswUpdateSchemaCase25759

}//namespace ChemSW.Nbt.Schema