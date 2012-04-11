using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Security;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25838
    /// </summary>
    public class CswUpdateSchemaCase25838 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Task "Done On" is no longer servermanaged = 1, to allow overwriting the automatic value
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );
            CswNbtMetaDataObjectClassProp TaskDoneOnOCP = TaskOC.getObjectClassProp( CswNbtObjClassTask.DoneOnPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TaskDoneOnOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );

        }//Update()

    }//class CswUpdateSchemaCase25838

}//namespace ChemSW.Nbt.Schema