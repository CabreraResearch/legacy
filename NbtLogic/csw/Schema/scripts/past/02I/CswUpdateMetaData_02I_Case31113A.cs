using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31113A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31113; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override string Title
        {
            get { return "Create new Material Component ObjectClassProps"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            if( null != MaterialComponentOC )
            {
                CswNbtMetaDataObjectClassProp PercentageOCP = MaterialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Percentage );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PercentageOCP, CswEnumNbtObjectClassPropAttributes.servermanaged, true );
                CswNbtMetaDataObjectClassProp LowPercentageValueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MaterialComponentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassMaterialComponent.PropertyName.LowPercentageValue,
                    FieldType = CswEnumNbtFieldType.Number,
                    NumberMinValue = 0,
                    NumberMaxValue = 100,
                    NumberPrecision = 3,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( LowPercentageValueOCP, 0, CswEnumNbtSubFieldName.Value );
                CswNbtMetaDataObjectClassProp TargetPercentageValueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MaterialComponentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassMaterialComponent.PropertyName.TargetPercentageValue,
                    FieldType = CswEnumNbtFieldType.Number,
                    NumberMinValue = 0,
                    NumberMaxValue = 100,
                    NumberPrecision = 3,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TargetPercentageValueOCP, 0, CswEnumNbtSubFieldName.Value );
                CswNbtMetaDataObjectClassProp HighPercentageValueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MaterialComponentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassMaterialComponent.PropertyName.HighPercentageValue,
                    FieldType = CswEnumNbtFieldType.Number,
                    NumberMinValue = 0,
                    NumberMaxValue = 100,
                    NumberPrecision = 3,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( HighPercentageValueOCP, 0, CswEnumNbtSubFieldName.Value );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema