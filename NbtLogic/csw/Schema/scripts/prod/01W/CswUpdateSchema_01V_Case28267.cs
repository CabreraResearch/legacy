using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28267
    /// </summary>
    public class CswUpdateSchema_01V_Case28267 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28267; }
        }

        public override void update()
        {

            //Make the Containers grid view group by location
            List<CswNbtView> containersViews = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Containers" );
            foreach( CswNbtView containerView in containersViews )
            {
                if( containerView.Visibility.Equals( NbtViewVisibility.Global ) && containerView.NbtViewMode.Equals( "Grid" ) )
                {
                    containerView.Root.GridGroupByCol = "Location";
                    containerView.save();
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28267

}//namespace ChemSW.Nbt.Schema