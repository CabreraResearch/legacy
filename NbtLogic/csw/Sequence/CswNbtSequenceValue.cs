using System;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt
{

    public class CswNbtSequenceValue
    {

        private CswNbtResources _CswNbtResources = null;
//        private Int32 _NodeTypePropId = 0;
        public CswNbtSequenceValue( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

        }//generic



        private DataRow _CurrentSequenceRow = null;
        
        
        private Int32 _NodeTypePropId = Int32.MinValue;
        public Int32 NodeTypePropId
        {
            set
            {
                _NodeTypePropId = value;

                string SelectText = @"select s.*,p.propname,p.sequenceid,t.nodetypename 
                                        from nodetype_props p 
                                        left outer join sequences s on ( p.sequenceid = s.sequenceid ) 
                                        left outer join nodetypes t on (p.nodetypeid=t.nodetypeid) 
                                       where p.nodetypepropid = " + _NodeTypePropId.ToString();

                CswArbitrarySelect SequencesSelect = _CswNbtResources.makeCswArbitrarySelect( "Sequence_nodetypeprop_select", SelectText );
                DataTable SequencesTable = SequencesSelect.getTable();

                if (SequencesTable.Rows.Count > 0)
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
            }//set

        }//NodeTypePropId

        private Int32 _SequenceId = Int32.MinValue;
        public Int32 SequenceId
        {
            set
            {
                _SequenceId = value;
                CswTableSelect SequencesSelect = _CswNbtResources.makeCswTableSelect( "SequencesSelect", "sequences" );
                _CurrentSequenceRow = SequencesSelect.getTable( "sequenceid", _SequenceId, true ).Rows[0];
            }
        }//SequenceId


        private string _Prep
        {
            get
            {
                string ret = "";
                if (!_CurrentSequenceRow.IsNull("prep"))
                    ret = _CurrentSequenceRow["prep"].ToString();
                return ret;
            }
        }

        private string _Post
        {
            get
            {
                string ret = "";
                if (!_CurrentSequenceRow.IsNull("Post"))
                    ret = _CurrentSequenceRow["Post"].ToString();
                return ret;
            }
        }

        private Int32 _Pad
        {
            get
            {
                Int32 ret= Int32.MinValue;
                if (!_CurrentSequenceRow.IsNull("Pad"))
                    ret = Convert.ToInt32(_CurrentSequenceRow["Pad"].ToString());
                return ret;
            }
        }

        private string _formatSequence( string RawSequenceVal )
        {
            string ReturnVal = "";
            
            if(_Pad > 0)
            {
                while( RawSequenceVal.Length < _Pad )
                    RawSequenceVal = "0" + RawSequenceVal;
            }
            ReturnVal = _Prep + RawSequenceVal + _Post;

            return( ReturnVal );
        }

        private Int32 _deformatSequence(string FormattedSequenceVal)
        {
            string RawSequenceVal = FormattedSequenceVal.Substring(_Prep.Length, (FormattedSequenceVal.Length - _Prep.Length - _Post.Length));
            if (!CswTools.IsInteger(RawSequenceVal))
                throw new CswDniException("Invalid sequence value", "CswNbtSequenceValue got an invalid sequence value: " + FormattedSequenceVal);
            return Convert.ToInt32(RawSequenceVal);
        }

        public string makeExample( Int32 ExampleValue )
        {
            if( null == _CurrentSequenceRow )
                throw ( new CswDniException( "Internal error", "There is no current row; you must set the NodeTypePropId or SequenceId property" ) );

            return( _formatSequence( ExampleValue.ToString() ) );
        }
        
        public string Next
        {
            get
            {
                // BZ 5163
                //if( null == _CurrentSequenceRow ) 
                //    throw( new CswDniException( "Internal error", "There is no current row; you must set the NodeTypePropId or SequenceId property" ) );

                string ReturnVal = "";

                if (null != _CurrentSequenceRow)
                {
                    CswSequenceName SequenceName = new CswSequenceName( _CurrentSequenceRow["sequencename"].ToString() );

                    if (!_CswNbtResources.doesUniqueSequenceExist(SequenceName.DBName))
                        _CswNbtResources.makeUniqueSequence(SequenceName.DBName, 1);

                    string RawSequenceVal = _CswNbtResources.getNextUniqueSequenceVal(SequenceName.DBName).ToString();

                    ReturnVal = _formatSequence(RawSequenceVal);
                }
                return ReturnVal;
            }

        }//Next

        public string Current
        {
            get
            {
                string ReturnVal = "";

                if (null != _CurrentSequenceRow)
                {
                    CswSequenceName SequenceName = new CswSequenceName( _CurrentSequenceRow["sequencename"].ToString() );

                    if (!_CswNbtResources.doesUniqueSequenceExist(SequenceName.DBName))
                        _CswNbtResources.makeUniqueSequence(SequenceName.DBName, 1);

                    string RawSequenceVal = _CswNbtResources.getCurrentUniqueSequenceVal(SequenceName.DBName).ToString();

                    ReturnVal = _formatSequence(RawSequenceVal);
                }
                return ReturnVal;
            }
        }

        /// <summary>
        /// Sets the sequence value to be one more than the current value supplied
        /// </summary>
        /// <param name="CurrentFormattedValue">Current Formatted Sequence value</param>
        public void Reset(string CurrentFormattedValue)
        {
            Int32 RealValue = _deformatSequence(CurrentFormattedValue) + 1;
            if (null != _CurrentSequenceRow)
            {
                CswSequenceName SequenceName = new CswSequenceName( _CurrentSequenceRow["sequencename"].ToString() );

                if( !_CswNbtResources.doesUniqueSequenceExist( SequenceName.DBName ) )
                {
                    _CswNbtResources.makeUniqueSequence( SequenceName.DBName, RealValue );
                }
                else
                {
                    _CswNbtResources.resetUniqueSequenceVal( SequenceName.DBName, RealValue );
                }
            }
        }
    }//CswNbtSequenceValue

}//namespace ChemSW.Nbt

