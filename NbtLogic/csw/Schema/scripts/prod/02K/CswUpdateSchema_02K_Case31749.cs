using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31749 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31749; }
        }

        public override string Title
        {
            get { return "Make Initial Quantity not required"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp InitialQuantityOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InitialQuantityOCP, CswEnumNbtObjectClassPropAttributes.isrequired, false );
        } // update()

    } // class CswUpdateSchema_02K_Case31749
} // namespace