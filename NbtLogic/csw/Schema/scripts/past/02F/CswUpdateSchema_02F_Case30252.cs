using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30252 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.DH; }
        }

        public override string Title
        {
            get
            {
                return "Populate OraViewName and OraViewColNames for OCs and NTs.";
            }
        }
        public override int CaseNo
        {
            get { return 30252; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30252"; }
        }


        private void AddRow( DataTable dt, int ft, string propcolname, string subfieldname, string reportable, string is_default )
        {
            DataRow dr = dt.NewRow();
            dr["fieldtypeid"] = ft;
            dr["propcolname"] = propcolname;
            dr["subfieldname"] = subfieldname;
            dr["reportable"] = reportable;
            dr["is_default"] = is_default;
            dt.Rows.Add( dr );
        }

        public override void update()
        {

            CswTableUpdate ftsTbl = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "field_types_subfields_upd", "field_types_subfields" );
            DataTable ftsDataTbl = ftsTbl.getTable();

            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.CASNo ).FieldTypeId, "gestalt", "", "1", "1" );
            ftsTbl.update( ftsDataTbl );


            //iterate objectclasses and set their viewname
            CswTableUpdate UpdObClass = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "objclassUpd", "object_class" ); //for each objectclass
            CswCommaDelimitedString cols = new CswCommaDelimitedString();
            cols.Add( "objectclassid" );
            cols.Add( "objectclass" );
            cols.Add( "oraviewname" );
            DataTable tblUpdObClass = UpdObClass.getTable( cols );
            foreach( DataRow r in tblUpdObClass.Rows )
            {
                r["oraviewname"] = CswConvert.ToDbVal( CswTools.MakeOracleCompliantIdentifier( r["objectclass"].ToString() ) );
            }
            UpdObClass.update( tblUpdObClass );

            //objprops too
            CswTableUpdate UpdObProp = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "objclasspropUpd", "object_class_props" ); //for each objectclass
            CswCommaDelimitedString colsp = new CswCommaDelimitedString();
            colsp.Add( "objectclasspropid" );
            colsp.Add( "propname" );
            colsp.Add( "oraviewcolname" );
            DataTable tblUpdObProp = UpdObProp.getTable( colsp );
            foreach( DataRow r2 in tblUpdObProp.Rows )
            {
                r2["oraviewcolname"] = CswConvert.ToDbVal( CswTools.MakeOracleCompliantIdentifier( r2["propname"].ToString() ) );
            }
            UpdObProp.update( tblUpdObProp );

            // nodetypes like objectclasses
            CswTableUpdate UpdNT = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ntUpd", "nodetypes" ); //for each objectclass
            CswCommaDelimitedString ntcols = new CswCommaDelimitedString();
            ntcols.Add( "nodetypeid" );
            ntcols.Add( "nodetypename" );
            ntcols.Add( "oraviewname" );
            DataTable tblUpdNT = UpdNT.getTable( ntcols );
            foreach( DataRow r in tblUpdNT.Rows )
            {
                r["oraviewname"] = CswConvert.ToDbVal( CswTools.MakeOracleCompliantIdentifier( r["nodetypename"].ToString() ) );
            }
            UpdNT.update( tblUpdNT );

            //nodetypeprops, only the ones where oraviewcolname is null direct to table
            CswTableUpdate UpdNtProp = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ntpropUpd", "nodetype_props" ); //for each objectclass
            CswCommaDelimitedString ntcolsp = new CswCommaDelimitedString();
            ntcolsp.Add( "nodetypeid" );
            ntcolsp.Add( "nodetypepropid" );
            ntcolsp.Add( "objectclasspropid" );
            ntcolsp.Add( "propname" );
            ntcolsp.Add( "oraviewcolname" );
            //case 30627 -- do ntprivateprops after objectclasprops to get the important unique names first
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>() { new OrderByClause( "nodetypeid", CswEnumOrderByType.Ascending ), new OrderByClause( "objectclasspropid", CswEnumOrderByType.Ascending ) };
            DataTable tblUpdNtProp = UpdNtProp.getTable( ntcolsp, "", Int32.MinValue, "", false, OrderBy );

            StringCollection DefinedProps = new StringCollection();
            int mynodetypeid = Int32.MinValue;

            foreach( DataRow r2 in tblUpdNtProp.Rows )
            {
                if( mynodetypeid.ToString() != r2["nodetypeid"].ToString() )
                {
                    mynodetypeid = CswConvert.ToInt32( r2["nodetypeid"] );
                    DefinedProps.Clear();
                }


                DataRow arow = null;
                if( CswConvert.ToDbVal( r2["objectclasspropid"] ) != null )
                {
                    DataRow[] foundRows;
                    string criteria = "objectclasspropid=" + CswConvert.ToInt32( r2["objectclasspropid"] ).ToString();
                    foundRows = tblUpdObProp.Select( criteria );
                    if( foundRows.GetLength( 0 ) > 0 )
                    {
                        arow = foundRows[0];
                    }
                }
                if( arow != null )
                {
                    r2["oraviewcolname"] = arow["oraviewcolname"]; //use the objectclassprop oraviewcolname if we have it
                    DefinedProps.Add( r2["oraviewcolname"].ToString() );
                }
                else
                {
                    //questions do not use the propname...
                    CswNbtMetaDataNodeTypeProp ntp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( CswConvert.ToInt32( r2["nodetypepropid"] ) );
                    if( ntp.getFieldType().FieldType == CswEnumNbtFieldType.Question )
                    {
                        r2["oraviewcolname"] = CswConvert.ToDbVal( CswTools.MakeOracleCompliantIdentifier( ntp.FullQuestionNo.Replace( ".", "x" ) ) );
                        DefinedProps.Add( r2["oraviewcolname"].ToString() );
                    }
                    else
                    {
                        //duplicate check for case 30627
                        //DataRow[] foundRows;
                        string colname = CswTools.MakeOracleCompliantIdentifier( r2["propname"].ToString() );
                        int i = 0;
                        string testcolname = colname;
                        while( DefinedProps.IndexOf( testcolname ) > -1 )
                        {
                            ++i;
                            if( colname.Length > 29 - i.ToString().Length )
                            {
                                testcolname = colname.Substring( 0, 29 - i.ToString().Length );
                            }
                            testcolname = colname + i.ToString();
                        }
                        colname = testcolname;
                        r2["oraviewcolname"] = CswConvert.ToDbVal( colname );
                        DefinedProps.Add( r2["oraviewcolname"].ToString() );
                    }
                }

            }
            UpdNtProp.update( tblUpdNtProp );


        } // update()

    }

}//namespace ChemSW.Nbt.Schema