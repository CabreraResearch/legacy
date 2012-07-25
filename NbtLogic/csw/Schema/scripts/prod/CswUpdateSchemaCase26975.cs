using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26975
    /// </summary>
    public class CswUpdateSchemaCase26975 : CswUpdateSchemaTo
    {
        public override void update()
        {
            foreach( CswNbtMetaDataNodeTypeProp GridProp in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Grid ) )
            {
                if( GridProp.PropName.ToLower().EndsWith( "grid" ) )
                {
                    GridProp.PropName = GridProp.PropName.Substring( 0, ( GridProp.PropName.Length - "grid".Length ) ).Trim();

                    CswNbtView GridView = _CswNbtSchemaModTrnsctn.restoreView( GridProp.ViewId );
                    GridView.ViewName = GridProp.PropName;
                    GridView.save();
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase26975

}//namespace ChemSW.Nbt.Schema