using System;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodeTypePropListOptions
    {
        public static char delimiter = ',';

        public CswNbtNodeTypePropListOptions( CswNbtResources CswNbtResources , Int32 NodeTypePropId )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = CswNbtResources.MetaData.getNodeTypeProp(NodeTypePropId);

            if (NodeTypeProp.IsFK && NodeTypeProp.FKType == "fkeydefid")
            {
                CswTableSelect FkeyDefsSelect = CswNbtResources.makeCswTableSelect( "fetch_fkey_def_sql", "fkey_definitions" );
                DataTable FkeyDefsTable = FkeyDefsSelect.getTable( "fkeydefid", NodeTypeProp.FKValue );
                string Sql = FkeyDefsTable.Rows[0]["sql"].ToString();

                CswArbitrarySelect ListOptsSelect = CswNbtResources.makeCswArbitrarySelect( "list_options_query", Sql );
                DataTable ListOptsTable = ListOptsSelect.getTable();
                
                int iOptionCnt = 0;
                _Options = new CswNbtNodeTypePropListOption[ListOptsTable.Rows.Count + 1];
                _Options[iOptionCnt] = new CswNbtNodeTypePropListOption("", "");
                iOptionCnt++;
                foreach (DataRow CurrentRow in ListOptsTable.Rows)
                {
                    _Options[iOptionCnt] = new CswNbtNodeTypePropListOption(CurrentRow[FkeyDefsTable.Rows[0]["ref_column"].ToString()].ToString(), 
                                                                            CurrentRow[FkeyDefsTable.Rows[0]["pk_column"].ToString()].ToString());
                    iOptionCnt++;
                }//iterate listopts rows
            }
            else
            {
                Override( NodeTypeProp.ListOptions );
            }
        }//ctor

        public void Override( string CommaDelimitedOptions )
        {
            char[] Delims = { delimiter };
            string[] Options = CommaDelimitedOptions.Split( Delims, StringSplitOptions.RemoveEmptyEntries );

            int iOptionCnt = 0;
            _Options = new CswNbtNodeTypePropListOption[Options.Length + 1];
            _Options[iOptionCnt] = new CswNbtNodeTypePropListOption( "", "" );
            iOptionCnt++;
            for( int i = 0; i < Options.Length; i++ )
            {
                _Options[iOptionCnt] = new CswNbtNodeTypePropListOption( Options[i].Trim(), Options[i].Trim() );
                iOptionCnt++;
            }
        }

        private CswNbtNodeTypePropListOption[] _Options;
        public CswNbtNodeTypePropListOption[] Options { get { return ( _Options ); } }


    }//CswNbtNodeTypePropListOptions

}//namespace ChemSW.Nbt.PropTypes
