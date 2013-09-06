using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02E_Case30484 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30484; }
        }

        public override void update()
        {
            //Inspection Design 'Status' property should be readonly (and thus, admin overridable)
            CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp StatusOCP = InspectionOC.getObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOCP, CswEnumNbtObjectClassPropAttributes.readOnly, CswConvert.ToDbVal( true ) );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOCP, CswEnumNbtObjectClassPropAttributes.isrequired, CswConvert.ToDbVal( true ) );
        } // update()
    } // class
}//namespace ChemSW.Nbt.Schema