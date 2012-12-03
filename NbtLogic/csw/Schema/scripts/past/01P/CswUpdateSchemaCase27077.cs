using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27077
    /// </summary>
    public class CswUpdateSchemaCase27077 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp UnitType = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( UnitOfMeasureOC.ObjectClassId, CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UnitType, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            foreach( CswNbtMetaDataNodeType UoMNodeType in UnitOfMeasureOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UnitTypeNTProp = UoMNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.removePropFromAllLayouts( UnitTypeNTProp );
            }
        }//Update()

    }//class CswUpdateSchemaCase27077

}//namespace ChemSW.Nbt.Schema