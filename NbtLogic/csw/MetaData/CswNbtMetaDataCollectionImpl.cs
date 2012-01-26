using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionImpl
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswTableUpdate _TableUpdate;
        private string _PkColumnName;

        public delegate ICswNbtMetaDataObject MakeMetaDataObjectHandler( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row );
        private MakeMetaDataObjectHandler _MetaDataObjectMaker = null;

        public CswNbtMetaDataCollectionImpl( CswNbtMetaDataResources CswNbtMetaDataResources,
                                             string PkColumnName,
                                             CswTableUpdate TableUpdate,
                                             MakeMetaDataObjectHandler MetaDataObjectMaker )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _TableUpdate = TableUpdate;
            _PkColumnName = PkColumnName;
            _MetaDataObjectMaker = MetaDataObjectMaker;
        } // constructor

        public Collection<ICswNbtMetaDataObject> getAll()
        {
            Collection<ICswNbtMetaDataObject> Coll = new Collection<ICswNbtMetaDataObject>();
            DataTable Table = _TableUpdate.getTable();
            foreach( DataRow Row in Table.Rows )
            {
                Coll.Add( _MetaDataObjectMaker( _CswNbtMetaDataResources, Row ) );
            }
            return Coll;
        } // getAll()

        public Collection<Int32> getPks()
        {
            return getPks( string.Empty );
        } // getPks()

        public Collection<Int32> getPks(string Where)
        {
            Collection<Int32> Coll = new Collection<Int32>();

            CswCommaDelimitedString Select = new CswCommaDelimitedString();
            Select.Add( _PkColumnName );
            DataTable Table = _TableUpdate.getTable( Select, string.Empty, Int32.MinValue, Where, false );

            foreach( DataRow Row in Table.Rows )
            {
                Coll.Add( CswConvert.ToInt32( Row[_PkColumnName] ) );
            }
            return Coll;
        } // getPks(Where)

        public ICswNbtMetaDataObject getByPk( Int32 Pk )
        {
            ICswNbtMetaDataObject ret = null;
            DataTable Table = _TableUpdate.getTable( _PkColumnName, Pk );
            if( Table.Rows.Count > 0 )
            {
                ret = _MetaDataObjectMaker( _CswNbtMetaDataResources, Table.Rows[0] );
            }
            return ret;
        } // getByPk()

        public Collection<ICswNbtMetaDataObject> getWhere( string WhereClause )
        {
            Collection<ICswNbtMetaDataObject> Coll = new Collection<ICswNbtMetaDataObject>();
            DataTable Table = _TableUpdate.getTable( WhereClause );
            foreach( DataRow Row in Table.Rows )
            {
                Coll.Add( _MetaDataObjectMaker( _CswNbtMetaDataResources, Row ) );
            }
            return Coll;
        } // getWhere()

        public ICswNbtMetaDataObject getWhereFirst( string WhereClause )
        {
            ICswNbtMetaDataObject ret = null;
            Collection<ICswNbtMetaDataObject> Coll = getWhere( WhereClause );
            if( Coll.Count > 0 )
            {
                ret = Coll[0];
            }
            return ret;
        } // getWhereFirst()

    } // public class CswNbtMetaDataCollectionImpl
} // namespace ChemSW.Nbt.MetaData
