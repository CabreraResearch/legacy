//using System;
//using System.Collections.Generic;
//using System.Collections;
//using System.Text;
//using System.Data;
//using System.Xml;
//using System.Xml.XPath;
//using ChemSW.Core;
//using ChemSW.Exceptions;


//namespace ChemSW.Nbt
//{
//    public class CswNbtTreeLoaderFromSearch : CswNbtTreeLoader
//    {

//        #region Private data members

//        private string _className = "CswNbtTreeLoaderFromSearchString";

//        private CswNbtResources _CswNbtResources = null;

//        private CswNbtSearch _NbtSearch;

//        #endregion

//        private bool _IncludeSystemNodes = false;

//        public CswNbtTreeLoaderFromSearch( CswNbtResources CswNbtResources, ICswNbtTree pCswNbtTree, CswNbtSearch NbtSearch )
//            : base( pCswNbtTree )
//        {
//            _CswNbtResources = CswNbtResources;
//            _NbtSearch = NbtSearch;
//        }//ctor

//        public override void load(ref CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship ChildRelationshipToStartWith, Int32 PageSize, bool FetchAllPrior, bool SingleLevelOnly, CswNbtNodeKey IncludedKey)
//        {
//        //    throw new CswDniException("Partial load not implemented yet for CswNbtTreeLoaderFromSearch");
//        //}

//        //public override void load(Int32 NodeCountLowerBoundExclusive, Int32 NodeCountUpperBoundInclusive)
//        //{
//            if (PageSize > 0)
//                throw new CswDniException("Paged load not implemented yet for CswNbtTreeLoaderFromSearch");
            
//            if(IncludedKey != null)
//                throw new CswDniException("IncludedKey load not implemented yet for CswNbtTreeLoaderFromSearch");

//            string Context = "load()";
//            try
//            {
//                CswTimer _Timer = new CswTimer();

//                _CswNbtTree.goToRoot();
//                _build();

//                _CswNbtResources.logTimerResult( "Built tree from search: " + _NbtSearch.Value + " (" + _NbtSearch.SearchType.ToString() + ")", _Timer.ElapsedDurationInSecondsAsString );
//            }
//            catch( Exception e )
//            {
//                throw new System.Exception( _makeGenericExceptionText( Context, e.Message ) );
//            }

//        } // load()

//        private void _build()
//        {
//            try
//            {
//                DataTable ResultTable = new DataTable();

//                // query for all nodes with properties with values that match search
//                CswDbResources DbResources = _CswNbtResources.CswDbResources;

//                string Sql = @"select distinct n.nodeid,
//       n.nodename,
//       t.iconfilename,
//       t.nodetypename,
//       t.nametemplate,
//       t.nodetypeid,
//       o.objectclass,
//       o.objectclassid,
//       lower(n.nodename) mssqlorder 
//  from nodes n
//  join nodetypes t on (n.nodetypeid = t.nodetypeid)
//  join object_class o on (t.objectclassid = o.objectclassid)
//  left outer join nodetype_props ntp on (t.nodetypeid = ntp.nodetypeid)
//  left outer join object_class_props ocp on (ntp.objectclasspropid = ocp.objectclasspropid)
//  left outer join jct_nodes_props jnp on (jnp.nodetypepropid = ntp.nodetypepropid and
//                                         n.nodeid = jnp.nodeid)";

//                string Where = "";

//                if (_NbtSearch.SearchType == NbtQuickSearchType.Barcode)
//                {
//                    Sql += " left outer join field_types f on ntp.fieldtypeid = f.fieldtypeid where f.fieldtype = 'Barcode' and lower(jnp.gestalt) like lower('%" + _NbtSearch.Value + "%') ";
//                }
//                else
//                {
//                    Where += " lower(jnp.gestalt) like lower('%" + _NbtSearch.Value + "%') ";
//                }

//                // BZ 6008
//                if (!_IncludeSystemNodes)
//                {
//                    if (Where != string.Empty)
//                        Where += " and ";
//                    Where += " n.issystem = '0' ";
//                }

//                if(Where != string.Empty)
//                    Sql += " where " + Where;
//                Sql += " order by lower(n.nodename)";

//                DbResources.setSqlSelectText( Sql );
//                try
//                {
//                    _CswNbtResources.CswDbResources.DataAdapter.Fill( ResultTable );
//                }
//                catch( Exception ex )
//                {
//                    throw new CswDniException( "Invalid Search", "CswNbtTreeLoaderFromSearch._build() attempted to run invalid SQL: " + Sql, ex );
//                }


//                // build tree from results
//                //CswNbtView TheView = _CswNbtResources.ViewCache.getView(_CswNbtTree.Key.SessionViewId);
//                //if (TheView.ViewName == string.Empty)
//                //{
//                //    TheView.SetViewName("Search Results for '" + _NbtSearch.Value + "' (" + _NbtSearch.SearchType.ToString() + ")");
//                //}
//                _CswNbtTree.makeRootNode( "Images/view/view.gif", true, NbtViewAddChildrenSetting.None );
//                _CswNbtTree.goToRoot();
//                Int32 NodeCount = 0;
//                foreach( DataRow CurrentRow in ResultTable.Rows )
//                {
//                    NodeCount++;
//                    if( _CswNbtResources.CurrentUser.CheckPermission( NodeTypePermission.View, CswConvert.ToInt32( CurrentRow[ "nodetypeid" ] ), null, null ) )
//                        _CswNbtTree.loadNodeAsChildFromRow(null, CurrentRow, "", true, true, true, NbtViewAddChildrenSetting.None, NodeCount);
//                }
//            }
//            catch( Exception e )
//            {
//                throw new System.Exception( _makeGenericExceptionText( "CswNbtTreeLoaderFromSearch._build()", e.Message ) );
//            }

//        }

//        #region Helper functions

//        private string _makeGenericExceptionText( string Context, string Message )
//        {
//            return ( "Exception in " + _className + "::" + Context + ": " + Message );
//        }

//        #endregion

//    }//CswNbtTreeLoader

//}//namespace ChemSW.Nbt
