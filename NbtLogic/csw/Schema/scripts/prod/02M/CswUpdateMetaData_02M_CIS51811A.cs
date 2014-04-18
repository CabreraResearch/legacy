using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS51811A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 51811; }
        }

        public override string Title
        {
            get { return "Create Request Item C3ProductId and C3CdbRegNo properties"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestItemOC )
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.C3ProductId,
                    FieldType =  CswEnumNbtFieldType.Number
                } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestItemOC )
            {
                PropName = CswNbtObjClassRequestItem.PropertyName.C3CdbRegNo,
                FieldType = CswEnumNbtFieldType.Number
            } );

        }
    }
}