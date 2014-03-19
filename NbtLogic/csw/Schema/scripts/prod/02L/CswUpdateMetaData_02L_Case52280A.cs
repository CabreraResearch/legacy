using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52280A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 52280; }
        }

        public override string Title
        {
            get { return "MLM2: Changes to Materials - Add new properties"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            // Add Manufacturing Sites Grid property (MLM only)
            _CswNbtSchemaModTrnsctn.createPropertySetProp( CswEnumNbtPropertySetName.MaterialSet, CswNbtPropertySetMaterial.PropertyName.ManufacturingSites, CswEnumNbtFieldType.Grid );

            // Requires Cleaning Event (MLM only)
            _CswNbtSchemaModTrnsctn.createPropertySetProp( CswEnumNbtPropertySetName.MaterialSet, CswNbtPropertySetMaterial.PropertyName.RequiresCleaningEvent, CswEnumNbtFieldType.Logical );

            // Obsolete Property 
            _CswNbtSchemaModTrnsctn.createPropertySetProp( CswEnumNbtPropertySetName.MaterialSet, CswNbtPropertySetMaterial.PropertyName.Obsolete, CswEnumNbtFieldType.Logical, true );
        } // update()
    }

}//namespace ChemSW.Nbt.Schema