using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Core;
using ChemSW.RscAdo;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{

    enum OpMode { Apply, Revert };
    public class CswConstraintDdlOps
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Dictionary<string, CswTableConstraint> _ConstraintsByKey = new Dictionary<string, CswTableConstraint>();
        private Dictionary<string, CswTableConstraint> _ConstraintsByColumn = new Dictionary<string, CswTableConstraint>();
        private Dictionary<string, Dictionary<string, CswTableConstraint>> _ConstraintsByTable = new Dictionary<string, Dictionary<string, CswTableConstraint>>();

        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswConstraintDdlOps( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public void clear()
        {
            _ConstraintsByKey.Clear();
            _ConstraintsByColumn.Clear();
            _ConstraintsByTable.Clear();

        }//clear()

        private void _addToCosntraintsByTable( string ConstraintKey, CswTableConstraint IndexedConstraint )
        {
            if ( !_ConstraintsByTable.ContainsKey( ConstraintKey ) )
            {
                _ConstraintsByTable.Add( ConstraintKey, new Dictionary<string, CswTableConstraint>() );

            }
            else
            {
                if ( _ConstraintsByTable[ ConstraintKey ].ContainsKey( IndexedConstraint.ReferencingColumnName ) )
                {
                    _ConstraintsByTable[ ConstraintKey ].Remove( IndexedConstraint.ReferencingColumnName );

                }//if-else the column already has a constraint

            }//if-else the table is already defined

            _ConstraintsByTable[ ConstraintKey ].Add( IndexedConstraint.ReferencingColumnName, IndexedConstraint );

        }//_addToCosntraintsByTable()

        private void _addConstraint( CswTableConstraint CswTableConstraint )
        {
            CswTableConstraint IndexedConstraint = null;
            if ( !_ConstraintsByKey.ContainsKey( CswTableConstraint.ToString() ) )
            {
                _ConstraintsByKey.Add( CswTableConstraint.ToString(), CswTableConstraint );
                IndexedConstraint = CswTableConstraint;
            }
            else
            {
                IndexedConstraint = _ConstraintsByKey[ CswTableConstraint.ToString() ];
            }

            _addToCosntraintsByTable( CswTableConstraint.ReferencingTableName, IndexedConstraint );
            _addToCosntraintsByTable( CswTableConstraint.ReferencedTableName, IndexedConstraint );

        }//_addConstraint()


        public void add( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName, bool ApplyToDb )
        {

            CswTableConstraint CswTableConstraint = new CswTableConstraint();
            CswTableConstraint.ConstraintOpType = ConstraintOpType.Create;
            CswTableConstraint.ReferencingTableName = ReferencingTableName;
            CswTableConstraint.ReferencingColumnName = ReferencingColumnName;
            CswTableConstraint.ReferencedTableName = ReferencedTableName;
            CswTableConstraint.ReferencedColumnName = ReferencedColumnName;
            CswTableConstraint.ApplyToDb = ApplyToDb;
            _addConstraint( CswTableConstraint );
        }//add()


        public void markColumnForRemoval( string TableName, string ColumnName )
        {
            _addReferencingConstraints( TableName, ColumnName, ConstraintOpType.Remove );
            _addReferencedByConstraints( TableName, ColumnName, ConstraintOpType.Remove );

        }//markColumnForRemoval()


        private void _addReferencingConstraints( string TableName, string ColumnName, ConstraintOpType ConstraintOpType )
        {
            //Fk references to this table
            List<CswTableConstraint> Constraints = new List<CswTableConstraint>( _CswNbtResources.CswResources.getConstraints( string.Empty, string.Empty, TableName, ColumnName ) );
            foreach ( CswTableConstraint CurrentConstraint in Constraints )
            {
                if ( string.Empty == ColumnName || ColumnName.ToLower() == CurrentConstraint.ReferencingColumnName.ToLower() )
                {
                    CurrentConstraint.ConstraintOpType = ConstraintOpType;
                    _addConstraint( CurrentConstraint );
                }

            }//iterate known constraints        

        }//_addReferencingConstraints()

        private void _addReferencedByConstraints( string TableName, string ColumnName, ConstraintOpType ConstraintOpType )
        {
            //This table's fk references to other tables

            List<CswTableConstraint> Constraints = new List<CswTableConstraint>( _CswNbtResources.CswResources.getConstraints( TableName, ColumnName, string.Empty, string.Empty ) );
            foreach ( CswTableConstraint CurrentConstraint in Constraints )
            {
                if ( string.Empty == ColumnName || ColumnName.ToLower() == CurrentConstraint.ReferencingColumnName.ToLower() )
                {
                    CurrentConstraint.ConstraintOpType = ConstraintOpType;
                    _addConstraint( CurrentConstraint );
                }

            }//iterate known constraints

        }//_addReferencedByConstraints()

        public void markTableForRemoval( string TableName )
        {
            _addReferencingConstraints( TableName, string.Empty, ConstraintOpType.Remove );
            _addReferencedByConstraints( TableName, string.Empty, ConstraintOpType.Remove );


        }//markTableForRemoval()

        public void removeConstraint( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName, string ConstraintName )
        {
           if(  string.Empty ==  ReferencingTableName ||
               string.Empty == ReferencingColumnName ||
               string.Empty == ReferencedTableName ||
               string.Empty == ReferencedColumnName )
               throw( new CswDniException( "Referencing and referenced table and column names must be supplied" ) ); 


            CswTableConstraint CswTableConstraint = new CswTableConstraint();
            CswTableConstraint.ConstraintOpType = ConstraintOpType.Remove;
            CswTableConstraint.ReferencingTableName = ReferencingTableName;
            CswTableConstraint.ReferencingColumnName = ReferencingColumnName;
            CswTableConstraint.ReferencedTableName = ReferencedTableName;
            CswTableConstraint.ReferencedColumnName = ReferencedColumnName;
            CswTableConstraint.ConstraintName = string.Empty != ConstraintName ? ConstraintName : getConstraintName( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName );
            CswTableConstraint.ApplyToDb = true;
            _addConstraint( CswTableConstraint );
        }//removeConstraint()

        //public void removeConstraint( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName )
        //{
        //    string ConstraintName = getConstraintName( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName );
        //    if ( string.Empty == ConstraintName )
        //        throw ( new CswDniException( "There is no contraint " + ReferencingTableName + "." + ReferencingColumnName + " referencing " + ReferencedTableName + "." + ReferencedColumnName ) );

        //    removeConstraint( ReferencedTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName, ConstraintName );

        //}//removeConstraint()

        public bool doesConstraintExistInDb( string ConstraintName )
        {
            return ( _CswNbtResources.CswResources.doesConstraintExistInDb( ConstraintName ) );
        }

        public List<CswTableConstraint> getConstraints( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName )
        {
            return ( _CswNbtResources.CswResources.getConstraints( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName ) );

        }//getConstraints()

        public string getConstraintName( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName )
        {
            string ReturnVal = string.Empty;
            List<CswTableConstraint> Constraints = new List<CswTableConstraint>( getConstraints( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName ) );

            if ( Constraints.Count > 1 )
                throw ( new CswDniException( "There is more than one constraint " + ReferencingTableName + "." + ReferencingColumnName + " referencing " + ReferencedTableName + "." + ReferencedColumnName ) );

            if ( 1 == Constraints.Count && string.Empty != Constraints[ 0 ].ConstraintName )
            {
                ReturnVal = Constraints[ 0 ].ConstraintName;
            }

            return ( ReturnVal );

        }//getConstraintName()

        public void apply( string ReferencingTableName, string ReferencingColumnName )
        {
            _visitConstraints( ReferencingTableName, ReferencingColumnName, OpMode.Apply );
        }//apply()

        public void revert( string ReferencingTableName, string ReferencingColumnName )
        {
            _visitConstraints( ReferencingTableName, ReferencingColumnName, OpMode.Revert );
        }//apply()


        private void _visitConstraints( string ReferencingTableName, string ReferencingColumnName, OpMode OpMode )
        {

            foreach ( CswTableConstraint CurrentConstraint in _ConstraintsByKey.Values )
            {
                if ( string.Empty == ReferencingTableName || CurrentConstraint.ReferencingTableName.ToLower() == ReferencingTableName.ToLower() || CurrentConstraint.ReferencedTableName.ToLower() == ReferencingTableName.ToLower() )
                {
                    if ( string.Empty == ReferencingColumnName || CurrentConstraint.ReferencingColumnName.ToLower() == ReferencingColumnName.ToLower() || CurrentConstraint.ReferencedColumnName.ToLower() == ReferencingColumnName.ToLower() )
                    {
                        if ( OpMode.Apply == OpMode && DdlProcessStatus.Applied != CurrentConstraint.DdlProcessStatus )
                        {
                            if ( ConstraintOpType.Create == CurrentConstraint.ConstraintOpType )
                            {
                                CurrentConstraint.ConstraintName = _CswNbtResources.CswResources.makeConstraint( CurrentConstraint.ReferencingTableName, CurrentConstraint.ReferencingColumnName, CurrentConstraint.ReferencedTableName, CurrentConstraint.ReferencedColumnName, CurrentConstraint.ApplyToDb );
                                CurrentConstraint.DdlProcessStatus = DdlProcessStatus.Applied;
                            }
                            else if ( ConstraintOpType.Remove == CurrentConstraint.ConstraintOpType )
                            {
                                _CswNbtResources.CswResources.removeConstraint( CurrentConstraint.ReferencingTableName, CurrentConstraint.ConstraintName );
                                CurrentConstraint.DdlProcessStatus = DdlProcessStatus.Applied;
                            }
                        }
                        else if ( OpMode.Revert == OpMode && DdlProcessStatus.Reverted != CurrentConstraint.DdlProcessStatus )
                        {
                            if ( ConstraintOpType.Create == CurrentConstraint.ConstraintOpType )
                            {
                                _CswNbtResources.CswResources.removeConstraint( CurrentConstraint.ReferencingTableName, CurrentConstraint.ConstraintName );
                                CurrentConstraint.DdlProcessStatus = DdlProcessStatus.Reverted;
                            }
                            else if ( ConstraintOpType.Remove == CurrentConstraint.ConstraintOpType )
                            {
                                CurrentConstraint.ConstraintName = _CswNbtResources.CswResources.makeConstraint( CurrentConstraint.ReferencingTableName, CurrentConstraint.ReferencingColumnName, CurrentConstraint.ReferencedTableName, CurrentConstraint.ReferencedColumnName, CurrentConstraint.ApplyToDb );
                                CurrentConstraint.DdlProcessStatus = DdlProcessStatus.Reverted;
                            }

                        }//if-else on opmode && opstatus

                    }//if pk or fk column matches target or we don't care about columnname

                }//if pk or fk table matches target or we don't care about tablename

            }//iterate all constraints

        }//apply()

    }//class CswConstraintDdlOps

}//ChemSW.Nbt.Schema
