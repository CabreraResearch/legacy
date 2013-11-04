using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02I_Case31057: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31057; }
        }

        public override string ScriptName
        {
            get { return "02I_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Change data size for Nodes.NodeName"; }
        }

        public override void update()
        {
            if( 255 != _CswNbtSchemaModTrnsctn.CswDataDictionary.getDataTypeSize( "nodes", "nodename" ) )
            {
                _CswNbtSchemaModTrnsctn.changeColumnDataType( "nodes", "nodename", CswEnumDataDictionaryPortableDataType.String, 255 );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema