using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{

    public class CswNbtNodePropColl : IEnumerable<CswNbtNodePropWrapper>
    {
        private Collection<CswNbtNodePropWrapper> _Props = new Collection<CswNbtNodePropWrapper>();
        private Dictionary<Int32, Int32> _PropsIndexByFirstVersionPropId = new Dictionary<Int32, Int32>();
        private Dictionary<Int32, Int32> _PropsIndexByNodeTypePropId = new Dictionary<Int32, Int32>();
        private Dictionary<string, Int32> _PropsIndexByObjectClassPropName = new Dictionary<string, Int32>();
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
        
        private ICswNbtNodePropCollData getPropCollData( string TableName, CswDateTime Date )
        {
            ICswNbtNodePropCollData ReturnVal = null;

            if( TableName.ToLower() == "nodes" )
            {
                if( _CswNbtNodePropCollDataNative == null )
                {
                    _CswNbtNodePropCollDataNative = new CswNbtNodePropCollDataNative( _CswNbtResources, _CswNbtNode );
                    _CswNbtNodePropCollDataNative.Date = Date;
                }
                ReturnVal = _CswNbtNodePropCollDataNative;
            }
            else
            {
                if( _CswNbtNodePropCollDataRelational == null )
                {
                    _CswNbtNodePropCollDataRelational = new CswNbtNodePropCollDataRelational( _CswNbtResources, _CswNbtNode );
                }
                ReturnVal = _CswNbtNodePropCollDataRelational;
            }
            return ( ReturnVal );
        }//getPropCollData()

        public bool Modified
        {
            get
            {
                return _Props.Any( CurrentProp => CurrentProp.wasAnySubFieldModified( IncludePendingUpdate: true ) );
            }
        }//Modified

        public bool CreatedFromNodeTypeId = false;

        public void clearModifiedFlag()
        {
            foreach( CswNbtNodePropWrapper CurrentProp in _Props )
            {
                CurrentProp.clearSubFieldModifiedFlags();
            }//iterate props

        }//_clearModifiedFlag()

        private void _clear()
        {
            _Props.Clear();

            _PropsIndexByFirstVersionPropId.Clear();
            _PropsIndexByNodeTypePropId.Clear();
            _PropsIndexByObjectClassPropName.Clear();

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


        public void fillFromNodePk( CswPrimaryKey NodePk, Int32 NodeTypeId, CswDateTime Date )
        {

            CswTimer Timer = new CswTimer();


            if( NodePk != null )
            {
                if( getPropCollData( NodePk.TableName, Date ).IsTableEmpty )
                {
                    _populateProps( Date );
                }
                else
                {
                    _refreshProps( Date );
                }

                _CswNbtResources.logTimerResult( "Fetched node (" + _CswNbtNode.NodeId.ToString() + ")", Timer.ElapsedDurationInSecondsAsString );

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

            _populateProps( null );

            _Filled = true;

        }//fillFromNodeTypeId()

        private void _populateProps( CswDateTime Date )// CswPrimaryKey NodePk, Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtNode.getNodeType();
            foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in MetaDataNodeType.getNodeTypeProps() )
            {
                ICswNbtNodePropCollData PropCollData = getPropCollData( MetaDataNodeType.TableName, Date );
                DataRow PropRow = PropCollData.PropsTable.Rows.Cast<DataRow>().FirstOrDefault( CurrentRow => CurrentRow["nodetypepropid"].ToString() == MetaDataProp.PropId.ToString() );

                CswNbtNodePropWrapper AddedProp = CswNbtNodePropFactory.makeNodeProp( _CswNbtResources, PropRow, PropCollData.PropsTable, _CswNbtNode, MetaDataProp, Date );
                
                _Props.Add( AddedProp );
                Int32 PropsIdx = _Props.Count - 1;
                _PropsIndexByFirstVersionPropId.Add( MetaDataProp.FirstPropVersionId, PropsIdx );
                _PropsIndexByNodeTypePropId.Add( MetaDataProp.PropId, PropsIdx );
                string ObjectClassPropName = MetaDataProp.getObjectClassPropName();
                if( false == string.IsNullOrEmpty( ObjectClassPropName ) )
                {
                    _PropsIndexByObjectClassPropName.Add( ObjectClassPropName, PropsIdx );
                }
                AddedProp.onNodePropRowFilled();
            }

            // This is evil -- See BZ 5963/5979
            //if(doUpdate)
            //    _PropsCaddy.update(_PropsTable);

            if( _CswNbtNode != null )
            {
                //CswNbtObjClass _CswNbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, MetaDataNodeType.ObjectClassId, _CswNbtNode );
                _CswNbtNode.ObjClass.triggerAfterPopulateProps();
            }

            //SuspendModifyTracking = false;
        }//_populateProps()


        //added for bz # 8287

        private void _refreshProps( CswDateTime Date )// CswPrimaryKey NodePk, Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtNode.getNodeType();
            ICswNbtNodePropCollData PropCollData = getPropCollData( MetaDataNodeType.TableName, Date );
            PropCollData.refreshTable();

            foreach( DataRow CurrentRow in PropCollData.PropsTable.Rows )
            {
                //CswNbtMetaDataNodeTypeProp CurrentMetaDataProp = MetaDataNodeType.getNodeTypeProp( CswConvert.ToInt32( CurrentRow["nodetypepropid"] ) );
                Int32 PropsIdx = _PropsIndexByNodeTypePropId[CswConvert.ToInt32( CurrentRow["nodetypepropid"] )];
                CswNbtNodePropWrapper CurrentPropWrapper = (CswNbtNodePropWrapper) _Props[PropsIdx];
                CurrentPropWrapper.refresh( CurrentRow );

            }//iterate props
        }//_refreshProps()


        public void update( CswNbtNode Node, bool IsCopy, bool OverrideUniqueValidation, bool Creating, CswDateTime Date )
        {
            // Do BeforeUpdateNodePropRow on each row

            ICswNbtNodePropCollData PropCollData = getPropCollData( _CswNbtNode.getNodeType().TableName, Date );

            //Case 29857 - we have to use a traditional for-loop here. onBeforeUpdateNodePropRow() can cause new rows in PropCollData.PropsTable to be created
            // see Document.ArchivedDate
            for( int i = 0; i < PropCollData.PropsTable.Rows.Count; i++ )
            {
                DataRow CurrentRow = PropCollData.PropsTable.Rows[i];

                if( CurrentRow.IsNull( "nodetypepropid" ) )
                    throw ( new CswDniException( "A node prop row is missing its nodetypepropid" ) );
                //bz # 6542
                //CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( CurrentRow["nodetypepropid"] ) );

                //if( null != CswNbtMetaDataNodeTypeProp )
                //    this[CswNbtMetaDataNodeTypeProp].onBeforeUpdateNodePropRow( IsCopy, OverrideUniqueValidation );
                this[CswConvert.ToInt32( CurrentRow["nodetypepropid"] )].onBeforeUpdateNodePropRow( Node, IsCopy, OverrideUniqueValidation, Creating );
            }

            // Do the Update
            PropCollData.update();
        }


        public CswNbtNodePropWrapper this[string ObjectClassPropName]
        {
            get
            {
                if( false == _PropsIndexByObjectClassPropName.ContainsKey( ObjectClassPropName ) )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid Property", "There is no property with this objectclasspropname: " + ObjectClassPropName + " on nodetypeid " + _CswNbtNode.NodeTypeId.ToString() );
                }
                return _Props[_PropsIndexByObjectClassPropName[ObjectClassPropName]];
            }
        }// this[ string ObjectClassPropName ]


        public CswNbtNodePropWrapper this[CswNbtMetaDataNodeTypeProp NodeTypeProp]
        {
            get
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtNode.getNodeType();
                if( NodeTypeProp == null )
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid Property", "CswNbtNodePropColl[] was passed a null CswNbtMetaDataNodeTypeProp object" );
                if( NodeTypeProp.getNodeType().FirstVersionNodeTypeId != NodeType.FirstVersionNodeTypeId )
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid Property", "CswNbtNodePropColl[] on nodetype " + NodeType.NodeTypeName + " (" + NodeType.NodeTypeId + ") was passed a CswNbtMetaDataNodeTypeProp: " + NodeTypeProp.PropName + " (" + NodeTypeProp.PropId + ") of the wrong nodetype: " + NodeTypeProp.getNodeType().NodeTypeName + " (" + NodeTypeProp.NodeTypeId + ")" );
                if( false == _PropsIndexByFirstVersionPropId.ContainsKey( NodeTypeProp.FirstPropVersionId ) )
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid Property", "There is no property with this firstpropversionid " + NodeTypeProp.FirstPropVersionId.ToString() + " on nodetypeid " + NodeType.NodeTypeId.ToString() );

                return _Props[_PropsIndexByFirstVersionPropId[NodeTypeProp.FirstPropVersionId]];
            }//get

        }//this[NodeTypeProp]

        public CswNbtNodePropWrapper this[Int32 NodeTypePropId]
        {
            get
            {
                return _Props[_PropsIndexByNodeTypePropId[NodeTypePropId]];
            }//get

        }//this[NodeTypeProp]


        public CswNbtPropEnmrtrFiltered this[CswEnumNbtFieldType FieldType]
        {
            get
            {
                return ( new CswNbtPropEnmrtrFiltered( _Props, FieldType ) );
            }//get

        }//this[FieldType]


        IEnumerator<CswNbtNodePropWrapper> IEnumerable<CswNbtNodePropWrapper>.GetEnumerator()
        {
            return _Props.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _Props.GetEnumerator();
        }

        public void AuditInsert()
        {
            ICswNbtNodePropCollData PropCollData = getPropCollData( _CswNbtNode.getNodeType().TableName, null );

            //Case 29857 - we have to use a traditional for-loop here. onBeforeUpdateNodePropRow() can cause new rows in PropCollData.PropsTable to be created
            // see Document.ArchivedDate
            for( int i = 0; i < PropCollData.PropsTable.Rows.Count; i++ )
            {
                DataRow CurrentRow = PropCollData.PropsTable.Rows[i];
                _CswNbtResources.AuditRecorder.addInsertRow( CurrentRow );
            }
        }
    }//CswNbtNodePropColl


}//namespace ChemSW.Nbt
