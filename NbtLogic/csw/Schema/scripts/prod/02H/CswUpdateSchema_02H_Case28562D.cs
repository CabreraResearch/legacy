using System.Data;
using System.IO;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case28562D : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28562; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "D"; }
        }

        public override string Title
        {
            get { return "Remove old HMIS action"; }
        }

        public override void update()
        {

            _CswNbtSchemaModTrnsctn.deleteAction( "HMIS_Reporting" );

        } // update()
    }

}//namespace ChemSW.Nbt.Schema