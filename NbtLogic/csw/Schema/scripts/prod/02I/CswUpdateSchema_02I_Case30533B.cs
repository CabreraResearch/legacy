using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case30533B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30533; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override string Title
        {
            get { return "Organize Request Item NodeType Layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            foreach( CswNbtMetaDataNodeType RequestItemNT in RequestItemOC.getNodeTypes() )
            {
                //TODO - organize add/edit/preview/table layouts
                //TODO - also add MLM module logic to module class
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema