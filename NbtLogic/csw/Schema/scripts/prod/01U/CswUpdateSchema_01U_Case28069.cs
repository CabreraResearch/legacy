using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28069
    /// </summary>
    public class CswUpdateSchema_01U_Case28069 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
            // Set creation date for all existing tasks and inspection designs to be 1/1/2000
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.TaskClass );
            foreach( CswNbtPropertySetGeneratorTarget TaskNode in TaskOC.getNodes( false, true, false, true ) )
            {
                if( DateTime.MinValue == TaskNode.CreatedDate.DateTimeValue )
                {
                    TaskNode.CreatedDate.DateTimeValue = new DateTime( 2000, 1, 1 );
                    TaskNode.postChanges( false );
                }
            }

            CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
            foreach( CswNbtPropertySetGeneratorTarget InspNode in InspectionOC.getNodes( false, true, false, true ) )
            {
                if( DateTime.MinValue == InspNode.CreatedDate.DateTimeValue )
                {
                    InspNode.CreatedDate.DateTimeValue = new DateTime( 2000, 1, 1 );
                    InspNode.postChanges( false );
                }
            }
        } // Update()

    }//class CswUpdateSchema_01U_Case28069

}//namespace ChemSW.Nbt.Schema