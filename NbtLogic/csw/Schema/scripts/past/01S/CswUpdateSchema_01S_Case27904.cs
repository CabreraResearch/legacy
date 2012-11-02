using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27904
    /// </summary>
    public class CswUpdateSchema_01S_Case27904 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataNodeType regListNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Regulatory List" );
            if( null != regListNT )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, regListNT.NodeTypeId );
            }

        }//Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27904; }
        }


    }//class CswUpdateSchema_01S_Case27904

}//namespace ChemSW.Nbt.Schema