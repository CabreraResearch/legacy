using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28229
    /// </summary>
    public class CswUpdateSchema_01U_Case28229 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28229; }
        }

        public override void update()
        {
            // Fix legacy inspections to be readonly
            CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
            foreach(CswNbtObjClassInspectionDesign InspectionNode in InspectionOC.getNodes( false, true ))
            {
                if(InspectionNode.Status.Value == CswNbtObjClassInspectionDesign.InspectionStatus.Completed ||
                    InspectionNode.Status.Value == CswNbtObjClassInspectionDesign.InspectionStatus.CompletedLate )
                {
                    InspectionNode.Finish.setReadOnly( true, true );
                    InspectionNode.SetPreferred.setReadOnly( true, true );
                    InspectionNode.Cancel.setReadOnly( true, true );
                    InspectionNode.Node.setReadOnly( value: true, SaveToDb: true );

                    InspectionNode.postChanges( false );
                }
            }
        } // update()

    }//class CswUpdateSchema_01U_Case28229

}//namespace ChemSW.Nbt.Schema