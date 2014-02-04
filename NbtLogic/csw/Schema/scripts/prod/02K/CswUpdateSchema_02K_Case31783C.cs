using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31783C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31783; }
        }

        public override string Title
        {
            get { return "Migrate Layout Data - Layout Column and Layout Property, Pass 2"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {

        } // update()

    } // class CswUpdateSchema_02K_Case31783
} // namespace