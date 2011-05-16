using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{
    public class CswNbtTreeCache
    {
        CswNbtResources _CswNbtResources = null;
        CswNbtTreeBuilder _CswNbtTreeBuilder = null;

        Hashtable _TreeCache = new Hashtable();
        //Hashtable _ArbitraryTreeIdsByDisplayName = new Hashtable();
        //Hashtable _ArbitraryTreeIdsByViewId = new Hashtable();

        //        CswConfigurationFileLocation _CswConfigurationFileLocation = null;
        //Int32 _ArbitraryTreeId = 0;

        //public CswNbtTreeCache( CswNbtResources CswNbtResources, CswConfigurationFileLocation CswConfigurationFileLocation, ICswNbtTreeFactory CswNbtTreeFactory ) 
        public CswNbtTreeCache( CswNbtResources CswNbtResources, ICswNbtTreeFactory CswNbtTreeFactory )
        {
            _CswNbtResources = CswNbtResources;

            _CswNbtTreeBuilder = new CswNbtTreeBuilder( _CswNbtResources, CswNbtTreeFactory );

        }//ctor


        public void clear()
        {
            _TreeCache.Clear();
        }//clear()

        private void _catalogueTree( ICswNbtTree CswNbtTree )
        {
            if( _TreeCache.ContainsKey( CswNbtTree.Key ) )
                _TreeCache[CswNbtTree.Key] = CswNbtTree;
            else
                _TreeCache.Add( CswNbtTree.Key, CswNbtTree );

            //if( CswNbtTree.Key.View.ViewId > 0 )
            //{
            //    _ArbitraryTreeIdsByViewId[CswNbtTree.Key.View.ViewId] = CswNbtTree.Key.ArbitraryTreeId;
            //}

        }//_catalogueTree()

        //private ICswNbtTree _getTreeFromCatalogue(Int32 ViewId)
        //{
        //    ICswNbtTree CswNbtTree = null;
        //    foreach (CswNbtTreeKey Key in _TreeCache.Keys)
        //    {
        //        if (_CswNbtResources.ViewCache.getView(Key.SessionViewId).ViewId == ViewId)
        //            CswNbtTree = (ICswNbtTree)_TreeCache[Key];
        //    }

        //    return (CswNbtTree);

        //}//_getTreeFromCatalogue()
        private ICswNbtTree _getTreeFromCatalogue( CswNbtView View )
        {
            ICswNbtTree CswNbtTree = null;
            foreach( CswNbtTreeKey Key in _TreeCache.Keys )
            {
                CswNbtView CacheView = _CswNbtResources.ViewSelect.getSessionView( Key.SessionViewId );
                if( CacheView == View )
                    CswNbtTree = (ICswNbtTree) _TreeCache[Key];
            }

            return ( CswNbtTree );

        }//_getTreeFromCatalogue()


        public ICswNbtTree getTreeFromCache( CswNbtTreeKey CswNbtTreeKey )
        {
            ICswNbtTree Ret = null;
            if( !_TreeCache.ContainsKey( CswNbtTreeKey ) )
            {
				CswNbtView View = _CswNbtResources.ViewSelect.getSessionView( CswNbtTreeKey.SessionViewId );
                if( View != null )
                    getTreeFromView( View, false, true, false, false );
            }
            if( _TreeCache.ContainsKey( CswNbtTreeKey ) )
                Ret = (ICswNbtTree) _TreeCache[CswNbtTreeKey];

            return Ret;

        }//getTreeFromCache()

        public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, bool FetchAllPrior, bool SingleLevelOnly, bool IncludeSystemNodes )
        {
            CswNbtNodeKey ParentNodeKey = null;
            return getTreeFromView( View, ForceReInit, ref ParentNodeKey, null, Int32.MinValue, FetchAllPrior, SingleLevelOnly, null, IncludeSystemNodes );
        }

        public ICswNbtTree getTreeFromView( ICswNbtUser RunAsUser, CswNbtView View, bool ForceReInit, bool FetchAllPrior, bool SingleLevelOnly, bool IncludeSystemNodes )
        {
            CswNbtNodeKey ParentNodeKey = null;
            return getTreeFromView( RunAsUser, View, ForceReInit, ref ParentNodeKey, null, Int32.MinValue, FetchAllPrior, SingleLevelOnly, null, IncludeSystemNodes );
        }

        public ICswNbtTree getTreeFromView( CswNbtView View, bool ForceReInit, ref CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship ChildRelationshipToStartWith, Int32 PageSize, bool FetchAllPrior, bool SingleLevelOnly, CswNbtNodeKey IncludedKey, bool IncludeSystemNodes )
        {
            return getTreeFromView( _CswNbtResources.CurrentNbtUser, View, ForceReInit, ref ParentNodeKey, ChildRelationshipToStartWith, PageSize, FetchAllPrior, SingleLevelOnly, IncludedKey, IncludeSystemNodes );
        }

        public ICswNbtTree getTreeFromView( ICswNbtUser RunAsUser, CswNbtView View, bool ForceReInit, ref CswNbtNodeKey ParentNodeKey, CswNbtViewRelationship ChildRelationshipToStartWith, Int32 PageSize, bool FetchAllPrior, bool SingleLevelOnly, CswNbtNodeKey IncludedKey, bool IncludeSystemNodes )
        {
            // BZ 10094
            // We don't use the tree catalog if the RunAsUser is not the CurrentUser
            CswTimer getTreeFromViewTimer = new CswTimer();
            ICswNbtTree ReturnVal = null;

            View.SaveToCache();  // BZ 8502

            if( RunAsUser == _CswNbtResources.CurrentNbtUser )
            {
                ReturnVal = _getTreeFromCatalogue( View );
            }

            if( ForceReInit || ReturnVal == null )
            {
                CswNbtTreeKey CurrentKey;
                if( ReturnVal == null )
                    CurrentKey = new CswNbtTreeKey( _CswNbtResources, View.SessionViewId );
                else
                    CurrentKey = ReturnVal.Key;
                ReturnVal = _CswNbtTreeBuilder.getTreeFromView( RunAsUser, CurrentKey, (CswNbtView) View, ref ParentNodeKey, ChildRelationshipToStartWith, PageSize, FetchAllPrior, SingleLevelOnly, IncludedKey, IncludeSystemNodes );
                ReturnVal.Key.SessionViewId = View.SessionViewId;

                if( RunAsUser == _CswNbtResources.CurrentNbtUser )
                    _catalogueTree( ReturnVal );
            }

            _CswNbtResources.logTimerResult( "CswNbtTreeCache.getTreeFromView()", getTreeFromViewTimer.ElapsedDurationInSecondsAsString );

            return ( ReturnVal );

        }//getTreeFromView()

        //public ICswNbtTree getTreeFromSearchString(CswNbtSearch NbtSearch, bool ForceReInit)
        //{
        //    ICswNbtTree ReturnVal = null;

        //    ReturnVal = _getTreeFromCatalogue(NbtSearch);
        //    if (ForceReInit || ReturnVal == null)
        //    {
        //        if (NbtSearch.Value == "")
        //        {
        //            throw new CswDniException("search string is invalid", "NbtSearch.Value parameter is empty");
        //        }

        //        CswNbtTreeKey CurrentKey = new CswNbtTreeKey( _CswNbtResources );

        //        //client-supplied display name takes precedence over the view-supplied display name
        //        //if( string.Empty != ViewDisplayName )
        //        //{
        //        //    CurrentKey.ViewDisplayName = ViewDisplayName;
        //        //}

        //        _catalogueTree(ReturnVal = _CswNbtTreeBuilder.getTreeFromSearchString(CurrentKey, NbtSearch));
        //    }//

        //    return ( ReturnVal );

        //}//getTreeFromSearchString()


        public CswNbtView getTreeViewOfNodeType( Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "getTreeViewOfNodeType(" + NodeTypeId + ")";
            View.AddViewRelationship( NodeType, true );

            return ( View );

        }//getTreeViewOfNodeType()

        public CswNbtView getTreeViewOfObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass )
        {
            CswNbtMetaDataObjectClass MetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClass );

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "getTreeViewOfObjectClass(" + ObjectClass.ToString() + ")";
            View.AddViewRelationship( MetaDataObjectClass, true );

            return ( View );

        }//getTreeViewOfObjectClass()

        public CswNbtView getTreeViewOfFilteredObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass, string ObjClassPropName, string ObjClassPropVal )
        {
            CswNbtView View = getTreeViewOfObjectClass( ObjectClass );
            CswNbtMetaDataObjectClass ArbitraryObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClass );

            CswNbtViewProperty ArbitraryObjectClassProp = View.AddViewProperty( ( (CswNbtViewRelationship) View.Root.ChildRelationships[0] ),
                                                                                ArbitraryObjectClass.getObjectClassProp( ObjClassPropName ) );
            //ArbitraryObjectClassProp.ObjectClassPropId = ArbitraryObjectClass.getObjectClassProp( ObjClassPropName ).PropId;
            //ArbitraryObjectClassProp.Type = CswNbtViewProperty.CswNbtPropType.ObjectClassPropId;

            //( ( CswNbtViewRelationship ) View.Root.ChildRelationships[ 0 ] ).addProperty( ArbitraryObjectClassProp );

            //            _addFilteredProperty( ArbitraryObjectClassProp, ObjClassPropVal );

            return ( View );

        }//getTreeViewOfFilteredObjectClass()


        private void _addProp( CswNbtView CswNbtView, string ObjClassName )
        {
        }//_addProp()

        private void _addPropFilter( CswNbtViewProperty CswNbtViewProperty, string Value )
        {

            CswNbtViewPropertyFilter ArbitraryPropFilter = CswNbtViewProperty.View.AddViewPropertyFilter( CswNbtViewProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, Value, false );
            //ArbitraryPropFilter.Value = Value;
            //ArbitraryPropFilter.FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Equals;
            //CswNbtViewProperty.addFilter( ArbitraryPropFilter );

        }//_addFilteredProperty()

        public ICswNbtTree getTreeFromNodeTypeId( Int32 NodeTypeId )
        {
            ICswNbtTree ReturnVal = null;

            CswNbtView View = getTreeViewOfNodeType( NodeTypeId );
            _catalogueTree( ReturnVal = getTreeFromView( View, true, true, false, false ) );

            return ( ReturnVal );

        }//getTreeFromNodeTypeId()

        public ICswNbtTree getTreeFromObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass )
        {
            ICswNbtTree ReturnVal = null;

            CswNbtView View = getTreeViewOfObjectClass( ObjectClass );
            _catalogueTree( ReturnVal = getTreeFromView( View, true, true, false, false ) );

            return ( ReturnVal );

        }//getTreeFromObjectClass()

        public ICswNbtTree getTreeFromFilteredObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass, string ObjClassPropName, string ObjClassPropVal )
        {
            ICswNbtTree ReturnVal = null;

            CswNbtView View = getTreeViewOfFilteredObjectClass( ObjectClass, ObjClassPropName, ObjClassPropVal );
            _catalogueTree( ReturnVal = getTreeFromView( View, true, true, false, false ) );

            return ( ReturnVal );

        }//getTreeFromObjectClass()

    }//CswNbtTreeCache

}//namespace ChemSW.Nbt
