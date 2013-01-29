using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28611
    /// </summary>
    public class CswUpdateSchema_01V_Case28611 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28611; }
        }

        public override void update()
        {

            //The Default Chemicals Request menu button does not have values for the menu options or the selected option
            CswNbtMetaDataNodeType chemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != chemicalNT )
            {
                foreach( CswNbtObjClassMaterial chemicalNode in chemicalNT.getNodes( false, false, false, true ) )
                {
                    if( string.IsNullOrEmpty( chemicalNode.Request.State ) || string.IsNullOrEmpty( chemicalNode.Request.MenuOptions ) )
                    {
                        chemicalNode.Request.State = CswNbtObjClassMaterial.Requests.Size;
                        chemicalNode.Request.MenuOptions = CswNbtObjClassMaterial.Requests.Options.ToString();
                        chemicalNode.postChanges( false );
                    }
                }
            }


        } //Update()

    }//class CswUpdateSchema_01V_Case28611

}//namespace ChemSW.Nbt.Schema