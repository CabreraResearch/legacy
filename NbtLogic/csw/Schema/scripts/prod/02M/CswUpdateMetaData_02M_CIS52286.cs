using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52286 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52286; }
        }

        public override string Title
        {
            get { return "New Enterprise Part OCPs"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass EPOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EnterprisePartClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( EPOC, new CswNbtWcfMetaDataModel.ObjectClassProp( EPOC )
                {
                    PropName = CswNbtObjClassEnterprisePart.PropertyName.Description,
                    FieldType = CswEnumNbtFieldType.Memo,
                    SetValOnAdd = true
                } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( EPOC, new CswNbtWcfMetaDataModel.ObjectClassProp( EPOC )
                {
                    PropName = CswNbtObjClassEnterprisePart.PropertyName.Version,
                    FieldType = CswEnumNbtFieldType.Text,
                    ReadOnly = true
                } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( EPOC, new CswNbtWcfMetaDataModel.ObjectClassProp( EPOC )
                {
                    PropName = CswNbtObjClassEnterprisePart.PropertyName.EPNoLookup,
                    FieldType = CswEnumNbtFieldType.Button
                } );
        }
    }
}