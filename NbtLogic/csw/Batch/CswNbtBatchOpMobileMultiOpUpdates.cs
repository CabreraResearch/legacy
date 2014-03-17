using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Mail;
using ChemSW.Nbt.csw.Mobile;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpMobileMultiOpUpdates : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private CswEnumNbtBatchOpName _BatchOpName = CswEnumNbtBatchOpName.MobileMultiOpUpdates;
        private CswNbtObjClassUser _User;
        private List<string> _HumanReadableLog = new List<string>();

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
            BatchData.Username = MobileRequest.data.username;
            //BatchData.

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
            if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.SyncLocation )
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
                if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.MobileMultiOpUpdates )
                {
                    BatchNode.start();

                    MobileMultiOpUpdatesBatchData BatchData = new MobileMultiOpUpdatesBatchData( BatchNode.BatchData.Text );
                    CswNbtObjClassUser UserNode = _CswNbtResources.Nodes.makeUserNodeFromUsername( BatchData.Username );
                    _User = UserNode;

                    if( BatchData.Operations.Count > 0 )
                    {
                        if( null != UserNode )
                        {
                            CswNbtObjClassRole RoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( UserNode.Rolename );
                            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                            CswNbtMetaDataNodeType ContainerNT = ContainerOC.getNodeTypes().FirstOrDefault();

                            int NodesProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                            for( int i = 0; i < NodesProcessedPerIteration && BatchData.Operations.Count > 0; i++ )
                            {
                                string operation = string.Empty;
                                operation = BatchData.Operations[0]["op"].ToString();
                                operation = operation.ToLower();
                                operation = CultureInfo.CurrentCulture.TextInfo.ToTitleCase( operation );

                                JObject update = (JObject) BatchData.Operations[0]["update"];
                                string barcode = BatchData.Operations[0]["barcode"].ToString();

                                if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, ContainerNT, RoleNode ) )
                                {
                                    switch( operation )
                                    {
                                        case "Dispose":
                                            _dispose( operation, barcode, BatchNode );
                                            break;
                                        case "Move":
                                            _move( operation, barcode, update, BatchNode );
                                            break;
                                        case "Owner":
                                            _updateOwner( operation, barcode, update, BatchNode );
                                            break;
                                        case "Transfer":
                                            _transfer( operation, barcode, update, BatchNode );
                                            break;
                                        case "Dispense":
                                            _dispense( operation, barcode, update, BatchNode );
                                            break;
                                        case "Reconcile":
                                            _reconcile( operation, barcode, update, BatchNode );
                                            break;
                                        default:
                                            string msg = "The operation " + operation + "doesn't exist.";
                                            _storeError( BatchNode, msg );
                                            break;
                                    } //switch (operation)
                                }
                                else
                                {
                                    string msg = "The user " + BatchData.Username + " does not have permission to edit a Container.";
                                    _storeError( BatchNode, msg );
                                }

                                BatchData.Operations.RemoveAt( 0 );

                            } //forloop

                        } //if(null != Usernode)
                        else
                        {
                            _storeError( BatchNode, "The user " + BatchData.Username + " does not exist." );
                        }
                    }
                    else
                    {
                        // Send errors via an email
                        if( false == BatchNode.Log.Empty )
                        {
                            _emailErrors( BatchNode );
                        }
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

        private CswNbtNode _getNodeFromBarcode( string barcode, CswEnumNbtObjectClass objClassType, string barcodePropertyName, bool IncludeDefaultFilters = true )
        {
            CswNbtNode returnNode = null;

            CswNbtView objClassView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass objClass = _CswNbtResources.MetaData.getObjectClass( objClassType );
            CswNbtMetaDataObjectClassProp barcodeOCP = objClass.getObjectClassProp( barcodePropertyName );

            CswNbtViewRelationship parent = objClassView.AddViewRelationship( objClass, IncludeDefaultFilters );
            objClassView.AddViewPropertyAndFilter( parent,
                                                    MetaDataProp: barcodeOCP,
                                                    Value: barcode,
                                                    FilterMode: CswEnumNbtFilterMode.Equals );

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( objClassView, false, true, true );
            Tree.goToRoot();
            for( int i = 0; i < Tree.getChildNodeCount(); i++ )
            {
                Tree.goToNthChild( i );
                returnNode = Tree.getNodeForCurrentPosition();
                Tree.goToParentNode();
            }

            return returnNode;
        }

        #endregion

        private void _dispose( string operation, string barcode, CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                CswNbtObjClassContainer ContainerNode = _getNodeFromBarcode( barcode, CswEnumNbtObjectClass.ContainerClass, CswNbtObjClassContainer.PropertyName.Barcode );
                if( null != ContainerNode )
                {
                    ContainerNode.DisposeContainer();
                    ContainerNode.postChanges( false );
                }
                else
                {
                    string msg = _generateErrorMessage( operation, barcode, null, "A container with barcode " + barcode + " does not exist." );
                    _storeError( BatchNode, msg );
                }
            }
            catch( Exception e )
            {
                string msg = "The dispose operation failed for the container barcode " + barcode + "with exception: " + e;
                _storeError( BatchNode, msg );
            }
        }//_dispose()

        private void _move( string operation, string barcode, JObject update, CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                string newLocationBarcode = update["location"].ToString();

                CswNbtNode newLocationNode = _getNodeFromBarcode( newLocationBarcode, CswEnumNbtObjectClass.LocationClass, CswNbtObjClassLocation.PropertyName.Barcode );
                if( null != newLocationNode )
                {
                    CswPrimaryKey newLocationNodeId = newLocationNode.NodeId;

                    CswNbtObjClassContainer containerNode = _getNodeFromBarcode( barcode, CswEnumNbtObjectClass.ContainerClass, CswNbtObjClassContainer.PropertyName.Barcode );
                    if( null != containerNode )
                    {
                        containerNode.MoveContainer( newLocationNodeId );
                        containerNode.postChanges( false );
                    }
                    else
                    {
                        string msg = _generateErrorMessage( operation, barcode, new Dictionary<string, string> { { "New Location", newLocationBarcode } }, "A container with barcode " + barcode + " does not exist." );
                        _storeError( BatchNode, msg );
                    }
                }
                else
                {
                    string msg = _generateErrorMessage( operation, barcode, new Dictionary<string, string> { { "New Location", newLocationBarcode } }, "The Location barcode, " + newLocationBarcode + ", does not exist" );
                    _storeError( BatchNode, msg );
                }
            }
            catch( Exception e )
            {
                string msg = "The move operation failed for the container barcode " + barcode + "with exception: " + e;
                _storeError( BatchNode, msg );
            }
        }//_move()

        private void _updateOwner( string operation, string barcode, JObject update, CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                string newOwnerBarcode = update["user"].ToString();

                CswNbtNode newOwnerNode = _getNodeFromBarcode( newOwnerBarcode, CswEnumNbtObjectClass.UserClass, CswNbtObjClassUser.PropertyName.Barcode );
                if( null != newOwnerNode )
                {
                    CswPrimaryKey newOwnerNodeId = newOwnerNode.NodeId;

                    CswNbtObjClassContainer containerNode = _getNodeFromBarcode( barcode, CswEnumNbtObjectClass.ContainerClass, CswNbtObjClassContainer.PropertyName.Barcode );
                    if( null != containerNode )
                    {
                        containerNode.UpdateOwner( newOwnerNodeId );
                        containerNode.postChanges( false );
                    }
                    else
                    {
                        string msg = _generateErrorMessage( operation, barcode, new Dictionary<string, string> { { "New Owner", newOwnerBarcode } }, "A container with barcode " + barcode + " does not exist." );
                        _storeError( BatchNode, msg );
                    }
                }
                else
                {
                    string msg = _generateErrorMessage( operation, barcode, new Dictionary<string, string> { { "New Owner", newOwnerBarcode } }, "The User barcode, " + newOwnerBarcode + ", does not exist" );
                    _storeError( BatchNode, msg );
                }
            }
            catch( Exception e )
            {
                string msg = "The update owner operation failed for the container barcode " + barcode + "with exception: " + e;
                _storeError( BatchNode, msg );
            }
        }//_updateOwner()

        private void _transfer( string operation, string barcode, JObject update, CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                string newOwnerBarcode = update["user"].ToString();
                CswNbtNode newOwnerNode = _getNodeFromBarcode( newOwnerBarcode, CswEnumNbtObjectClass.UserClass, CswNbtObjClassUser.PropertyName.Barcode );
                if( null != newOwnerNode )
                {
                    CswPrimaryKey newOwnerNodeId = newOwnerNode.NodeId;

                    CswNbtObjClassContainer containerNode = _getNodeFromBarcode( barcode, CswEnumNbtObjectClass.ContainerClass, CswNbtObjClassContainer.PropertyName.Barcode );
                    if( null != containerNode )
                    {
                        containerNode.TransferContainer( newOwnerNodeId );
                        containerNode.postChanges( false );
                    }
                    else
                    {
                        string msg = _generateErrorMessage( operation,
                                                           barcode,
                                                           new Dictionary<string, string>
                                                               {
                                                                   {"New Owner", newOwnerBarcode},
                                                                   {"New Location", update["location"].ToString()}
                                                               },
                                                           "A container with barcode " + barcode + " does not exist." );
                        _storeError( BatchNode, msg );
                    }
                }
                else
                {
                    string msg = _generateErrorMessage( operation,
                                                           barcode,
                                                           new Dictionary<string, string>
                                                               {
                                                                   {"New Owner", newOwnerBarcode},
                                                                   {"New Location", update["location"].ToString()}
                                                               },
                                                           "The User barcode, " + newOwnerBarcode + ", does not exist" );
                    _storeError( BatchNode, msg );
                }
            }
            catch( Exception e )
            {
                string msg = "The transfer operation failed for the container barcode " + barcode + "with exception: " + e;
                _storeError( BatchNode, msg );
            }
        }//_transfer()

        private void _dispense( string operation, string barcode, JObject update, CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                string uom = update["uom"].ToString().ToLower();
                CswPrimaryKey uomId = null;
                CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
                foreach( CswNbtObjClassUnitOfMeasure currentUnitOfMeasureNode in UnitOfMeasureOC.getNodes( false, false, false, true ) )
                {
                    string unitName = currentUnitOfMeasureNode.NodeName.ToLower();
                    if( unitName == uom )
                    {
                        uomId = currentUnitOfMeasureNode.NodeId;
                    }
                }

                if( null != uomId )
                {
                    CswNbtObjClassContainer containerNode = _getNodeFromBarcode( barcode, CswEnumNbtObjectClass.ContainerClass, CswNbtObjClassContainer.PropertyName.Barcode );
                    if( null != containerNode )
                    {
                        containerNode.DispenseOut( CswEnumNbtContainerDispenseType.Dispense, CswConvert.ToDouble( update["qty"] ), uomId );
                        containerNode.postChanges( false );
                    }
                    else
                    {
                        string msg = _generateErrorMessage( operation, barcode, new Dictionary<string, string> { { "Dispense Data", update.ToString() } }, "A container with barcode " + barcode + " does not exist." );
                        _storeError( BatchNode, msg );
                    }
                }
                else
                {
                    string msg = _generateErrorMessage( operation, barcode, new Dictionary<string, string> { { "Dispense Data", update.ToString() } }, "The UOM of " + uom + " that was provided does not exist." );
                    _storeError( BatchNode, msg );
                }
            }
            catch( Exception e )
            {
                string msg = "The dispense operation failed for the container barcode " + barcode + "with exception: " + e;
                _storeError( BatchNode, msg );
            }
        }//_dispense()

        private void _reconcile( string operation, string barcode, JObject update, CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                string newLocationBarcode = update["location"].ToString();

                CswNbtObjClassContainer containerNode = _getNodeFromBarcode( barcode, CswEnumNbtObjectClass.ContainerClass, CswNbtObjClassContainer.PropertyName.Barcode, false );
                if( null != containerNode )
                {
                    containerNode.CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions.ReconcileScans, newLocationBarcode, barcode );
                    containerNode.Location.RefreshNodeName();
                    containerNode.postChanges( false );
                }
                else
                {
                    string msg = _generateErrorMessage( operation, barcode, null, "A container with barcode " + barcode + " does not exist." );
                    _storeError( BatchNode, msg );
                }
            }
            catch( Exception e )
            {
                string msg = "The reconcile operation failed for the container barcode " + barcode + "with exception: " + e;
                _storeError( BatchNode, msg );
            }
        }//_reconcile()

        private string _generateErrorMessage( string operation, string barcode, Dictionary<string, string> ExtraInfo, string error )
        {
            string Ret = "";
            Ret += "Operation: " + operation + ";";
            Ret += "Container Barcode: " + barcode + ";";
            if( null != ExtraInfo )
            {
                Ret = ExtraInfo.Aggregate( Ret, ( current, keyValuePair ) => current + ( keyValuePair.Key + ": " + keyValuePair.Value + ";" ) );
            }
            Ret += " Failure with error message: " + error;

            return Ret;
        }//_generateErrorMessage()

        private void _emailErrors( CswNbtObjClassBatchOp BatchNode )
        {
            if( null != _User )
            {
                if( false == _User.EmailProperty.Empty )
                {
                    CswMail cswMail = _CswNbtResources.CswMail;
                    string Subject = BatchNode.OpName.Text;
                    string Message = _HumanReadableLog.Aggregate( "", ( current, ErrorMessage ) => current + ( ErrorMessage + System.Environment.NewLine ) );
                    string Email = _User.Email;
                    string DisplayName = _User.FirstName + " " + _User.LastName;
                    CswMailMessage mailMessage = CswMail.makeMailMessage( Subject, Message, Email, DisplayName );

                    if( cswMail.send( mailMessage ) )
                    {
                        BatchNode.appendToLog( "Batch operation log message emailed to " + _User.Username + " successfully." );
                    }
                    else
                    {
                        BatchNode.appendToLog( "Batch operation log message email to " + _User.Username + " failed." );
                    }
                }
                else
                {
                    BatchNode.appendToLog( "Could not email log to " + _User.Username + " because they have not provided an email address." );
                }
            }
            else
            {
                BatchNode.appendToLog( "The user does not exist." );
            }
        }//_emailErrors()

        private void _storeError( CswNbtObjClassBatchOp BatchNode, string error )
        {
            BatchNode.appendToLog( error );
            _HumanReadableLog.Add( error );
        }//_storeError()

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

            private string _Username = "";
            public string Username
            {
                get
                {
                    if( string.IsNullOrEmpty( _Username ) )
                    {
                        if( null != _BatchData["username"] )
                        {
                            _Username = CswConvert.ToString( _BatchData["username"] );
                        }
                    }
                    return _Username;
                }
                set
                {
                    _Username = value;
                    _BatchData["username"] = _Username;
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
