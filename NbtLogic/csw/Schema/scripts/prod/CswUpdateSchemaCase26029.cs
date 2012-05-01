using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26029
    /// </summary>
    public class CswUpdateSchemaCase26029 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // This is also done in RunBeforeEveryExecutionOfUpdater_01, but we need to populate the initial values here:
            _CswNbtSchemaModTrnsctn.MetaData.ResetEnabledNodeTypes();


        }//Update()

    }//class CswUpdateSchemaCase26029

}//namespace ChemSW.Nbt.Schema