using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27939
    /// </summary>
    public class CswUpdateSchemaCase27939 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtObjClassUser UserNode in UserOC.getNodes( false, true, IncludeDefaultFilters: false ) )
            {
                if( UserNode.Archived.Checked == Tristate.Null )
                {
                    UserNode.Archived.Checked = Tristate.False;
                    UserNode.postChanges( false );
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase27939

}//namespace ChemSW.Nbt.Schema