using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_Case52300B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52300; }
        }

        public override string Title
        {
            get { return "MLM2: CIS-52300: Create Method NT"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MethodOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass );

            _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( MethodOC )
                {
                    Category = "MLM",
                    ObjectClass = MethodOC,
                    Searchable = true
                } );
        }

        // update()
    }
}

//namespace ChemSW.Nbt.Schema