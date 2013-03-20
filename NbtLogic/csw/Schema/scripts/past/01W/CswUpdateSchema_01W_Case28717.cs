using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28717
    /// </summary>
    public class CswUpdateSchema_01W_Case28717 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28717; }
        }

        public override void update()
        {
            // Set user language options
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LanguageNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Language );
                LanguageNTP.IsRequired = true;
                LanguageNTP.ListOptions = "en,fr,es,de";

                // Set value for all current users
                foreach( CswNbtObjClassUser User in UserNT.getNodes( false, true ) )
                {
                    User.LanguageProperty.Value = "en";
                    User.postChanges( false );
                }
            }
        } //update()

    }//class CswUpdateSchema_01V_Case28717

}//namespace ChemSW.Nbt.Schema