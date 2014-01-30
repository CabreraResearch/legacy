using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateSchema_02K_Case31329: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31329; }
        }

        public override string Title
        {
            get { return "Property sequences have no cache"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DesignSequenceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );

            foreach( CswNbtObjClassDesignSequence Sequence in DesignSequenceOC.getNodes( false, true, false, true ) )
            {
                string DbName = Sequence.getDbName();
                string CurrentValStr = Sequence.getCurrent();
                int CurrentVal = Sequence.deformatSequence( CurrentValStr );
                _CswNbtSchemaModTrnsctn.ResetSequenceForProperty( DbName, CurrentVal );
            }
        }

    }//class CswUpdateMetaData_02K_Case31517B

}//namespace ChemSW.Nbt.Schema