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
    public class CswUpdateSchema_02M_CIS51775B : CswUpdateSchemaTo
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
            get { return "Fix hidden on DesignNodeTypeProp"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DesignNtpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass );

            foreach( CswNbtMetaDataNodeType DesignNtpNT in DesignNtpOC.getNodeTypes() )
            {
                // Set existing values of hidden
                foreach( CswNbtObjClassDesignNodeTypeProp DesignNtpNode in DesignNtpNT.getNodes( false, true ) )
                {
                    DesignNtpNode.Hidden.Checked = CswConvert.ToTristate( DesignNtpNode.RelationalNodeTypeProp.Hidden );
                    DesignNtpNode.postChanges( false );
                }

                // Fix layout
                CswNbtMetaDataNodeTypeProp HiddenNTP = DesignNtpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Hidden );
                CswNbtMetaDataNodeTypeProp ReadOnlyNTP = DesignNtpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.ReadOnly );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, HiddenNTP, ReadOnlyNTP, true );
            }

            // Synchronize property with nodetype_props.hidden
            CswTableUpdate jctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "51775_jctddntp_update", "jct_dd_ntp" );
            DataTable jctTable = jctUpdate.getEmptyTable();
            foreach( CswNbtMetaDataNodeType DesignNtpNT in DesignNtpOC.getNodeTypes() )
            {
                _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( "nodetype_props", "hidden" );
                CswNbtMetaDataNodeTypeProp HiddenNTP = DesignNtpNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDesignNodeTypeProp.PropertyName.Hidden );

                DataRow NodeTypeNameRow = jctTable.NewRow();
                NodeTypeNameRow["nodetypepropid"] = HiddenNTP.PropId;
                NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
                if( null != HiddenNTP.getFieldTypeRule().SubFields.Default )
                {
                    NodeTypeNameRow["subfieldname"] = HiddenNTP.getFieldTypeRule().SubFields.Default.Name;
                }
                jctTable.Rows.Add( NodeTypeNameRow );
            }
            jctUpdate.update( jctTable );


            // make Request module require the Multi-Inventory-Group module
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update modules 
                                                                          set prereq = (select moduleid from modules 
                                                                                         where name = '" + CswEnumNbtModuleName.MultiInventoryGroup + @"')
                                                                        where name = '" + CswEnumNbtModuleName.Requesting + @"'" );

        } // update()
    } // class
} // namespace