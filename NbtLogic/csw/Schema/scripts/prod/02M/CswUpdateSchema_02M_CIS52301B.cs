using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52301B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52301; }
        }

        public override string Title
        {
            get { return "Method Characteristic nodetype"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MethodCharacteristicOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodCharacteristicClass );

            CswNbtMetaDataNodeType MethodCharacteristicNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( MethodCharacteristicOC )
                {
                    NodeTypeName = "Method Characteristic",
                    Category = "Testing",
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMethodCharacteristic.PropertyName.Method ) + ": " +
                                   CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMethodCharacteristic.PropertyName.Characteristic )
                } );

            CswNbtMetaDataNodeTypeProp CharacteristicNTP      = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.Characteristic );
            CswNbtMetaDataNodeTypeProp ConstituentMaterialNTP = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.ConstituentMaterial );
            CswNbtMetaDataNodeTypeProp MethodNTP              = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.Method );
            CswNbtMetaDataNodeTypeProp ObsoleteNTP            = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.Obsolete );
            CswNbtMetaDataNodeTypeProp PrecisionNTP           = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.Precision );
            CswNbtMetaDataNodeTypeProp ResultOptionsNTP       = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.ResultOptions );
            CswNbtMetaDataNodeTypeProp ResultTypeNTP          = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.ResultType );
            CswNbtMetaDataNodeTypeProp ResultUnitsNTP         = MethodCharacteristicNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMethodCharacteristic.PropertyName.ResultUnits );

            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, MethodCharacteristicNT.NodeTypeId, MethodNTP,              true, MethodCharacteristicNT.getFirstNodeTypeTab().TabId, 1, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, MethodCharacteristicNT.NodeTypeId, CharacteristicNTP,      true, MethodCharacteristicNT.getFirstNodeTypeTab().TabId, 2, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, MethodCharacteristicNT.NodeTypeId, ResultTypeNTP,          true, MethodCharacteristicNT.getFirstNodeTypeTab().TabId, 3, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, MethodCharacteristicNT.NodeTypeId, ResultUnitsNTP,         true, MethodCharacteristicNT.getFirstNodeTypeTab().TabId, 4, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, MethodCharacteristicNT.NodeTypeId, PrecisionNTP,           true, MethodCharacteristicNT.getFirstNodeTypeTab().TabId, 5, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, MethodCharacteristicNT.NodeTypeId, ResultOptionsNTP,       true, MethodCharacteristicNT.getFirstNodeTypeTab().TabId, 6, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, MethodCharacteristicNT.NodeTypeId, ConstituentMaterialNTP, true, MethodCharacteristicNT.getFirstNodeTypeTab().TabId, 7, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, MethodCharacteristicNT.NodeTypeId, ObsoleteNTP,            true, MethodCharacteristicNT.getFirstNodeTypeTab().TabId, 8, 1 );

            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, MethodCharacteristicNT.NodeTypeId, MethodNTP,              true, 1, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, MethodCharacteristicNT.NodeTypeId, CharacteristicNTP,      true, 2, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, MethodCharacteristicNT.NodeTypeId, ResultTypeNTP,          true, 3, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, MethodCharacteristicNT.NodeTypeId, ResultUnitsNTP,         true, 4, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, MethodCharacteristicNT.NodeTypeId, PrecisionNTP,           true, 5, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, MethodCharacteristicNT.NodeTypeId, ResultOptionsNTP,       true, 6, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, MethodCharacteristicNT.NodeTypeId, ConstituentMaterialNTP, true, 7, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, MethodCharacteristicNT.NodeTypeId, ObsoleteNTP,            true, 8, 1 );

            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Preview, MethodCharacteristicNT.NodeTypeId, MethodNTP,          true, 1, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Preview, MethodCharacteristicNT.NodeTypeId, CharacteristicNTP,  true, 2, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Preview, MethodCharacteristicNT.NodeTypeId, ResultTypeNTP,      true, 3, 1 );

            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Table, MethodCharacteristicNT.NodeTypeId, MethodNTP,            true, 1, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Table, MethodCharacteristicNT.NodeTypeId, CharacteristicNTP,    true, 2, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Table, MethodCharacteristicNT.NodeTypeId, ResultTypeNTP,        true, 3, 1 );

            CharacteristicNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleList.AttributeName.Options].AsText.Text = "pH,Arsenic,Lead,Cadmium,Mercury";
        } // update()
    } // class CswUpdateMetaData_02M_CIS52301
}