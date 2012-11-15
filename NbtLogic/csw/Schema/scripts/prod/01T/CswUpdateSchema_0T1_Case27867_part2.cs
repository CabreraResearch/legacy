using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_0T1_Case27867_part2
    /// </summary>
    public class CswUpdateSchema_0T1_Case27867_part2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            List<CswNbtView> views = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Receipt Lots" );
            if( 1 == views.Count )
            {
                views[0].IsDemo = true;
                views[0].save();
            }
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27867; }
        }

        //Update()

    }//class CswUpdateSchema_0T1_Case27867_part2.cs

}//namespace ChemSW.Nbt.Schema