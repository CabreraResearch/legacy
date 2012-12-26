using System;
using System.Collections.ObjectModel;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt.csw.Mobile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpMobileMultiOpUpdates : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.MobileMultiOpUpdates;

        public CswNbtBatchOpMobileMultiOpUpdates( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public CswNbtObjClassBatchOp makeBatchOp( CswNbtCISProNbtMobileData.MobileRequest MobileRequest )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            MobileMultiOpUpdatesBatchData BatchData = new MobileMultiOpUpdatesBatchData( string.Empty );
            BatchData.StartingCount = MobileRequest.data.MultiOpRows.Count;
            BatchData.Operations = _operationsCollectionToJArray( MobileRequest.data.MultiOpRows );

            BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            return BatchNode;
        } // makeBatchOp()

        private JArray _operationsCollectionToJArray( Collection<CswNbtCISProNbtMobileData.MobileRequest.Operation> Operations )
        {
            JArray ReturnVal = new JArray();

            ReturnVal = JArray.Parse( JsonConvert.SerializeObject( Operations ) );

            return ReturnVal;
        } //_operationsCollectionToJArray()

        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 100;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.SyncLocation )
            {
                MobileMultiOpUpdatesBatchData BatchData = new MobileMultiOpUpdatesBatchData( BatchNode.BatchData.Text );
                if( BatchData.StartingCount > 0 )
                {
                    ret = Math.Round( (Double) ( BatchData.StartingCount - BatchData.Operations.Count ) / BatchData.StartingCount * 100, 0 );
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
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MobileMultiOpUpdates )
                {
                    BatchNode.start();

                    MobileMultiOpUpdatesBatchData BatchData = new MobileMultiOpUpdatesBatchData( BatchNode.BatchData.Text );

                    if( BatchData.Operations.Count > 0 )
                    {
                        int NodesProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                        for( int i = 0; i < NodesProcessedPerIteration && BatchData.Operations.Count > 0; i++ )
                        {

                            string operation = string.Empty;
                            operation = BatchData.Operations[0]["op"].ToString();

                            //Get parameters from record
                            JObject update = (JObject) BatchData.Operations[0]["update"];
                            string barcode = BatchData.Operations[0]["barcode"].ToString();

                            switch( operation )
                            {
                                case "Dispose":
                                    try
                                    {
                                        bool success = _dispose( barcode, update );
                                        if( success )
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                + "; Container Barcode: " + barcode
                                                + "; Success: " + success );
                                        }
                                        else
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                + "; Container Barcode: " + barcode
                                                + "; Success: " + success + "(Container node doesn't exist)" );
                                        }
                                    }
                                    catch( Exception e )
                                    {
                                        BatchNode.appendToLog( "The dispose operation failed for the container barcode " + barcode + "with exception: " + e );
                                    }
                                    break;
                                case "Move":
                                    try
                                    {
                                        bool success = _move( barcode, update );
                                        if( success )
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                                   + "; Container Barcode: " + barcode
                                                                   + "; New Location: " + update["location"]
                                                                   + "; Success: " + success );
                                        }
                                        else
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                                   + "; Container Barcode: " + barcode
                                                                   + "; New Location: " + update["location"]
                                                                   + "; Success: " + success + "(Container node doesn't exist)" );
                                        }
                                    }
                                    catch( Exception e )
                                    {
                                        BatchNode.appendToLog( "The move operation failed for the container barcode " + barcode + "with exception: " + e );
                                    }
                                    break;
                                case "Owner":
                                    try
                                    {
                                        bool success = _owner( barcode, update );
                                        if( success )
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                + "; Container Barcode: " + barcode
                                                + "; New Owner: " + update["user"]
                                                + "; Success: " + success );
                                        }
                                        else
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                + "; Container Barcode: " + barcode
                                                + "; New Owner: " + update["user"]
                                                + "; Success: " + success + "(Container node doesn't exist)" );
                                        }
                                    }
                                    catch( Exception e )
                                    {
                                        BatchNode.appendToLog( "The update owner operation failed for the container barcode " + barcode + "with exception: " + e );
                                    }
                                    break;
                                case "Transfer":
                                    try
                                    {
                                        bool success = _transfer( barcode, update );
                                        if( _transfer( barcode, update ) )
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                                  + "; Container Barcode: " + barcode
                                                                  + "; New Owner: " + update["user"]
                                                                  + "; New Location: " + update["location"]
                                                                  + "; Success: " + success );
                                        }
                                        else
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                                  + "; Container Barcode: " + barcode
                                                                  + "; New Owner: " + update["user"]
                                                                  + "; New Location: " + update["location"]
                                                                  + "; Success: " + success + "(Container node doesn't exist)" );
                                        }
                                    }
                                    catch( Exception e )
                                    {
                                        BatchNode.appendToLog( "The transfer operation failed for the container barcode " + barcode + "with exception: " + e );
                                    }
                                    break;
                                case "Dispense":
                                    try
                                    {
                                        bool success = _dispense( barcode, update );
                                        if( success )
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                                  + "; Container Barcode: " + barcode
                                                                  + "; Dispense Data: " + update
                                                                  + "; Success: " + success );
                                        }
                                        else
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                                  + "; Container Barcode: " + barcode
                                                                  + "; Dispense Data: " + update
                                                                  + "; Success: " + success + "(Container node doesn't exist)" );
                                        }
                                    }
                                    catch( Exception e )
                                    {
                                        BatchNode.appendToLog( "The dispense operation failed for the container barcode " + barcode + "with exception: " + e );
                                    }
                                    break;
                                case "Reconcile":
                                    try
                                    {
                                        bool success = _reconcile( barcode, update );
                                        if( success )
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                                  + "; Container Barcode: " + barcode
                                                                  + "; Location: " + update["location"]
                                                                  + "; Success: " + success );
                                        }
                                        else
                                        {
                                            BatchNode.appendToLog( "Operation: " + operation
                                                                  + "; Container Barcode: " + barcode
                                                                  + "; Location: " + update["location"]
                                                                  + "; Success: " + success + "(Container node doesn't exist)" );
                                        }
                                    }
                                    catch( Exception e )
                                    {
                                        BatchNode.appendToLog( "The reconcile operation failed for the container barcode " + barcode + "with exception: " + e );
                                    }
                                    break;
                                default:
                                    BatchNode.appendToLog( "The operation " + operation + "doesn't exist." );
                                    break;
                            }

                            BatchData.Operations.RemoveAt( 0 );
                        }
                    }
                    else
                    {
                        BatchNode.finish();
                    }

                    BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    BatchNode.BatchData.Text = BatchData.ToString();
                    BatchNode.postChanges( false );
                }
            }
            catch( Exception ex )
            {
                BatchNode.error( ex );
            }

        } // runBatchOp()

        #region Private Helper Methods

        #region Helper Methods

        private CswPrimaryKey _getLocationNodeIdFromBarcode( string barcode )
        {
            CswPrimaryKey ReturnVal = null;

            CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            CswNbtMetaDataObjectClass LocationOC =
                _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );

            foreach( CswNbtObjClassLocation currentLocationNode in LocationOC.getNodes( false, false, false, true ) )
            {
                if( currentLocationNode.Barcode.Barcode == barcode )
                {
                    ReturnVal = currentLocationNode.NodeId;
                }
            }

            return ReturnVal;
        }

        private CswPrimaryKey _getUserNodeIdFromBarcode( string barcode )
        {
            CswPrimaryKey ReturnVal = null;

            CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            CswNbtMetaDataObjectClass UserOC =
                _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );

            foreach( CswNbtObjClassUser currentLocationNode in UserOC.getNodes( false, false, false, true ) )
            {
                if( currentLocationNode.Barcode.Barcode == barcode )
                {
                    ReturnVal = currentLocationNode.NodeId;
                }
            }

            return ReturnVal;
        }

        private CswNbtObjClassContainer _getContainerNodeFromBarcode( string barcode )
        {
            CswNbtObjClassContainer ReturnNode = null;

            CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            CswNbtMetaDataObjectClass ContainerOC =
                _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

            foreach( CswNbtObjClassContainer currentContainerNode in ContainerOC.getNodes( false, false, false, true ) )
            {
                if( currentContainerNode.Barcode.Barcode == barcode )
                {
                    ReturnNode = currentContainerNode;
                }
            }

            return ReturnNode;
        }

        #endregion

        private bool _dispose( string barcode, JObject update )
        {
            bool ReturnVal = false;

            CswNbtObjClassContainer ContainerNode = _getContainerNodeFromBarcode( barcode );
            if( null != ContainerNode )
            {
                ContainerNode.DisposeContainer();
                ContainerNode.postChanges( false );
                ReturnVal = true;
            }

            return ReturnVal;
        }

        private bool _move( string barcode, JObject update )
        {
            bool ReturnVal = false;
            string newLocation = update["location"].ToString();

            CswNbtObjClassContainer ContainerNode = _getContainerNodeFromBarcode( barcode );
            if( null != ContainerNode )
            {
                ContainerNode.MoveContainer( _getLocationNodeIdFromBarcode( newLocation ) );
                ContainerNode.postChanges( false );
                ReturnVal = true;
            }

            return ReturnVal;
        }

        private bool _owner( string barcode, JObject update )
        {
            bool ReturnVal = false;
            string newOwner = update["user"].ToString();

            CswNbtObjClassContainer ContainerNode = _getContainerNodeFromBarcode( barcode );
            if( null != ContainerNode )
            {
                ContainerNode.UpdateOwner( _getUserNodeIdFromBarcode( newOwner ) );
                ContainerNode.postChanges( false );
                ReturnVal = true;
            }

            return ReturnVal;
        }

        private bool _transfer( string barcode, JObject update )
        {
            bool ReturnVal = false;
            string newOwner = update["user"].ToString();
            string newLocation = update["location"].ToString();

            CswNbtObjClassContainer ContainerNode = _getContainerNodeFromBarcode( barcode );
            if( null != ContainerNode )
            {
                ContainerNode.TransferContainer( _getLocationNodeIdFromBarcode( newLocation ), _getUserNodeIdFromBarcode( newOwner ) );
                ContainerNode.postChanges( false );
                ReturnVal = true;
            }

            return ReturnVal;
        }

        private bool _dispense( string barcode, JObject update )
        {
            bool ReturnVal = false;

            CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            string uom = update["uom"].ToString().ToLower();
            CswPrimaryKey uomId = null;
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );

            foreach( CswNbtObjClassUnitOfMeasure currentUnitOfMeasureNode in UnitOfMeasureOC.getNodes( false, false, false, true ) )
            {
                string unitName = currentUnitOfMeasureNode.NodeName.ToLower();
                if( unitName == uom )
                {
                    uomId = currentUnitOfMeasureNode.NodeId;
                }
            }

            CswNbtObjClassContainer ContainerNode = _getContainerNodeFromBarcode( barcode );
            if( null != ContainerNode )
            {
                ContainerNode.DispenseOut( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, CswConvert.ToDouble( update["qty"] ), uomId );
                ContainerNode.postChanges( false );
                ReturnVal = true;
            }

            return ReturnVal;
        }

        private bool _reconcile( string barcode, JObject update )
        {
            bool ReturnVal = false;
            string newLocationBarcode = update["location"].ToString();

            CswNbtObjClassContainer ContainerNode = _getContainerNodeFromBarcode( barcode );
            if( null != ContainerNode )
            {
                ContainerNode.CreateContainerLocationNode( CswNbtObjClassContainerLocation.TypeOptions.Scan, newLocationBarcode, barcode );
                ContainerNode.Location.RefreshNodeName();
                ContainerNode.postChanges( false );
                ReturnVal = true;
            }

            return ReturnVal;
        }

        #endregion

        #region MobileMultiOpUpdatesBatchData

        // This internal class is specific to this batch operation
        private class MobileMultiOpUpdatesBatchData
        {



            private JObject _BatchData;

            public MobileMultiOpUpdatesBatchData( string BatchData )
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

            private JArray _Operations = null;
            public JArray Operations
            {
                get
                {
                    if( null == _Operations )
                    {
                        if( null != _BatchData["operations"] )
                        {
                            _Operations = (JArray) _BatchData["operations"];
                        }
                    }
                    return _Operations;
                }
                set
                {
                    _Operations = value;
                    _BatchData["operations"] = _Operations;
                }
            }

            private Int32 _StartingCount = Int32.MinValue;
            public Int32 StartingCount
            {
                get
                {
                    if( Int32.MinValue == _StartingCount )
                    {
                        if( null != _BatchData["startingcount"] )
                        {
                            _StartingCount = CswConvert.ToInt32( _BatchData["startingcount"].ToString() );
                        }
                    }
                    return _StartingCount;
                }
                set
                {
                    _StartingCount = value;
                    _BatchData["startingcount"] = _StartingCount;
                }
            }

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        } // class MobileMultiOpUpdatesBatchData

        #endregion MobileMultiOpUpdatesBatchData

    } // class CswNbtBatchOpMobileMultiOpUpdates
} // namespace ChemSW.Nbt.Batch
