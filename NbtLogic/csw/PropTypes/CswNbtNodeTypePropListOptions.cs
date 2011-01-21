using System;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;

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
                CswCommaDelimitedString Opts = new CswCommaDelimitedString();
                Opts.FromString( NodeTypeProp.ListOptions );
                Override( Opts );
            }
        }//ctor

        public void Override( CswCommaDelimitedString CommaDelimitedOptions )
        {
            int iOptionCnt = 0;
            _Options = new CswNbtNodeTypePropListOption[CommaDelimitedOptions.Count + 1];
            _Options[iOptionCnt] = new CswNbtNodeTypePropListOption( "", "" );
            iOptionCnt++;
            for( int i = 0; i < CommaDelimitedOptions.Count; i++ )
            {
                _Options[iOptionCnt] = new CswNbtNodeTypePropListOption( CommaDelimitedOptions[i].Trim(), CommaDelimitedOptions[i].Trim() );
                iOptionCnt++;
            }
        }

        private CswNbtNodeTypePropListOption[] _Options;
        public CswNbtNodeTypePropListOption[] Options { get { return ( _Options ); } }

        public override string ToString()
        {
            CswCommaDelimitedString ret = new CswCommaDelimitedString();
            foreach( CswNbtNodeTypePropListOption Option in Options )
            {
                ret.Add( Option.Text );
            }
            return ret.ToString();
        } // ToString()

    }//CswNbtNodeTypePropListOptions

}//namespace ChemSW.Nbt.PropTypes
