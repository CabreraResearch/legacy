﻿using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27278
    /// </summary>
    public class CswUpdateSchemaCase27278 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContDispTransOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataNodeType ContDispTransNt = ContDispTransOc.FirstNodeType;
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataNodeType ContainerNt = ContainerOc.FirstNodeType;

            if( null != ContDispTransNt )
            {
                CswNbtMetaDataNodeTypeProp ContainerDispensesGridProp = ContainerNt.getNodeTypeProp( "Container Dispense Transactions" );
                if( null != ContainerDispensesGridProp )
                {
                    CswNbtView GridView = _CswNbtSchemaModTrnsctn.restoreView( ContainerDispensesGridProp.ViewId );
                    if( null != GridView )
                    {
                        CswNbtMetaDataNodeTypeProp SourceContainerNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.SourceContainer );
                        CswNbtMetaDataNodeTypeProp DestinationContainerNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.DestinationContainer );
                        CswNbtMetaDataNodeTypeProp QuantityDispensedNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.QuantityDispensed );
                        CswNbtMetaDataNodeTypeProp TypeNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.Type );
                        CswNbtMetaDataNodeTypeProp DispensedDateNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.DispensedDate );
                        CswNbtMetaDataNodeTypeProp RemainingSourceContainerQuantityNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.RemainingSourceContainerQuantity );
                        CswNbtMetaDataNodeTypeProp RequestNtp = ContDispTransNt.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerDispenseTransaction.PropertyName.RequestItem );

                        if( null != SourceContainerNtp && null != DestinationContainerNtp )
                        {
                            GridView.Root.ChildRelationships.Clear();
                            CswNbtViewRelationship RootRel = GridView.AddViewRelationship( ContainerNt, false );

                            List<CswNbtMetaDataNodeTypeProp> ContainerNtps = new List<CswNbtMetaDataNodeTypeProp>();
                            ContainerNtps.Add( SourceContainerNtp );
                            ContainerNtps.Add( DestinationContainerNtp );
                            foreach( CswNbtMetaDataNodeTypeProp ContainerNtp in ContainerNtps )
                            {
                                CswNbtViewRelationship ContDispTransRel = GridView.AddViewRelationship( RootRel, NbtViewPropOwnerType.Second, ContainerNtp, false );

                                CswNbtViewProperty DispensedDateVp = GridView.AddViewProperty( ContDispTransRel, DispensedDateNtp );
                                DispensedDateVp.Order = 1;
                                DispensedDateVp.SortBy = true;
                                DispensedDateVp.SortMethod = NbtViewPropertySortMethod.Descending;

                                CswNbtViewProperty SourceContainerVp = GridView.AddViewProperty( ContDispTransRel, SourceContainerNtp );
                                SourceContainerVp.Order = 2;

                                CswNbtViewProperty DestinationContainerVp = GridView.AddViewProperty( ContDispTransRel, DestinationContainerNtp );
                                DestinationContainerVp.Order = 3;

                                CswNbtViewProperty TypeVp = GridView.AddViewProperty( ContDispTransRel, TypeNtp );
                                TypeVp.Order = 4;

                                CswNbtViewProperty QuantityDispensedVp = GridView.AddViewProperty( ContDispTransRel, QuantityDispensedNtp );
                                QuantityDispensedVp.Order = 5;

                                CswNbtViewProperty RemainingSourceContainerQuantityVp = GridView.AddViewProperty( ContDispTransRel, RemainingSourceContainerQuantityNtp );
                                RemainingSourceContainerQuantityVp.Order = 6;

                                CswNbtViewProperty RequestVp = GridView.AddViewProperty( ContDispTransRel, RequestNtp );
                                RequestVp.Order = 7;
                            }
                            GridView.save();
                        }
                    }
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase27278

}//namespace ChemSW.Nbt.Schema