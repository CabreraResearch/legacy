using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31090C: CswUpdateNbtMasterSchemaTo
    {
        public override string Title { get { return "Add Regulatory List Id to Regulatory List CAS binding"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31090; }
        }

        public override void doUpdate()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            //RegList CAS No
            ImpMgr.importBinding( "regulatorylistid", CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList,  CswEnumNbtSubFieldName.NodeID.ToString(), DestNodeTypeName : "Regulatory List CAS" );

            //RegList
            ImpMgr.importBinding( "reglistcode", CswNbtObjClassRegulatoryList.PropertyName.ListCode, "", DestNodeTypeName : "Regulatory List" );

            ImpMgr.finalize();

        }

    }
}