using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case31798: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31798; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override string Title
        {
            get { return "Set Inspection Design Status DefaultValue to Pending"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp StatusOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StatusOCP, "Pending", CswEnumNbtSubFieldName.Value );
        } // update()

    }//class CswUpdateMetaData_02K_Case31798

}//namespace ChemSW.Nbt.Schema