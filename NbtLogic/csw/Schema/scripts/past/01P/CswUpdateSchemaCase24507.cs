using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24507
    /// </summary>
    public class CswUpdateSchemaCase24507 : CswUpdateSchemaTo
    {
        public override void update()
        {
            String UnitTypeList = String.Join( ",", CswNbtObjClassUnitOfMeasure.UnitTypes._All );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass,
                CswNbtObjClassUnitOfMeasure.UnitTypePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.List,
                ServerManaged: true,
                ListOptions: UnitTypeList );

            CswNbtMetaDataNodeType WeightUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Weight Unit" );
            if( WeightUnitNodeType != null )
            {
                CswNbtMetaDataNodeTypeProp WeightUnitTypeProp = WeightUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
                WeightUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Weight.ToString();

                foreach( CswNbtNode WeightNode in WeightUnitNodeType.getNodes( false, false ) )
                {
                    WeightNode.Properties[CswNbtObjClassUnitOfMeasure.UnitTypePropertyName].AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Weight.ToString();
                    WeightNode.postChanges( true );
                }
            }

            CswNbtMetaDataNodeType VolumeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Volume Unit" );
            if( VolumeUnitNodeType != null )
            {
                CswNbtMetaDataNodeTypeProp VolumeUnitTypeProp = VolumeUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
                VolumeUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Volume.ToString();

                foreach( CswNbtNode VolumeNode in VolumeUnitNodeType.getNodes( false, false ) )
                {
                    VolumeNode.Properties[CswNbtObjClassUnitOfMeasure.UnitTypePropertyName].AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Volume.ToString();
                    VolumeNode.postChanges( true );
                }
            }

            CswNbtMetaDataNodeType TimeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Time Unit" );
            if( TimeUnitNodeType != null )
            {
                CswNbtMetaDataNodeTypeProp TimeUnitTypeProp = TimeUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
                TimeUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Time.ToString();

                foreach( CswNbtNode TimeNode in TimeUnitNodeType.getNodes( false, false ) )
                {
                    TimeNode.Properties[CswNbtObjClassUnitOfMeasure.UnitTypePropertyName].AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Time.ToString();
                    TimeNode.postChanges( true );
                }
            }

            CswNbtMetaDataNodeType EachUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Each Unit" );
            if( EachUnitNodeType != null )
            {
                CswNbtMetaDataNodeTypeProp EachUnitTypeProp = EachUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
                EachUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Each.ToString();

                foreach( CswNbtNode EachNode in EachUnitNodeType.getNodes( false, false ) )
                {
                    EachNode.Properties[CswNbtObjClassUnitOfMeasure.UnitTypePropertyName].AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Each.ToString();
                    EachNode.postChanges( true );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase24507

}//namespace ChemSW.Nbt.Schema