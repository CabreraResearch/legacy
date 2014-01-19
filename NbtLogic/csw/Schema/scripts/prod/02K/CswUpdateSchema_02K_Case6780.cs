using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case6780 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 6780; }
        }

        public override void update()
        {
            // remove the 'seq_' sequence
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "drop sequence seq_" );

        } // update()
    } // class CswUpdateSchema_02K_Case6780
} // namespace