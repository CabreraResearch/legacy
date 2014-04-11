using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52285 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52285; }
        }

        public override string AppendToScriptName()
        {
            return "InternalProperty";
        }

        public override void update()
        {
            // Add a new property to the Vendor object class, "Internal", fieldtype=Logical, with servermanaged = true.
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    PropName = CswNbtObjClassVendor.PropertyName.Internal,
                    FieldType = CswEnumNbtFieldType.Logical,
                    ServerManaged = true
                } );

        } // update()

    } // class CswUpdateSchema_02L_Case52285

}//namespace ChemSW.Nbt.Schema