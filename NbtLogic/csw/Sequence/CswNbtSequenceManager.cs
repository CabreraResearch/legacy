using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Configuration;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Manages use of Sequences for Barcode and Sequence fields
    /// </summary>
    public class CswNbtSequenceManager
    {
        private CswNbtResources _CswNbtResources = null;
        private CswTableUpdate _SequenceTableUpdate = null;
        private CswTableUpdate SequenceTableUpdate
        {
            get
            {
                if( _SequenceTableUpdate == null )
                    _SequenceTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtSequenceManager_update", "sequences" );
                return _SequenceTableUpdate;
            }
        }

        private Collection<OrderByClause> OrderByClause = new Collection<OrderByClause> { new OrderByClause( "sequencename", OrderByType.Ascending ) };


        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtSequenceManager( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor


        /// <summary>
        /// Returns a DataTable of all current sequences
        /// </summary>
        public DataTable CurrentSequences
        {
            get
            {
                return SequenceTableUpdate.getTable( string.Empty, OrderByClause );
            }
        }//CurrentSequences

        /// <summary>
        /// Returns a DataTable with a single row for a given primary key
        /// </summary>
        /// <param name="SequenceId">Primary key of sequence</param>
        public DataTable getSequence( Int32 SequenceId )
        {
            return SequenceTableUpdate.getTable( null, "sequenceid", SequenceId, string.Empty, true, OrderByClause );
        }//getSequence(Int32)

        /// <summary>
        /// Returns a DataTable with a single row for a given sequence name
        /// </summary>
        /// <param name="SequenceName">Name of sequence</param>
        public DataTable getSequence( string SequenceName )
        {
            return SequenceTableUpdate.getTable( " where lower(sequencename)= '" + SequenceName.ToLower() + "'" );
        }//getSequence(string)

        /// <summary>
        /// Returns a DataTable with a row for each sequence
        /// </summary>
        public DataTable getAllSequences()
        {
            return SequenceTableUpdate.getTable();
        }//getSequence(string)


        /// <summary>
        /// Returns a DB-safe version of the sequence name, for use in creating the actual sequence object
        /// </summary>
        /// <param name="SequenceName">Name of sequence</param>
        public static string getDBSequenceName( string SequenceName )
        {
            return SequenceName.Replace( " ", string.Empty );
        }

        /// <summary>
        /// Creates a new sequence entry and sequence object in the DB
        /// </summary>
        /// <param name="SequenceName">Name of new sequence</param>
        /// <param name="Prepend">String to prepend before all sequence values</param>
        /// <param name="Append">String to append after all sequence values</param>
        /// <param name="Pad">Pad the numeric portion of the sequence value to this many characters</param>
        /// <returns>Primary key of new sequence entry</returns>
        public Int32 makeSequence( string SequenceName, string Prepend, string Append, string Pad , Int32 InitialValueIn )
        {
            if( doesSequenceExist( SequenceName ) )
                throw( new CswDniException( "A sequence named " + SequenceName + " is already defined"  ) );

            string SafeSequenceName = getDBSequenceName( SequenceName );

            Int32 InitialValue = ( Int32.MinValue != InitialValueIn ) ? InitialValueIn : 1;
            makeDbSequence( SafeSequenceName, InitialValue );

            DataTable SequencesTable = getSequence( SequenceName );
            DataRow NewSequencesRow = SequencesTable.NewRow();
            NewSequencesRow[ "sequencename" ] = SequenceName;
            if ( Prepend.Length > 0 )
                NewSequencesRow[ "prep" ] = Prepend;
            if ( Append.Length > 0 )
                NewSequencesRow[ "post" ] = Append;
            if ( Pad.Length > 0 )
                NewSequencesRow[ "pad" ] = Pad;
            SequencesTable.Rows.Add( NewSequencesRow );
            SequenceTableUpdate.update( SequencesTable );

            return CswConvert.ToInt32( NewSequencesRow[ "sequenceid" ] );
        }//makeSequence()

        public void makeDbSequence( string SequenceName , Int32 InitialValue )
        {
            _CswNbtResources.makeUniqueSequence( SequenceName , InitialValue );

        }//makeSequenceInDb

        /// <summary>
        /// Assigns the sequence to a property
        /// </summary>
        /// <param name="SequenceName">Name of sequence</param>
        /// <param name="NodeTypePropId">Primary Key of Property</param>
        public void assignSequence( string SequenceName, Int32 NodeTypePropId )
        {
            DataTable SequenceTable = getSequence( SequenceName );
            if ( SequenceTable.Rows.Count > 0 )
            {
                Int32 SequenceId = CswConvert.ToInt32( SequenceTable.Rows[ 0 ][ "sequenceid" ] );
                // We're ignoring property versioning here -- this might be a problem
                _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId ).setSequence( SequenceId );
            }
            else
            {
                throw new CswDniException( "Invalid Sequence", "User attempted to assign a non-existant sequence name: " + SequenceName );
            }
        }//assignSequence()

        /// <summary>
        /// Removes a sequence from a property
        /// </summary>
        /// <param name="NodeTypePropId">Primary Key of Property</param>
        public void unAssignSequence( Int32 NodeTypePropId )
        {
            // We're ignoring property versioning here -- this might be a problem
            _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId ).setSequence( Int32.MinValue );
        }//unAssignSequence()

        /// <summary>
        /// Removes a sequence entry and deletes the sequence DB object
        /// </summary>
        /// <param name="SequenceName">Name of Sequence</param>
        public void removeSequence( string SequenceName )
        {
            DataTable SequencesTable = getSequence( SequenceName );
            if ( 0 == SequencesTable.Rows.Count )
                throw ( new CswDniException( "There is no sequence table record named " + SequenceName ) );

            Int32 SequencesId = CswConvert.ToInt32( SequencesTable.Rows[ 0 ][ "sequenceid" ] );

            CswTableSelect NodeTypePropsSelect = _CswNbtResources.makeCswTableSelect( "removeSequence_nodetypeprops_select", "nodetype_props" );
            DataTable NodeTypePropsTable = NodeTypePropsSelect.getTable( "sequenceid", SequencesId );

            if ( NodeTypePropsTable.Rows.Count > 0 )
                throw ( new CswDniException( "Sequence " + SequenceName + " cannot be removed bacause there are " + NodeTypePropsTable.Rows.Count.ToString() + " NodeType_Props records referencing it" ) );

            SequencesTable.Rows[ 0 ].Delete();
            SequenceTableUpdate.update( SequencesTable );

            removeDbSequence( SequenceName );
        }//removeSequence()

        public void removeDbSequence( string SequenceName )
        {
            _CswNbtResources.removeUniqueSequence( getDBSequenceName( SequenceName ) );
        }//removeSequenceFromDb()

        public bool doesSequenceExist( string SequenceName )
        {
            bool ReturnVal = false;
            DataTable SequencesTable = getSequence( SequenceName );
            if ( false == ( ReturnVal = SequencesTable.Rows.Count > 0 ) )
            {
                string SafeSequenceName = getDBSequenceName( SequenceName );
                ReturnVal = _CswNbtResources.doesUniqueSequenceExist( SafeSequenceName );
            }

            return( ReturnVal );

            //if ( SequencesTable.Rows.Count > 0 )
            //    throw ( new CswDniException( "There is already a sequence table record named " + SequenceName ) );

            //string SafeSequenceName = getDBSequenceName( SequenceName );
            //if ( _CswNbtResources.CswDbResources.doesuniqueSequenceExist( SafeSequenceName ) )
            //    throw ( new CswDniException( "There is already a unique sequence object in the database named " + SafeSequenceName ) );

        }//doesSequenceExist

        public Int32 getSequenceValue( string SequenceName )
        {
            Int32 ReturnVal = Int32.MinValue;

            if( ! doesSequenceExist( SequenceName ) )
                throw( new CswDniException("No such sequence: " + SequenceName ) );

            ReturnVal = _CswNbtResources.getCurrentUniqueSequenceVal( getDBSequenceName( SequenceName ) );

            return( ReturnVal );
        }//getSequenceValue()

    }//class CswNbtSequenceManager

}//namespace
