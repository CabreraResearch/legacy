using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26896
    /// </summary>
    public class CswUpdateSchemaCase26896 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //delete the material subclass nodetype
            CswNbtMetaDataNodeType materialSubclassNodetype = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Subclass" );
            if( null != materialSubclassNodetype )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( materialSubclassNodetype );
            }

        }//Update()

    }//class CswUpdateSchemaCase26896

}//namespace ChemSW.Nbt.Schema