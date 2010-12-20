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
        public DataTable getSequence( CswSequenceName SequenceName )
        {
            return SequenceTableUpdate.getTable( " where lower(sequencename)= '" + SequenceName.DisplayName.ToLower() + "'" );
        }//getSequence(string)

        /// <summary>
        /// Returns a DataTable with a row for each sequence
        /// </summary>
        public DataTable getAllSequences()
        {
            return SequenceTableUpdate.getTable();
        }//getSequence(string)


        /// <summary>
        /// Creates a new sequence entry and sequence object in the DB
        /// </summary>
        /// <param name="SequenceName">Name of new sequence</param>
        /// <param name="Prepend">String to prepend before all sequence values</param>
        /// <param name="Append">String to append after all sequence values</param>
        /// <param name="Pad">Pad the numeric portion of the sequence value to this many characters</param>
        /// <returns>Primary key of new sequence entry</returns>
        public Int32 makeSequence( CswSequenceName SequenceName, string Prepend, string Append, Int32 Pad, Int32 InitialValueIn )
        {
            if(SequenceName.DisplayName == string.Empty)
                throw ( new CswDniException( "Sequence Name is Required", "To create a new Sequence, the Sequence Name is Required" ) );

            if( doesSequenceExist( SequenceName ) )
                throw ( new CswDniException( "Sequence Error", "A sequence named " + SequenceName.DisplayName + " is already defined" ) );

            Int32 InitialValue = ( Int32.MinValue != InitialValueIn ) ? InitialValueIn : 1;
            makeDbSequence( SequenceName, InitialValue );

            DataTable SequencesTable = getSequence( SequenceName );
            DataRow NewSequencesRow = SequencesTable.NewRow();
            NewSequencesRow["sequencename"] = SequenceName.DisplayName;
            NewSequencesRow["prep"] = Prepend;
            NewSequencesRow["post"] = Append;
            NewSequencesRow["pad"] = CswConvert.ToDbVal( Pad );
            SequencesTable.Rows.Add( NewSequencesRow );
            SequenceTableUpdate.update( SequencesTable );

            return CswConvert.ToInt32( NewSequencesRow["sequenceid"] );
        }//makeSequence()

        public void editSequence( Int32 SequenceId, CswSequenceName NewSequenceName, string Prepend, string Append, Int32 Pad )
        {
            DataTable SequencesTable = getSequence( SequenceId );
            if( SequencesTable.Rows.Count > 0 )
            {
                // Update Sequence
                DataRow EditSequenceRow = SequencesTable.Rows[0];
                CswSequenceName OldSequenceName = new CswSequenceName( EditSequenceRow["sequencename"].ToString() );
                EditSequenceRow["sequencename"] = NewSequenceName.DisplayName;
                EditSequenceRow["prep"] = Prepend;
                EditSequenceRow["post"] = Append;
                EditSequenceRow["pad"] = CswConvert.ToDbVal( Pad );
                SequenceTableUpdate.update( SequencesTable );

                // Fix DB Sequence
                Int32 InitialValue = _CswNbtResources.getCurrentUniqueSequenceVal( OldSequenceName.DBName );
                removeDbSequence( OldSequenceName );
                makeDbSequence( NewSequenceName, InitialValue );
            }
            else
            {
                throw ( new CswDniException( "Sequence Error", "Sequence with SequenceId " + SequenceId.ToString() + " is not defined" ) );
            }

        } // editSequence()

        public void makeDbSequence( CswSequenceName SequenceName, Int32 InitialValue )
        {
            _CswNbtResources.makeUniqueSequence( SequenceName.DBName, InitialValue );

        }//makeSequenceInDb

        /// <summary>
        /// Assigns the sequence to a property
        /// </summary>
        /// <param name="SequenceName">Name of sequence</param>
        /// <param name="NodeTypePropId">Primary Key of Property</param>
        public void assignSequence( CswSequenceName SequenceName, Int32 NodeTypePropId )
        {
            DataTable SequenceTable = getSequence( SequenceName );
            if( SequenceTable.Rows.Count > 0 )
            {
                Int32 SequenceId = CswConvert.ToInt32( SequenceTable.Rows[0]["sequenceid"] );
                // We're ignoring property versioning here -- this might be a problem
                _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId ).setSequence( SequenceId );
            }
            else
            {
                throw new CswDniException( "Invalid Sequence", "User attempted to assign a non-existant sequence name: " + SequenceName.DisplayName );
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
        public void removeSequence( CswSequenceName SequenceName )
        {
            DataTable SequencesTable = getSequence( SequenceName );
            if( 0 == SequencesTable.Rows.Count )
                throw ( new CswDniException( "There is no sequence table record named " + SequenceName.DisplayName ) );

            Int32 SequencesId = CswConvert.ToInt32( SequencesTable.Rows[0]["sequenceid"] );

            CswTableSelect NodeTypePropsSelect = _CswNbtResources.makeCswTableSelect( "removeSequence_nodetypeprops_select", "nodetype_props" );
            DataTable NodeTypePropsTable = NodeTypePropsSelect.getTable( "sequenceid", SequencesId );

            if( NodeTypePropsTable.Rows.Count > 0 )
                throw ( new CswDniException( "Sequence " + SequenceName.DisplayName + " cannot be removed bacause there are " + NodeTypePropsTable.Rows.Count.ToString() + " NodeType_Props records referencing it" ) );

            SequencesTable.Rows[0].Delete();
            SequenceTableUpdate.update( SequencesTable );

            removeDbSequence( SequenceName );
        }//removeSequence()

        public void removeDbSequence( CswSequenceName SequenceName )
        {
            _CswNbtResources.removeUniqueSequence( SequenceName.DBName );
        }//removeSequenceFromDb()

        public bool doesSequenceExist( CswSequenceName SequenceName )
        {
            bool ReturnVal = false;
            DataTable SequencesTable = getSequence( SequenceName );
            if( false == ( ReturnVal = SequencesTable.Rows.Count > 0 ) )
            {
                ReturnVal = _CswNbtResources.doesUniqueSequenceExist( SequenceName.DBName );
            }

            return ( ReturnVal );

        }//doesSequenceExist

        public bool doesSequenceExist( Int32 SequenceId )
        {
            DataTable SequencesTable = getSequence( SequenceId );
            return ( SequencesTable.Rows.Count > 0 );
        }//doesSequenceExist

        public Int32 getSequenceValue( CswSequenceName SequenceName )
        {
            Int32 ReturnVal = Int32.MinValue;

            if( !doesSequenceExist( SequenceName ) )
                throw ( new CswDniException( "No such sequence: " + SequenceName ) );

            ReturnVal = _CswNbtResources.getCurrentUniqueSequenceVal( SequenceName.DBName );

            return ( ReturnVal );
        }//getSequenceValue()

    }//class CswNbtSequenceManager

}//namespace
