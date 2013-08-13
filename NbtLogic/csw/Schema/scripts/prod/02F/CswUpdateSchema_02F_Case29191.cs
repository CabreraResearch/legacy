using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for Case 29191
    /// </summary>
    public class CswUpdateSchema_02F_Case29191 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29191; }
        }

        public override string ScriptName
        {
            get { return "02F_Case29191"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        public override void update()
        {
            // We need to correct the spelling of 'baled' to 'bailed' on one specific FireClassExemptAmount Node
            CswNbtMetaDataObjectClass FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountClass );
            foreach( ObjClasses.CswNbtNode Node in FireClassExemptAmountOC.getNodes( false, true, true, true ) )
            {
                CswNbtObjClassFireClassExemptAmount CurrentFCEANode = (CswNbtObjClassFireClassExemptAmount) Node;
                if( CurrentFCEANode.NodeName == "Combustible Fiber/Dust (baled)" )
                {
                    CurrentFCEANode.Class.Text = "(bailed)";
                    CurrentFCEANode.HazardClass.Value = "CF/D (bailed)";
                    CurrentFCEANode.HazardClass.SyncGestalt();
                    CurrentFCEANode.postChanges( true );
                    break;
                }
            }

        } // update()

    }//CswUpdateSchema_02F_Case29191

}//namespace ChemSW.Nbt.Schema