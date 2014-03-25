using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02M_CISXXXXX : CswUpdateSchemaTo
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
            //Do DDL Stuff Here
        }
    }
}


