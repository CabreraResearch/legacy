using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28020
    /// </summary>
    public class CswUpdateSchema_01T_Case28020 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28020; }
        }

        public override void update()
        {
            //Change Material Documents File Type default value from an empty string to "File"
            CswNbtMetaDataNodeType materialDocNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
            if( null != materialDocNT )
            {
                CswNbtMetaDataNodeTypeProp fileTypeNTP = materialDocNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.FileType );
                fileTypeNTP.DefaultValue.AsList.Value = "File";
            }

        }

        //Update()

    }//class CswUpdateSchemaCase28020

}//namespace ChemSW.Nbt.Schema