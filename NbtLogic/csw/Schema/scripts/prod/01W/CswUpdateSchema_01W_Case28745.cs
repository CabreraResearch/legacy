using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28745
    /// </summary>
    public class CswUpdateSchema_01W_Case28745 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28745; }
        }

        public override void update()
        {
            List<CswNbtView> ContainerViews = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews("Containers");
            foreach ( CswNbtView View in ContainerViews )
            {
                if( View.Visibility != NbtViewVisibility.Property )
                {
                    View.Delete();
                }
            }
        }//Update()
    }//class CswUpdateSchemaCase_01W_28745
}//namespace ChemSW.Nbt.Schema