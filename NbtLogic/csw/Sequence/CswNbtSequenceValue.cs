using System;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt
{

    public class CswNbtSequenceValue
    {

        private CswNbtResources _CswNbtResources = null;
        private Int32 _NodeTypePropId = Int32.MinValue;
        private Int32 _SequenceId = Int32.MinValue;
        private DataRow _CurrentSequenceRow = null;

        public CswNbtSequenceValue( CswNbtResources CswNbtResources, Int32 SequenceId )
        {
            _CswNbtResources = CswNbtResources;

            _SequenceId = SequenceId;
            CswTableSelect SequencesSelect = _CswNbtResources.makeCswTableSelect( "SequencesSelect", "sequences" );
            _CurrentSequenceRow = SequencesSelect.getTable( "sequenceid", _SequenceId, true ).Rows[0];
        }


        public CswNbtSequenceValue( Int32 NodeTypePropId, CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            _NodeTypePropId = NodeTypePropId;

            string SelectText = @"select s.*,p.propname,p.sequenceid,t.nodetypename 
                                        from nodetype_props p 
                                        left outer join sequences s on ( p.sequenceid = s.sequenceid ) 
                                        left outer join nodetypes t on (p.nodetypeid=t.nodetypeid) 
                                       where p.nodetypepropid = " + _NodeTypePropId.ToString();

            CswArbitrarySelect SequencesSelect = _CswNbtResources.makeCswArbitrarySelect( "Sequence_nodetypeprop_select", SelectText );
            DataTable SequencesTable = SequencesSelect.getTable();

            if( SequencesTable.Rows.Count > 0 )
            {
                DataRow SequenceRow = SequencesTable.Rows[0];

                // BZ 5163
                //if( SequenceRow.IsNull( "sequenceid" )  )
                //{
                //    string NodeTypeName = SequenceRow[ "nodetypename" ].ToString();
                //    string PropName = SequenceRow[ "propname" ].ToString();

                //    throw ( new CswDniException( "The " + PropName + " property of node type " + NodeTypeName + " needs to have a sequence assigned", 
                //                                 "The query for SEQUENCES from nodetypepropid " + _NodeTypePropId.ToString() + " did not find a sequenceid" ) );
                //}


                //if( SequenceRow.IsNull( "sequencename" ) )
                //{
                //    throw ( new CswDniException( "A data error occurred", "There is no sequence name for sequenceid: " + SequencesTable.Rows[ 0 ]["sequenceid"].ToString()  ) );
                //}            

                _CurrentSequenceRow = SequenceRow;
            }
        }

        private string _Prep
        {
            get
            {
                string ret = "";
                if( !_CurrentSequenceRow.IsNull( "prep" ) )
                    ret = _CurrentSequenceRow["prep"].ToString();
                return ret;
            }
        }

        private string _Post
        {
            get
            {
                string ret = "";
                if( !_CurrentSequenceRow.IsNull( "Post" ) )
                    ret = _CurrentSequenceRow["Post"].ToString();
                return ret;
            }
        }

        private Int32 _Pad
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( !_CurrentSequenceRow.IsNull( "Pad" ) )
                    ret = CswConvert.ToInt32(_CurrentSequenceRow["Pad"].ToString());
                return ret;
            }
        }

        public string formatSequence( Int32 RawSequenceVal )
        {
            string ReturnVal = "";
            ReturnVal = RawSequenceVal.ToString();

            if( _Pad > 0 )
            {
                while( ReturnVal.Length < _Pad )
                    ReturnVal = "0" + ReturnVal;
            }
            ReturnVal = _Prep + ReturnVal + _Post;

            return ( ReturnVal );
        }

        public Int32 deformatSequence( string FormattedSequenceVal )
        {
            Int32 ret = Int32.MinValue;
            if( FormattedSequenceVal.Length > ( _Prep.Length + _Post.Length ) )
            {
                if( FormattedSequenceVal.Substring( 0, _Prep.Length ) == _Prep &&
                    FormattedSequenceVal.Substring( FormattedSequenceVal.Length - _Post.Length, _Post.Length ) == _Post )
                {
                    string RawSequenceVal = FormattedSequenceVal.Substring( _Prep.Length, ( FormattedSequenceVal.Length - _Prep.Length - _Post.Length ) );
                    if( CswTools.IsInteger( RawSequenceVal ) )
                        ret = CswConvert.ToInt32( RawSequenceVal );
                }
            }
            return ret;
        }

        public string makeExample( Int32 ExampleValue )
        {
            if( null == _CurrentSequenceRow )
				throw ( new CswDniException( ErrorType.Error, "Internal error", "There is no current row; you must set the NodeTypePropId or SequenceId property" ) );

            return ( formatSequence( ExampleValue ) );
        }

        public string Next
        {
            get
            {
                // BZ 5163
                //if( null == _CurrentSequenceRow ) 
                //    throw( new CswDniException( "Internal error", "There is no current row; you must set the NodeTypePropId or SequenceId property" ) );

                string ReturnVal = "";

                if( null != _CurrentSequenceRow )
                {
                    CswSequenceName SequenceName = new CswSequenceName( _CurrentSequenceRow["sequencename"].ToString() );

                    if( !_CswNbtResources.doesUniqueSequenceExist( SequenceName.DBName ) )
                        _CswNbtResources.makeUniqueSequence( SequenceName.DBName, 1 );

                    Int32 RawSequenceVal = _CswNbtResources.getNextUniqueSequenceVal( SequenceName.DBName );

                    ReturnVal = formatSequence( RawSequenceVal );
                }
                return ReturnVal;
            }

        }//Next

        public string Current
        {
            get
            {
                string ReturnVal = "";

                if( null != _CurrentSequenceRow )
                {
                    CswSequenceName SequenceName = new CswSequenceName( _CurrentSequenceRow["sequencename"].ToString() );

                    if( !_CswNbtResources.doesUniqueSequenceExist( SequenceName.DBName ) )
                        _CswNbtResources.makeUniqueSequence( SequenceName.DBName, 1 );

                    Int32 RawSequenceVal = _CswNbtResources.getCurrentUniqueSequenceVal( SequenceName.DBName );

                    ReturnVal = formatSequence( RawSequenceVal );
                }
                return ReturnVal;
            }
        }

        /// <summary>
        /// Resets next sequence value based on maximum existing value in the database.
        /// </summary>
        public void reSync()
        {
            reSync( Int32.MinValue );
        }

        /// <summary>
        /// Resets next sequence value based on newest entry and existing values in the database.
        /// </summary>
        public void reSync( Int32 NewSeqVal )
        {
            string SelectCols = string.Empty;
            SelectCols += "j." + CswNbtFieldTypeRuleBarCode.SequenceNumberColumn.ToString();
            if( CswNbtFieldTypeRuleBarCode.SequenceNumberColumn != CswNbtFieldTypeRuleSequence.SequenceNumberColumn )
            {
                SelectCols += ", j." + CswNbtFieldTypeRuleSequence.SequenceNumberColumn.ToString();
            }

            string SelectText = @"select " + SelectCols + @"
                                    from sequences s
                                    join nodetype_props p on ( p.sequenceid = s.sequenceid ) 
                                    join jct_nodes_props j on ( p.nodetypepropid = j.nodetypepropid and j.nodeid is not null )
                                   where s.sequenceid = " + _CurrentSequenceRow["sequenceid"].ToString();

            CswArbitrarySelect SeqValueSelect = _CswNbtResources.makeCswArbitrarySelect( "syncSequence_maxvalue_select", SelectText );
            DataTable SeqValueTable = SeqValueSelect.getTable();

            Int32 MaxSeqVal = NewSeqVal;
            foreach( DataRow SeqValueRow in SeqValueTable.Rows )
            {
                Int32 ThisSeqVal = CswConvert.ToInt32( SeqValueRow[CswNbtFieldTypeRuleBarCode.SequenceNumberColumn.ToString()] );
                if( ThisSeqVal == Int32.MinValue )
                    ThisSeqVal = CswConvert.ToInt32( SeqValueRow[CswNbtFieldTypeRuleSequence.SequenceNumberColumn.ToString()] );
                if( ThisSeqVal > MaxSeqVal )
                    MaxSeqVal = ThisSeqVal;
            } // foreach( DataRow SeqValueRow in SeqValueTable.Rows )

            CswSequenceName SequenceName = new CswSequenceName( _CurrentSequenceRow["sequencename"].ToString() );
            _CswNbtResources.resetUniqueSequenceVal( SequenceName.DBName, MaxSeqVal + 1 );

        } // Resync()


    }//CswNbtSequenceValue




}//namespace ChemSW.Nbt

