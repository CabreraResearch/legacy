//using System;
//using System.Collections.Generic;
//using System.Text;
//using ChemSW.Nbt.Security;

//namespace ChemSW.Nbt
//{
//    public class CswNbtTreeLoaderFactory
//    {
//        protected ICswNbtTree _pCswNbtTree;

//        CswNbtResources _CswNbtResources = null;

//        public CswNbtTreeLoaderFactory( CswNbtResources CswNbtResources )
//        {
//            //_CswDbResources = CswDbResources;
//            _CswNbtResources = CswNbtResources;
//        }//ctor

//        public CswNbtTreeLoader makeTreeLoaderFromXmlView( ICswNbtUser RunAsUser, ICswNbtTree CswNbtTree, CswNbtView View, bool IncludeSystemNodes )
//        {
//            return ( new CswNbtTreeLoaderFromXmlView( _CswNbtResources, RunAsUser, CswNbtTree, View, IncludeSystemNodes ) );
//        }//makeTreeLoaderFromXmlView()

//        //public CswNbtTreeLoader makeTreeLoaderFromXmlViewDsTest(ICswNbtTree CswNbtTree, string TreeViewDoc)
//        //{
//        //    return (new CswNbtTreeLoaderFromXmlViewDsTest(_CswDbResources, CswNbtTree, TreeViewDoc));
//        //}//makeTreeLoaderFromXmlView()

//        //public CswNbtTreeLoader makeTreeLoaderFromSearchString( ICswNbtTree CswNbtTree, CswNbtSearch NbtSearch )
//        //{
//        //    return ( new CswNbtTreeLoaderFromSearch( _CswNbtResources, CswNbtTree, NbtSearch ) );
//        //}//makeTreeLoaderFromSearchString()


//    }//CswNbtTreeLoaderFactory

//}//namespace ChemSW.Nbt
