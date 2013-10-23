using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionImpl
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswTableSelect _TableSelect;
        private CswTableUpdate _TableUpdate;
        private string _PkColumnName;
        private string _NameColumnName;

        public delegate ICswNbtMetaDataObject MakeMetaDataObjectHandler( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row, CswDateTime Date );
        private MakeMetaDataObjectHandler _MetaDataObjectMaker = null;



        public CswNbtMetaDataCollectionImpl( CswNbtMetaDataResources CswNbtMetaDataResources,
                                             string PkColumnName,
                                             string NameColumnName,
                                             CswTableSelect TableSelect,
                                             CswTableUpdate TableUpdate,
                                             MakeMetaDataObjectHandler MetaDataObjectMaker,
                                             ModuleWhereClauseHandler makeModuleWhereClause )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _TableSelect = TableSelect;
            _TableUpdate = TableUpdate;
            _PkColumnName = PkColumnName;
            _NameColumnName = NameColumnName;
            _MetaDataObjectMaker = MetaDataObjectMaker;
            _makeModuleWhereClause = makeModuleWhereClause;
        } // constructor


        public delegate string ModuleWhereClauseHandler();
        private ModuleWhereClauseHandler _makeModuleWhereClause = null;

        private void addModuleWhereClause( ref string WhereClause )
        {
            if( _CswNbtMetaDataResources.ExcludeDisabledModules )
            {
                string newWhereClause = _makeModuleWhereClause();
                if( newWhereClause != string.Empty )
                {
                    WhereClause += ( WhereClause == string.Empty ) ? " where " : " and ";
                    WhereClause += newWhereClause;
                }
            }
        } // addModuleWhereClause()


        /// <summary>
        ///  Add an ICswNbtMetaDataObject to the Cache 
        ///  (for use by MetaData for newly created objects)
        /// </summary>
        public void AddToCache( ICswNbtMetaDataObject NewObj )
        {
            if( false == _Cache.ContainsKey( NewObj.UniqueId ) )
            {
                _Cache.Add( NewObj.UniqueId, NewObj );
            }
        }

        private Dictionary<Int32, ICswNbtMetaDataObject> _Cache = new Dictionary<Int32, ICswNbtMetaDataObject>();
        private ICswNbtMetaDataObject _makeObj( DataRow Row, CswDateTime Date = null, bool useCache = true )
        {
            ICswNbtMetaDataObject ret = null;
            Int32 PkValue = CswConvert.ToInt32( Row[_PkColumnName] );
            if( useCache && _Cache.ContainsKey( PkValue ) )
            {
                // In order to guarantee only one reference per row, use the existing reference
                // and, to prevent dirty writes, remove the row
                ret = _Cache[PkValue];
                Row.Table.Rows.Remove( Row );
            }
            else
            {
                ret = _MetaDataObjectMaker( _CswNbtMetaDataResources, Row, Date );
                if( useCache )
                {
                    _Cache[PkValue] = ret;
                }
            }
            return ret;
        }


        private Collection<ICswNbtMetaDataObject> _makeObjs( DataTable Table, CswDateTime Date = null, bool useCache = true )
        {
            Collection<ICswNbtMetaDataObject> Coll = new Collection<ICswNbtMetaDataObject>();
            Collection<DataRow> RowsToIterate = new Collection<DataRow>();
            // We have to iterate rows separately, because _makeObj() can remove a row
            foreach( DataRow Row in Table.Rows )
            {
                RowsToIterate.Add( Row );
            }
            foreach( DataRow Row in RowsToIterate )
            {
                Coll.Add( _makeObj( Row, Date, useCache ) );
            }
            return Coll;
        } // _makeObjs()

        public void clearCache()
        {
            _All = null;
            _ByPk = null;
            _getWhere = null;
            _getNameWhere = null;
            //_PkDict = null;
            _PkDictsWhere = null;
            _Pks = null;
            _PksWhere = null;

            // Don't clear this one
            // _Cache = null;
        }

        private Collection<ICswNbtMetaDataObject> _All = null;
        public Collection<ICswNbtMetaDataObject> getAll()
        {
            if( _All == null )
            {
                string WhereClause = string.Empty;
                addModuleWhereClause( ref WhereClause );

                DataTable Table = _TableUpdate.getTable( WhereClause );

                _All = _makeObjs( Table );
            }
            return _All;
        } // getAll()

        private Collection<Int32> _Pks = null;
        public Collection<Int32> getPks()
        {
            if( _Pks == null )
            {
                _Pks = getPks( string.Empty );
            }
            return _Pks;
        } // getPks()


        public Collection<Int32> getPks( CswDateTime Date )
        {
            return getPks( string.Empty, Date );
        } // getPks(Where)

        private Dictionary<string, Collection<Int32>> _PksWhere = null;
        public Collection<Int32> getPks( string Where )
        {
            if( _PksWhere == null )
            {
                _PksWhere = new Dictionary<string, Collection<Int32>>();
            }
            if( false == _PksWhere.ContainsKey( Where ) )
            {
                CswCommaDelimitedString Select = new CswCommaDelimitedString();
                Select.Add( _PkColumnName );

                string WhereClause = Where;
                addModuleWhereClause( ref WhereClause );

                DataTable Table = _TableSelect.getTable( Select, string.Empty, Int32.MinValue, WhereClause, false );

                Collection<Int32> Coll = new Collection<Int32>();
                foreach( DataRow Row in Table.Rows )
                {
                    Coll.Add( CswConvert.ToInt32( Row[_PkColumnName] ) );
                }
                _PksWhere[Where] = Coll;
            }
            return _PksWhere[Where];
        } // getPks(Where)

        public Collection<Int32> getPks( string Where, CswDateTime Date )
        {
            Collection<Int32> ret = null;
            if( false == CswTools.IsDate( Date ) )
            {
                ret = getPks( Where );
            }
            else
            {
                string Sql = "select " + _PkColumnName + " from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtMetaDataResources.CswNbtResources, _TableSelect.TableName, Date ) + " " + _TableSelect.TableName + " ";
                addModuleWhereClause( ref Where );
                Sql += Where;

                CswArbitrarySelect AuditSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswArbitrarySelect( "MetaDataCollectionImpl_getpks_audit_select", Sql );
                DataTable Table = AuditSelect.getTable();
                ret = new Collection<Int32>();
                foreach( DataRow Row in Table.Rows )
                {
                    ret.Add( CswConvert.ToInt32( Row[_PkColumnName] ) );
                }
            }
            return ret;
        } // getPks(Where)

        public Int32 getPksFirst( string WhereClause )
        {
            Int32 ret = Int32.MinValue;
            Collection<Int32> Coll = getPks( WhereClause );
            if( Coll.Count > 0 )
            {
                ret = Coll[0];
            }
            return ret;
        } // getPksFirst()



        //private Dictionary<Int32, string> _PkDict = null;
        //public Dictionary<Int32, string> getPkDict()
        //{
        //    if( _PkDict == null )
        //    {
        //        _PkDict = getPkDict( string.Empty );
        //    }
        //    return _PkDict;
        //} // getPkDict()

        private class PkDictKey : IEquatable<PkDictKey>
        {
            public CswDateTime Date;
            public string Where;

            #region IEquatable
            /// <summary>
            /// IEquatable: ==
            /// </summary>
            public static bool operator ==( PkDictKey k1, PkDictKey k2 )
            {
                // If both are null, or both are same instance, return true.
                if( System.Object.ReferenceEquals( k1, k2 ) )
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if( ( (object) k1 == null ) || ( (object) k2 == null ) )
                {
                    return false;
                }

                // Now we know neither are null.  Compare values.
                return ( k1.Date == k2.Date && k1.Where == k2.Where );
            }

            /// <summary>
            /// IEquatable: !=
            /// </summary>
            public static bool operator !=( PkDictKey k1, PkDictKey k2 )
            {
                return !( k1 == k2 );
            }

            /// <summary>
            /// IEquatable: Equals
            /// </summary>
            public override bool Equals( object obj )
            {
                if( !( obj is PkDictKey ) )
                    return false;
                return this == (PkDictKey) obj;
            }

            /// <summary>
            /// IEquatable: Equals
            /// </summary>
            public bool Equals( PkDictKey obj )
            {
                return this == (PkDictKey) obj;
            }

            /// <summary>
            /// IEquatable: GetHashCode
            /// </summary>
            public override int GetHashCode()
            {
                int ret = 0;
                if( null != this.Date )
                {
                    ret += this.Date.GetHashCode();
                }
                if( null != this.Where )
                {
                    ret += this.Where.GetHashCode();
                }
                return ret;
            }

            #endregion IEquatable
        }

        private Dictionary<PkDictKey, Dictionary<Int32, string>> _PkDictsWhere = null;
        public Dictionary<Int32, string> getPkDict( string Where = "", CswDateTime Date = null )
        {
            if( _PkDictsWhere == null )
            {
                _PkDictsWhere = new Dictionary<PkDictKey, Dictionary<int, string>>();
            }

            PkDictKey Key = new PkDictKey() { Date = Date, Where = Where };
            if( false == _PkDictsWhere.ContainsKey( Key ) )
            {
                DataTable Table;
                if( null == Date )
                {
                    CswCommaDelimitedString Select = new CswCommaDelimitedString();
                    Select.Add( _PkColumnName );
                    Select.Add( _NameColumnName );

                    string WhereClause = Where;
                    addModuleWhereClause( ref WhereClause );

                    Table = _TableSelect.getTable( Select, string.Empty, Int32.MinValue, WhereClause, false );
                }
                else
                {
                    string Sql = "select " + _PkColumnName + "," + _NameColumnName +
                                 "  from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtMetaDataResources.CswNbtResources, _TableSelect.TableName, Date ) + " " + _TableSelect.TableName + " ";
                    addModuleWhereClause( ref Where );
                    Sql += Where;

                    CswArbitrarySelect AuditSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswArbitrarySelect( "MetaDataCollectionImpl_getpkdict_audit_select", Sql );
                    Table = AuditSelect.getTable();
                }
                Dictionary<Int32, string> Coll = new Dictionary<Int32, string>();
                foreach( DataRow Row in Table.Rows )
                {
                    Coll.Add( CswConvert.ToInt32( Row[_PkColumnName] ), CswConvert.ToString( Row[_NameColumnName] ) );
                }
                _PkDictsWhere[Key] = Coll;
            }
            return _PkDictsWhere[Key];
        } // _PkDictsWhere(Where)

        private Dictionary<Int32, ICswNbtMetaDataObject> _ByPk = null;
        public ICswNbtMetaDataObject getByPk( Int32 Pk )
        {
            ICswNbtMetaDataObject ret = null;
            if( Pk != Int32.MinValue )
            {
                if( _ByPk == null )
                {
                    _ByPk = new Dictionary<Int32, ICswNbtMetaDataObject>();
                }
                if( false == _ByPk.ContainsKey( Pk ) )
                {
                    string WhereClause = string.Empty;
                    addModuleWhereClause( ref WhereClause );

                    DataTable Table = _TableUpdate.getTable( _PkColumnName, Pk, WhereClause, false );
                    if( Table.Rows.Count > 0 )
                    {
                        _ByPk[Pk] = _makeObj( Table.Rows[0] );
                    }
                    else
                    {
                        _ByPk[Pk] = null;
                    }
                }
                ret = _ByPk[Pk];
            } // if( Pk != Int32.MinValue )
            return ret;
        } // getByPk()

        public ICswNbtMetaDataObject getByPk( Int32 Pk, CswDateTime Date )
        {
            ICswNbtMetaDataObject ret = null;
            if( false == CswTools.IsDate( Date ) )
            {
                ret = getByPk( Pk );
            }
            else
            {
                string Sql = "select * from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtMetaDataResources.CswNbtResources, _TableSelect.TableName, Date ) + " " + _TableSelect.TableName;
                string Where = " where " + _PkColumnName + " = " + Pk.ToString();
                addModuleWhereClause( ref Where );
                Sql += Where;

                CswArbitrarySelect AuditSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswArbitrarySelect( "MetaDataCollectionImpl_getbypk_audit_select", Sql );
                DataTable Table = AuditSelect.getTable();
                if( Table.Rows.Count > 0 )
                {
                    ret = _makeObj( Table.Rows[0], Date, useCache: false );
                }
            }
            return ret;
        }

        private Dictionary<string, Collection<ICswNbtMetaDataObject>> _getWhere = null;
        public Collection<ICswNbtMetaDataObject> getWhere( string Where )
        {
            if( _getWhere == null )
            {
                _getWhere = new Dictionary<string, Collection<ICswNbtMetaDataObject>>();
            }
            if( false == _getWhere.ContainsKey( Where ) )
            {
                string WhereClause = Where;
                addModuleWhereClause( ref WhereClause );

                DataTable Table = _TableUpdate.getTable( WhereClause );

                _getWhere[Where] = _makeObjs( Table );
            }
            return _getWhere[Where];
        } // getWhere(Where)

        public Collection<ICswNbtMetaDataObject> getWhere( string Where, CswDateTime Date )
        {
            Collection<ICswNbtMetaDataObject> ret = null;
            if( false == CswTools.IsDate( Date ) )
            {
                ret = getWhere( Where );
            }
            else
            {
                string Sql = "select * from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtMetaDataResources.CswNbtResources, _TableSelect.TableName, Date ) + " " + _TableSelect.TableName + " " + Where;
                addModuleWhereClause( ref Sql );

                CswArbitrarySelect AuditSelect = _CswNbtMetaDataResources.CswNbtResources.makeCswArbitrarySelect( "MetaDataCollectionImpl_getWhere_audit_select", Sql );
                DataTable Table = AuditSelect.getTable();
                ret = _makeObjs( Table, Date, useCache: false );
            }
            return ret;
        } // getWhere(Where,Date)

        public ICswNbtMetaDataObject getWhereFirst( string WhereClause, CswDateTime Date = null )
        {
            ICswNbtMetaDataObject ret = null;
            Collection<ICswNbtMetaDataObject> Coll = getWhere( WhereClause, Date );
            if( Coll.Count > 0 )
            {
                ret = Coll[0];
            }
            return ret;
        } // getWhereFirst()

        private Dictionary<string, string> _getNameWhere = null;
        public string getNameWhereFirst( string Where )
        {
            if( _getNameWhere == null )
            {
                _getNameWhere = new Dictionary<string, string>();
            }
            if( false == _getNameWhere.ContainsKey( Where ) )
            {
                CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
                SelectCols.Add( _NameColumnName );

                string WhereClause = Where;
                addModuleWhereClause( ref WhereClause );

                DataTable Table = _TableSelect.getTable( SelectCols, string.Empty, Int32.MinValue, WhereClause, false );
                if( Table.Rows.Count > 0 )
                {
                    _getNameWhere[Where] = Table.Rows[0][_NameColumnName].ToString();
                }
                else
                {
                    _getNameWhere[Where] = string.Empty;
                }
            }
            return _getNameWhere[Where];
        } // getNameWhereFirst()


    } // public class CswNbtMetaDataCollectionImpl
} // namespace ChemSW.Nbt.MetaData
