using System;
using System.Data;
using System.IO;
using System.Text;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpBatchEdit : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private CswEnumNbtBatchOpName _BatchOpName = CswEnumNbtBatchOpName.BatchEdit;

        public CswNbtBatchOpBatchEdit( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new batch operation to handle results of the BatchEdit action
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( DataTable excelData )
        {
            BatchEditBatchData batchData = new BatchEditBatchData( string.Empty );
            batchData.excelData = excelData;
            batchData.CurrentRow = 0;

            return CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, batchData.ToString() );

        } // makeBatchOp()

        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 100;
            if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.BatchEdit )
            {
                BatchEditBatchData BatchData = new BatchEditBatchData( BatchNode.BatchData.Text );
                if( BatchData.TotalRows > 0 )
                {
                    ret = Math.Round( (Double) BatchData.CurrentRow / BatchData.TotalRows * 100, 0 );
                }
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.BatchEdit )
                {
                    BatchNode.start();

                    BatchEditBatchData BatchData = new BatchEditBatchData( BatchNode.BatchData.Text );

                    if( BatchData.CurrentRow < BatchData.TotalRows )
                    {
                        if( null != BatchData.excelData.Columns["nodeid"] )
                        {
                            int NodesProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                            Int32 r;
                            for( r = BatchData.CurrentRow; r < BatchData.excelData.Rows.Count && ( r - BatchData.CurrentRow ) < NodesProcessedPerIteration; r++ )
                            {
                                DataRow row = BatchData.excelData.Rows[r];

                                CswPrimaryKey NodeId = new CswPrimaryKey();
                                NodeId.FromString( row["nodeid"].ToString() );
                                if( CswTools.IsPrimaryKey( NodeId ) )
                                {
                                    CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
                                    if( null != Node )
                                    {
                                        foreach( DataColumn col in BatchData.excelData.Columns )
                                        {
                                            if( col.ColumnName != "nodeid" )
                                            {
                                                CswNbtMetaDataNodeTypeProp Prop = Node.getNodeType().getNodeTypeProp( col.ColumnName );
                                                CswNbtSubField SubField;
                                                if( null != Prop )
                                                {
                                                    SubField = Prop.getFieldTypeRule().SubFields.Default;
                                                }
                                                else
                                                {
                                                    string propName = col.ColumnName.Substring( 0, col.ColumnName.LastIndexOf( " " ) );
                                                    string subFieldName = col.ColumnName.Substring( col.ColumnName.LastIndexOf( " " ) + 1 );
                                                    Prop = Node.getNodeType().getNodeTypeProp( propName );
                                                    SubField = Prop.getFieldTypeRule().SubFields[(CswEnumNbtSubFieldName) subFieldName];
                                                }
                                                Node.Properties[Prop].SetSubFieldValue( SubField, row[col.ColumnName] );
                                            } // if( col.ColumnName != "nodeid" )
                                        } // foreach( DataColumn col in BatchData.excelData.Columns )
                                    } // if( null != Node )
                                    Node.postChanges( false );
                                } // if( CswTools.IsPrimaryKey( NodeId ) )
                            } // for

                            // Setup for next iteration
                            BatchData.CurrentRow = r;
                            BatchNode.BatchData.Text = BatchData.ToString();
                            BatchNode.PercentDone.Value = getPercentDone( BatchNode );

                        } // if( null != BatchData.excelData.Columns["nodeid"] )
                    } // if(BatchData.CurrentRow < BatchData.TotalRows)
                    else
                    {
                        BatchNode.finish();
                    }
                    BatchNode.postChanges( false );
                } // if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MultiEdit )
            }
            catch( Exception ex )
            {
                BatchNode.error( ex );
            }
        } // runBatchOp()


        // This internal class is specific to this batch operation
        private class BatchEditBatchData
        {
            private JObject _BatchData;

            public BatchEditBatchData( string BatchData )
            {
                if( BatchData != string.Empty )
                {
                    _BatchData = JObject.Parse( BatchData );
                }
                else
                {
                    _BatchData = new JObject();
                }
            }

            private Int32 _CurrentRow = Int32.MinValue;
            public Int32 CurrentRow
            {
                get
                {
                    if( Int32.MinValue == _CurrentRow )
                    {
                        _CurrentRow = CswConvert.ToInt32( _BatchData["CurrentRow"].ToString() );
                    }
                    if( _CurrentRow < 0 )
                    {
                        _CurrentRow = 0;
                    }
                    return _CurrentRow;
                }
                set
                {
                    _CurrentRow = value;
                    _BatchData["CurrentRow"] = value.ToString();
                }
            }

            public Int32 TotalRows
            {
                get { return CswConvert.ToInt32( _BatchData["TotalRows"].ToString() ); }
                private set { _BatchData["TotalRows"] = value.ToString(); }
            }

            private DataTable _excelData = null;
            public DataTable excelData
            {
                get
                {
                    if( null == _excelData )
                    {
                        if( null != _BatchData["excelData"] )
                        {
                            _excelData = new DataTable();
                            StringReader sr = new StringReader( _BatchData["excelData"].ToString() );
                            _excelData.ReadXml( sr );
                        }
                    }
                    return _excelData;
                }
                set
                {
                    _excelData = value;
                    TotalRows = value.Rows.Count;

                    StringBuilder sb = new StringBuilder();
                    StringWriter sw = new StringWriter( sb );
                    _excelData.WriteXml( sw );

                    _BatchData["excelData"] = sb.ToString();
                }
            } // excelData

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        } // class BatchEditBatchData

    } // class CswNbtBatchOpMultiEdit
} // namespace ChemSW.Nbt.Batch
