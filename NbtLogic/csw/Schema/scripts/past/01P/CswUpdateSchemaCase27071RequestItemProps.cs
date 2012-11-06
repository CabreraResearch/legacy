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
    /// Schema Update for case 27071RequestItemProps
    /// </summary>
    public class CswUpdateSchemaCase27071RequestItemProps : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataObjectClassProp TypeOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, false );

        }//Update()

    }//class CswUpdateSchemaCase27071RequestItemProps

}//namespace ChemSW.Nbt.Schema