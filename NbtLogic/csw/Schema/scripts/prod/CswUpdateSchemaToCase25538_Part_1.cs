using System;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25538
    /// </summary>
    public class CswUpdateSchemaToCase25538_Part_1 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataNodeType ComponentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Component" );
            if( ComponentNT != null )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( ComponentNT );
            }
            _CswNbtSchemaModTrnsctn.deleteView( "Components", true );

        }//Update()

    }//class CswUpdateSchemaToCase25538_Part_1

}//namespace ChemSW.Nbt.Schema