using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28196
    /// </summary>
    public class CswUpdateSchema_01U_Case28196 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28196; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerGroupClass );

            CswNbtMetaDataNodeType ContainerGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Group" );
            if( ContainerGroupNT != null )
            {
                ContainerGroupNT.Category = "MLM";
            }

        }//Update()

    }//class CswUpdateSchema_01U_Case28196

}//namespace ChemSW.Nbt.Schema