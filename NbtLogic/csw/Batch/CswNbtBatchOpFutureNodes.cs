using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpFutureNodes : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.FutureNodes;

        public CswNbtBatchOpFutureNodes( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new batch operation to handle future node generation
        /// </summary>
        /// <param name="GeneratorNodeId">Primary key of Generator</param>
        public CswNbtBatchRow makeBatchOp( CswPrimaryKey GeneratorNodeId, DateTime FinalDate )
        {
            CswNbtBatchRow BatchRow = null;
            if( null != GeneratorNodeId )
            {
                CswNbtNode GenNode = _CswNbtResources.Nodes[GeneratorNodeId];
                if( null != GenNode )
                {
                    CswNbtObjClassGenerator GeneratorNode = CswNbtNodeCaster.AsGenerator( GenNode );


                    // BZ 6752 - The first future node is the first node generated 
                    // after today + warning days, according to the time interval
                    // But it has to include initial due date, no matter what the time interval.

                    CswNbtNodePropTimeInterval NextDueDateTimeInterval = GeneratorNode.DueDateInterval;
                    Double WarningDays = 0;
                    if( GeneratorNode.WarningDays.Value > 0 )
                    {
                        WarningDays = GeneratorNode.WarningDays.Value;
                    }
                    DateTime StartDate = DateTime.Now.AddDays( WarningDays ).Date; //bz# 6937 (capture date only, not time)

                    DateTime DateOfNextOccurance = DateTime.MinValue;
                    if( GeneratorNode.DueDateInterval.getStartDate().Date >= StartDate ) //bz # 6937 (change gt to gteq)
                    {
                        StartDate = GeneratorNode.DueDateInterval.getStartDate().Date;
                        DateOfNextOccurance = StartDate;
                    }
                    else
                    {
                        DateOfNextOccurance = NextDueDateTimeInterval.getNextOccuranceAfter( StartDate );
                    }

                    FutureNodesBatchData BatchData = new FutureNodesBatchData( string.Empty );
                    BatchData.GeneratorNodeId = GeneratorNodeId;
                    BatchData.NextStartDate = DateOfNextOccurance;
                    BatchData.FinalDate = FinalDate;

                    BatchRow = new CswNbtBatchRow( _CswNbtResources, _BatchOpName, BatchData.ToString() );

                } // if(null != GeneratorNode)
            } // if( null != GeneratorNodeId )
            return BatchRow;
        } // makeBatchOp()

        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        /// <param name="BatchRow"></param>
        public void runBatchOp( CswNbtBatchRow BatchRow )
        {
            try
            {
                BatchRow.start();

                FutureNodesBatchData BatchData = new FutureNodesBatchData( BatchRow.BatchData );
                if( BatchData.GeneratorNodeId != null && BatchData.NextStartDate != DateTime.MinValue )
                {
                    CswNbtNode GenNode = _CswNbtResources.Nodes[BatchData.GeneratorNodeId];
                    if( null != GenNode )
                    {
                        CswNbtObjClassGenerator GeneratorNode = CswNbtNodeCaster.AsGenerator( GenNode );
                        DateTime ThisDate = BatchData.NextStartDate;

                        CswNbtActGenerateNodes CswNbtActGenerateNodes = new CswNbtActGenerateNodes( _CswNbtResources );

                        // Run this iteration
                        if( ThisDate != DateTime.MinValue &&
                            ThisDate.Date <= BatchData.FinalDate.Date &&
                            ( GeneratorNode.FinalDueDate.Empty || ThisDate.Date <= GeneratorNode.FinalDueDate.DateTimeValue.Date ) )
                        {
                            CswNbtActGenerateNodes.makeNode( GenNode, ThisDate );
                            BatchRow.appendToLog( "Created future task for " + ThisDate.ToString() + "." );
                        }
                        else
                        {
                            BatchRow.finish();
                        }

                        // Setup for next iteration
                        BatchData.NextStartDate = GeneratorNode.DueDateInterval.getNextOccuranceAfter( ThisDate );
                        if( BatchData.NextStartDate.Date == ThisDate.Date ) // infinite loop guard
                        {
                            BatchData.NextStartDate = DateTime.MinValue;
                        }

                        BatchRow.BatchData = BatchData.ToString();

                    } // if( null != GenNode )
                } // if( _BatchData.GeneratorNodeId != null && _BatchData.NextStartDate != DateTime.MinValue )

                BatchRow.save();
            }
            catch( Exception ex )
            {
                BatchRow.error( ex );
            }
        } // runBatchOp()


        #region FutureNodesBatchData

        // This internal class is specific to this batch operation
        private class FutureNodesBatchData
        {
            private JObject _BatchData;

            public FutureNodesBatchData( string BatchData )
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

            private CswPrimaryKey _GeneratorNodeId = null;
            public CswPrimaryKey GeneratorNodeId
            {
                get
                {
                    if( null == _GeneratorNodeId )
                    {
                        if( null != _BatchData["generatornodeid"] )
                        {
                            _GeneratorNodeId = new CswPrimaryKey();
                            _GeneratorNodeId.FromString( _BatchData["generatornodeid"].ToString() );
                        }
                    }
                    return _GeneratorNodeId;
                }
                set
                {
                    _GeneratorNodeId = value;
                    _BatchData["generatornodeid"] = _GeneratorNodeId.ToString();
                }
            }

            private DateTime _NextStartDate = DateTime.MinValue;
            public DateTime NextStartDate
            {
                get
                {
                    if( null == _NextStartDate )
                    {
                        if( null != _BatchData["nextstartdate"] )
                        {
                            _NextStartDate = CswConvert.ToDateTime( _BatchData["nextstartdate"].ToString() );
                        }
                    }
                    return _NextStartDate;
                }
                set
                {
                    _NextStartDate = value;
                    _BatchData["nextstartdate"] = CswConvert.ToString( _NextStartDate );
                }
            }

            private DateTime _FinalDate = DateTime.MinValue;
            public DateTime FinalDate
            {
                get
                {
                    if( null == _FinalDate )
                    {
                        if( null != _BatchData["finaldate"] )
                        {
                            _FinalDate = CswConvert.ToDateTime( _BatchData["finaldate"].ToString() );
                        }
                    }
                    return _FinalDate;
                }
                set
                {
                    _FinalDate = value;
                    _BatchData["finaldate"] = CswConvert.ToString( _FinalDate );
                }
            }

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        } // class FutureNodesBatchData

        #endregion FutureNodesBatchData


    } // class CswNbtBatchOpFutureNodes
} // namespace ChemSW.Nbt.Batch
