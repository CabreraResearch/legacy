//using System;
//using System.Data;
//using System.Collections;
//using System.Reflection;
//using System.Text.RegularExpressions;
//using ChemSW.Core;

//namespace ChemSW.Nbt.TableEvents
//{

//    public class CswNbtTblFactory : CswDnTblFactory
//    {
//        //CswDnTblFactory _CswDnTblFactory = null;
//        private CswNbtResources _CswNbtResources;
//        public CswNbtTblFactory( CswNbtResources CswNbtResources )
//        {
//            _CswNbtResources = CswNbtResources;
//        }//ctor


//        public DataTable makeDnTable( string TableName )
//        {
//            DataTable ReturnVal = null;

//            //First get the generic table
//            ReturnVal = _CswNbtResources.CswResources.makeDnTable( TableName );

//            //node type events
//            if( _LoadTableSpecificEvents )
//            {
//                /*
//                if( "nodetypes" == TableName.ToLower() )
//                {
//                    CswTblEvntRowChg CswTblEvntRowChg = new CswNbtTblEvntRowChgNodeTypes();
//                    CswTblEvntRowChg.setCswDataObjects( _CswDataObjects );
//                    CswTblEvntRowChg.setCswTblFactory( this );

//                    DataRowChangeEventHandler DataRowChangeEventHandler = new DataRowChangeEventHandler( CswTblEvntRowChg.DataTableRowChangedEventHandler );
//                    ReturnVal.RowChanged += DataRowChangeEventHandler;
//                }//if table is nodetypes
//                 * */
//            }//if we're loading table events

//            return ( ReturnVal );

//        }//makeDnTable()

//        public void setFillMetaData( bool FillMetaData )
//        {
//            _CswNbtResources.CswResources.setFillMetaData( FillMetaData );
//        }//setFillMetaData()

//        bool _LoadTableSpecificEvents = true;
//        public void setLoadTableSpecificEvents( bool LoadTableSpecificEvents )
//        {
//            _LoadTableSpecificEvents = LoadTableSpecificEvents;
//            _CswNbtResources.CswResources.setLoadTableSpecificEvents( LoadTableSpecificEvents );
//        }//setLoadTableSpecificEvents()

//        public void setLoadGenericEvents( bool LoadGenericEvents )
//        {
//            _CswNbtResources.CswResources.setLoadGenericEvents( LoadGenericEvents );
//        }//setLoadGenericEvents()

//        public void unLoadTableEvents( DataTable DataTable ) { _CswNbtResources.CswResources.unLoadTableEvents( DataTable ); }
//        public void reLoadTableEvents( DataTable DataTable ) { _CswNbtResources.CswResources.reLoadTableEvents( DataTable ); }


//    }//class CswDnTblFactory

//}//namespace ChemSW.Nbt.TableEvents


