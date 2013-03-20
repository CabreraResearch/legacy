using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01W_TreeGrouping_Case27882: CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 27882; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update node_views set groupbysiblings=" + CswConvert.ToDbVal( false ) );

            foreach( CswNbtView EquipmentView in _CswNbtSchemaModTrnsctn.restoreViews("All Equipment") )
            {
                if( EquipmentView.ViewMode == NbtViewRenderingMode.Tree )
                {
                    EquipmentView.GroupBySiblings = true;
                    EquipmentView.save();
                }
            }
        } //Update()

    }//class CswUpdateSchema_01V_CaseXXXXX

}//namespace ChemSW.Nbt.Schema