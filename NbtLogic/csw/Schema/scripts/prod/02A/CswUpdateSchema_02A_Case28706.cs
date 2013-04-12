using ChemSW.Nbt.csw.Dev;
using System.Data;
using System.Collections.Generic;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02A_Case28706 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28706; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                CswNbtMetaDataNodeTypeProp ContainersNTP = ChemicalNT.getNodeTypeProp( "Chemical Containers" );
                if( null != ContainersNTP )
                {
                    CswNbtViewId ChemContainersViewId = ContainersNTP.ViewId;
                    CswNbtView ChemContainersView = _CswNbtSchemaModTrnsctn.restoreView( ChemContainersViewId );
                    CswNbtViewProperty DisposedViewProp = ChemContainersView.findPropertyByName( "Disposed" );
                    foreach( CswNbtViewPropertyFilter CurrentFilter in DisposedViewProp.Filters )
                    {
                        CurrentFilter.ShowAtRuntime = true;
                    }

                    ChemContainersView.save();

                }//if we ahve a containers NTP

            }//if we have a chemical NT


            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType CurrentLocationNT in LocationOC.getNodeTypes() )
            {
                    CswNbtMetaDataNodeTypeProp ContainersNTP = CurrentLocationNT.getNodeTypeProp( "Containers" );
                    if( null != ContainersNTP )
                    {
                        CswNbtViewId ChemContainersViewId = ContainersNTP.ViewId;
                        CswNbtView ChemContainersView = _CswNbtSchemaModTrnsctn.restoreView( ChemContainersViewId );
                        CswNbtViewProperty DisposedViewProp = ChemContainersView.findPropertyByName( "Disposed" );
                        foreach( CswNbtViewPropertyFilter CurrentFilter in DisposedViewProp.Filters )
                        {
                            CurrentFilter.ShowAtRuntime = true;
                        }

                        ChemContainersView.save();

                    }//if we ahve a containers NTP

            }//iterate location node types





        } // update()

    }//class CswUpdateSchema_02A_Case28706

}//namespace ChemSW.Nbt.Schema