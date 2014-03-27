using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS53115 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 53115; }
        }

        public override string Title
        {
            get { return "Set Quantity pendingupdate=1"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            // Quantity properties were not updating their cached units when pendingupdate=1.  
            // See CIS-52562.  
            // This will reset all existing quantities to fix their units.

            string SQL = @"update jct_nodes_props 
                              set pendingupdate = '" + CswConvert.ToDbVal( true ) + @"' 
                            where jctnodepropid in (select j.jctnodepropid
                                                      from jct_nodes_props j
                                                      join nodetype_props p on j.nodetypepropid = p.nodetypepropid
                                                      join field_types f on p.fieldtypeid = f.fieldtypeid
                                                     where (f.fieldtype = 'Quantity'))";

            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( SQL );

        }
    }
}