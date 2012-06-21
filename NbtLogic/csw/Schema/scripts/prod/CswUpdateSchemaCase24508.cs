using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24508
    /// </summary>
    public class CswUpdateSchemaCase24508 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass RequestItemObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );

            #region Create new ObjectClass

            CswNbtMetaDataObjectClass ContDispTransObjClass =
                _CswNbtSchemaModTrnsctn.createObjectClass(
                    CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass,
                    IconFileName: "docs.gif",
                    AuditLevel: true,
                    UseBatchEntry: false );

            #endregion

            #region Create ObjectClassProps

            CswNbtMetaDataObjectClassProp SourceContainerProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    ContDispTransObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassContainerDispenseTransaction.SourceContainerPropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                        IsFk = true,
                        FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = ContainerObjClass.ObjectClassId
                    }
                );

            CswNbtMetaDataObjectClassProp DestinationContainerProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    ContDispTransObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassContainerDispenseTransaction.DestinationContainerPropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                        IsFk = true,
                        FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = ContainerObjClass.ObjectClassId
                    }
                );

            CswNbtMetaDataObjectClassProp QuantityDispensedProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    ContDispTransObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassContainerDispenseTransaction.QuantityDispensedPropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
                    }
                );

            Collection<String> DispenseTypes = new Collection<string>();
            DispenseTypes.Add( CswNbtObjClassContainerDispenseTransaction.DispenseType.Receive.ToString() );
            DispenseTypes.Add( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense.ToString() );
            DispenseTypes.Add( CswNbtObjClassContainerDispenseTransaction.DispenseType.Waste.ToString() );
            DispenseTypes.Add( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispose.ToString() );
            DispenseTypes.Add( CswNbtObjClassContainerDispenseTransaction.DispenseType.Add.ToString() );

            CswNbtMetaDataObjectClassProp TypeProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    ContDispTransObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassContainerDispenseTransaction.TypePropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                        ListOptions = String.Join( ",", DispenseTypes )
                    }
                );

            CswNbtMetaDataObjectClassProp DispensedDateProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    ContDispTransObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassContainerDispenseTransaction.DispensedDatePropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime
                    }
                );

            CswNbtMetaDataObjectClassProp RemainingSourceContainerQuantityProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    ContDispTransObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassContainerDispenseTransaction.RemainingSourceContainerQuantityPropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
                    }
                );

            CswNbtMetaDataObjectClassProp RequestItemProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    ContDispTransObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassContainerDispenseTransaction.RequestItemPropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                        IsFk = true,
                        FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = RequestItemObjClass.ObjectClassId
                    }
                );

            #endregion

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, ContDispTransObjClass.ObjectClassId );

        }//Update()

    }//class CswUpdateSchemaCase24508

}//namespace ChemSW.Nbt.Schema