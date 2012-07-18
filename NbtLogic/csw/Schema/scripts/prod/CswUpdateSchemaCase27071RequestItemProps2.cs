using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27071RequestItemProps2
    /// </summary>
    public class CswUpdateSchemaCase27071RequestItemProps2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp(new CswNbtWcfMetaDataModel.ObjectClassProp(RequestItemOc)
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.TotalDispensed,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    ServerManaged = true
                });

        }//Update()

    }//class CswUpdateSchemaCase27071RequestItemProps2

}//namespace ChemSW.Nbt.Schema