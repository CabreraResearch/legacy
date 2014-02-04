using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31329B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31329; }
        }

        public override string Title
        {
            get { return "Remove default values for barcodes, do not let users set default values for Barcodes"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DesignPropOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );
            foreach( CswNbtObjClassDesignNodeTypeProp DesignProp in DesignPropOC.getNodes( false, true, false, true )
                .Where( P => ( ( CswNbtObjClassDesignNodeTypeProp ) P).FieldTypeValue == CswEnumNbtFieldType.Barcode ) )
            {
                //Remove the default value property from all layouts and hide it - users cant and shouldn't set default values for barcodes
                DesignProp.AttributeProperty[CswEnumNbtPropertyAttributeName.DefaultValue].NodeTypeProp.removeFromAllLayouts();
                DesignProp.AttributeProperty[CswEnumNbtPropertyAttributeName.DefaultValue].NodeTypeProp.Hidden = true;
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema