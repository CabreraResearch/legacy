using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26410
    /// </summary>
    public class CswUpdateSchemaCase26410 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //update all quantity props to have isFK = true, FKType=ObjectClassId, and FKValue
            CswNbtMetaDataObjectClass UnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );
            Collection<CswNbtMetaDataObjectClassProp> PropsToUpdate = new Collection<CswNbtMetaDataObjectClassProp>();

            CswNbtMetaDataObjectClass AliquotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass );
            CswNbtMetaDataObjectClassProp AliquotQuantityOCP = AliquotOC.getObjectClassProp( CswNbtObjClassAliquot.QuantityPropertyName );
            PropsToUpdate.Add( AliquotQuantityOCP );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp ExpirationIntervalOCP = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.ExpirationIntervalPropName );
            PropsToUpdate.Add( ExpirationIntervalOCP );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp ContainerQuantityOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.QuantityPropertyName );
            PropsToUpdate.Add( ContainerQuantityOCP );

            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp CapacityOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName );
            PropsToUpdate.Add( CapacityOCP );

            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataObjectClassProp ReqItemQuantityOCP = RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Quantity._Name );
            PropsToUpdate.Add( ReqItemQuantityOCP );

            CswNbtMetaDataObjectClass ContainerDispenseTransactionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataObjectClassProp QuantityDispensedOCP = ContainerDispenseTransactionOC.getObjectClassProp( CswNbtObjClassContainerDispenseTransaction.QuantityDispensedPropertyName );
            PropsToUpdate.Add( QuantityDispensedOCP );
            CswNbtMetaDataObjectClassProp SourceQuantityOCP = ContainerDispenseTransactionOC.getObjectClassProp( CswNbtObjClassContainerDispenseTransaction.RemainingSourceContainerQuantityPropertyName );
            PropsToUpdate.Add( SourceQuantityOCP );

            CswNbtView UnitView = _CswNbtSchemaModTrnsctn.makeView();
            UnitView.makeNew( "CswNbtNodePropQuantity()", NbtViewVisibility.Property );
            UnitView.AddViewRelationship( UnitOC, true );
            UnitView.save();

            foreach( CswNbtMetaDataObjectClassProp QuantityProp in PropsToUpdate )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isfk, true );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, UnitOC.ObjectClassId );

                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in QuantityProp.getNodeTypeProps() )
                {
                    NodeTypeProp.ViewId = UnitView.ViewId;
                    NodeTypeProp.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), UnitOC.ObjectClassId );
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase26410

}//namespace ChemSW.Nbt.Schema