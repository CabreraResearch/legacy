﻿using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02G_Case30564 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Remove LocationContents field type"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30564; }
        }

        public override string AppendToScriptName()
        {
            return "LocationContents";
        }

        public override void update()
        {
            // Farewell LocationContents, I hardly knew ye

            // delete box's LocationContents property and Contents tab
            CswNbtMetaDataNodeType BoxNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Box" );
            if( null != BoxNT )
            {
                CswNbtMetaDataNodeTypeProp BoxLocationContentsNTP = BoxNT.getNodeTypeProp( "LocationContents" );
                if( null != BoxLocationContentsNTP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( BoxLocationContentsNTP );
                }

                CswNbtMetaDataNodeTypeTab BoxContentsNTT = BoxNT.getNodeTypeTab( "Contents" );
                if( null != BoxContentsNTT )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( BoxContentsNTT );
                }
            }

            // field_types_subfields
            {
                CswTableUpdate FieldTypeSubFieldUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "30564_ft_update", "field_types_subfields" );
                DataTable Table = FieldTypeSubFieldUpdate.getTable( "where fieldtypeid = (select fieldtypeid from field_types where fieldtype= 'LocationContents')" );
                foreach( DataRow Row in Table.Rows )
                {
                    Row.Delete();
                }
                FieldTypeSubFieldUpdate.update( Table );
            }

            // field_types
            {
                CswTableUpdate FieldTypeUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "30564_ft_update", "field_types" );
                DataTable Table = FieldTypeUpdate.getTable( "where fieldtype= 'LocationContents'" );
                foreach( DataRow Row in Table.Rows )
                {
                    Row.Delete();
                }
                FieldTypeUpdate.update( Table );
            }
        }
    } // class CswUpdateMetaData_02G_Case30564
}