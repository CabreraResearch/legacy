using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28146
    /// </summary>
    public class CswUpdateSchema_01T_Case28146 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28146; }
        }

        public override void update()
        {
            // Reset warning days
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GeneratorClass );
            
            foreach( CswNbtObjClassGenerator GeneratorNode in GeneratorOC.getNodes( forceReInit: false,
                                                                                    includeSystemNodes: true, 
                                                                                    IncludeDefaultFilters: false, 
                                                                                    IncludeHiddenNodes: true ) )
            {
                Int32 max = GeneratorNode.DueDateInterval.getMaximumWarningDays();
                if(GeneratorNode.WarningDays.Value > max)
                {
                    GeneratorNode.WarningDays.Value = max;
                }
                GeneratorNode.postChanges( false );
            } // foreach
        } // update()

    }//class CswUpdateSchema_01T_Case28146

}//namespace ChemSW.Nbt.Schema