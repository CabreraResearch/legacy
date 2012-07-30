using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27423
    /// </summary>
    public class CswUpdateSchemaCase27423Part1 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass WorkUnitOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( WorkUnitOc )
                {
                    PropName = CswNbtObjClassWorkUnit.NamePropertyName,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );

        }//Update()

    }//class CswUpdateSchemaCase27423

}//namespace ChemSW.Nbt.Schema