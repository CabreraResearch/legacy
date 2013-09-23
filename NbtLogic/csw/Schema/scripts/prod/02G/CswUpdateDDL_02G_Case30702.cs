using System.Data;
using ChemSW.Audit;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for MetaData changes
    /// </summary>
    public class CswUpdateDDL_02G_Case30702 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Audit nodetype_layout"; } }

        public override string ScriptName
        {
            get { return "Case_30702"; }
        }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30702; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "nodetype_layout" );
        } // update()

    }
}


