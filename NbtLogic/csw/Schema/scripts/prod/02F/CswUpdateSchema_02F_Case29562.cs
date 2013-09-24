using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case29562 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29562; }
        }

        public override string ScriptName
        {
            get { return "02F_" + CaseNo; }
        }

        public override string Title
        {
            get { return "Performance improvements"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.indexColumn( "jct_nodes_props", "nodetypepropid, field1, field2, field3, field1_numeric, field1_date", "JCT10" );
            _CswNbtSchemaModTrnsctn.indexColumn( "jct_nodes_props", "nodeid, nodetypepropid, field1, field1_fk", "JCT11" );


        } // update()

    }

}//namespace ChemSW.Nbt.Schema