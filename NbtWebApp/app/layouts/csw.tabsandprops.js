/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.layouts.tabsAndProps = Csw.layouts.tabsAndProps ||
        Csw.layouts.register('tabsAndProps', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: '',
                urls: {
                    TabsUrlMethod: 'getTabs',
                    SinglePropUrlMethod: 'getSingleProp',
                    PropsUrlMethod: 'getProps',
                    MovePropUrlMethod: 'moveProp',
                    RemovePropUrlMethod: 'removeProp',
                    SavePropUrlMethod: 'saveProps',
                    CopyPropValuesUrlMethod: 'copyPropValues',
                    NodePreviewUrlMethod: 'getNodePreview',
                    QuotaUrlMethod: 'checkQuota'
                },
                globalState: {
                    nodePks: [],
                    nodekeys: [],
                    nodeids: [],
                    propertyData: null,
                    filterToPropId: '',
                    title: '',
                    date: '',
                    excludeOcProps: [],
                    ShowCheckboxes: false,
                    ShowAsReport: true
                },
                tabState: {
                    nodeid: 'newnode',
                    nodename: '',
                    EditMode: Csw.enums.editMode.Edit,
                    Multi: false,
                    ReadOnly: false,
                    Config: false,
                    showSaveButton: true,
                    relatednodeid: '',
                    relatednodename: '',
                    relatednodetypeid: '',
                    relatedobjectclassid: '',
                    tabid: '',
                    nodetypeid: ''
                },
                IdentityTab: null,
                onNodeIdSet: null,
                onSave: null,
                ReloadTabOnSave: true,
                Refresh: null,
                onBeforeTabSelect: function () { return true; },
                onTabSelect: null,
                onPropertyChange: null,
                onPropertyRemove: null,
                onInitFinish: null,
                AjaxWatchGlobal: true,
                nodeTreeCheck: null,
                onEditView: null,
                onAfterButtonClick: null,
                atLeastOne: {},
                saveBtn: {},
                properties: [],
                async: true
            };
            var cswPublic = {};

            (function _preCtor() {
                if (options) {
                    Csw.extend(cswPrivate, options, true);
                }

                //hide first then show if content exists
                cswPrivate.titleDiv = cswParent.div();
                cswPrivate.identityWrapDiv = cswParent.div();
                cswPrivate.identityWrapDiv.css({
                    border: '1px solid #cddded',
                    background: '#e5f0ff',
                    margin: '0px',
                    padding: '10px'
                });

                cswPrivate.tabsTable = cswPrivate.identityWrapDiv.table({ width: '100%' });
                cswPrivate.identityDiv = cswPrivate.tabsTable.cell(1, 1); //.div();

                cswPrivate.outerTabDiv = cswPrivate.tabsTable
                    .cell(3, 1)
                    .tabDiv({ ID: cswPrivate.ID + '_tabdiv' });
                cswPrivate.tabcnt = 0;
            }());

            //#region Tabs

            cswPrivate.clearTabs = function () {
                cswPrivate.outerTabDiv.empty();
            };

            cswPrivate.makeTabContentDiv = function (tabParent, tabid, canEditLayout) {
                'use strict';
                var tabContentDiv = tabParent.tabDiv({
                    ID: tabid
                });
                tabContentDiv.form({
                    ID: tabid + '_form',
                    onsubmit: 'return false;'
                });

                var handle = function () {
                    tabParent.empty();
                    Csw.unsubscribe(Csw.enums.events.CswNodeDelete, handle);
                    return false;
                };

                if (false === Csw.isNullOrEmpty(tabParent.parent(), true)) {
                    Csw.subscribe(Csw.enums.events.CswNodeDelete, handle);
                }
                tabContentDiv.data('canEditLayout', canEditLayout);
                return tabContentDiv;
            };

            cswPrivate.setPrivateProp = function (obj, propName) {
                if (false === Csw.isNullOrEmpty(obj) && false === Csw.isNullOrEmpty(propName)) {
                    cswPrivate[propName] = obj[propName];
                    delete obj[propName];
                }
            };

            cswPrivate.setTabStateProp = function (obj, propName) {
                if (false === Csw.isNullOrEmpty(obj) && false === Csw.isNullOrEmpty(propName)) {
                    cswPrivate.tabState[propName] = obj[propName];
                    delete obj[propName];
                }
            };

            cswPrivate.makeIdentityTab = function (data) {

                if (false === Csw.isNullOrEmpty(data)) {
                    cswPrivate.setTabStateProp(data, 'nodename');
                    cswPrivate.setTabStateProp(data, 'nodetypeid');
                    cswPrivate.setPrivateProp(data, 'IdentityTab');

                    if (Csw.isNullOrEmpty(cswPrivate.IdentityTab)) {
                        cswPrivate.identityWrapDiv.remove();
                    } else {

                        var layoutOpts = {
                            ID: cswPrivate.ID + window.Ext.id(),
                            OddCellRightAlign: true,
                            ReadOnly: (cswPrivate.tabState.EditMode === Csw.enums.editMode.PrintReport || cswPrivate.tabState.ReadOnly),
                            cellSet: {
                                rows: 1,
                                columns: 2
                            },
                            showConfigButton: false, //o.tabState.Config,
                            showExpandRowButton: false,
                            showExpandColButton: false,
                            showRemoveButton: false
                        };
                        var tabId = cswPrivate.IdentityTab.tabid;
                        delete cswPrivate.IdentityTab.tabid;

                        cswPrivate.titleDiv.append(cswPrivate.tabState.nodename).css({ 'font-size': '22px' });
                        cswPrivate.identityLayoutTable = cswPrivate.identityDiv.layoutTable(layoutOpts);
                        cswPrivate.handleProperties(cswPrivate.identityLayoutTable, cswPrivate.identityDiv, tabId, false, cswPrivate.IdentityTab);
                    }
                }
            };

            cswPrivate.getTabs = function (tabContentDiv) {
                'use strict';
                // For performance, don't bother getting tabs if we're in Add, Temp, Preview or Table
                if (cswPrivate.tabState.EditMode === Csw.enums.editMode.Add ||
                    cswPrivate.tabState.EditMode === Csw.enums.editMode.Temp ||
                    cswPrivate.tabState.EditMode === Csw.enums.editMode.Preview ||
                    cswPrivate.tabState.EditMode === Csw.enums.editMode.Table) {

                    var tabid = cswPrivate.tabState.EditMode + "_tab";
                    tabContentDiv = cswPrivate.makeTabContentDiv(tabContentDiv, tabid, false);
                    cswPrivate.getProps(tabContentDiv, tabid);

                } else {

                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.urls.TabsUrlMethod,
                        data: {
                            EditMode: Csw.string(cswPrivate.tabState.EditMode, 'Edit'),
                            NodeId: cswPublic.getNodeId(),
                            SafeNodeKey: Csw.tryParseObjByIdx(cswPrivate.globalState.nodekeys, 0),
                            NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                            PropId: '',
                            Date: Csw.string(cswPrivate.globalState.date, new Date().toDateString()),
                            filterToPropId: Csw.string(cswPrivate.globalState.filterToPropId),
                            Multi: Csw.bool(cswPrivate.tabState.Multi),
                            ConfigMode: cswPrivate.tabState.Config
                        },
                        success: function (data) {
                            cswPrivate.clearTabs();
                            var tabdivs = [];
                            var selectedtabno = 0;
                            var tabno = 0;
                            var tabDiv, tabUl;

                            cswPrivate.makeIdentityTab(data);

                            var tabFunc = function (thisTab) {
                                var thisTabId = thisTab.id;

                                if (cswPrivate.tabState.EditMode === Csw.enums.editMode.PrintReport || tabdivs.length === 0) {
                                    // For PrintReports, we're going to make a separate tabstrip for each tab
                                    tabDiv = cswPrivate.outerTabDiv.tabDiv();
                                    tabUl = tabDiv.ul();
                                    tabdivs[tabdivs.length] = tabDiv;
                                }
                                tabDiv = tabDiv || tabdivs[tabdivs.length - 1];
                                tabUl = tabUl || tabDiv.ul();
                                tabUl.li().a({ href: '#' + thisTabId, text: thisTab.name });
                                cswPrivate.makeTabContentDiv(tabDiv, thisTabId, thisTab.canEditLayout);
                                if (thisTabId === cswPrivate.tabState.tabid) {
                                    selectedtabno = tabno;
                                }
                                tabno += 1;
                                return false;
                            };
                            Csw.crawlObject(data, tabFunc, false);

                            cswPrivate.tabcnt = tabno;

                            Csw.each(tabdivs, function (thisTabDiv) {
                                thisTabDiv.tabs({
                                    selected: selectedtabno,
                                    select: function (event, ui) {
                                        var ret = false;
                                        var selectTabContentDiv = thisTabDiv.children('div:eq(' + Csw.number(ui.index) + ')');
                                        var selectTabid = selectTabContentDiv.getId();
                                        if (Csw.tryExec(cswPrivate.onBeforeTabSelect, selectedtabid)) {
                                            if (false === Csw.isNullOrEmpty(selectTabContentDiv)) {
                                                cswPrivate.form.empty();
                                                cswPrivate.getProps(selectTabContentDiv, selectTabid);
                                                Csw.tryExec(cswPrivate.onTabSelect, selectTabid);
                                                ret = true;
                                            }
                                        }
                                        Csw.publish('initPropertyTearDown_' + cswPublic.getNodeId());
                                        return ret;
                                    } // select()
                                }); // tabs
                                var eachTabContentDiv = thisTabDiv.children('div:eq(' + Csw.number(thisTabDiv.tabs('option', 'selected')) + ')');
                                if (eachTabContentDiv.isValid) {
                                    var selectedtabid = eachTabContentDiv.getId();
                                    cswPrivate.getProps(eachTabContentDiv, selectedtabid);
                                    Csw.tryExec(cswPrivate.onTabSelect, selectedtabid);
                                }
                            }); // for(var t in tabdivs)
                        } // success
                    }); // ajax
                } // if-else editmode is add or preview
            }; // getTabs()

            //#endregion Tabs

            //#region Validator

            cswPrivate.initValidator = function () {
                cswPublic.validator = cswPrivate.form.$.validate({
                    highlight: function (element) {
                        var $elm = $(element);
                        $elm.attr('csw_invalid', '1');
                        $elm.animate({ backgroundColor: '#ff6666' });
                    },
                    unhighlight: function (element) {
                        var $elm = $(element);
                        if ($elm.attr('csw_invalid') === '1')  // only unhighlight where we highlighted
                        {
                            $elm.css('background-color', '#66ff66');
                            $elm.attr('csw_invalid', '0');
                            setTimeout(function () { $elm.animate({ backgroundColor: 'transparent' }); }, 500);
                        }
                    }
                }); // validate()
            };

            cswPublic.isFormValid = function () {
                return cswPrivate.form.$.valid();
            };

            //#endregion Validator

            //#region Helper Methods

            cswPrivate.setNodeId = function (data) {
                var nodeid = data.nodeid;
                Csw.tryExec(cswPrivate.onNodeIdSet, nodeid);
                cswPrivate.tabState.nodeid = nodeid;
                cswPrivate.globalState.nodeids[0] = nodeid;
                cswPrivate.tabState.cswnbtnodekey = cswPrivate.globalState.nodekeys[0];
                delete data.nodeid;
                return nodeid;
            };

            cswPublic.getNodeId = function () {
                var nodeid = Csw.string(cswPrivate.globalState.nodeids[0], 'newnode');
                cswPrivate.tabState.nodeid = nodeid;
                cswPrivate.tabState.cswnbtnodekey = cswPrivate.globalState.nodekeys[0];
                return nodeid;
            };

            cswPrivate.enableSaveBtn = function () {
                if (false === Csw.isNullOrEmpty(cswPrivate.saveBtn, true)) {
                    cswPrivate.saveBtn.enable();
                }
            };

            cswPublic.getPropJson = function () {
                cswPrivate.updatePropJsonFromLayoutTable();
                return cswPrivate.globalState.propertyData;
            };

            //#endregion Helper Methods

            //#region Layout Config

            cswPrivate.onRemove = function (tabid, onRemoveData) {
                'use strict';
                var propid = '';
                var propDiv = cswPrivate.getPropertyCell(onRemoveData.cellSet).children('div');
                if (false === Csw.isNullOrEmpty(propDiv)) {
                    propid = propDiv.first().propNonDom('propId');
                }

                Csw.ajax.post({
                    watchGlobal: cswPrivate.AjaxWatchGlobal,
                    urlMethod: cswPrivate.urls.RemovePropUrlMethod,
                    data: { PropId: propid, EditMode: cswPrivate.tabState.EditMode, TabId: tabid },
                    success: function () {
                        cswPrivate.onPropertyRemove(propid);
                    }
                });

            };

            // onRemove()

            cswPrivate.onSwap = function (tabid, onSwapData) {
                //case 26418
                var propIdOrig = cswPrivate.moveProp(cswPrivate.getPropertyCell(onSwapData.cellSet), tabid, onSwapData.swaprow, onSwapData.swapcolumn, onSwapData.cellSet[1][1].propNonDom('propid'));
                var propIdSwap = cswPrivate.moveProp(cswPrivate.getPropertyCell(onSwapData.swapcellset), tabid, onSwapData.row, onSwapData.column, onSwapData.swapcellset[1][1].propNonDom('propid'));
                onSwapData.cellSet[1][1].propNonDom('propid', propIdSwap);
                onSwapData.swapcellset[1][1].propNonDom('propid', propIdOrig);
            };

            // onSwap()

            cswPrivate.moveProp = function (propDiv, tabid, newrow, newcolumn, propId) {
                'use strict';
                if (propDiv.length() > 0) {
                    var propid = Csw.string(propDiv.propNonDom('propid'), propId);
                    var dataJson = {
                        PropId: propid,
                        TabId: tabid,
                        NewRow: newrow,
                        NewColumn: newcolumn,
                        EditMode: cswPrivate.tabState.EditMode
                    };

                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.urls.MovePropUrlMethod,
                        data: dataJson
                    });
                }
                return propid;
            };

            // _moveProp()

            //#endregion Layout Config

            //#region Layout Table Cell Set

            cswPrivate.getLabelCell = function (cellSet) {
                return cellSet[1][1].children('div');
            };

            cswPrivate.getPropertyCell = function (cellSet) {
                return cellSet[1][2].children('div');
            };

            cswPrivate.getCellSet = function (layoutTable, tabgroup, displayrow, displaycol) {
                var ret;
                if (false === Csw.isNullOrEmpty(tabgroup)) {
                    var safetabgroup = Csw.makeSafeId(tabgroup);
                    if (Csw.isNullOrEmpty(cswPrivate.tabgrouptables)) {
                        cswPrivate.tabgrouptables = [];
                    }
                    if (Csw.isNullOrEmpty(cswPrivate.tabgrouptables[safetabgroup])) {
                        var cellSet = layoutTable.cellSet(displayrow, displaycol);
                        var propCell = cswPrivate.getPropertyCell(cellSet);
                        var fieldSet = propCell.fieldSet();
                        fieldSet.legend({ value: tabgroup });

                        var div = fieldSet.div();

                        var tabgroupLayoutTable = div.layoutTable({
                            ID: safetabgroup,
                            OddCellRightAlign: true,
                            ReadOnly: (cswPrivate.tabState.EditMode === Csw.enums.editMode.PrintReport || cswPrivate.tabState.ReadOnly),
                            cellSet: {
                                rows: 1,
                                columns: 2
                            },
                            onSwap: function (e, onSwapData) {
                                cswPrivate.onSwap(tabid, onSwapData);
                            },
                            showConfigButton: false,
                            showExpandRowButton: false,
                            showExpandColButton: false,
                            showRemoveButton: false
                        });
                        cswPrivate.tabgrouptables[safetabgroup] = tabgroupLayoutTable;
                    }
                    ret = cswPrivate.tabgrouptables[safetabgroup].cellSet(displayrow, displaycol);
                } else {
                    ret = layoutTable.cellSet(displayrow, displaycol);
                }
                return ret;
            }; // getCellSet()

            //#endregion Layout Table Cell Set

            //#region Properties

            cswPrivate.getProps = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                if (cswPrivate.tabState.EditMode === Csw.enums.editMode.Add && cswPrivate.tabState.Config === false) {
                    // case 20970 - make sure there's room in the quota
                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.urls.QuotaUrlMethod,
                        data: {
                            NodeTypeId: cswPrivate.tabState.nodetypeid,
                            NodeKey: ''
                        },
                        success: function (data) {
                            if (Csw.bool(data.result)) {
                                cswPrivate.getPropsImpl(tabContentDiv, tabid, onSuccess);
                            } else {
                                tabContentDiv.append('You have used all of your purchased quota, and must purchase additional quota space in order to add more.');
                                Csw.tryExec(cswPrivate.onInitFinish, false);
                            }
                        }
                    });
                } else {
                    cswPrivate.getPropsImpl(tabContentDiv, tabid, onSuccess);
                }
            }; // getProps()

            cswPrivate.getPropsImpl = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                cswPrivate.tabgrouptables = [];  // case 26957, 27117

                function makePropLayout() {
                    cswPrivate.form = tabContentDiv.children('form');
                    cswPrivate.form.empty();

                    if (false === Csw.isNullOrEmpty(cswPrivate.globalState.title)) {
                        cswPrivate.form.append(cswPrivate.globalState.title);
                    }

                    var formTable = cswPrivate.form.table({
                        ID: cswPrivate.ID + '_formtbl_' + tabid + window.Ext.id(),
                        width: '100%'
                    });

                    var layoutOpts = {
                        ID: cswPrivate.ID + '_props_' + tabid,
                        OddCellRightAlign: true,
                        ReadOnly: (cswPrivate.tabState.EditMode === Csw.enums.editMode.PrintReport || cswPrivate.tabState.ReadOnly),
                        cellSet: {
                            rows: 1,
                            columns: 2
                        },
                        onSwap: function (e, onSwapData) {
                            cswPrivate.onSwap(tabid, onSwapData);
                        },
                        showConfigButton: false, //o.tabState.Config,
                        showExpandRowButton: cswPrivate.tabState.Config,
                        showExpandColButton: (cswPrivate.tabState.Config && (cswPrivate.tabState.EditMode !== Csw.enums.editMode.Table && cswPrivate.tabState.EditMode !== Csw.enums.editMode.Temp)),
                        showRemoveButton: cswPrivate.tabState.Config,
                        onConfigOn: function () {
                            doUpdateSubProps(true);
                        }, // onConfigOn
                        onConfigOff: function () {
                            doUpdateSubProps(false);
                        }, // onConfigOff
                        onRemove: function (event, onRemoveData) {
                            cswPrivate.onRemove(tabid, onRemoveData);
                        } // onRemove
                    };

                    cswPrivate.layoutTable = formTable.cell(1, 1).layoutTable(layoutOpts);

                    function doUpdateSubProps(configOn) {
                        var updOnSuccess = function (thisProp, key) {
                            if (false === Csw.isNullOrEmpty(thisProp) &&
                                false === Csw.isNullOrEmpty(key) &&
                                Csw.bool(thisProp.hassubprops)) {

                                var subTable = cswPrivate.layoutTable[thisProp.id + '_subproptable'];
                                var parentCell = Csw.literals.factory(subTable.table.$.parent().parent().parent());
                                var cellSet = cswPrivate.getCellSet(cswPrivate.layoutTable, thisProp.tabgroup, parentCell.propNonDom('row'), parentCell.propNonDom('column'));

                                cswPrivate.layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
                                var propCell = cswPrivate.getPropertyCell(cellSet);

                                if (subTable.length > 0) {
                                    cswPrivate.updateSubProps(thisProp, propCell, tabContentDiv, tabid, configOn);
                                }
                            }
                            return false;
                        };
                        Csw.crawlObject(cswPrivate.globalState.propertyData, updOnSuccess, false);
                    }

                    if (cswPrivate.tabState.EditMode !== Csw.enums.editMode.PrintReport && Csw.bool(cswPrivate.tabState.showSaveButton)) {
                        cswPrivate.saveBtn = formTable.cell(2, 1).buttonExt({
                            ID: 'SaveTab' + window.Ext.id(),
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                            enabledText: 'Save Changes',
                            disabledText: 'Saving...',
                            onClick: function () { cswPublic.save(tabContentDiv, tabid); }
                        });
                    }
                    cswPrivate.atLeastOne = cswPrivate.handleProperties(null, tabContentDiv, tabid, false);
                    if (false === Csw.isNullOrEmpty(cswPrivate.layoutTable.cellSet(1, 1)) &&
                            false === Csw.isNullOrEmpty(cswPrivate.layoutTable.cellSet(1, 1)[1][2])) {
                        cswPrivate.layoutTable.cellSet(1, 1)[1][2].trigger('focus');
                    }
                    // Validation
                    cswPrivate.initValidator();

                    if (Csw.bool(cswPrivate.tabState.Config)) {
                        cswPrivate.layoutTable.configOn();
                    } else if (!cswPrivate.tabState.Config &&
                            Csw.isNullOrEmpty(cswPrivate.globalState.date) &&
                                cswPrivate.globalState.filterToPropId === '' &&
                                    cswPrivate.tabState.EditMode !== Csw.enums.editMode.PrintReport &&
                                        Csw.bool(tabContentDiv.data('canEditLayout'))) {
                            /* Case 24437 */
                        var editLayoutOpt = {
                            ID: cswPrivate.ID,
                            globalState: {
                                nodeids: cswPrivate.globalState.nodeids,
                                nodekeys: cswPrivate.globalState.nodekeys,
                                nodetypeid: cswPrivate.tabState.nodetypeid
                            },
                            tabState: {
                                tabid: cswPrivate.tabState.tabid
                            },
                            Refresh: function () {
                                Csw.tryExec(cswPrivate.Refresh);
                                cswPrivate.tabState.Config = false;
                                cswPrivate.getTabs(tabContentDiv);
                            }
                        };

                        /* Show the 'fake' config button to open the dialog */
                        formTable.cell(1, 2).icon({
                            ID: cswPrivate.ID + 'configbtn',
                            iconType: Csw.enums.iconType.wrench,
                            hovertext: 'Configure',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                cswPrivate.clearTabs();
                                $.CswDialog('EditLayoutDialog', editLayoutOpt);
                            }
                        });
                    }

                    /* case 8494 */
                    if (!cswPrivate.tabState.Config && !cswPrivate.atLeastOne.Saveable && cswPrivate.tabState.EditMode === Csw.enums.editMode.Add) {
                        cswPublic.save(tabContentDiv, tabid);
                    } else {
                        Csw.tryExec(cswPrivate.onInitFinish, cswPrivate.atLeastOne.Property);
                        Csw.tryExec(onSuccess);
                    }
                }

                if (Csw.isNullOrEmpty(cswPrivate.globalState.propertyData) ||
                    (cswPrivate.tabState.EditMode !== Csw.enums.editMode.Add &&
                    cswPrivate.tabState.EditMode !== Csw.enums.editMode.Temp)) {

                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.urls.PropsUrlMethod,
                        data: {
                            EditMode: cswPrivate.tabState.EditMode,
                            NodeId: Csw.tryParseObjByIdx(cswPrivate.globalState.nodeids, 0),
                            TabId: tabid,
                            SafeNodeKey: Csw.tryParseObjByIdx(cswPrivate.globalState.nodekeys, 0),
                            NodeTypeId: cswPrivate.tabState.nodetypeid,
                            Date: Csw.string(cswPrivate.globalState.date, new Date().toDateString()),
                            Multi: Csw.bool(cswPrivate.tabState.Multi),
                            filterToPropId: cswPrivate.globalState.filterToPropId,
                            ConfigMode: cswPrivate.tabState.Config,
                            RelatedNodeId: Csw.string(cswPrivate.tabState.relatednodeid),
                            RelatedNodeTypeId: Csw.string(cswPrivate.tabState.relatednodetypeid),
                            RelatedObjectClassId: Csw.string(cswPrivate.tabState.relatedobjectclassid),
                            GetIdentityTab: Csw.bool(Csw.isNullOrEmpty(cswPrivate.IdentityTab))
                        },
                        success: function (data) {
                            if (Csw.isNullOrEmpty(data) && cswPrivate.tabState.EditMode === Csw.enums.editMode.Edit) {
                                Csw.error.throwException({
                                    type: 'warning',
                                    message: 'No properties have been configured for this layout: ' + cswPrivate.tabState.EditMode,
                                    name: 'Csw_client_exception',
                                    fileName: 'csw.tabsandprops.js',
                                    lineNumber: 387
                                });
                            }
                            if (data.nodeid) {
                                cswPrivate.setNodeId(data);
                            }
                            cswPrivate.globalState.propertyData = data;
                            makePropLayout();
                        } // success{}
                    }); // ajax
                } else {
                    if (cswPrivate.globalState.propertyData.nodeid) {
                        cswPrivate.setNodeId(cswPrivate.globalState.propertyData);
                    }
                    makePropLayout();
                }
            }; // getPropsImpl()

            cswPrivate.handleProperties = function (layoutTable, tabContentDiv, tabid, configMode, tabPropData) {
                'use strict';
                layoutTable = layoutTable || cswPrivate.layoutTable;
                cswPrivate.atLeastOne = { Property: false, Saveable: false };
                var handleSuccess = function (propObj) {
                    cswPrivate.atLeastOne.Property = true;
                    cswPrivate.handleProp(layoutTable, propObj, tabContentDiv, tabid, configMode);
                    return false;
                };
                tabPropData = tabPropData || cswPrivate.globalState.propertyData;
                Csw.crawlObject(tabPropData, handleSuccess, false);

                if (false === Csw.isNullOrEmpty(cswPrivate.saveBtn, true)) {
                    if (cswPrivate.tabState.Config || (cswPrivate.atLeastOne.Saveable === false && cswPrivate.tabState.EditMode != Csw.enums.editMode.Add)) {
                        cswPrivate.saveBtn.hide();
                    } else {
                        cswPrivate.saveBtn.show();
                    }
                }
                Csw.publish('render_' + cswPublic.getNodeId());
                return cswPrivate.atLeastOne;
            }; // _handleProperties()


            cswPrivate.handleProp = function (layoutTable, propData, tabContentDiv, tabid, configMode) {
                'use strict';
                Csw.debug.assert(Csw.isPlainObject(propData), 'handleProp was given an invalid object representing propertyData');
                if (Csw.isPlainObject(propData)) {
                    var propid = propData.id,
                        cellSet,
                        helpText = Csw.string(propData.helptext),
                        propName = Csw.string(propData.name),
                        labelCell = {};

                    cellSet = cswPrivate.getCellSet(layoutTable, propData.tabgroup, propData.displayrow, propData.displaycol);
                    layoutTable.addCellSetAttributes(cellSet, { propId: propid });

                    if (cswPrivate.canDisplayProp(propData, configMode) &&
                        Csw.bool(propData.showpropertyname)) {

                        labelCell = cswPrivate.getLabelCell(cellSet);

                        labelCell.addClass('propertylabel');
                        if (Csw.bool(propData.highlight)) {
                            labelCell.addClass('ui-state-highlight');
                        }

                        if (false === Csw.isNullOrEmpty(helpText)) {
                            labelCell.a({
                                cssclass: 'cswprop_helplink',
                                title: helpText,
                                onClick: function () {
                                    return false;
                                },
                                value: propName
                            });

                        } else {
                            labelCell.setLabelText(propName, propData.required, propData.readonly);
                        }

                        var inpPropCheck = labelCell.input({
                            ID: 'check_' + propid,
                            type: Csw.enums.inputTypes.checkbox,
                            value: false, // Value --not defined?,
                            cssclass: cswPrivate.ID + '_check'
                        });
                        inpPropCheck.propNonDom('propid', propid);
                        inpPropCheck.hide();
                        if (false === Csw.bool(propData.readonly)) {
                            cswPrivate.atLeastOne.Saveable = true;
                        }
                        Csw.subscribe('CswMultiEdit', (function () {
                            var onMultiEdit = function (eventObj, multiOpts) {
                                /* Case 25936 */
                                var showCheckBoxes = (cswPrivate.globalState.ShowCheckboxes && Csw.bool(propData.copyable));
                                if (multiOpts && multiOpts.nodeid === cswPublic.getNodeId()) {
                                    cswPrivate.tabState.Multi = multiOpts.multi;
                                    if (showCheckBoxes || multiOpts.multi) {
                                        inpPropCheck.show();
                                    } else {
                                        inpPropCheck.hide();
                                    }
                                } else {
                                    //Csw.debug.assert(multiOpts.nodeid === cswPublic.getNodeId(), 'CswMultiEdit event pusblished for nodeid "' + multiOpts.nodeid + '" but was subscribed to from nodeid "' + cswPublic.getNodeId() + '".');
                                    Csw.unsubscribe('CswMultiEdit', onMultiEdit);
                                }
                                return showCheckBoxes;
                            };
                            return onMultiEdit;
                        }()));
                    }

                    var propCell = cswPrivate.getPropertyCell(cellSet);
                    propCell.addClass('propertyvaluecell');

                    if (Csw.bool(propData.highlight)) {
                        propCell.addClass('ui-state-highlight');
                    }
                    cswPrivate.makeProp(propCell, propData, tabContentDiv, tabid, configMode, layoutTable);
                }
            };

            cswPrivate.canDisplayProp = function (propData, configMode) {
                /*The prop is set to display or we're in layout config mode*/
                var ret = (Csw.bool(propData.display, true) || configMode);
                /*And either no filter is set or the filter is set to this property */
                ret = ret && (cswPrivate.globalState.filterToPropId === '' || cswPrivate.globalState.filterToPropId === propData.id);
                /* We're not excluding any OC Props or this prop has not been excluded */
                ret = ret && ((Csw.isNullOrEmpty(cswPrivate.globalState.excludeOcProps) || cswPrivate.globalState.excludeOcProps.length === 0) || false === Csw.contains(cswPrivate.globalState.excludeOcProps, Csw.string(propData.ocpname).toLowerCase()));
                return ret;
            };

            cswPrivate.makeProp = function (propCell, propData, tabContentDiv, tabid, configMode, layoutTable) {
                'use strict';
                propCell.empty();
                if (cswPrivate.canDisplayProp(propData, configMode)) {
                    var propId = propData.id;
                    var propName = propData.name;

                    var tabState = Csw.extend({}, cswPrivate.tabState, true);
                    tabState.tabid = tabid;

                    var fieldOpt = Csw.nbt.propertyOption({
                        fieldtype: propData.fieldtype,
                        tabState: tabState,
                        propid: propData.id,
                        saveBtn: cswPrivate.saveBtn,
                        propData: propData,
                        Required: Csw.bool(propData.required),
                        onReload: function (afterReload) {
                            cswPrivate.getProps(tabContentDiv, tabid, afterReload);
                        },
                        onChange: function () {
                            if (Csw.bool(propData.hassubprops)) {
                                Csw.tryExec(cswPrivate.updateSubProps, propData, propCell, tabContentDiv, tabid, false, layoutTable);
                            }
                            Csw.tryExec(cswPrivate.onPropertyChange, fieldOpt.propid, propName);
                        },
                        doSave: function (saveopts) {
                            var s = {
                                onSuccess: null
                            };
                            Csw.extend(s, saveopts);
                            cswPublic.save(tabContentDiv, tabid, s.onSuccess);
                        },
                        onEditView: cswPrivate.onEditView,
                        onAfterButtonClick: cswPrivate.onAfterButtonClick
                    }, propCell.div());

                    cswPrivate.properties[propId] = Csw.nbt.property(fieldOpt);

                    if (Csw.contains(propData, 'subprops') && false === Csw.isNullOrEmpty(propData.subprops)) {
                        // recurse on sub-props
                        var subProps = propData.subprops;

                        var subLayoutTable = propCell.layoutTable({
                            ID: fieldOpt.propid + '_subproptable',
                            width: '',
                            styles: {
                                border: '1px solid #ccc'
                            },
                            OddCellRightAlign: true,
                            ReadOnly: (cswPrivate.tabState.EditMode === Csw.enums.editMode.PrintReport || cswPrivate.tabState.ReadOnly),
                            cellSet: {
                                rows: 1,
                                columns: 2
                            },
                            onSwap: function (e, onSwapData) {
                                cswPrivate.onSwap(tabid, onSwapData);
                            },
                            showConfigButton: false,
                            showExpandRowButton: false,
                            showExpandColButton: false,
                            showRemoveButton: false
                        });
                        layoutTable[fieldOpt.propid + '_subproptable'] = subLayoutTable;
                        var subOnSuccess = function (subProp, key) {
                            subProp.propId = key;
                            if (Csw.bool(subProp.display) || configMode) {
                                cswPrivate.handleProp(subLayoutTable, subProp, tabContentDiv, tabid, configMode);
                                if (configMode) {
                                    subLayoutTable.configOn();
                                } else {
                                    subLayoutTable.configOff();
                                }
                            }
                            return false;
                        };
                        Csw.crawlObject(subProps, subOnSuccess, false);
                    }
                } // if (propData.display != 'false' || ConfigMode )
            }; // _makeProp()

            cswPrivate.updateSubProps = function (singlePropData, propCell, tabContentDiv, tabid, configMode, layoutTable) {
                /// <summary>Update a properties sub props</summary>
                /// <param name="fieldOpt" type="Object"> An object defining a prop's fieldtype </param>
                /// <param name="propId" type="String"> A propertyid </param>
                /// <param name="propData" type="Object"> Property definition </param>
                /// <param name="$propcell" type="JQuery"> An element to append to </param>
                /// <param name="$tabcontentdiv" type="JQuery"> A tab element </param>
                /// <param name="tabid" type="String"> TabId </param>
                /// <param name="configMode" type="Boolean"> True if config mode </param>
                /// <param name="$savebtn" type="JQuery"> A save button </param>
                /// <returns type="void"></returns>
                'use strict';

                Csw.defer(function () {
                    if (singlePropData.wasmodified) {
                        var jsonData = {
                            EditMode: Csw.string(cswPrivate.tabState.EditMode, 'Edit'),
                            NodeId: Csw.tryParseObjByIdx(cswPrivate.globalState.nodeids, 0),
                            SafeNodeKey: Csw.string(cswPrivate.tabState.cswnbtnodekey),
                            PropId: singlePropData.id,
                            NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                            NewPropJson: JSON.stringify(singlePropData)
                        };

                        Csw.ajax.post({
                            watchGlobal: cswPrivate.AjaxWatchGlobal,
                            urlMethod: cswPrivate.urls.SinglePropUrlMethod,
                            data: jsonData,
                            success: function (data) {
                                singlePropData.wasmodified = true;
                                singlePropData.subprops = data.subprops;

                                // keep the fact that the parent property was modified
                                cswPrivate.makeProp(propCell, singlePropData, tabContentDiv, tabid, configMode, layoutTable);
                                Csw.publish('render_' + cswPublic.getNodeId());
                            }
                        });
                    }
                }, 150);
            }; // _updateSubProps()

            cswPrivate.updatePropJsonFromLayoutTable = function (layoutTable, propData) {
                /// <summary>
                /// Update the prop JSON from the main LayoutTable or from a subProp LayoutTable
                ///</summary>
                /// <param name="layoutTable" type="Csw.composites.layoutTable">(Optional) a layoutTable containing the properties to parse.</param>
                /// <param name="propData" type="Object">(Optional) an object representing CswNbt node properties.</param>
                /// <returns type="Array">An array of propIds</returns>
                'use strict';
                return Csw.tryExec(function () {

                    layoutTable = layoutTable || cswPrivate.layoutTable;
                    propData = propData || cswPrivate.globalState.propertyData;

                    var propIds = [];
                    var updSuccess = function (thisProp) {
                        var propOpt = {
                            propData: thisProp,
                            propDiv: '',
                            fieldtype: thisProp.fieldtype,
                            nodeid: Csw.tryParseObjByIdx(cswPrivate.globalState.nodeids, 0),
                            Multi: cswPrivate.tabState.Multi,
                            cswnbtnodekey: cswPrivate.cswnbtnodekey
                        };

                        var cellSet = cswPrivate.getCellSet(layoutTable, thisProp.tabgroup, thisProp.displayrow, thisProp.displaycol);
                        layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
                        propOpt.propCell = cswPrivate.getPropertyCell(cellSet);
                        propOpt.propDiv = propOpt.propCell.children('div').first();

                        //$.CswFieldTypeFactory('save', propOpt);
                        if (propOpt.propData.wasmodified) {
                            propIds.push(propOpt.propData.id);
                        }

                        // recurse on subprops
                        if (Csw.bool(thisProp.hassubprops) && Csw.contains(thisProp, 'subprops')) {
                            var subProps = thisProp.subprops;
                            if (false === Csw.isNullOrEmpty(subProps)) {
                                var subTable = layoutTable[thisProp.id + '_subproptable'];
                                if (false === Csw.isNullOrEmpty(subTable)) {
                                    cswPrivate.updatePropJsonFromLayoutTable(subTable, subProps);
                                }
                            }
                        }
                        return false;
                    };
                    Csw.crawlObject(propData, updSuccess, false);
                    return propIds;
                });
            }; // updatePropJsonFromLayoutTable()



            cswPublic.save = Csw.method(function (tabContentDiv, tabid, onSuccess, async) {
                'use strict';
                if (cswPrivate.tabState.Multi || cswPublic.isFormValid()) {
                    var propIds = cswPrivate.updatePropJsonFromLayoutTable();
                    var sourcenodeid = Csw.tryParseObjByIdx(cswPrivate.globalState.nodeids, 0);
                    var sourcenodekey = Csw.tryParseObjByIdx(cswPrivate.globalState.nodekeys, 0);
                    async = Csw.bool(async, true) && false === cswPrivate.tabState.Multi;
                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.urls.SavePropUrlMethod,
                        async: async,
                        data: {
                            EditMode: cswPrivate.tabState.EditMode,
                            NodeId: Csw.string(sourcenodeid),
                            SafeNodeKey: Csw.string(sourcenodekey),
                            TabId: Csw.string(tabid),
                            NodeTypeId: cswPrivate.tabState.nodetypeid,
                            NewPropsJson: Csw.serialize(cswPrivate.globalState.propertyData),
                            IdentityTabJson: Csw.serialize(cswPrivate.IdentityTab),
                            ViewId: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId)
                        },
                        success: function (successData) {
                            var doSave = true;
                            var dataJson = {
                                SourceNodeKey: sourcenodekey,
                                CopyNodeIds: [],
                                CopyNodeKeys: [],
                                PropIds: []
                            };

                            function copyNodeProps() {
                                Csw.ajax.post({
                                    watchGlobal: cswPrivate.AjaxWatchGlobal,
                                    urlMethod: cswPrivate.urls.CopyPropValuesUrlMethod,
                                    data: dataJson,
                                    success: function (data) {
                                        if (false === Csw.isNullOrEmpty(data.batch)) {
                                            $.CswDialog('BatchOpDialog', {
                                                opname: 'multi-edit',
                                                onViewBatchOperation: function () {
                                                    Csw.tryExec(cswPrivate.Refresh, {
                                                        nodeid: data.batch,
                                                        viewid: '',
                                                        viewmode: 'tree',
                                                        IncludeNodeRequired: true
                                                    });
                                                }
                                            });
                                        }
                                    }
                                }); // ajax						        
                            }

                            if (cswPrivate.globalState.ShowCheckboxes) {
                                // apply the newly saved checked property values on this node to the checked nodes
                                var nodechecks = cswPrivate.nodeTreeCheck.checkedNodes();
                                var $propchecks = $('.' + cswPrivate.ID + '_check:checked');

                                if (nodechecks.length > 0 && $propchecks.length > 0) {
                                    Csw.each(nodechecks, function (thisObj) {
                                        dataJson.CopyNodeIds.push(thisObj.nodeid);
                                    });

                                    $propchecks.each(function () {
                                        var propid = $(this).attr('propid');
                                        dataJson.PropIds.push(propid);
                                    });
                                    copyNodeProps();
                                } // if (nodechecks.length > 0 && $propchecks.length > 0)
                                else {
                                    doSave = false;
                                    $.CswDialog('AlertDialog', 'You have not selected any properties to save.');
                                }
                            } // if(o.globalState.ShowCheckboxes)
                            else if (cswPrivate.tabState.Multi) {
                                dataJson.CopyNodeIds = cswPrivate.globalState.nodeids;
                                dataJson.CopyNodeKeys = cswPrivate.globalState.nodekeys;
                                dataJson.PropIds = propIds;
                                copyNodeProps();
                            }

                            cswPrivate.enableSaveBtn();

                            if (doSave) {
                                // reload tab
                                var onSaveSuccess = function () {
                                    var onSaveRefresh = function () {
                                        Csw.tryExec(cswPrivate.onSave, successData.nodeid, successData.cswnbtnodekey, cswPrivate.tabcnt, successData.nodename);
                                        Csw.tryExec(onSuccess);
                                    };

                                    onSaveRefresh();
                                };
                                if (cswPrivate.ReloadTabOnSave) {
                                    cswPrivate.getProps(tabContentDiv, tabid, onSaveSuccess);
                                } else {
                                    onSaveSuccess();
                                }
                            }

                        }, // success
                        error: cswPrivate.enableSaveBtn
                    }); // ajax
                } // if(cswPrivate.isValid())
                else {
                    cswPrivate.enableSaveBtn();
                }
            });// Save()

            //#endregion Properties

            (function _postCtor() {
                cswPrivate.getTabs(cswPrivate.outerTabDiv);

                if (cswPrivate.tabState.EditMode !== Csw.enums.editMode.PrintReport) {
                    cswPrivate.linkDiv = cswParent.div({ ID: cswPrivate.ID + '_linkdiv', align: 'right' });
                    if (cswPrivate.globalState.ShowAsReport && false === cswPrivate.tabState.Multi) {
                        cswPrivate.linkDiv.a({
                            text: 'As Report',
                            onClick: function () {
                                Csw.openPopup('NodeReport.html?nodeid=' + Csw.tryParseObjByIdx(cswPrivate.globalState.nodeids, 0) + '&cswnbtnodekey=' + Csw.tryParseObjByIdx(cswPrivate.globalState.nodekeys, 0));
                            }
                        });
                    }
                }
            }());

            return cswPublic;
        });
}());

