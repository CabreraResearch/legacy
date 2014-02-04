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
using ChemSW.DB;

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
        private CswTableUpdate _PropsUpdate = null;
        private CswNbtNodePropCollDataRelational _propCollRelational = null;

        public CswNbtNodePropColl( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNode = CswNbtNode;
            _propCollRelational = new CswNbtNodePropCollDataRelational( _CswNbtResources );
        }//ctor()

        public bool Modified
        {
            get
            {
                return _Props.Any( CurrentProp => CurrentProp.wasAnySubFieldModified( IncludePendingUpdate: true ) );
            }
        }//Modified

        public bool New = false;

        public void clearModifiedFlag()
        {
            foreach( CswNbtNodePropWrapper CurrentProp in _Props )
            {
                CurrentProp.clearSubFieldModifiedFlags();
            }//iterate props

        }//_clearModifiedFlag()

        //private void _clear()
        //{
        //    _Props.Clear();

        //    _PropsIndexByFirstVersionPropId.Clear();
        //    _PropsIndexByNodeTypePropId.Clear();
        //    _PropsIndexByObjectClassPropName.Clear();

        //    if( null != _PropsTable )
        //    {
        //        _PropsTable.Clear();
        //    }
        //    _Filled = false;

        //}//clear()


        private bool _Filled = false;
        public bool Filled
        {
            get
            {
                return ( _Filled );
            }//get

        }//Filled

        public int Count
        {
            get
            {
                return ( _Props.Count );
            }//Get
        }//Count


        public void fill( bool IsNew )
        {
            New = IsNew;

            CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtNode.getNodeType();
            foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in MetaDataNodeType.getNodeTypeProps() )
            {
                DataRow PropRow = PropsTable.Rows.Cast<DataRow>().FirstOrDefault( CurrentRow => CurrentRow["nodetypepropid"].ToString() == MetaDataProp.PropId.ToString() );

                CswNbtNodePropWrapper AddedProp = CswNbtNodePropFactory.makeNodeProp( _CswNbtResources, PropRow, PropsTable, _CswNbtNode, MetaDataProp, _CswNbtNode._Date );

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

            if( _CswNbtNode != null )
            {
                _CswNbtNode.ObjClass.triggerAfterPopulateProps();
            }
            
            _Filled = true;
        }//fill()


        private DataTable _PropsTable = null;
        public DataTable PropsTable
        {
            get
            {
                if( null == _PropsTable )
                {
                    _PropsUpdate = _CswNbtResources.makeCswTableUpdate( "Props_update", "jct_nodes_props" );
                    if( _CswNbtNode.NodeId == null )
                    {
                        _PropsTable = _PropsUpdate.getEmptyTable();
                    }
                    else
                    {
                        if( false == CswTools.IsDate( _CswNbtNode._Date ) )
                        {
                            _PropsTable = _PropsUpdate.getTable( "nodeid", _CswNbtNode.NodeId.PrimaryKey );
                        }
                        else
                        {
                            // see case 30702 - we're only using audit data here, not live data
                            //string Sql = "select t.*, '' as auditchanged " +
                            //             "  from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtResources, "jct_nodes_props", Date, _NodeKey.PrimaryKey ) + " t ";

                            string Sql = @"select a.*, '' as auditchanged
                                             from jct_nodes_props_audit a
                                            where a.auditeventtype <> 'PhysicalDelete'
                                              and a.nodeid = " + _CswNbtNode.NodeId.PrimaryKey + @"
                                              and a.jctnodespropsauditid = (select max(jctnodespropsauditid)
                                                                              from jct_nodes_props_audit a2
                                                                             where a2.recordcreated <= " + _CswNbtResources.getDbNativeDate( _CswNbtNode._Date.ToDateTime().AddSeconds( 1 ) ) + @"
                                                                               and a2.jctnodepropid = a.jctnodepropid)";

                            CswArbitrarySelect PropsSelect = _CswNbtResources.makeCswArbitrarySelect( "propcolldata_audit_select", Sql );
                            _PropsTable = PropsSelect.getTable();
                            foreach( DataRow AuditRow in _PropsTable.Rows )
                            {
                                if( CswDateTime.EqualsNoMs( CswConvert.ToDateTime( AuditRow["recordcreated"] ), _CswNbtNode._Date.ToDateTime() ) )
                                {
                                    AuditRow["auditchanged"] = CswConvert.ToDbVal( true );
                                }
                            }
                        }
                    } // if-else( _Node.NodeId == null )
                } // if( null == _PropsTable )
                return ( _PropsTable );
            } // get
        }//PropsTable


        public void update( CswNbtNode Node, bool IsCopy, bool OverrideUniqueValidation, bool Creating, bool AllowAuditing, bool SkipEvents )
        {
            // Do BeforeUpdateNodePropRow on each row

            //Case 29857 - we have to use a traditional for-loop here. onBeforeUpdateNodePropRow() can cause new rows in PropCollData.PropsTable to be created
            // see Document.ArchivedDate
            for( int i = 0; i < PropsTable.Rows.Count; i++ )
            {
                DataRow CurrentRow = PropsTable.Rows[i];

                if( CurrentRow.IsNull( "nodetypepropid" ) )
                    throw ( new CswDniException( "A node prop row is missing its nodetypepropid" ) );
                //bz # 6542
                //CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( CurrentRow["nodetypepropid"] ) );

                //if( null != CswNbtMetaDataNodeTypeProp )
                //    this[CswNbtMetaDataNodeTypeProp].onBeforeUpdateNodePropRow( IsCopy, OverrideUniqueValidation );
                if( false == SkipEvents )
                {
                    this[CswConvert.ToInt32( CurrentRow["nodetypepropid"] )].onBeforeUpdateNodePropRow( Node, IsCopy, OverrideUniqueValidation, Creating );
                }
            }

            // Do the Update
            if( null != _CswNbtNode )
            {
                _PropsUpdate.update( PropsTable, ( AllowAuditing && false == _CswNbtNode.IsTemp ) );

                if( CswTools.IsPrimaryKey( _CswNbtNode.RelationalId ) && "nodes" != _CswNbtNode.RelationalId.TableName.ToLower() )
                {
                    _propCollRelational.update( _CswNbtNode.NodeTypeId, _CswNbtNode.RelationalId, PropsTable );
                }
            }
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


        public bool Contains( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            return ( NodeTypeProp != null &&
                     NodeTypeProp.getNodeType().FirstVersionNodeTypeId == _CswNbtNode.getNodeType().FirstVersionNodeTypeId &&
                     _PropsIndexByFirstVersionPropId.ContainsKey( NodeTypeProp.FirstPropVersionId ) );
        }

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

        public bool Contains( Int32 NodeTypePropId )
        {
            return _PropsIndexByFirstVersionPropId.ContainsKey( NodeTypePropId );
        }

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
            //Case 29857 - we have to use a traditional for-loop here. onBeforeUpdateNodePropRow() can cause new rows in PropCollData.PropsTable to be created
            // see Document.ArchivedDate
            for( int i = 0; i < PropsTable.Rows.Count; i++ )
            {
                DataRow CurrentRow = PropsTable.Rows[i];
                _CswNbtResources.AuditRecorder.addInsertRow( CurrentRow );
            }
        }
    }//CswNbtNodePropColl


}//namespace ChemSW.Nbt
