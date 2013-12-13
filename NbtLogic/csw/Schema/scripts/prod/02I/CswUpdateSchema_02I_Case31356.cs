using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31356 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31356; }
        }

        public override string Title
        {
            get { return "Fix Fire Class Set Add Layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass FireClassSetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountSetClass );
            foreach( CswNbtMetaDataNodeType FireClassSetNT in FireClassSetOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp SetNameNTP = FireClassSetNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmountSet.PropertyName.SetName );
                SetNameNTP.updateLayout( CswEnumNbtLayoutType.Add, true, DisplayRow: 1, DisplayColumn: 1 );
            }
        } // update()
    }

}//namespace ChemSW.Nbt.Schema