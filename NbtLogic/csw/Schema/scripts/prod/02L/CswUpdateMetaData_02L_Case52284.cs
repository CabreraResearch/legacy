using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52284: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52284; }
        }

        public override string Title
        {
            get { return "Create Manufacturer Prop on MEP"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MEPOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerEquivalentPartClass );
            CswNbtMetaDataObjectClass ManufacturerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerClass );
            if( null != ManufacturerOC )
            {
                CswNbtMetaDataObjectClassProp MaterialOCP = MEPOC.getObjectClassProp( "Material" );
                if( null != MaterialOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( MaterialOCP, true );
                }
                CswNbtMetaDataObjectClassProp ManufacturerOCP = MEPOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Manufacturer );
                if( null != ManufacturerOCP )
                {
                    CswNbtView ManufacturerView = _CswNbtSchemaModTrnsctn.makeView();
                    CswNbtViewRelationship parent = ManufacturerView.AddViewRelationship( ManufacturerOC, true );
                    ManufacturerView.Visibility = CswEnumNbtViewVisibility.Property;
                    ManufacturerView.ViewName = "Manufacturer";
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ManufacturerOCP, CswEnumNbtObjectClassPropAttributes.viewxml, ManufacturerView.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ManufacturerOCP, CswEnumNbtObjectClassPropAttributes.fktype, CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ManufacturerOCP, CswEnumNbtObjectClassPropAttributes.fkvalue, ManufacturerOC.ObjectClassId.ToString() );
                    foreach( CswNbtMetaDataNodeType MEPNT in MEPOC.getNodeTypes() )
                    {
                        CswNbtMetaDataNodeTypeProp ManufacturerNTP = MEPNT.getNodeTypePropByObjectClassProp( ManufacturerOCP );
                        ManufacturerNTP.DesignNode.syncFromObjectClassProp();
                    }
                }
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, MEPOC.ObjectClassId );
            }
        } // update()
    }
}//namespace ChemSW.Nbt.Schema