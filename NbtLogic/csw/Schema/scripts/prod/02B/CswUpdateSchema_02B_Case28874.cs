using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case28874 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28874; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update nodes set isdemo='1' where nodeid in (select n.nodeid
                                                                      from nodes n
                                                                      join nodetypes t on (n.nodetypeid = t.nodetypeid)
                                                                     where (t.nodetypename = 'Vendor' and
                                                                           n.nodename in ('PerkinElmer', 'Fisher Scientific',
                                                                            'VWR International', 'Cole-Parmer', 'Default Vendor',
                                                                            'Sigma-Aldrich', 'VWR Scientific', 'Alfa-Aesar'))
                                                                        or (t.nodetypename = 'Department' and
                                                                           n.nodename in ('Sales', 'Maintenance', 'R&' || 'D')) )" );
        } // update()

    }//class CswUpdateSchema_02B_Case28874

}//namespace ChemSW.Nbt.Schema