using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.ImportExport;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31091_Chemicals_Props: CswUpdateSchemaTo
    {
        public override string Title { get { return "Set up CAF Chemical NTPs and bindings "; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31091; }
        }

        public override string AppendToScriptName()
        {
            return "Chemicals_Props";
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );

            string sql = CswNbtImportTools.GetCAFPropertiesSQL( "properties_values" );
            CswArbitrarySelect cafChemPropAS = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "cafChemProps", sql );
            DataTable cafChemPropsDT = cafChemPropAS.getTable();

            foreach( DataRow row in cafChemPropsDT.Rows )
            {
                foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
                {
                    string PropName = row["propertyname"].ToString();
                    PropName = CswNbtImportTools.GetUniquePropName( ChemicalNT, PropName ); //keep appending numbers until we have a unique prop name

                    CswEnumNbtFieldType propFT = CswNbtImportTools.GetFieldTypeFromCAFPropTypeCode( row["propertytype"].ToString() );

                    CswNbtMetaDataNodeTypeProp newProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNT, propFT, PropName, Int32.MinValue );
                    newProp.IsRequired = CswConvert.ToBoolean( row["required"] );
                    newProp.ReadOnly = CswConvert.ToBoolean( row["readonly"] );
                    newProp.ListOptions = row["listopts"].ToString();
                    newProp.removeFromAllLayouts();

                    string cafColPropName = "prop" + row["propertyid"];
                    string cafSourceCol = "propvaltext";
                    if( CswEnumNbtFieldType.DateTime == propFT )
                    {
                        cafSourceCol = "propvaldate";
                    }
                    else if( CswEnumNbtFieldType.Number == propFT )
                    {
                        cafSourceCol = "propvalnumber";
                    }
                    ImpMgr.importBinding( cafSourceCol, PropName, "", "CAF", ChemicalNT.NodeTypeName, ClobTableName : "properties_values", LobDataPkColOverride : cafColPropName, LobDataPkColName: "propertiesvaluesid" );
                }
            }

            ImpMgr.finalize();

        }

    }
}