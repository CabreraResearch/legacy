/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    var DesignSidebar = Ext.define('Ext.design.sidebar', {
        extend: 'Ext.panel.Panel',
        title: 'Design Mode',
        width: 325,
        height: 600,
        collapsible: true,
        collapseDirection: 'left',
        collapsed: true,
        expandOnShow: false,
        //hideCollapseTool: false,
        closable: true,
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
                config: {
                    buttonNames: {
                        versionsBtn: 'Versions',
                        copyBtn: 'Copy',
                        deleteBtn: 'Delete',
                        newBtn: 'New'
                    }
                },
                Refresh: null,
                isGenericClass: true,
                ajax: {

                }
            };
            var cswPublic = {};

            //Constructor
            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                if (Csw.isNullOrEmpty(cswParent)) {
                    //Todo: throw exception
                }
                
                // Hide the left div
                Csw.main.leftDiv.hide();

                // Create the sizebar
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
                var versionBtnCell = btnTbl.cell(1, 1).empty();
                var copyBtnCell = btnTbl.cell(1, 2).empty();
                var deleteBtnCell = btnTbl.cell(1, 3).empty();
                var newBtnCell = btnTbl.cell(1, 4).empty();

                cswPrivate.makeButton('Versions', versionBtnCell);
                cswPrivate.makeButton('Copy', copyBtnCell);
                cswPrivate.makeButton('Delete', deleteBtnCell);
                cswPrivate.makeButton('New', newBtnCell);
                //#endregion Buttons

                cswPublic.componentItem.br({ number: 2 });

                //#region Edit NodeType Form - Version 2

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
                    async: false,
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
                                    $.CswDialog('EditNodeDialog', {
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

                cswPublic.componentItem.br({ number: 2 });

                //#endregion Edit NodeType Form - Version 2

                //#region Add Properties
                var ajaxdata = {
                    NodeId: Csw.string(cswPrivate.tabState.nodeid),
                    NodeKey: Csw.string(cswPrivate.tabState.nodekey),
                    NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                    TabId: Csw.string(cswPrivate.tabState.tabid),
                    LayoutType: 'Edit' //always be edit? how will the user choose to add to the add-layout??
                };

                cswPrivate.ajax.addLayoutProps = Csw.ajax.post({
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
                        ds.loadData(propOpts);
                    } // success
                }); // Csw.ajax

                var extjscontrol = cswPublic.componentItem.div({ align: 'center' });

                var ds = Ext.create('Ext.data.ArrayStore', {
                    fields: ['value', 'display'],
                    data: [],
                    autoLoad: false
                });

                window.Ext.widget('form', {
                    title: 'Add Existing Property',
                    width: 275,
                    height: 150,
                    renderTo: extjscontrol.getId(),
                    layout: 'fit',
                    items: [{
                        anchor: '100%',
                        xtype: 'multiselect',
                        msgTarget: 'bottom',
                        name: 'addexistingprop',
                        id: 'add-existing-prop',
                        store: ds,
                        valueField: 'value',
                        displayField: 'display',
                        ddReorder: true,
                        maxSelections: 1 //NOT WORKING FOR SOME REASON
                    }]
                });

                extjscontrol.br({ number: 2 });

                var extjscontrol2 = cswPublic.componentItem.div({ align: 'center' });

                var ds2 = Ext.create('Ext.data.ArrayStore', {
                    fields: ['value', 'display'],
                    data: [],
                    autoLoad: false
                });

                cswPrivate.ajax.fieldTypes = Csw.ajax.post({
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
                        ds2.loadData(fieldTypes);
                    } // success
                }); // Csw.ajax


                window.Ext.widget('form', {
                    title: 'Add New Property',
                    width: 275,
                    height: 150,
                    //bodyPadding: 10,
                    renderTo: extjscontrol2.getId(),
                    layout: 'fit',
                    items: [{
                        anchor: '100%',
                        xtype: 'multiselect',
                        msgTarget: 'bottom',
                        name: 'addnewprop',
                        id: 'add-new-prop',
                        store: ds2,
                        valueField: 'value',
                        displayField: 'display',
                        ddReorder: true
                    }]
                });
                //#endregion Add Properties
                
                Ext.create('Ext.Button', {
                    text: 'Click me',
                    renderTo: cswPublic.componentItem.div().getId(),
                    handler: function () {
                        alert('You clicked the button!');
                    }
                });
            };


            cswPrivate.onTearDown = function () {
                cswParent.empty();
                Csw.iterate(cswPrivate.ajax, function (call, name) {
                    call.ajax.abort();
                    delete cswPrivate.ajax[name];
                });
                Csw.main.leftDiv.show();
            };

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

            cswPrivate.makeButton = function (bntName, cell) {
                cell.buttonExt({
                    enabledText: bntName,
                    disableOnClick: false,
                    onClick: function () {
                        Csw.tryExec(cswPrivate.onButtonClick, bntName);
                    }
                });
            };

            cswPrivate.onButtonClick = function (buttonName) {
                switch (buttonName) {
                    case 'Versions':

                        var posX = (document.documentElement.clientWidth / 2) - (400 / 2) + 0;
                        var posY = (document.documentElement.clientHeight / 2) - (200 / 2) + 0;

                        cswPrivate.extWindowVersions = Csw.composites.window(cswParent, {
                            title: 'Versions of ' + cswPrivate.tabState.nodetypename,
                            x: posX,
                            y: posY,
                            height: 200,
                            width: 400,
                            layout: 'fit',
                            modal: true
                        });

                        cswPrivate.extWindowVersions.attachToMe().span({
                            text: "So we really aren't sure what this will do yet but here is a placeholder for when we figure it out! :p"
                        });

                        break;
                    case 'Copy':
                        var windowItems = {};
                        
                        var posX = (document.documentElement.clientWidth / 2) - (400 / 2) + 0;
                        var posY = (document.documentElement.clientHeight / 2) - (200 / 2) + 0;

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

                                        // Prevent copy if quota is reached
                                        cswPrivate.ajax.checkQuota = Csw.ajaxWcf.post({
                                            urlMethod: 'Quotas/check',
                                            data: {
                                                NodeTypeId: cswPrivate.designNodeType.nodetypeid,
                                                NodeKey: cswPrivate.designNodeType.nodekey
                                            },
                                            success: function (data) {
                                                if (Csw.bool(data.HasSpace)) {

                                                    Csw.copyNode({
                                                        'nodeid': cswPrivate.designNodeType.nodeid,
                                                        'nodekey': '',
                                                        'onSuccess': function (nodeid, nodekey) {
                                                            //To do:
                                                            //  1.Reopen design mode with the new copied nodetype
                                                            cswPrivate.extWindowCopy.close();
                                                            //cswDlgPrivate.onCopyNode(nodeid, nodekey);
                                                            //note: the above calls the following which needs to be incorporated: 
                                                            //onAlterNode: function (nodeid, nodekey) {
                                                            //var state = Csw.clientState.getCurrent();
                                                            //refreshSelected({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true, 'searchid': state.searchid });
                                                            //},
                                                        },
                                                        'onError': function () {
                                                            //error
                                                        }
                                                    });

                                                } else {
                                                    cswPrivate.extWindowCopy.attachToMe().span({ text: 'You have used all of your purchased quota, and must purchase additional quota space in order to add more.' });
                                                } // if-else (Csw.bool(data.result)) {
                                            } // success()
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

                        cswPrivate.extWindowCopy.attachToMe().input({
                            name: 'testinput',
                            labelText: 'Copy As ',
                            value: cswPrivate.tabState.nodetypename + ' Copy'
                        });

                        break;
                    case 'Delete':
                        var windowItems = {};
                        
                        var posX = (document.documentElement.clientWidth / 2) - (400 / 2) + 0;
                        var posY = (document.documentElement.clientHeight / 2) - (200 / 2) + 0;

                        cswPrivate.extWindowDelete = Csw.composites.window(cswParent, {
                            title: 'Delete ' + cswPrivate.tabState.nodetypename,
                            y: posY,
                            x: posX,
                            height: 100,
                            width: 400,
                            layout: 'fit',
                            buttons: [
                                {
                                    text: 'Delete', handler: function () {
                                        var nodeids = [];
                                        nodeids.push(cswPrivate.designNodeType.nodeid);

                                        Csw.deleteNodes({
                                            nodeids: nodeids,
                                            //nodekeys: cswDlgPrivate.cswnbtnodekeys,
                                            onSuccess: function (nodeid, nodekey) {
                                                //To do:
                                                //  1. Delete the nodetype
                                                //  2. Close design mode
                                                cswPrivate.extWindowDelete.close();
                                                cswPublic.close();
                                                //NOTE: DO WE NEED THE FOLLOWING:
                                                //Csw.publish(Csw.enums.events.CswNodeDelete,
                                                //    { nodeids: nodeids, cswnbtnodekeys: [] 
                                                //    });
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
                            text: "Are you sure you want to delete the " + cswPrivate.tabState.nodetypename + " nodetype?"
                        });

                        break;
                    case 'New':

                        var windowItems = {};
                        
                        var posX = (document.documentElement.clientWidth / 2) - (400 / 2) + 0;
                        var posY = (document.documentElement.clientHeight / 2) - (200 / 2) + 0;

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
                            onSave: function (nodeid, nodekey, tabcount, nodename, nodelink) {
                                //To do:
                                //  1. Create the new nodetype
                                //  2. Create a temporary node
                                //  3. Change the view to the temporary node
                                //  4. Open design mode on the temporary node
                                cswPublic.close(nodeid, nodekey, tabcount, nodename, nodelink);
                                cswPrivate.extWindowNew.close();
                            },
                            onInitFinish: function () { }
                        });

                        break;
                    default:
                }
            };

            //constructor
            (function _postCtor() {
                cswPrivate.init();
            }());

            //#endregion _postCtor

            return cswPublic;

        });
})();
