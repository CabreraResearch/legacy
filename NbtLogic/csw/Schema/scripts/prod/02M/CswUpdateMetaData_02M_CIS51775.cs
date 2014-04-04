using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS51775 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 51755; }
        }

        public override string Title
        {
            get { return "Add hidden to DesignNodeTypeProp"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            // Add new object_class_prop 'hidden' to DesignNodeTypeProp
            CswNbtMetaDataObjectClass DesignNtpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( DesignNtpOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.Hidden,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true
                } );

        } // update()
    } // class
} // namespace