using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30022
    /// </summary>
    public class CswUpdateSchema_02C_Case30022 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30022; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GeneratorClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GeneratorClass );
            foreach( CswNbtObjClassGenerator GeneratorNode in GeneratorClass.getNodes( false, false ) )
            {
                DateTime LastDueDate = GeneratorNode.DueDateInterval.getLastOccuranceBefore( GeneratorNode.NextDueDate.DateTimeValue );
                if( LastDueDate > DateTime.Today && CswEnumRateIntervalType.Hourly != GeneratorNode.DueDateInterval.RateInterval.RateType )
                {
                    GeneratorNode.NextDueDate.DateTimeValue = DateTime.MinValue;
                    GeneratorNode.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
                    GeneratorNode.postChanges( true );
                }
            }
        } // update()

    }//class CswUpdateSchema_02C_Case30022

}//namespace ChemSW.Nbt.Schema