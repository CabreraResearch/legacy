using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27904
    /// </summary>
    public class CswUpdateSchemaCase27904 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataNodeType regListNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Regulatory List" );
            if( null != regListNT )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, regListNT.NodeTypeId );
            }

        }//Update()

    }//class CswUpdateSchemaCase27904

}//namespace ChemSW.Nbt.Schema