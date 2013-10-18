using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02H_Case30279 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30279; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Favorites table"; }
        }

        public override void update()
        {
            string FavoritesTableName = "favorites";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( FavoritesTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( FavoritesTableName, "favoriteid" );
                _CswNbtSchemaModTrnsctn.addLongColumn( FavoritesTableName, "userid", "nodeid of the user who favorited this item", false, true );
                _CswNbtSchemaModTrnsctn.addLongColumn( FavoritesTableName, "itemid", "nodeid of the favorited item", false, true );
            }
        } // update()
    }

}//namespace ChemSW.Nbt.Schema