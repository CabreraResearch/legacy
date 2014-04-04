using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS53135 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 53135; }
        }

        public override string Title
        {
            get { return "Script for " + CaseNo + ": Add composite property to Location"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            // Add a new composite property, Full Path, to the Location object class
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( LocationOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassLocation.PropertyName.FullPath,
                    FieldType = CswEnumNbtFieldType.Composite,
                } );
        }
    }
}