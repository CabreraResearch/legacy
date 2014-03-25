using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CISXXXXX : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override string Title
        {
            get { return "Script for " + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            //Do Schema Stuff Here
        }
    }
}