/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.managelocations = Csw.actions.managelocations ||
        Csw.actions.register('managelocations', function (cswParent, options) {
            'use strict';
            var o = {
                saveUrlMethod: 'savemanagelocations',
                name: 'action_managelocations',
                actionjson: null,
                onQuotaChange: null // function () { }
            };

            if (options) {
                Csw.extend(o, options);
            }

            o.action = Csw.layouts.action( cswParent, {
                title: 'Manage Locations',
                useFinish: false,
                useCancel: false
                } );

    
            o.action.actionDiv.css( { padding: '10px' } ); 

            //HTML table kung-fu
            var actionTable = o.action.actionDiv.table();

            //actionTable.p
//            debugger;
//            actionTable.propDom( 'border', '1'); 
//            actionTable.css({ width: '100%' }); 
            var treeCell = actionTable.cell(2, 1);
            //treeCell.css( {'width' : '1500px' } ); 

             
            var includeChildrenTable = actionTable.cell(1, 1).table();
            actionTable.cell(1, 1).css( { 'vertical-align' : 'bottom' } );
            var includeChildrenCheckboxLabelCell = includeChildrenTable.cell(1, 1);
            var includeChildrenCheckboxCell = includeChildrenTable.cell(1, 2);


            var closeButtonCell = actionTable.cell(3, 1);

            var rightSideTable = actionTable.cell(2, 2).table();
            actionTable.cell(1, 2).propDom( 'rowspan', 2 ); 
            actionTable.cell(1, 2).css( { 'vertical-align' : 'top' } );

            //cells for labels (row must correspond to control cell in next group)
//            var inventory_group_label_cell = rightSideTable.cell(1, 1);
//            var storageCompatability_label_cell = rightSideTable.cell(2, 1);
//            var allowInventory_label_cell = rightSideTable.cell(3, 1);
//            var control_zone_label_cell = rightSideTable.cell(4, 1);

            //cells for controls
//            var select_inventory_group_cell = rightSideTable.cell(1, 2);
//            var select_storageCompatability_cell = rightSideTable.cell(2, 2);
//            var select_allowInventory_cell = rightSideTable.cell(3, 2);
//            var select_control_zone_cell = rightSideTable.cell(4, 2);
//            var monster_controls_div =


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
            
            var selectedLocationValues = { };
            

            var inventory_group_select = null;
            var storageCompatability_select = null;
            var propertyControls = null; 
            var allowInventory_select = null;
            var control_zone_select = null;

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

                    //Labels
//                    inventory_group_label_cell.span({ text: 'Inventory Group:' }).addClass('propertylabel');
//                    storageCompatability_label_cell.span({ text: 'Storage Compatability:' }).addClass('propertylabel');
//                    allowInventory_label_cell.span({ text: 'Allow Inventory:' }).addClass('propertylabel');
//                    control_zone_label_cell.span({ text: 'Control Zone:' }).addClass('propertylabel');
//                

                    propertyControlsContainer.empty();
                    //Retrieve the node data for the currently selected node
                    propertyControls = Csw.layouts.tabsAndProps(propertyControlsContainer, {
                        tabState: {
                            excludeOcProps: ['name', 'child location type', 'location template', 'location', 'order', 'rows', 'columns', 'barcode', 'location code', 'containers', 'save', 'inventory levels' ],
//                            propertyData: cswPrivate.state.properties,
                            //nodeid: 24704,
                            //nodetypeid: 969,
                            ShowAsReport: false,
                            nodeid: selectedNode.raw.nodeid,
                            nodetypeid: selectedNode.raw.nodetypeid ,
                            EditMode: Csw.enums.editMode.Temp, //sic.
                            showSaveButton: true
                        },
                        ReloadTabOnSave: false,
                        async: false,
                        onPropertyChange: function (propid, propName, propData) {
                            //TODO: This seems like a really bad plan. Why are we doing this?
                            var foo = "";
                            
                            switch( propName ) {
                                case 'Allow Inventory':
                                    selectedLocationValues.allowInventory = propData.values.checked;
                                    break;
                                    
                                case 'Storage Compatibility':
                                    selectedLocationValues.storageCompatability = propData.values.value;
                                    break;
                                    
                                case 'Inventory Group':
                                    selectedLocationValues.inventoryGroupId = propData.values.nodeid;
                                    break;
                                    
                                case 'Control Zone':
                                    selectedLocationValues.controlZoneId = propData.values.nodeid;
                                    break;
                                    
                            }//switch()

                        } 
                    });
                    

            } //initPropValSelectors()

            //checkChildrenOfCurrentCheckBox.checked 
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

            function initButtons() {

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

                        var AssignRequest = {
                            LocationNodeKeys: selectedLocationsNodeKeys,
                            SelectedInventoryGroupNodeId: selectedLocationValues.inventoryGroupId,
                            SelectedImages: selectedLocationValues.storageCompatability,
                            AllowInventory: selectedLocationValues.allowInventory,
                            SelectedControlZoneId: selectedLocationValues.controlZoneId
                        };

                        Csw.ajaxWcf.post({
                            urlMethod: 'Locations/assignPropsToLocations',
                            data: AssignRequest,
                            success: function (ajaxdata) { 
                                    initCheckBox();
                                    initTree();
                                }
                            });

                    },
                    enabledText: 'Apply Changes'
                });

                closeButtonCell.buttonExt({
                    name: 'close_action',
                    disableOnClick: false,
                    onClick: function () {
                        Csw.tryExec(options.onCancel);
                    },
                    enabledText: 'Close'
                });

            } //initButtons() 


            initTree();
            initCheckBox();
            initButtons();


            //initTable();
        }); // methods
} ());
