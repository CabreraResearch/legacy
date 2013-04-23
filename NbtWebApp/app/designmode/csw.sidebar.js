/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

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
                // Setting up the css for the sidebar
                cswParent.addClass('CswDesignMode');

                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create a Button Group without a valid Csw Parent object.', 'Csw.controls.buttonGroup', 'csw.buttongroup.js', 62);
                }

            }());


            cswPrivate.init = function () {
                /// <summary>
                /// Initialize the sidebar
                /// </summary>

                cswPrivate.sidebarContent = cswParent.div({
                    width: '100%'
                });

                cswPrivate.nodetypeNameDiv = cswPrivate.sidebarContent.div();
                cswPrivate.nodetypeNameDiv.span({ text: cswPrivate.tabState.nodetypename, cssclass: 'CswDesignMode_NTName' });
                cswPrivate.nodetypeNameDiv.br({ number: 2 });

                //#region Buttons
                var btnTbl = cswPrivate.sidebarContent.table({
                    width: '100%',
                    //border: '1px',
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

                cswPrivate.sidebarContent.br({ number: 2 });

                //#region Edit NodeType Form

                cswPrivate.editNodeTypeFormDiv = cswPrivate.sidebarContent.div();

                // Table 1 - Category
                var categoryTbl = cswPrivate.editNodeTypeFormDiv.table({
                    width: '100%'
                });
                categoryTbl.cell(1, 1).span({ text: 'Category ', cssclass: 'CswDesignMode_AlignRight' });
                cswPrivate.categoryInput = categoryTbl.cell(2, 1).input({
                    name: 'category',
                    value: '',
                    isRequired: false
                });
                categoryTbl.br({ number: 1 });

                // Table 2 - Node Name Template
                var nodeNameTemplateTbl = cswPrivate.editNodeTypeFormDiv.table({
                    width: '100%'
                    //border: '1px'
                });
                nodeNameTemplateTbl.cell(1, 1).span({ text: 'Node Name Template' });
                cswPrivate.nodeNameTemplateText = nodeNameTemplateTbl.cell(2, 1).input({
                    name: 'nodenametemplate',
                    value: '',
                    isRequired: false,
                    size: '20'
                });
                cswPrivate.nodeNameTemplatePropList = nodeNameTemplateTbl.cell(2, 2).empty({});
                //get a list of nodetype properties
                Csw.ajax.post({
                    urlMethod: 'getPropNames',
                    data: { Type: 'NodeTypeId', Id: cswPrivate.tabState.nodetypeid },
                    success: function (data) {
                        if (false === Csw.isNullOrEmpty(data)) {

                            var nodeTypeProps = [];

                            nodeTypeProps.push({ value: '', display: 'Add Prop..' });

                            for (var prop in data) {
                                nodeTypeProps.push({ value: data[prop].propid, display: data[prop].propname });
                            }

                            cswPrivate.nodeNameTemplatePropList.select({
                                name: 'NtPickList',
                                values: nodeTypeProps,
                                onChange: function (selectedVal) {
                                    var selectedText = cswPrivate.nodeNameTemplatePropList.find(':selected').text();
                                    cswPrivate.nodeNameTemplateText.val(cswPrivate.nodeNameTemplateText.val() + ' {' + selectedText + '}');
                                }
                            });
                        }
                    },
                    error: function () {
                        //ERRRRRRRRRRRRROR
                    }
                });
                nodeNameTemplateTbl.br({ number: 1 });

                // Table 3 - Convert Object Class
                var convertOCTbl = cswPrivate.editNodeTypeFormDiv.table({
                    width: '100%'
                    //border: '1px'
                });
                convertOCTbl.cell(1, 1).span({ text: 'Convert Object Class' });
                cswPrivate.objClassInput = convertOCTbl.cell(2, 1).empty();
                if (cswPrivate.isGenericClass) {
                    //todo: This will probably come from somewhere else as opposed to cswPrivate.isGenericClass
                    //get a list of objectclasses
                    Csw.ajax.post({
                        urlMethod: 'getObjectClasses',
                        success: function (data) {
                            console.log(data);
                            if (false === Csw.isNullOrEmpty(data)) {

                                var objClasses = [];
                                for (var oc in data) {
                                    objClasses.push(data[oc].objectclass);
                                }

                                cswPrivate.objClassInput.select({
                                    name: 'OCPickList',
                                    values: objClasses
                                });
                            }
                        },
                        error: function () {
                            //ERRRRRRRRRRRRROR
                        }
                    });
                } else {
                    //todo: covert to oc name not nodetype name
                    cswPrivate.objClassInput.span({ text: cswPrivate.tabState.nodetypename });
                }
                convertOCTbl.br({ number: 1 });

                // Save button
                cswPrivate.saveButton = cswPrivate.editNodeTypeFormDiv.div();
                cswPrivate.makeButton('Save', cswPrivate.saveButton);

                cswPrivate.saveButton.br({ number: 2 });

                //#endregion Edit NodeType Form

                //#region Add Properties
                var ajaxdata = {
                    NodeId: Csw.string(cswPrivate.tabState.nodeid),
                    NodeKey: Csw.string(cswPrivate.tabState.nodekey),
                    NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                    TabId: Csw.string(cswPrivate.tabState.tabid),
                    LayoutType: 'Edit' //always be edit? how will the user choose to add to the add-layout??
                };

                Csw.ajax.post({
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

                var extjscontrol = cswPrivate.sidebarContent.div({ align: 'center' });

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

                var extjscontrol2 = cswPrivate.sidebarContent.div({ align: 'center' });

                var ds2 = Ext.create('Ext.data.ArrayStore', {
                    fields: ['value', 'display'],
                    data: [],
                    autoLoad: false
                });

                Csw.ajax.post({
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
            };

            cswPrivate.onTearDown = function () {
                //Csw.iterate(cswPrivate.ajax, function (call, name) {
                //    call.ajax.abort();
                //    delete cswPrivate.ajax[name];
                //});
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
                        cswPrivate.extWindowVersions = Csw.components.window(cswParent, {
                            title: 'Versions of ' + cswPrivate.tabState.nodetypename,
                            y: 0,
                            x: 300,
                            height: 200,
                            width: 400,
                            layout: 'fit'
                        });

                        cswPrivate.extWindowVersions.attachToMe().span({
                            text: "So we really aren't sure what this will do yet but here is a placeholder for when we figure it out! :p"
                        });

                        break;
                    case 'Copy':
                        var windowItems = {};

                        cswPrivate.extWindowCopy = Csw.composites.window(cswParent, {
                            title: 'Copy ' + cswPrivate.tabState.nodetypename,
                            y: 0,
                            x: 300,
                            height: 200,
                            width: 400,
                            layout: 'fit',
                            buttons: [
                                { text: 'Copy' },
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
                            labelText: 'Copy As',
                            value: 'Chemical Copy'
                        });

                        break;
                    case 'Delete':
                        var windowItems = {};

                        cswPrivate.extWindowDelete = Csw.composites.window(cswParent, {
                            title: 'Delete ' + cswPrivate.tabState.nodetypename,
                            y: 0,
                            x: 300,
                            height: 100,
                            width: 400,
                            layout: 'fit',
                            buttons: [
                                { text: 'Delete' },
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

                        cswPrivate.extWindowNew = Csw.composites.window(cswParent, {
                            title: 'Delete ' + cswPrivate.tabState.nodetypename,
                            y: 0,
                            x: 300,
                            height: 200,
                            width: 400,
                            layout: 'fit',
                            buttons: [
                                { text: 'Create' },
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

                        table.cell(1, 1).span({ text: 'Name' });
                        table.cell(1, 2).input({
                            name: 'designmode_newNodeTypeName',
                            value: ''
                        });

                        table.cell(2, 1).span({ text: 'Object Class' });
                        cswPrivate.ajax.objectClasses = Csw.ajax.post({
                            urlMethod: 'getObjectClasses',
                            success: function (data) {
                                if (false === Csw.isNullOrEmpty(data)) {

                                    var objClasses = [];
                                    for (var oc in data) {
                                        objClasses.push(data[oc].objectclass);
                                    }

                                    table.cell(2,2).select({
                                        name: 'OCPickList',
                                        values: objClasses
                                    });
                                }
                            },
                            error: function () {
                                //ERRRRRRRRRRRRROR
                            }
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
