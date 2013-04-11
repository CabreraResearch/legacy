﻿using System;
using System.Collections.Generic;
using ChemSW.Core;
//using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{

    public enum DdlTableOpType { Unknown, Add, Drop, Column };
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    /// 
    public class CswTableDdlOp
    {
        public string PkColumnName = string.Empty;
        private Dictionary<string, CswColumnDdlOp> _Columns = new Dictionary<string, CswColumnDdlOp>();
        private CswConstraintDdlOps _CswConstraintDdlOps = null;
        private CswNbtResources _CswNbtResources = null;

        private bool dbg_ManageConstraints = true;
        //public bool dbg_ManageConstraints
        //{
        //    set { _ManageConstraints = value; }
        //    get { return ( _ManageConstraints ); }
        //}//dbg_ManageConstraints

        private string _TableName = string.Empty;
        private string _TableCopyName = string.Empty;

        //private bool _BackTableExists = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswTableDdlOp( CswNbtResources CswNbtResources, CswConstraintDdlOps CswConstraintDdlOps, string TableName )
        {
            _CswNbtResources = CswNbtResources;
            _CswConstraintDdlOps = CswConstraintDdlOps;
            _TableName = TableName;
        }//ctor

        private void _clear()
        {
            _Columns.Clear();
            _TableName = string.Empty;
            _TableCopyName = string.Empty;
        }//_clear()


        private DdlTableOpType _DdlTableOpType = DdlTableOpType.Unknown;
        public DdlTableOpType DdlTableOpType
        {
            set
            {
                _DdlTableOpType = value;

                if( dbg_ManageConstraints )
                {
                    if( DdlTableOpType.Drop == _DdlTableOpType )
                    {
                        _CswConstraintDdlOps.markTableForRemoval( _TableName );
                    }
                }
            }
            get
            {
                return ( _DdlTableOpType );
            }
        }//DdlTableOpType




        public void addColumn( string columnname, CswEnumDataDictionaryColumnType columntype, Int32 datatypesize, Int32 dblprecision,
                               string defaultvalue, string description, string foreignkeycolumn, string foreignkeytable, bool constrainfkref, bool isview,
                               bool logicaldelete, string lowerrangevalue, bool lowerrangevalueinclusive, CswEnumDataDictionaryPortableDataType portabledatatype, bool ReadOnly,
                               bool Required, CswEnumDataDictionaryUniqueType uniquetype, bool uperrangevalueinclusive, string upperrangevalue )
        //Int32 NodeTypePropId, string SubFieldName )
        {

            CswColumnDdlOp CswColumnDdlOp = new CswColumnDdlOp( columnname, CswEnumDdlColumnOpType.Add );

            CswColumnDdlOp.columntype = columntype;
            CswColumnDdlOp.datatypesize = datatypesize;
            CswColumnDdlOp.dblprecision = dblprecision;
            CswColumnDdlOp.defaultvalue = defaultvalue;
            CswColumnDdlOp.description = description;
            CswColumnDdlOp.isview = isview;
            CswColumnDdlOp.logicaldelete = logicaldelete;
            CswColumnDdlOp.lowerrangevalue = lowerrangevalue;
            CswColumnDdlOp.lowerrangevalueinclusive = lowerrangevalueinclusive;
            CswColumnDdlOp.portabledatatype = portabledatatype;
            CswColumnDdlOp.ReadOnly = ReadOnly;
            CswColumnDdlOp.Required = Required;
            CswColumnDdlOp.uniquetype = uniquetype;
            CswColumnDdlOp.uperrangevalueinclusive = uperrangevalueinclusive;
            CswColumnDdlOp.upperrangevalue = upperrangevalue;
            //CswColumnDdlOp.NodeTypePropId = NodeTypePropId;
            //CswColumnDdlOp.SubFieldName = SubFieldName;

            _Columns.Add( columnname, CswColumnDdlOp );

            if( dbg_ManageConstraints )
            {
                if( constrainfkref && string.Empty != foreignkeycolumn && string.Empty != foreignkeytable )
                {
                    _CswConstraintDdlOps.add( _TableName, CswColumnDdlOp.columnname, foreignkeytable, foreignkeycolumn, constrainfkref );
                }
            }

        }//addColumn()

        public void dropColumn( string ColumnName )
        {
            CswColumnDdlOp CswColumnDdlOp = new CswColumnDdlOp( ColumnName, CswEnumDdlColumnOpType.Drop );

            _CswNbtResources.CswResources.DataDictionary.setCurrentColumn( _TableName, ColumnName );

            //We have to capture all this incase we need to rollback the drop operation
            CswColumnDdlOp.columnname = ColumnName;
            CswColumnDdlOp.columntype = _CswNbtResources.DataDictionary.ColumnType;
            //CswColumnDdlOp.constrainfkref = _CswNbtResources.DataDictionary.ConstrainFkRef;
            CswColumnDdlOp.datatypesize = _CswNbtResources.DataDictionary.DataTypeSize;
            CswColumnDdlOp.dblprecision = _CswNbtResources.DataDictionary.DblPrecision;
            CswColumnDdlOp.defaultvalue = _CswNbtResources.DataDictionary.DefaultValue;
            CswColumnDdlOp.description = _CswNbtResources.DataDictionary.Description;
            //CswColumnDdlOp.foreignkeycolumn = _CswNbtResources.DataDictionary.ForeignKeyColumn;
            //CswColumnDdlOp.foreignkeytable = _CswNbtResources.DataDictionary.ForeignKeyTable;
            CswColumnDdlOp.isview = _CswNbtResources.DataDictionary.IsView;
            CswColumnDdlOp.logicaldelete = _CswNbtResources.DataDictionary.LogicalDelete;
            CswColumnDdlOp.lowerrangevalue = _CswNbtResources.DataDictionary.LowerRangeValue;
            CswColumnDdlOp.lowerrangevalueinclusive = _CswNbtResources.DataDictionary.LowerRangeValueInclusive;
            CswColumnDdlOp.portabledatatype = _CswNbtResources.DataDictionary.PortableDataType;
            CswColumnDdlOp.ReadOnly = _CswNbtResources.DataDictionary.ReadOnly;
            CswColumnDdlOp.Required = _CswNbtResources.DataDictionary.Required;
            CswColumnDdlOp.uniquetype = _CswNbtResources.DataDictionary.UniqueType;
            CswColumnDdlOp.uperrangevalueinclusive = _CswNbtResources.DataDictionary.UpperRangeValueInclusive;
            CswColumnDdlOp.upperrangevalue = _CswNbtResources.DataDictionary.UpperRangeValue;
            //CswColumnDdlOp.NodeTypePropId = _CswNbtResources.DataDictionary.NodeTypePropId;
            //CswColumnDdlOp.SubFieldName = _CswNbtResources.DataDictionary.SubFieldName;

            _Columns.Add( ColumnName, CswColumnDdlOp );

            if( dbg_ManageConstraints )
            {

                _CswConstraintDdlOps.markColumnForRemoval( _TableName, ColumnName );
            }

        }//dropColumn()


        public void renameColumn( string OriginalColumnName, string NewColumnName )
        {

            //We don't need to capture the same info as we do for dropColumn() because 
            //drop would just be renaming back to the original name. 

            //We are assuming that the native db rename op will schlep constraints along,
            //but this needs to be proven (see test cases)

            CswColumnDdlOp CswColumnDdlOp = new CswColumnDdlOp( NewColumnName, CswEnumDdlColumnOpType.Rename );
            CswColumnDdlOp.originalcolumnname = OriginalColumnName;
            _Columns.Add( NewColumnName, CswColumnDdlOp );


        }//renameColumn() 


        private bool _DropColumnExists
        {
            get
            {
                bool ReturnVal = false;
                foreach( CswColumnDdlOp CurrentColumn in _Columns.Values )
                {
                    if( CswEnumDdlColumnOpType.Drop == CurrentColumn.DdlColumnOpType )
                    {
                        ReturnVal = true;
                        break;
                    }
                }//iterate columns

                return ( ReturnVal );

            }//get

        }//_DropColumnExists

        private void _copyTable()
        {
            _TableCopyName = _CswNbtResources.CswResources.makeTemporaryTableCopy( _TableName );
        }//_copyTable()

        public CswEnumDdlProcessStatus DdlProcessStatus;
        public void apply()
        {

            if( _DropColumnExists || DdlTableOpType.Drop == DdlTableOpType )
            {
                _copyTable();
                //_BackTableExists = true;

            }//if we're dropping

            if( DdlTableOpType.Drop == DdlTableOpType )
            {
                if( CswEnumDdlProcessStatus.Applied != DdlProcessStatus )
                {
                    if( dbg_ManageConstraints )
                    {
                        _CswConstraintDdlOps.apply( _TableName, string.Empty );
                    }

                    _CswNbtResources.CswResources.dropTable( _TableName );
                    DdlProcessStatus = CswEnumDdlProcessStatus.Applied;
                }
            }
            else if( DdlTableOpType.Add == DdlTableOpType )
            {
                if( CswEnumDdlProcessStatus.Applied != DdlProcessStatus )
                {
                    _CswNbtResources.CswResources.addTable( _TableName, PkColumnName );
                    if( dbg_ManageConstraints )
                    {

                        _CswConstraintDdlOps.apply( _TableName, string.Empty );
                    }
                    DdlProcessStatus = CswEnumDdlProcessStatus.Applied;
                }
            }

            foreach( CswColumnDdlOp CurrentColumn in _Columns.Values )
            {
                if( CswEnumDdlProcessStatus.Applied != CurrentColumn.DdlProcessStatus )
                {
                    if( CswEnumDdlColumnOpType.Drop == CurrentColumn.DdlColumnOpType )
                    {
                        if( dbg_ManageConstraints )
                        {
                            _CswConstraintDdlOps.apply( _TableName, CurrentColumn.columnname );
                        }
                        _CswNbtResources.CswResources.dropColumn( _TableName, CurrentColumn.columnname );


                    }
                    else if( CswEnumDdlColumnOpType.Add == CurrentColumn.DdlColumnOpType )
                    {
                        _CswNbtResources.CswResources.addColumn( _TableName, CurrentColumn );

                        if( dbg_ManageConstraints )
                        {
                            _CswConstraintDdlOps.apply( _TableName, CurrentColumn.columnname );
                        }
                    }
                    else // DdlColumnOpType.Rename == CurrentColumn.DdlColumnOpType 
                    {
                        _CswNbtResources.CswResources.renameColumn( _TableName, CurrentColumn.originalcolumnname, CurrentColumn.columnname );
                    }

                    CurrentColumn.DdlProcessStatus = CswEnumDdlProcessStatus.Applied;

                }//if column was not yet applied

            }//iterate columns

        }//apply()


        public void revert()
        {
            if( DdlTableOpType.Add == DdlTableOpType )
            {
                if( CswEnumDdlProcessStatus.Applied == DdlProcessStatus )
                {
                    if( dbg_ManageConstraints )
                    {

                        _CswConstraintDdlOps.revert( _TableName, string.Empty );
                    }
                    _CswNbtResources.CswResources.dropTable( _TableName );
                }

                DdlProcessStatus = CswEnumDdlProcessStatus.Reverted;
            }
            else if( DdlTableOpType.Drop == DdlTableOpType )
            {
                if( CswEnumDdlProcessStatus.Applied == DdlProcessStatus )
                {
                    _CswNbtResources.CswResources.copyTable( _TableCopyName, _TableName );
                    if( dbg_ManageConstraints )
                    {

                        _CswConstraintDdlOps.revert( _TableName, string.Empty );
                    }
                }

                DdlProcessStatus = CswEnumDdlProcessStatus.Reverted;
            }
            else
            {
                foreach( CswColumnDdlOp CurrentColumnDdlOp in _Columns.Values )
                {
                    if( CswEnumDdlProcessStatus.Applied == CurrentColumnDdlOp.DdlProcessStatus )
                    {
                        _revertColumn( CurrentColumnDdlOp );
                        CurrentColumnDdlOp.DdlProcessStatus = CswEnumDdlProcessStatus.Reverted;
                    }
                }
            }

            if( string.Empty != _TableCopyName && _CswNbtResources.CswResources.isTableDefinedInDataBase( _TableCopyName ) )
                _CswNbtResources.CswResources.dropTable( _TableCopyName );

        }//revert()

        private void _revertColumn( CswColumnDdlOp ColumnDdlOp )
        {
            if( CswEnumDdlColumnOpType.Add == ColumnDdlOp.DdlColumnOpType )
            {

                if( dbg_ManageConstraints )
                {
                    _CswConstraintDdlOps.apply( _TableName, ColumnDdlOp.columnname );
                }
                _CswNbtResources.CswResources.dropColumn( _TableName, ColumnDdlOp.columnname );
            }
            else if( CswEnumDdlColumnOpType.Drop == ColumnDdlOp.DdlColumnOpType )
            {

                _CswNbtResources.CswResources.addColumn( _TableName, ColumnDdlOp, false ); //we know in this context that the deletion of dd data will get rolled back, so we leave dd data alone

                string PkColumn = _CswNbtResources.CswResources.getPrimeKeyColName( _TableName );

                string UpdateSql = "update " + _TableName + " set " + ColumnDdlOp.columnname + " = (select " + ColumnDdlOp.columnname + " from " + _TableCopyName + " where " + _TableCopyName + "." + PkColumn + "=" + _TableName + "." + PkColumn + ")";

                _CswNbtResources.CswResources.execArbitraryPlatformNeutralSql( UpdateSql );

                if( dbg_ManageConstraints )
                {
                    _CswConstraintDdlOps.apply( _TableName, ColumnDdlOp.columnname );
                }

            }
            else //DdlColumnOpType.Rename == ColumnDdlOp.DdlColumnOpType
            {
                _CswNbtResources.CswResources.renameColumn( _TableName, ColumnDdlOp.columnname, ColumnDdlOp.originalcolumnname ); 
            }

        }//_revertColumn()

        public void confirm()
        {
            if( string.Empty != _TableCopyName )
                _CswNbtResources.CswResources.dropTable( _TableCopyName );

        }//confirm()

    }//class CswTableDdlOp

}//ChemSW.Nbt.Schema
