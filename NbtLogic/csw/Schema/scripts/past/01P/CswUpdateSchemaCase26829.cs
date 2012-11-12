using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26829
    /// </summary>
    public class CswUpdateSchemaCase26829 : CswUpdateSchemaTo
    {
        public override void update()
        {

            IEnumerable<CswNbtMetaDataNodeTypeProp> gridNTPs = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Grid );
            foreach( CswNbtMetaDataNodeTypeProp curGrid in gridNTPs )
            {
                if( curGrid.Extended.Equals( "Small" ) ) //'small' grids are thin grids
                {
                    curGrid.MaxValue = 3; //this translates to maxnumbervalue in the db. This is what's causing the current thin grids to show 10 and not 3
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase26829

}//namespace ChemSW.Nbt.Schema