/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.managelocations = Csw.actions.managelocations ||
        Csw.actions.register('managelocations', function (cswParent, options) {
            'use strict';
            var o = {
                saveUrlMethod: 'savemanagelocations',
                name: 'action_managelocations',
                actionjson: null,
                onQuotaChange: null 
            };

            if (options) {
                Csw.extend(o, options);
            }

            o.action = Csw.layouts.action( cswParent, {
                title: 'Manage Locations',
                useFinish: false,
                useCancel: true,
                cancelText: 'Close',
                onCancel: function () {
                   Csw.tryExec(options.onCancel);
                }
            } );

    
            o.action.actionDiv.css( { padding: '10px' } ); 

            //HTML table kung-fu
            var actionTable = o.action.actionDiv.table();


            var treeCell = actionTable.cell(2, 1);
            //treeCell.css( {'width' : '1500px' } ); 

             
            var includeChildrenTable = actionTable.cell(1, 1).table();
            actionTable.cell(1, 1).css( { 'vertical-align' : 'bottom' } );
            var includeChildrenCheckboxLabelCell = includeChildrenTable.cell(1, 1);
            var includeChildrenCheckboxCell = includeChildrenTable.cell(1, 2);

            var rightSideTable = actionTable.cell(2, 2).table();
            actionTable.cell(1, 2).propDom( 'rowspan', 2 ); 
            actionTable.cell(1, 2).css( { 'vertical-align' : 'top' } );

            var propertyControlsLeftPadding = '10px';
            var propertyControlsText = rightSideTable.cell(1, 1);
            propertyControlsText.css( { 'padding-left' : propertyControlsLeftPadding , 'padding-bottom' : '25px' , 'font-size' : '120%'  });
            propertyControlsText.append('Select the property values to assign to the checked locations. <BR>Then click <b>Apply Changes</b>'); 
            
            var propertyControlsCell = rightSideTable.cell(2, 1);
            propertyControlsCell.css( { 'padding-left' : propertyControlsLeftPadding , 'padding-bottom' : '10px'} );
            var propertyControlsContainer = propertyControlsCell.div();
            propertyControlsContainer.css( { 'background-color' : ':#D0EFFF' , 
                                               'border-radius' : '10px' } );
            var saveButtonCell = rightSideTable.cell(3, 1);
            saveButtonCell.css( { 'padding-left' : '400px' } );


            var mainTree = null;
            var selectedNode = null;
            
            var propertyControls = null; 

            var checkChildrenOfCurrentCheckBox = null;

            function initTree() {
                var propsArePopulated = false;
                Csw.ajaxWcf.get({
                    urlMethod: "Trees/locations",
                    success: function (data) {
                        treeCell.empty();
                        mainTree = Csw.nbt.nodeTreeExt(treeCell, {
                            width: 500,
                            overrideBeforeSelect: true,
                            ExpandAll: true,
                            forSearch: false,
                            PropsToShow: ['inventory group'],
                            onBeforeSelectNode: function ( node , tree ) 
                            {

                                selectedNode = node;
                                if( false == propsArePopulated ) {
                                    initPropValSelectors();
                                    propsArePopulated = true;
                                }
                                if( null != checkChildrenOfCurrentCheckBox && true == checkChildrenOfCurrentCheckBox.checked() ) 
                                {
                                    if( false == node.raw.checked ) //in other words, we are now toggling it to checked :-( 
                                    {
                                        mainTree.nodeTree.checkChildrenOfNode( node , true );

                                    } else 
                                    {
                                        mainTree.nodeTree.checkChildrenOfNode( node , false );
                                    }//if the client says to check children of checked node

                                }//if the next state of the node is checked

                                return (true);  //allow selection of multiple node types
                            }, 
                            isMulti: true, //checkboxes
                            state: {
                                viewId: data.NewViewId,
                                viewMode: "tree",
                                includeInQuickLaunch: false
                            }
                        });
                        

                    } //success
                }); //ajaxget

                
            } //initTree()

            function initPropValSelectors() {
            
                    var selectedNodeId = null;
                    if( ( null !== o.actionjson ) && ( null !== o.actionjson.ivgnodeid ) ) 
                    {
                        selectedNodeId = o.actionjson.ivgnodeid;
                    }


                    propertyControlsContainer.empty();
                    //Retrieve the node data for the currently selected node
                    propertyControls = Csw.layouts.tabsAndProps(propertyControlsContainer, {
                        tabState: {
                            excludeOcProps: ['name', 'child location type', 'location template', 'location', 'order', 'rows', 'columns', 'barcode', 'location code', 'containers', 'save', 'inventory levels' ],
                            ShowAsReport: false,
                            nodeid: selectedNode.raw.nodeid,
                            nodetypeid: selectedNode.raw.nodetypeid ,
                            EditMode: Csw.enums.editMode.Temp, //sic.
                            showSaveButton: true
                        },
                        Multi: true,
                       ReloadTabOnSave: false,
                        async: false 
                    });

            } //initPropValSelectors()

            function initCheckBox() {

                includeChildrenCheckboxLabelCell.empty();
                includeChildrenCheckboxLabelCell.span({ text: 'Include Children:' }).addClass('propertylabel');

                includeChildrenCheckboxCell.empty();
                checkChildrenOfCurrentCheckBox = includeChildrenCheckboxCell.input({
                    name: "include_children",
                    type: Csw.enums.inputTypes.checkbox,
                    checked: Csw.bool("false"),
                })
            }; //initCheckBox()

            function initSubmitButton() {

                saveButtonCell.buttonExt({
                    name: 'save_action',
                    disableOnClick: false,
                    onClick: function () {

                        var selectedLocationsNodeKeys = '';

                        Csw.each( mainTree.checkedNodes() , function ( node ) {
                            if( null !== node.nodeid ) 
                            {
                                selectedLocationsNodeKeys += node.nodekey + ',';
                            }
                        });
                        

                        var assignRequest = {
                            LocationNodeKeys: selectedLocationsNodeKeys,
                            UpdateAllowInventory: false,
                            UpdateStorageCompatability: false,
                            UpdateInventoryGroup: false,
                            UpdateControlZone: false
                        };



                        var selectedPropIds = propertyControls.getSelectedProps().split(',');
                        var allProps = propertyControls.getProps();

                        selectedPropIds.forEach( function ( currentPropId ) {
                            var currentKey = 'prop_' + currentPropId;
                            var currentProp = allProps[currentKey];
                            if( null != currentProp ) {
                                switch( currentProp.name ) {
                                    case 'Allow Inventory':
                                        //selectedLocationValues.allowInventory = currentProp.values.checked;
                                        assignRequest.AllowInventory = currentProp.values.checked;
                                        assignRequest.UpdateAllowInventory = true;
                                        break;
                                    
                                    case 'Storage Compatibility':
                                        //selectedLocationValues.storageCompatability = currentProp.values.value;
                                        assignRequest.SelectedImages = currentProp.values.value;
                                        assignRequest.UpdateStorageCompatability = true;
                                        break;
                                    
                                    case 'Inventory Group':
                                        //selectedLocationValues.inventoryGroupId = currentProp.values.nodeid;
                                        assignRequest.SelectedInventoryGroupNodeId = currentProp.values.nodeid;
                                        assignRequest.UpdateInventoryGroup = true;
                                        break;
                                    
                                    case 'Control Zone':
                                        //selectedLocationValues.controlZoneId = currentProp.values.nodeid;
                                        assignRequest.SelectedControlZoneId = currentProp.values.nodeid;
                                        assignRequest.UpdateControlZone = true;
                                        break;
                                    
                                }//switch()                               
                            }//function
                        });//forEach

                        Csw.ajaxWcf.post({
                            urlMethod: 'Locations/assignPropsToLocations',
                            data: assignRequest,
                            success: function (ajaxdata) { 
                                    initCheckBox();
                                    initTree();
                                }
                            });

                    },
                    enabledText: 'Apply Changes'
                });


            } //initSubmitButton() 


            initTree();
            initCheckBox();
            initSubmitButton();

        }); // methods
} ());
