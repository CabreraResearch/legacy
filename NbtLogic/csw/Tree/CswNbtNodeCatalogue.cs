//using System;
//using System.Collections.Generic;
//using System.Collections;
//using System.Text;

//namespace ChemSW.Nbt
//{

//    public class CswNbtNodeCatalogue
//    {
////        IComparer _CswComparerByTableRow = new CswComparerByTableRow();
////        IComparer _CswComparerByInstanceId = new CswComparerByInstanceId();
//        //        IComparer _CswComparerByTableAndNodeId = new CswComparerByTableAndNodeId();
//        IComparer _CswComparerByNodeId = new CswComparerByNodeId();
//        IComparer _CswComparerByPath = new CswComparerByPath();
//        public CswNbtNodeCatalogue() 
//        {
//        }//ctor

//        ArrayList _MainKeyList = new ArrayList();
//        public void addNodeKey( CswNbtNodeKey CswNbtNodeKey )
//        {
//            _MainKeyList.Add( CswNbtNodeKey );

//        }//addNode()

//        public void removeNodeKey( CswNbtNodeKey CswNbtNodeKey )
//        {
//            if( ! _MainKeyList.Contains( CswNbtNodeKey ) )
//                throw( new System.Exception( "Specified key does not exist " + CswNbtNodeKey.ToString() ) );

//            _MainKeyList.Remove( CswNbtNodeKey );
//        }//addNode()

//        public bool isCatalogued( CswNbtNodeKey CswNbtNodeKey )
//        {
//            return( _MainKeyList.Contains( CswNbtNodeKey ) );
//        }//isCatalogued()


//        private bool _getNodeKeyForComparer( CswNbtNodeKey CswNbtNodeKeyToCompare,  ref CswNbtNodeKey CswNbtNodeKeyToRetrieve, IComparer Comparer )
//        {
//            bool ReturnVal = false;
//            CswNbtNodeKeyToRetrieve = null;

//            _MainKeyList.Sort( Comparer );

//            int CandidateIdx = -1;
//            if( ( CandidateIdx = _MainKeyList.BinarySearch( CswNbtNodeKeyToCompare, Comparer ) ) > -1 )
//            {
//                ReturnVal = true;
//                CswNbtNodeKeyToRetrieve = ( CswNbtNodeKey ) _MainKeyList[ CandidateIdx ];
//            }//

//            return ( ReturnVal );
//        }//


//        public bool getKeysForNodeId( Int32 NodeId, ref ArrayList KeyList )
//        {
//            bool ReturnVal = false;

//            _MainKeyList.Sort( _CswComparerByNodeId );

//            CswNbtNodeKey NodeKeyToCompare = new CswNbtNodeKey( null, "", NodeId, 0, 0, NodeSpecies.Plain );


//            int CurrentIdx = -1;
//            if( ( CurrentIdx = _MainKeyList.BinarySearch( NodeKeyToCompare, _CswComparerByNodeId ) ) > -1 )
//            {
//                ReturnVal = true;
//                CswNbtNodeKey CurrentKey = null;
//                int TotalKeys = _MainKeyList.Count;
//                while( ( CurrentIdx < TotalKeys ) && ( CurrentKey = _MainKeyList[ CurrentIdx ] as CswNbtNodeKey ).NodeId == NodeId )
//                {
//                    KeyList.Add( CurrentKey );
//                    CurrentIdx++;
//                }
//            }//if we found any instance of the nodekey

//            //            throw( new System.Exception( "Dude, so, like, this needs to be refactored?" ) );

//            return ( ReturnVal );

//        }//getKeysForNodeId()


//        public class CswComparerByPath : IComparer
//        {
//            #region IComparer Members

//            public int Compare( object x, object y )
//            {
//                //Return val is:
//                // < 0: x is less than y
//                //   0: x equals y
//                // > 0: x is greater than y
//                int ReturnVal = -1;

//                CswNbtNodeKey NodeKeyX = ( CswNbtNodeKey ) x;
//                CswNbtNodeKey NodeKeyY = ( CswNbtNodeKey ) y;

//                ReturnVal = NodeKeyX.TreePath.CompareTo( NodeKeyY.TreePath );
//                /*
//                 * 0: X == Y
//                 * Less than 0: X is less than Y
//                 * Greater than 0: X is greater than Y
//                 */

//                return ( ReturnVal );

//            }//Compare()

//        }//class CswComparerByTableRow

//            #endregion
//        /*
//        public class CswComparerByInstanceId : IComparer
//        {
//            #region IComparer Members

//            public int Compare( object x, object y )
//            {
//                //Return val is:
//                // < 0: x is less than y
//                //   0: x equals y
//                // > 0: x is greater than y
//                int ReturnVal = -1;

//                CswNbtNodeKey NodeKeyX = ( CswNbtNodeKey ) x;
//                CswNbtNodeKey NodeKeyY = ( CswNbtNodeKey ) y;

//                if(
//                      NodeKeyX.NodeId == NodeKeyY.NodeId &&
//                      NodeKeyX.NodeInstanceId == NodeKeyY.NodeInstanceId
//                    )
//                {
//                    ReturnVal = 0;
//                }
//                else if( NodeKeyX.NodeId > NodeKeyY.NodeId )
//                {
//                    ReturnVal = 1;
//                }
//                else if( NodeKeyX.NodeId < NodeKeyY.NodeId )
//                {
//                    ReturnVal = -1;
//                }
//                else
//                {
//                    if( NodeKeyX.NodeInstanceId > NodeKeyY.NodeInstanceId )
//                    {
//                        ReturnVal = 1;
//                    }
//                    else
//                    {
//                        ReturnVal = -1;
//                    }//
//                }//if-else

//                return ( ReturnVal );

//            }//Compare()

//        }//CswComparerByInstanceId()
//         */

//        /*
//        public class CswComparerByTableAndNodeId : IComparer
//        {

//            public int Compare( object x, object y )
//            {
//                //Return val is:
//                // < 0: x is less than y
//                //   0: x equals y
//                // > 0: x is greater than y
//                int ReturnVal = -1;

//                CswNbtNodeKey NodeKeyX = ( CswNbtNodeKey ) x;
//                CswNbtNodeKey NodeKeyY = ( CswNbtNodeKey ) y;

//                if(
//                      NodeKeyX.TableLevel == NodeKeyY.TableLevel &&
//                      NodeKeyX.NodeId == NodeKeyY.NodeId
//                    )
//                {
//                    ReturnVal = 0;
//                }
//                else if( NodeKeyX.TableLevel > NodeKeyY.TableLevel )
//                {
//                    ReturnVal = 1;
//                }
//                else if( NodeKeyX.TableLevel < NodeKeyY.TableLevel )
//                {
//                    ReturnVal = -1;
//                }
//                else
//                {
//                    if( NodeKeyX.NodeId > NodeKeyY.NodeId )
//                    {
//                        ReturnVal = 1;
//                    }
//                    else
//                    {
//                        ReturnVal = -1;
//                    }//
//                }//if-else

//                return ( ReturnVal );

//            }//Compare()

//        }//CswComparerByTableAndNodeId()
//         */

//        public class CswComparerByNodeId : IComparer
//        {

//            public int Compare( object x, object y )
//            {
//                //Return val is:
//                // < 0: x is less than y
//                //   0: x equals y
//                // > 0: x is greater than y
//                int ReturnVal = -1;

//                CswNbtNodeKey NodeKeyX = ( CswNbtNodeKey ) x;
//                CswNbtNodeKey NodeKeyY = ( CswNbtNodeKey ) y;

//                if(
//                      NodeKeyX.NodeId == NodeKeyY.NodeId
//                    )
//                {
//                    ReturnVal = 0;
//                }
//                else
//                {
//                    if( NodeKeyX.NodeId > NodeKeyY.NodeId )
//                    {
//                        ReturnVal = 1;
//                    }
//                    else
//                    {
//                        ReturnVal = -1;
//                    }//
//                }//if-else

//                return ( ReturnVal );

//            }//Compare()

//        }//CswComparerByNodeId()

//    }//CswNbtNodeCatalogue

//}//namespace ChemSW.Nbt
