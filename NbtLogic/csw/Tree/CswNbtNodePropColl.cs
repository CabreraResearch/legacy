using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{

    public class CswNbtNodePropColl : IEnumerable, IEnumerable<CswNbtNodePropWrapper>
    {
        private ArrayList _Props = new ArrayList();
        private Hashtable _PropsIndexByFirstVersionPropId = new Hashtable();
        private CswNbtMetaDataNodeType _NodeType { get { return _CswNbtResources.MetaData.getNodeType( _NodeTypeId ); } }
        private CswNbtResources _CswNbtResources = null;
        private CswNbtNode _CswNbtNode = null;
        private CswNbtNodePropCollDataNative _CswNbtNodePropCollDataNative = null;
        private CswNbtNodePropCollDataRelational _CswNbtNodePropCollDataRelational = null;
        private CswNbtMetaDataNodeTypeTab _CswNbtMetaDataNodeTypeTab = null;

        public CswNbtNodePropColl( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode, CswNbtMetaDataNodeTypeTab CswNbtMetaDataNodeTypeTab )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNode = CswNbtNode;
            _CswNbtMetaDataNodeTypeTab = CswNbtMetaDataNodeTypeTab;
        
        }//ctor()

        private Int32 __NodeTypeId = Int32.MinValue;
        private Int32 _NodeTypeId
        {
            get { return __NodeTypeId; }
            set
            {
                __NodeTypeId = value;
                if( _CswNbtNodePropCollDataNative != null )
                    _CswNbtNodePropCollDataNative.NodeTypeId = __NodeTypeId;
                if( _CswNbtNodePropCollDataRelational != null )
                    _CswNbtNodePropCollDataRelational.NodeTypeId = __NodeTypeId;
            }
        }


        private CswPrimaryKey __NodePk = null;
        public CswPrimaryKey _NodePk
        {
            get { return __NodePk; }
            set
            {
                __NodePk = value;
                if( _CswNbtNodePropCollDataNative != null )
                    _CswNbtNodePropCollDataNative.NodePk = __NodePk;
                if( _CswNbtNodePropCollDataRelational != null )
                    _CswNbtNodePropCollDataRelational.NodePk = __NodePk;

                if( CreatedFromNodeTypeId )
                {
                    foreach( CswNbtNodePropWrapper Prop in _Props )
                        Prop.NodeId = value;
                }
            }
        }


        private ICswNbtNodePropCollData getPropCollData( string TableName, DateTime Date )
        {
            ICswNbtNodePropCollData ReturnVal = null;

            if( TableName.ToLower() == "nodes" )
            {
                if( _CswNbtNodePropCollDataNative == null )
                {
                    _CswNbtNodePropCollDataNative = new CswNbtNodePropCollDataNative( _CswNbtResources );
                    _CswNbtNodePropCollDataNative.NodePk = _NodePk;
                    _CswNbtNodePropCollDataNative.NodeTypeId = _NodeTypeId;
                    _CswNbtNodePropCollDataNative.Date = Date;
                }
                ReturnVal = _CswNbtNodePropCollDataNative;
            }
            else
            {
                if( _CswNbtNodePropCollDataRelational == null )
                {
                    _CswNbtNodePropCollDataRelational = new CswNbtNodePropCollDataRelational( _CswNbtResources );
                    _CswNbtNodePropCollDataRelational.NodePk = _NodePk;
                    _CswNbtNodePropCollDataRelational.NodeTypeId = _NodeTypeId;
                }
                ReturnVal = _CswNbtNodePropCollDataRelational;
            }
            return ( ReturnVal );
        }//getPropCollData()

        public bool Modified
        {
            get
            {
                bool ReturnVal = false;
                foreach( CswNbtNodePropWrapper CurrentProp in _Props )
                {
                    if( CurrentProp.WasModified )
                    {
                        ReturnVal = true;
                        break;
                    }//
                }//iterate props

                return ( ReturnVal );
            }//
        }//Modified


        public bool SuspendModifyTracking
        {
            get
            {
                bool ReturnVal = false;
                foreach( CswNbtNodePropWrapper CurrentProp in _Props )
                {
                    if( CurrentProp.SuspendModifyTracking )
                    {
                        ReturnVal = true;
                        break;
                    }//
                }//iterate props

                return ( ReturnVal );
            }

            set
            {
                foreach( CswNbtNodePropWrapper CurrentProp in _Props )
                {
                    CurrentProp.SuspendModifyTracking = value;
                }//iterate props
            }//
        }//SuspendModifyTracking

        public bool CreatedFromNodeTypeId = false;


        public void clearModifiedFlag()
        {
            foreach( CswNbtNodePropWrapper CurrentProp in _Props )
            {
                CurrentProp.clearModifiedFlag();
            }//iterate props

        }//_clearModifiedFlag()

        private void _clear()
        {
            _Props.Clear();

            _PropsIndexByFirstVersionPropId.Clear();
            if( _CswNbtNodePropCollDataNative != null )
                _CswNbtNodePropCollDataNative.PropsTable.Clear();
            if( _CswNbtNodePropCollDataRelational != null )
                _CswNbtNodePropCollDataRelational.PropsTable.Clear();

            _Filled = false;

        }//clear()


        private bool _Filled = false;
        public bool Filled
        {
            get
            {
                return ( _Filled );
            }//get

        }//Filled

        //private DataTable _PropsTable = null;




        public void fillFromNodePk( CswPrimaryKey NodePk, Int32 NodeTypeId, DateTime Date )
        {
            _NodePk = NodePk;
            _NodeTypeId = NodeTypeId;

            CswTimer Timer = new CswTimer();


            if( NodePk != null )
            {
                if( getPropCollData( NodePk.TableName, Date ).IsTableEmpty )
                {
                    _populateProps();
                }
                else
                {
                    _refreshProps();
                }

                _CswNbtResources.logTimerResult( "Fetched node (" + _NodePk.ToString() + ")", Timer.ElapsedDurationInSecondsAsString );

                _Filled = true;
            }

        }//fillFromNodePk()

        public int Count
        {
            get
            {
                return ( _Props.Count );
            }//Get
        }//Count


        public void fillFromNodeTypeId( Int32 NodeTypeId )
        {
            CreatedFromNodeTypeId = true;
            _clear();
            _NodeTypeId = NodeTypeId;

            //[9:35:54 AM] Phil Glaser: 
            //CswNbtNodePropColl has semantics for filling props based on a nodetypeid as opposed to a node id. How would I know whether to fill the props based on jct_nodes_props or the native CISPro table given just a nodetypeid?
            //[9:36:58 AM] Steven Salter: nodetypeid -> collection of nodetypepropids -> objectclasspropid -> data_dictionary.objectclasspropid exists?
            //[9:37:21 AM] Steven Salter: it differs per prop

            //I am assuming taht what steve describes can be baked into meta data somehow so that 
            //at this point the CswPrimeKey just has a table name, whcih is all we need here: 


            //**************** BEGIN KLUDGE ALERT
            //string InClause = string.Empty;
            //foreach ( Int32 CurrentNodeTypePropId in _CswNbtResources.MetaData.getNodeType( NodeTypeId ).NodeTypePropIds )
            //{
            //    InClause += CurrentNodeTypePropId.ToString() + ",";
            //}//
            //InClause.Remove( InClause.LastIndexOf( "," ) );
            //CswTableCaddy DataDictionaryCaddy = _CswNbtResources.makeCswTableCaddy( "data_dictionary" );
            //DataDictionaryCaddy.WhereClause = " where objectclasspropid in (" + InClause + ")";
            //DataTable DdTable = DataDictionaryCaddy.Table;


            //CswPrimaryKey NodePk = null;
            //if ( DdTable.Rows.Count > 0 )
            //{
            //    NodePk = new CswPrimaryKey( DdTable.Rows[ 0 ][ "tablename" ] );
            //}
            //else
            //{
            //    NodePk = new CswPrimaryKey( DdTable.Rows[ 0 ][ "nodes" ] );
            //}


            //**************** END KLUDGE ALERT

            //this[ NodePk ].NodeTypeId = NodeTypeId;
            ICswNbtNodePropCollData PropCollData = getPropCollData( _CswNbtResources.MetaData.getNodeType( NodeTypeId ).TableName, DateTime.MinValue );

            _populateProps(); // null, NodeTypeId );

            _Filled = true;

        }//fillFromNodeTypeId()

        private void _populateProps()// CswPrimaryKey NodePk, Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( _NodeTypeId );
            foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in MetaDataNodeType.getNodeTypeProps() )
            {
                DataRow PropRow = null;
                ICswNbtNodePropCollData PropCollData = getPropCollData( MetaDataNodeType.TableName, DateTime.MinValue );
                foreach( DataRow CurrentRow in PropCollData.PropsTable.Rows )
                {
                    if( CurrentRow["nodetypepropid"].ToString() == MetaDataProp.PropId.ToString() )
                    {
                        PropRow = CurrentRow;
                        break;
                    }
                }
                CswNbtNodePropWrapper AddedProp = CswNbtNodePropFactory.makeNodeProp( _CswNbtResources, PropRow, PropCollData.PropsTable, _CswNbtNode, MetaDataProp );
                //if( MetaDataProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode ||
                //    MetaDataProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence )
                //{
                //    AddedProp.ensureEmptyVal();
                //}

                int PropsIdx = _Props.Add( AddedProp );
                _PropsIndexByFirstVersionPropId.Add( MetaDataProp.FirstPropVersionId, PropsIdx );
                AddedProp.onNodePropRowFilled();
            }

            // This is evil -- See BZ 5963/5979
            //if(doUpdate)
            //    _PropsCaddy.update(_PropsTable);

            if( _CswNbtNode != null )
            {
                //CswNbtObjClass _CswNbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, MetaDataNodeType.ObjectClassId, _CswNbtNode );
                _CswNbtNode.ObjClass.afterPopulateProps();
            }

            SuspendModifyTracking = false;
        }//_populateProps()


        //added for bz # 8287

        private void _refreshProps()// CswPrimaryKey NodePk, Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( _NodeTypeId );
            ICswNbtNodePropCollData PropCollData = getPropCollData( MetaDataNodeType.TableName, DateTime.MinValue );
            PropCollData.refreshTable();

            foreach( DataRow CurrentRow in PropCollData.PropsTable.Rows )
            {
                CswNbtMetaDataNodeTypeProp CurrentMetaDataProp = MetaDataNodeType.getNodeTypeProp( CswConvert.ToInt32( CurrentRow["nodetypepropid"] ) );
                Int32 PropsIdx = CswConvert.ToInt32( _PropsIndexByFirstVersionPropId[CurrentMetaDataProp.FirstPropVersionId] );
                CswNbtNodePropWrapper CurrentPropWrapper = (CswNbtNodePropWrapper) _Props[PropsIdx];
                CurrentPropWrapper.refresh( CurrentRow );

            }//iterate props
        }//_refreshProps()


        public void update( bool IsCopy, bool OverrideUniqueValidation )
        {
            try
            {
                // Do BeforeUpdateNodePropRow on each row

                ICswNbtNodePropCollData PropCollData = getPropCollData( _NodeType.TableName, DateTime.MinValue );
                foreach( DataRow CurrentRow in PropCollData.PropsTable.Rows )
                {
                    if( CurrentRow.IsNull( "nodetypepropid" ) )
                        throw ( new CswDniException( "A node prop row is missing its nodetypepropid" ) );
                    //bz # 6542
                    CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( CurrentRow["nodetypepropid"] ) );

                    if( null != CswNbtMetaDataNodeTypeProp )
                        this[CswNbtMetaDataNodeTypeProp].onBeforeUpdateNodePropRow( IsCopy, OverrideUniqueValidation );
                }

                // Do the Update
                PropCollData.update();

            }//try
            catch( System.Exception Exception )
            {
                throw ( Exception );
            }//catch

        }


        public CswNbtNodePropWrapper this[string ObjectClassPropName]
        {
            get
            {
                CswNbtNodePropWrapper ReturnVal = null;
                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( _NodeTypeId, ObjectClassPropName );
                if( null != MetaDataProp )
                {
                    ReturnVal = this[MetaDataProp];
                }
                else
                {
                    throw new CswDniException( ErrorType.Error, "Invalid Property", "CswNbtNodePropColl[] for node ["+ _NodePk.ToString() + "] could not find object class prop: " + ObjectClassPropName );
                }
                return ReturnVal;
            }
        }// this[ string ObjectClassPropName ]


        public CswNbtNodePropWrapper this[CswNbtMetaDataNodeTypeProp NodeTypeProp]
        {
            get
            {
                if( NodeTypeProp == null )
                    throw new CswDniException( ErrorType.Error, "Invalid Property", "CswNbtNodePropColl[] was passed a null CswNbtMetaDataNodeTypeProp object" );
                if( NodeTypeProp.getNodeType().FirstVersionNodeTypeId != _NodeType.FirstVersionNodeTypeId )
                    throw new CswDniException( ErrorType.Error, "Invalid Property", "CswNbtNodePropColl[] on nodetype " + _NodeType.NodeTypeName + " (" + _NodeType.NodeTypeId + ") was passed a CswNbtMetaDataNodeTypeProp of the wrong nodetype: " + NodeTypeProp.getNodeType().NodeTypeName + " (" + NodeTypeProp.NodeTypeId + ")" );
                if( false == _PropsIndexByFirstVersionPropId.Contains( NodeTypeProp.FirstPropVersionId ) )
                    throw new CswDniException( ErrorType.Error, "Invalid Property", "There is no property with this firstpropversionid " + NodeTypeProp.FirstPropVersionId.ToString() + " on nodetypeid " + _NodeTypeId.ToString() );

                return ( _Props[CswConvert.ToInt32( _PropsIndexByFirstVersionPropId[NodeTypeProp.FirstPropVersionId] )] as CswNbtNodePropWrapper );
            }//get

        }//this[NodeTypeProp]

        public CswNbtPropEnmrtrFiltered this[CswNbtMetaDataFieldType.NbtFieldType FieldType]
        {
            get
            {
                return ( new CswNbtPropEnmrtrFiltered( _Props, FieldType ) );
            }//get

        }//this[FieldType]


        IEnumerator<CswNbtNodePropWrapper> IEnumerable<CswNbtNodePropWrapper>.GetEnumerator()
        {
            return _Props.Cast<CswNbtNodePropWrapper>().GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return new CswEnmrtrGeneric( _Props );
        }

        
    }//CswNbtNodePropColl


}//namespace ChemSW.Nbt
