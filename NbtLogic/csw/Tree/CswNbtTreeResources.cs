//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ChemSW.Nbt
//{
//    public class CswNbtTreeResources
//    {
//        private CswNbtResources _CswNbtResources;

//        public CswNbtTreeResources( CswNbtResources CswNbtResources ) 
//        {
//            _CswNbtResources = CswNbtResources;
//        }//ctor
//        /*
//        public CswNbtTree getCswNbtTreeFromXml( string TreeView )
//        {
//            CswNbtTree ReturnVal = new CswNbtTree( _CswDataObjects );
//            CswNbtTreeLoader CswNbtTreeLoader = new CswNbtTreeLoaderFromXmlViewDsTest( _CswDataObjects, ReturnVal, TreeView );
//            CswNbtTreeLoader.load();
//            return( ReturnVal ); 

//        }//getCswNbtTreeFromXml() 
//    */


//        /*
//                CswNbtTree * getCswNbtTreeEmpty() throw( CSWException );
//                CswNbtTree * getCswNbtTreeFromXml( const CSWString & TreeView ) throw( CSWException );
//                CswNbtTree * getCswNbtTreeFromNodeTypeSetId( const unsigned int NodeTypeSetId ) throw( CSWException );
//                CswNbtTree * getCswNbtTreeFromNodeVector( tpNodeParentMap * pNPMap , NbtTreeMode TreeMode ) throw( CSWException );
//                CswNbtTree * getCswNbtTreeFull( DWORD RootNodeId , NbtTreeMode TreeMode, CSWString ObjectClassNames ) throw( CSWException );
//                CswNbtTree * getCswNbtTreeGeometry( DWORD RootNodeId , unsigned int iDepth, NbtTreeMode TreeMode ) throw( CSWException );
//                CswNbtTree * getCswNbtTreeAudit( DWORD RootNodeId , NbtTreeMode TreeMode ) throw( CSWException );

//                CswLogger * getCswLogger() throw( CSWException );

//                ICswNbtTreeIterator * getCswNbtTreeIterator( CswNbtTree * pCswNbtTree , NbtIterationMode imMode ) throw( CSWException );

//                CSWString getNodeTypeNameFromId( unsigned int NodeTypeId ) throw( CSWException );
//                unsigned int getObjectClassIdFromName( CSWString ObjectClassName ) throw( CSWException );

//                CswUser * getCswUser() throw( CSWException );
//                CswNbtTreePermissions * getCswNbtTreePermissions() throw( CSWException );
//        * 
//         * */

//    }//CswNbtTreeResources

//}//namespace ChemSW.Nbt
