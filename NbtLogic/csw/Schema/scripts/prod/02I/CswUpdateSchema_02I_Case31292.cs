using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31292 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31292; }
        }

        public override string Title
        {
            get { return "GHS compound uniqueness"; }
        }

        public override void update()
        {
            // Make GHS compound unique on material and chemical
            CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction ), CswEnumNbtObjectClassPropAttributes.iscompoundunique, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material ), CswEnumNbtObjectClassPropAttributes.iscompoundunique, true );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema