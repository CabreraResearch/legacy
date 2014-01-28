/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    var DesignSidebar = Ext.define('Ext.design.sidebar', {
        extend: 'Ext.panel.Panel',
        title: 'Design Mode',
        width: 290,
        height: 655,
        collapsible: true,
        collapseDirection: 'left',
        collapsed: true,
        expandOnShow: false,
        //hideCollapseTool: false,
        closable: false,
        items: [
            { //This is the empty component that you can attach outside elements to
                xtype: 'component',
                layout: 'fit'
            }
        ],
        initComponent: function () {
            this.callParent();
        },
        //header: false,
        bodyStyle: {
            background: '#164399',
            color: '#FFFFFF'
        },
        listeners: {
            beforeclose: null
        }
    });
    
    // This needs to be defined globally. It should only be defined once and then 
    //call new Sidebar to create instances of it

    Csw.designmode.sidebar = Csw.designmode.sidebar ||
        Csw.designmode.register('sidebar', function (cswParent, options) {
            'use strict';

            //#region Properties

            var cswPrivate = {
                name: 'sidebar',
                tabState: {
                    nodeid: '',
                    nodekey: '',
                    tabid: '',
                    tabNo: 0,
                    nodetypeid: 0,
                    nodetypename: '',
                    EditMode: 'Edit'
                },
                designNodeType: {
                    nodeid: '',
                    nodekey: '',
                    nodetypeid: '',
                    nodetypename: '',
                    objectclassid: ''
                },
                designNodeTypeProp: {
                    nodetypeid: ''
                },  
                Refresh: null,
                isGenericClass: true,
                ajax: {

                },
                buttons: {},
                existingPropIdToAdd: '',
                fieldTypeIdToAdd: '',
                nodeLayout: {}
            };
            var cswPublic = {};

            var buttons = {
                copyBtn: 'Copy',
                deleteBtn: 'Delete',
                newBtn: 'New',
                loadBtn: 'Load',
                addExistingBtn: 'Add Existing Prop',
                addNewBtn: 'Add New Prop'
            };

            var existingProperties = {
                div: {},
                dataStore: {},
                control: {}
            };
            
            var newProperties = {
                div: {},
                dataStore: {},
                control: {}
            };
            
            var nodeTypeSelect = {
                table: {},
                control: {}
            };
            
            //#endregion Properties

            //#region Constructor
            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                cswPrivate.buttons = {};

                if (Csw.isNullOrEmpty(cswParent)) {
                    //Todo: throw exception
                }
                
                //Hide the Tree
                Csw.main.leftDiv.hide();

                // Create the sidebar
                cswPrivate.newSidebar = new DesignSidebar({
                    renderTo: cswParent.getId(),
                    listeners: {
                        beforeclose: function(panel) {
                            cswPublic.tearDown();
                        }
                    }
                }).toggleCollapse();

                cswPublic.componentItem = Csw.domNode({
                    el: cswPrivate.newSidebar.items.items[0].getEl().dom,
                    ID: cswPrivate.newSidebar.items.items[0].getEl().id
                }).css({ overflow: 'auto' });

            }());
            //#endregion Constructor

            cswPrivate.init = function () {
                /// <summary>
                /// Initialize the sidebar
                /// </summary>

                // Add the title
                cswPrivate.nodetypeNameDiv = cswPublic.componentItem.div({ text: cswPrivate.tabState.nodetypename, cssclass: 'CswDesignMode_NTName' });
                cswPrivate.nodetypeNameDiv.br({ number: 2 });

                //#region Buttons
                var btnTbl = cswPublic.componentItem.table({
                    width: '100%',
                    cellalign: 'center'
                });
                var copyBtnCell = btnTbl.cell(1, 1).empty();
                var deleteBtnCell = btnTbl.cell(1, 2).empty();
                var newBtnCell = btnTbl.cell(1, 3).empty();
                var loadBtnCell = btnTbl.cell(1, 4).empty();

                cswPrivate.makeButton(buttons.copyBtn, copyBtnCell);
                cswPrivate.makeButton(buttons.deleteBtn, deleteBtnCell);
                cswPrivate.makeButton(buttons.newBtn, newBtnCell);
                //#endregion Buttons
                
                //#region Load NodeType
                
                cswPublic.componentItem.br();

                nodeTypeSelect.table = cswPublic.componentItem.table({
                    width: '100%',
                    cellalign: 'center'
                }).hide();
                
                nodeTypeSelect.table.cell(1, 1).div({ text: '&nbsp;Load: ', cssclass: 'CswDesignMode_NTName' });

                nodeTypeSelect.control = nodeTypeSelect.table.cell(1, 2).nodeTypeSelect({
                    name: 'design_nt_select',
                    value: cswPrivate.tabState.nodetypeid,
                    onSelect: function (val) {
                        cswPrivate.createTempNode(val);
                    }
                });
                
                cswPrivate.buttons[buttons.loadBtn] = loadBtnCell.buttonExt({
                    enabledText: buttons.loadBtn,
                    disableOnClick: false,
                    onClick: function () {
                        nodeTypeSelect.table.show();
                    }
                });

                cswPublic.componentItem.br();
                
                //#endregion Load NodeType

                //#region Edit NodeType Form

                var nodeTypePropsDiv = cswPublic.componentItem.div({
                    width: '100%',
                    //styles: { border: '1px solid' }
                });

                var editButtonDiv = cswPublic.componentItem.div({
                    width: '100%',
                    styles: { 'text-align': 'center' }
                });

                // Get the design nodetype node that corresponds to the current node
                cswPrivate.ajax.designNodeType = Csw.ajaxWcf.post({
                    urlMethod: 'Design/getDesignNodeType',
                    data: cswPrivate.tabState.nodetypeid,
                    success: function (data) {
                        if (false === Csw.isNullOrEmpty(data)) {
                            cswPrivate.designNodeType.nodeid = data.NodePk;
                            cswPrivate.designNodeType.nodetypeid = data.NodeTypeId;

                            cswPrivate.tabsAndProps = Csw.layouts.tabsAndProps(nodeTypePropsDiv, {
                                tabState: {
                                    nodeid: cswPrivate.designNodeType.nodeid,
                                    ShowAsReport: false,
                                    nodetypeid: cswPrivate.designNodeType.nodetypeid,
                                    EditMode: Csw.enums.editMode.Table, //Note: Design NodeType nodes Table layout is configured specifically for the sidebar
                                    ReadOnly: true //Note: We want this to be readonly because users need to click the edit button to edit the nodetype
                                },
                                ReloadTabOnSave: false,
                                async: false
                            });//cswPrivate.tabsAndProps

                            editButtonDiv.buttonExt({
                                enabledText: 'Edit',
                                disableOnClick: false,
                                onClick: function () {
                                    // Open a dialog to edit the properties of the design nodetype
                                    Csw.dialogs.editnode({
                                        currentNodeId: cswPrivate.designNodeType.nodeid,
                                        filterToPropId: '',
                                        title: 'Edit Node',
                                        onEditNode: null // function (nodeid, nodekey) { }
                                    });
                                }
                            });//editButtonDiv.buttonExt
                        }
                    },
                    error: function () {
                        //ERRRRRRRRRRRRROR
                    }
                });

                cswPublic.componentItem.br();

                //#endregion Edit NodeType Form

                //#region Add Properties
                existingProperties.div = cswPublic.componentItem.div({ align: 'center' });

                newProperties.div = cswPublic.componentItem.div({ align: 'center' });
                cswPrivate.loadNewProperties();
                
                //#endregion Add Properties
            };


            cswPrivate.onTearDown = function () {
                cswParent.empty();
                Csw.iterate(cswPrivate.ajax, function (call, name) {
                    if (call.ajax) {
                        call.ajax.abort();
                    }
                    delete cswPrivate.ajax[name];
                });
                Csw.main.leftDiv.show();
            };
            
            //#region Button Logic

            cswPrivate.makeButton = function (btnName, div) {
                cswPrivate.buttons[btnName] = div.buttonExt({
                    enabledText: btnName,
                    disableOnClick: false,
                    onClick: function () {
                        Csw.tryExec(cswPrivate.onButtonClick, btnName);
                    }
                });
            };
            
            var posX = (document.documentElement.clientWidth / 2) - (400 / 2) + 0;
            var posY = (document.documentElement.clientHeight / 2) - (200 / 2) + 0;

            cswPrivate.onButtonClick = function (buttonName) {
                
                switch (buttonName) {
                    case buttons.copyBtn:

                        cswPrivate.extWindowCopy = Csw.composites.window(cswParent, {
                            title: 'Copy ' + cswPrivate.tabState.nodetypename,
                            x: posX,
                            y: posY,
                            height: 200,
                            width: 400,
                            layout: 'fit',
                            buttons: [
                                {
                                    text: 'Copy', handler: function () {
                                        Csw.ajax.deprecatedWsNbt({
                                            urlMethod: 'CopyNode',
                                            data: {
                                                NodeId: cswPrivate.designNodeType.nodeid,
                                                NodeKey: Csw.string('')
                                            },
                                            success: function (result) {
                                                cswPrivate.createTempNode(result.NewRelationalId);
                                                cswPrivate.extWindowCopy.close();
                                            }
                                        });
                                    }
                                },
                                {
                                    text: 'Cancel',
                                    handler: function () {
                                        cswPrivate.extWindowCopy.close();
                                    }
                                }
                            ]
                        });
                        
                        cswPrivate.extWindowCopy.attachToMe().div({
                            text: 'Copying ' + cswPrivate.tabState.nodetypename
                        });

                        break;
                    case buttons.deleteBtn:

                        cswPrivate.extWindowDelete = Csw.composites.window(cswParent, {
                            title: 'Delete ' + cswPrivate.tabState.nodetypename,
                            y: posY,
                            x: posX,
                            height: 150,
                            width: 450,
                            layout: 'fit',
                            buttons: [
                                {
                                    text: 'Delete', handler: function () {
                                        var nodeids = [];
                                        nodeids.push(cswPrivate.designNodeType.nodeid);
                                        Csw.deleteNodes({
                                            nodeids: nodeids,
                                            onSuccess: function (nodeid, nodekey) {
                                                cswPrivate.extWindowDelete.close();
                                                cswPublic.close();
                                                Csw.main.clear({ 'all': true });
                                                //TODO - Case 31657 - maybe look into reopening a blank design mode after delete?
                                                Csw.main.refreshWelcomeLandingPage();
                                            },
                                            onError: function () { }
                                        });
                                    }
                                },
                                {
                                    text: 'Cancel', handler: function () {
                                        cswPrivate.extWindowDelete.close();
                                    }
                                }
                            ]
                        });

                        cswPrivate.extWindowDelete.attachToMe().div({
                            text: 'Are you sure you want to delete the "' + cswPrivate.tabState.nodetypename +
                                '" nodetype?  All instances of "' + cswPrivate.tabState.nodetypename + '" will be deleted.' +
                                '<br/><br/>Warning: this action <strong>cannot</strong> be undone.'
                        });

                        break;
                    case buttons.newBtn:

                        cswPrivate.extWindowNew = Csw.composites.window(cswParent, {
                            title: 'New Design NodeType',
                            y: posY,
                            x: posX,
                            height: 325,
                            width: 500,
                            layout: 'fit',
                            buttons: [
                                {
                                    text: 'Cancel', handler: function () {
                                        cswPrivate.extWindowNew.close();
                                    }
                                }
                            ]
                        });

                        var table = cswPrivate.extWindowNew.attachToMe().table({
                            name: 'table',
                            width: '100%'
                        });

                        cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(table.cell(1, 1), {
                            name: 'tabsAndProps',
                            tabState: {
                                ShowAsReport: false,
                                nodetypeid: cswPrivate.designNodeType.nodetypeid,
                                relatednodeid: cswPrivate.designNodeType.nodeid,
                                relatednodename: cswPrivate.tabState.nodetypename,
                                relatednodetypeid: cswPrivate.designNodeType.nodetypeid,
                                relatedobjectclassid: cswPrivate.designNodeType.objectclassid,
                                EditMode: Csw.enums.editMode.Add
                            },
                            ReloadTabOnSave: false,
                            onSave: function (nodeid, nodekey, tabcount, nodename, nodelink, relationalid) {
                                cswPrivate.createTempNode(relationalid);
                                cswPrivate.extWindowNew.close();
                            },
                            onInitFinish: function () { }
                        });

                        break;
                    default:
                }
            };
            
            //#endregion Button Logic

            cswPrivate.createTempNode = function(designNTNodeId) {
                Csw.ajaxWcf.post({
                    urlMethod: 'Nodes/createTempNode',
                    data: designNTNodeId,
                    success: function (data) {
                        Csw.clientDb.setItem('openDesignMode', true);
                        Csw.main.handleItemSelect({
                            type: 'view',
                            mode: 'tree',
                            itemid: data.ViewId
                        });
                    },
                    error: function () {
                        //ERRRRRRRRRRRRROR
                    }
                });
            };
            
            //#region Existing Properties

            cswPrivate.loadExistingProperties = function (data) {
                existingProperties.dataStore = Ext.create('Ext.data.ArrayStore', {
                    fields: ['value', 'display'],
                    data: [],
                    autoLoad: false
                });

                var ajaxdata = {
                    NodeId: Csw.string(cswPrivate.tabState.nodeid),
                    NodeKey: Csw.string(cswPrivate.tabState.nodekey),
                    NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                    TabId: Csw.string(cswPrivate.tabState.tabid),
                    LayoutType: 'Edit'
                };
                
                if (data) {
                    Csw.extend(ajaxdata, data);
                }

                cswPrivate.ajax.addLayoutProps = Csw.ajax.deprecatedWsNbt({
                    urlMethod: 'getPropertiesForLayoutAdd',
                    data: ajaxdata,
                    success: function (data) {
                        var propOpts = [];
                        Csw.each(data.add, function (p) {
                            var display = p.propname;
                            if (Csw.bool(p.hidden)) {
                                display += ' (hidden)';
                            }
                            propOpts.push([
                                p.propid,
                               display
                            ]);
                        });
                        existingProperties.dataStore.loadData(propOpts);
                    }
                });

                existingProperties.control = window.Ext.widget('form', {
                    title: 'Add Existing Property',
                    width: 275,
                    height: 150,
                    renderTo: existingProperties.div.getId(),
                    layout: 'fit',
                    items: [{
                        anchor: '100%',
                        xtype: 'multiselect',
                        msgTarget: 'bottom',
                        name: 'addexistingprop',
                        id: 'add-existing-prop',
                        store: existingProperties.dataStore,
                        valueField: 'value',
                        displayField: 'display',
                        ddReorder: true,
                        maxSelections: 1, //NOT WORKING FOR SOME REASON
                        listeners: {
                            change: function (thisList, newVal, oldVal, eOpts) {
                                if (newVal.length === 1) {
                                    cswPrivate.existingPropIdToAdd = newVal[0];
                                    cswPrivate.buttons[buttons.addExistingBtn].enable();
                                    cswPrivate.buttons[buttons.addExistingBtn + '_menu'].enable();
                                } else {
                                    cswPrivate.buttons[buttons.addExistingBtn].disable();
                                    cswPrivate.buttons[buttons.addExistingBtn + '_menu'].disable();
                                }
                            }
                        }
                    }]
                });
                existingProperties.div.br();
                cswPrivate.createAddExistingButtons();
            };

            cswPrivate.createAddExistingButtons = function () {
                var onAddExistingClick = function (isIdentityTab) {
                    var tabid = cswPrivate.nodeLayout.getActiveTabId();
                    if (isIdentityTab) {
                        tabid = cswPrivate.nodeLayout.getIdentityTabId();
                    }
                    //TODO (bonus) - implement drag and drop from the existing props list to the nodelayout
                    Csw.ajaxWcf.post({
                        urlMethod: 'Design/updateLayout',
                        data: {
                            layout: cswPrivate.nodeLayout.getActiveLayout(),
                            nodetypeid: cswPrivate.tabState.nodetypeid,
                            tabid: tabid,
                            props: [{
                                nodetypepropid: cswPrivate.existingPropIdToAdd
                            }]
                        },
                        success: function () {
                            cswPrivate.nodeLayout.refresh();
                        }
                    });
                    
                };
                var existingPropertiesTable = existingProperties.div.table();
                var menuOn = cswPrivate.nodeLayout.getActiveLayout() === 'Edit';
                
                cswPrivate.buttons[buttons.addExistingBtn] = existingPropertiesTable.cell(1,1).buttonExt({
                    enabledText: buttons.addExistingBtn,
                    disableOnClick: false,
                    onClick: Csw.method(function () { onAddExistingClick(); })
                });
                cswPrivate.buttons[buttons.addExistingBtn].disable();

                cswPrivate.buttons[buttons.addExistingBtn + '_menu'] = existingPropertiesTable.cell(1,2).menuButton({
                    selectedText: buttons.addExistingBtn,
                    menu: [{
                        text: buttons.addExistingBtn,
                        icon: Csw.getIconUrlString(16, Csw.enums.iconType.add),
                        handler: Csw.method(function () { onAddExistingClick(); })
                    }, {
                        text: 'Add to Identity Tab',
                        icon: Csw.getIconUrlString(16, Csw.enums.iconType.info),
                        handler: Csw.method(function () { onAddExistingClick(true); })
                    }],
                    onClick: Csw.method(function () { onAddExistingClick(); })
                });
                cswPrivate.buttons[buttons.addExistingBtn + '_menu'].disable();
                
                if (menuOn) {
                    cswPrivate.buttons[buttons.addExistingBtn].hide();
                } else {
                    cswPrivate.buttons[buttons.addExistingBtn + '_menu'].hide();
                }
                
                existingProperties.div.br({ number: 2 });
            };
            
            //#endregion Existing Properties
            
            //#region New Properties
            
            cswPrivate.loadNewProperties = function () {
                newProperties.dataStore = Ext.create('Ext.data.ArrayStore', {
                    fields: ['value', 'display'],
                    data: [],
                    autoLoad: false
                });

                cswPrivate.ajax.fieldTypes = Csw.ajax.deprecatedWsNbt({
                    urlMethod: 'getFieldTypes',
                    success: function (data) {
                        var fieldTypes = [];
                        Csw.each(data.fieldtypes, function (f) {
                            var display = f.fieldtypename;
                            fieldTypes.push([
                               f.fieldtypeid,
                               display
                            ]);
                        });
                        newProperties.dataStore.loadData(fieldTypes);
                    } // success
                }); // Csw.ajax


                newProperties.control = window.Ext.widget('form', {
                    title: 'Add New Property',
                    width: 275,
                    height: 150,
                    //bodyPadding: 10,
                    renderTo: newProperties.div.getId(),
                    layout: 'fit',
                    items: [{
                        anchor: '100%',
                        xtype: 'multiselect',
                        msgTarget: 'bottom',
                        name: 'addnewprop',
                        id: 'add-new-prop',
                        store: newProperties.dataStore,
                        valueField: 'value',
                        displayField: 'display',
                        ddReorder: true,
                        listeners: {
                            change: function (thisList, newVal, oldVal, eOpts) {
                                if (newVal.length === 1) {
                                    cswPrivate.fieldTypeIdToAdd = newVal[0];
                                    cswPrivate.buttons[buttons.addNewBtn].enable();
                                    cswPrivate.buttons[buttons.addNewBtn + '_menu'].enable();
                                } else {
                                    cswPrivate.buttons[buttons.addNewBtn].disable();
                                    cswPrivate.buttons[buttons.addNewBtn + '_menu'].disable();
                                }
                            }
                        }
                    }]

                });

                newProperties.div.br();
                
                var onAddNewClick = function (isIdentityTab) {
                    cswPrivate.ajax.designNodeType = Csw.ajaxWcf.post({
                        urlMethod: 'Design/getDesignNodeTypePropDefinition',
                        data: cswPrivate.fieldTypeIdToAdd,
                        success: function (data) {
                            if (false === Csw.isNullOrEmpty(data)) {
                                cswPrivate.designNodeTypeProp.nodetypeid = data.NodeTypeId;

                                cswPrivate.extWindowNew = Csw.composites.window(cswParent, {
                                    title: 'Add New Property',
                                    y: posY,
                                    x: posX,
                                    height: 325,
                                    width: 500,
                                    layout: 'fit',
                                    buttons: [{
                                        text: 'Cancel', handler: function () {
                                            cswPrivate.extWindowNew.close();
                                        }
                                    }]
                                });

                                var table = cswPrivate.extWindowNew.attachToMe().table({
                                    name: 'table',
                                    width: '100%'
                                });

                                cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(table.cell(1, 1), {
                                    name: 'tabsAndProps',
                                    tabState: {
                                        ShowAsReport: false,
                                        nodetypeid: cswPrivate.designNodeTypeProp.nodetypeid,
                                        relatednodeid: cswPrivate.designNodeType.nodeid,
                                        relatednodename: cswPrivate.tabState.nodetypename,
                                        relatednodetypeid: cswPrivate.designNodeTypeProp.nodetypeid,
                                        relatedobjectclassid: cswPrivate.designNodeType.objectclassid,
                                        EditMode: Csw.enums.editMode.Add
                                    },
                                    ReloadTabOnSave: false,
                                    onSave: function (nodeid, nodekey, tabcount, nodename, nodelink, relationalid) {
                                        var tabid = cswPrivate.nodeLayout.getActiveTabId();
                                        if (isIdentityTab) {
                                            tabid = cswPrivate.nodeLayout.getIdentityTabId();
                                        }
                                        Csw.ajaxWcf.post({
                                            urlMethod: 'Design/updateLayout',
                                            data: {
                                                layout: cswPrivate.nodeLayout.getActiveLayout(),
                                                nodetypeid: cswPrivate.tabState.nodetypeid,
                                                tabid: tabid,
                                                props: [{
                                                    nodetypepropid: relationalid,
                                                    removeexisting: true
                                                }]
                                            }
                                        });
                                        cswPrivate.nodeLayout.refresh();
                                        cswPrivate.extWindowNew.close();
                                    },
                                    onInitFinish: function () { }
                                });
                            }
                        }
                    });
                };
                
                var newPropertiesTable = newProperties.div.table();

                cswPrivate.buttons[buttons.addNewBtn] = newPropertiesTable.cell(1,1).buttonExt({
                    enabledText: buttons.addNewBtn,
                    disableOnClick: false,
                    onClick: Csw.method(function () { onAddNewClick(); })
                }).hide();
                cswPrivate.buttons[buttons.addNewBtn].disable();
                cswPrivate.buttons[buttons.addNewBtn + '_menu'] = newPropertiesTable.cell(1,2).menuButton({
                    selectedText: buttons.addNewBtn,
                    menu: [{
                        text: buttons.addNewBtn,
                        icon: Csw.getIconUrlString(16, Csw.enums.iconType.add),
                        handler: Csw.method(function () { onAddNewClick(); })
                    }, {
                        text: 'Add to Identity Tab',
                        icon: Csw.getIconUrlString(16, Csw.enums.iconType.info),
                        handler: Csw.method(function () { onAddNewClick(true); })
                    }],
                    onClick: Csw.method(function () { onAddNewClick(); })
                });
                cswPrivate.buttons[buttons.addNewBtn + '_menu'].disable();
            };
            
            //#endregion New Properties
            
            //#region Public
            
            cswPublic.tearDown = function () {
                cswPrivate.onTearDown();
            };

            cswPublic.close = function () {
                cswParent.$.animate({ "left": "-=320px" }, "slow");
                cswParent.data('hidden', true);
            };

            cswPublic.open = function () {
                cswParent.$.animate({ "left": "+=320px" }, "slow");
                cswParent.data('hidden', false);
            };

            cswPublic.setNodeLayout = function(nodeLayout) {
                cswPrivate.nodeLayout = nodeLayout;
            };
            
            cswPublic.refreshExistingProperties = function (layoutMode, tabid) {
                if (Csw.isNullOrEmpty(layoutMode)) {
                    layoutMode = 'Edit';
                }
                if (tabid) {
                    cswPrivate.tabState.tabid = tabid;
                }
                existingProperties.div.empty();
                cswPrivate.loadExistingProperties({
                    NodeId: Csw.string(cswPrivate.tabState.nodeid),
                    NodeKey: Csw.string(cswPrivate.tabState.nodekey),
                    NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                    TabId: Csw.string(tabid),
                    LayoutType: layoutMode
                });
            };

            cswPublic.toggleIdentityTabOption = function(on) {
                if (cswPrivate.buttons[buttons.addNewBtn] && cswPrivate.buttons[buttons.addExistingBtn]) {
                    if (on) {
                        cswPrivate.buttons[buttons.addNewBtn].hide();
                        cswPrivate.buttons[buttons.addExistingBtn].hide();
                        cswPrivate.buttons[buttons.addNewBtn + '_menu'].show();
                        cswPrivate.buttons[buttons.addExistingBtn + '_menu'].show();
                    } else {
                        cswPrivate.buttons[buttons.addNewBtn + '_menu'].hide();
                        cswPrivate.buttons[buttons.addExistingBtn + '_menu'].hide();
                        cswPrivate.buttons[buttons.addNewBtn].show();
                        cswPrivate.buttons[buttons.addExistingBtn].show();
                    }
                }
            };

            //#endregion Public

            //#region constructor
            (function _postCtor() {
                cswPrivate.init();
            }());

            //#endregion _postCtor

            return cswPublic;

        });
})();
