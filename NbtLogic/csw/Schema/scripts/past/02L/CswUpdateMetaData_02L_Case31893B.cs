using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case31893B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31893; }
        }

        public override string Title
        {
            get { return "Promote Legacy Material Id to PS property"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createPropertySetProp( CswEnumNbtPropertySetName.MaterialSet, CswNbtPropertySetMaterial.PropertyName.LegacyMaterialId, CswEnumNbtFieldType.Text );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema