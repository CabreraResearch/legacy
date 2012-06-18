using System.Collections.Generic;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26410
    /// </summary>
    public class CswUpdateSchemaCase26410 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass UnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );
            IEnumerable<CswNbtMetaDataObjectClassProp> OCPropsToUpdate = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProps( CswNbtMetaDataFieldType.NbtFieldType.Quantity );

            foreach( CswNbtMetaDataObjectClassProp QuantityProp in OCPropsToUpdate )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isfk, true );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, UnitOC.ObjectClassId );
            }

            IEnumerable<CswNbtMetaDataNodeTypeProp> NTPropsToUpdate = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Quantity );

            foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NTPropsToUpdate )
            {
                CswNbtViewId NewViewId = NodeTypeProp.ViewId;
                CswNbtView UnitView = _CswNbtSchemaModTrnsctn.restoreView( NewViewId );
                UnitView.Root.ChildRelationships.Clear();
                UnitView.ViewName = "CswNbtNodeTypePropQuantity_" + NodeTypeProp.NodeTypeId.ToString();
                UnitView.AddViewRelationship( UnitOC, true );
                UnitView.save();

                NodeTypeProp.ViewId = UnitView.ViewId;
                NodeTypeProp.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), UnitOC.ObjectClassId );
            }

        }//Update()

    }//class CswUpdateSchemaCase26410

}//namespace ChemSW.Nbt.Schema