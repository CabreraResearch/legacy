using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_CaseXXXXX : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override string ScriptName
        {
            get { throw new NotImplementedException(); }
        }

        public override bool AlwaysRun
        {
            get { throw new NotImplementedException(); }
        }

        public override void update()
        {
            // This is a placeholder script that does nothing.
        } // update()

    }

}//namespace ChemSW.Nbt.Schema