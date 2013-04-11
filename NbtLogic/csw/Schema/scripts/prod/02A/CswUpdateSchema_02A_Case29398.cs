using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29398
    /// </summary>
    public class CswUpdateSchema_02A_Case29398 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29398; }
        }

        public override void update()
        {
            //remove any nodetype_layout rows referencing tabs that no longer exist in nodetype_tabset
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                @"delete from nodetype_layout where nodetypetabsetid in 
                (
                    select nodetypetabsetid from (
                        (select l.nodetypetabsetid from nodetype_layout l)
                        minus
                        (select t.nodetypetabsetid from nodetype_tabset t)
                    ) where nodetypetabsetid is not null
                );" );
        } // update()

    }//class CswUpdateSchema_02A_Case29398

}//namespace ChemSW.Nbt.Schema