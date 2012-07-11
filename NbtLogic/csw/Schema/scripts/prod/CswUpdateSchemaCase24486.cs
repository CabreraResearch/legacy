using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24486
    /// </summary>
    public class CswUpdateSchemaCase24486 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //Creates the preview layout for mol files
            CswNbtMetaDataNodeType chemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != chemicalNT )
            {
                CswNbtMetaDataNodeTypeProp structureNTP = chemicalNT.getNodeTypeProp( "Structure" );
                if( null != structureNTP )
                {
                    structureNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true );
                }
            }


        }//Update()

    }//class CswUpdateSchemaCase24486

}//namespace ChemSW.Nbt.Schema