using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30914B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30914; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "B"; }
        }

        public override string Title
        {
            get { return "Add Report Link properties"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ControlZoneOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ControlZoneOC )
                {
                    FieldType = CswEnumNbtFieldType.ReportLink,
                    PropName = CswNbtObjClassControlZone.PropertyName.HMISReport
                } );
        } // update()

    } // class CswUpdateMetaData_02H_Case30914

}//namespace ChemSW.Nbt.Schema