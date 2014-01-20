using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02K_Case31517 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31517; }
        }

        public override string AppendToScriptName()
        {
            return "New columns for filter";
        }

        public override void update()
        {
            // Add new columns to nodetype_props for display filters
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "filtersubfield" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "filtersubfield", "Subfield for display condition filter", false, 40 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "filtermode" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "filtermode", "Mode for display condition filter", false, 20 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "filtervalue" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "filtervalue", "Value for display condition filter", false, 100 );
            }
            
            // Add new columns to object_class_props for display filters
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "filtersubfield" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_props", "filtersubfield", "Subfield for display condition filter", false, 40 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "filtermode" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_props", "filtermode", "Mode for display condition filter", false, 20 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "filtervalue" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_props", "filtervalue", "Value for display condition filter", false, 100 );
            }
        } // update()
    } // class CswUpdateDDL_02K_Case31517
} // namespace