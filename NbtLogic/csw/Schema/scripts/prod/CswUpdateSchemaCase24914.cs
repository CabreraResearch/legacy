using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version case24914
    /// </summary>
    public class CswUpdateSchemaToCase24914 : CswUpdateSchemaTo
    {

        public override void update()
        {

            CswNbtMetaDataObjectClass roleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            if( null != roleOC )
            {
                CswNbtMetaDataObjectClassProp timeoutProp = roleOC.getObjectClassProp( "Timeout" );
                if( null != timeoutProp )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( timeoutProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberminvalue, 5 );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( timeoutProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberprecision, 0 );
                }
            }

        }//Update()

    }//class CswUpdateSchemaTo01M13

}//namespace ChemSW.Nbt.Schema