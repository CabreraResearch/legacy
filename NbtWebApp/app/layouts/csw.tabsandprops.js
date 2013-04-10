/// <reference path="~/app/CswApp-vsdoc.js" />

/* global Csw: true */
(function () {

    Csw.layouts.tabsAndProps = Csw.layouts.tabsAndProps ||
        Csw.layouts.register('tabsAndProps', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                Multi: false,
                urls: {
                    TabsUrlMethod: 'getTabs',
                    SinglePropUrlMethod: 'getSingleProp',
                    PropsUrlMethod: 'getProps',
                    MovePropUrlMethod: 'moveProp',
                    RemovePropUrlMethod: 'removeProp',
                    SavePropUrlMethod: 'saveProps',
                    CopyPropValuesUrlMethod: 'copyPropValues',
                    NodePreviewUrlMethod: 'getNodePreview'
                },
                globalState: {
                    currentNodeId: 'newnode',
                    currentNodeKey: '',
                    selectedNodeKeys: Csw.delimitedString(),
                    selectedNodeIds: Csw.delimitedString(),
                    selectedPropIds: Csw.delimitedString(),
                    propertyData: null,
                    filterToPropId: '',
                    title: '',
                    date: '',
                    excludeOcProps: [],
                    ShowAsReport: true,
                    viewid: '',
                    checkBoxes: {},
                    removeTempStatus: true,
                    selectedTabId: ''
                },
                tabState: {
                    nodename: '',
                    EditMode: Csw.enums.editMode.Edit,
                    ReadOnly: false,
                    Config: false,
                    showSaveButton: true,
                    relatednodeid: '',
                    relatednodename: '',
                    relatednodetypeid: '',
                    relatedobjectclassid: '',
                    //tabid: '',
                    tabNo: 0,
                    nodetypeid: ''
                },
                ajax: {

                },
                tabs: {

                },
                IdentityTab: null,
                tabContentDiv: null,
                propid: '',
                showTitle: true,
                onNodeIdSet: null,
                onSave: null,
                onSaveError: null,
                ReloadTabOnSave: true,
                Refresh: null,
                onBeforeTabSelect: function () { return true; },
                onTabSelect: null,
                onOwnerPropChange: null, // case 28514
                onPropertyChange: null,
                onPropertyRemove: null,
                onInitFinish: null,
                AjaxWatchGlobal: true,
                nodeTreeCheck: null,
                onEditView: null,
                onAfterButtonClick: null,
                //saveBtn: {},
                properties: [],
                async: true
            };
            var cswPublic = {};

            (function () {
                var tabid = '';
                Object.defineProperty(cswPrivate.tabState, 'tabid', {
                    get: function () {
                        return tabid;
                    },
                    set: function (val) {
                        if (typeof (val) !== 'string' && typeof (val) !== 'number') {
                            throw new Error('Tabid must be a string or a number');
                        }
                        tabid = val;
                    }
                });
                


            }());
            cswPrivate.onMultiEdit = function (eventObj, multiOpts) {
                var isMulti = false;
                /* Case 25936 */
                if (multiOpts &&
                    multiOpts.viewid === cswPublic.getViewId()) {
                    isMulti = cswPrivate.toggleMulti();
                    cswPrivate.toggleConfigIcon(false === isMulti);
                    if (multiOpts.nodeid === cswPublic.getNodeId()) {
                        Csw.iterate(cswPrivate.globalState.checkBoxes, function (chk) {
                            if (isMulti) {
                                chk.show();
                            } else {
                                chk.hide();
                            }
                        });
                        cswPrivate.onRenderProps(cswPrivate.globalState.selectedTabId);
                        cswPrivate.refreshLinkDiv();
                    }
                }
                return isMulti;
            };

            Object.defineProperty(cswPrivate, 'initAtLeastOne', {
                value: function _initAtLeastOne() {

                    if (cswPrivate.atLeastOne) {
                        cswPrivate.atLeastOne.saveable = false;
                        cswPrivate.atLeastOne.property = false;
                    } else {
                        Object.defineProperty(cswPrivate, 'atLeastOne', {
                            value: {
                                saveable: false,
                                get Saveable() {
                                    return cswPrivate.atLeastOne.saveable;
                                },
                                set Saveable(val) {
                                    if (true === val) {
                                        cswPrivate.atLeastOne.saveable = true;
                                    }
                                },
                                property: false,
                                get Property() {
                                    return cswPrivate.atLeastOne.property;
                                },
                                set Property(val) {
                                    if (true === val) {
                                        cswPrivate.atLeastOne.property = true;
                                    }
                                }
                            }
                        });
                    }
                }
            });
            cswPrivate.initAtLeastOne(true);

       

            //#region Events

            cswPrivate.onRenderProps = function (tabid) {
                Csw.publish('render_' + cswPublic.getNodeId() + '_' + tabid);
            };

            cswPrivate.onTearDownProps = function () {
                Csw.unsubscribe('onPropChange_' + cswPrivate.propid);
                Csw.publish('initPropertyTearDown_' + cswPublic.getNodeId());
            };

            cswPrivate.onTearDown = function () {
                cswPrivate.onTearDownProps();
                cswPrivate.clearTabs();
                cswPrivate.globalState.checkBoxes = {};
                Csw.iterate(cswPrivate.ajax, function (call, name) {
                    call.ajax.abort();
                    delete cswPrivate.ajax[name];
                });
                cswPrivate.initAtLeastOne(true);
            };

            cswPublic.tearDown = function () {
                Csw.unsubscribe('CswMultiEdit', null, cswPrivate.onMultiEdit);
                cswPrivate.onTearDown();
            };


            cswPrivate.onAnyPropChange = function (obj, data, tabContentDiv) {
                Csw.tryExec(cswPrivate.onOwnerPropChange, obj, data, tabContentDiv);
            };

            //#endregion Events

            //#region Tabs

            cswPrivate.clearTabs = function () {
                cswPrivate.titleDiv.empty();
                cswPrivate.identityForm.empty();
                cswPrivate.outerTabDiv.empty();
                Csw.iterate(cswPrivate.tabs, function (tabId) {
                    if (cswPrivate.tabs[tabId] && cswPrivate.tabs[tabId].remove) {
                        cswPrivate.tabs[tabId].remove();
                    }
                });
                cswPrivate.tabs = [];
            };

            cswPrivate.makeTabContentDiv = function (tabParent, tabid, canEditLayout) {
                'use strict';
                if (cswPrivate.tabs[tabid] && cswPrivate.tabs[tabid].remove) {
                    cswPrivate.tabs[tabid].remove();
                }
                cswPrivate.tabs[tabid] = tabParent.tabDiv({});

                cswPrivate.tabs[tabid].data({
                    tabid: tabid,
                    canEditLayout: canEditLayout
                });

                cswPrivate.tabs[tabid].form({
                    name: tabid + '_form',
                    onsubmit: 'return false;'
                });

                var handle = function () {
                    tabParent.empty();
                    Csw.unsubscribe(Csw.enums.events.CswNodeDelete, null, handle);
                    return false;
                };

                if (false === Csw.isNullOrEmpty(tabParent.parent(), true)) {
                    Csw.subscribe(Csw.enums.events.CswNodeDelete, handle);
                }
                return cswPrivate.tabs[tabid];


            };

            cswPrivate.setPrivateProp = function (obj, propName) {
                if (Csw.contains(obj, propName)) {
                    cswPrivate[propName] = obj[propName];
                    delete obj[propName];
                }
            };

            cswPrivate.setTabStateProp = function (obj, propName) {
                if (Csw.contains(obj, propName)) {
                    cswPrivate.tabState[propName] = obj[propName];
                    delete obj[propName];
                }
            };

            cswPrivate.makeIdentityTab = function () {
                cswPrivate.ajax.tabs = Csw.ajax.post({
                    watchGlobal: cswPrivate.AjaxWatchGlobal,
                    urlMethod: 'getIdentityTabProps',
                    data: {
                        EditMode: cswPrivate.tabState.EditMode,
                        NodeId: cswPublic.getNodeId(),
                        SafeNodeKey: cswPublic.getNodeKey(),
                        //NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                        Date: Csw.string(cswPrivate.globalState.date, new Date().toDateString()),
                        Multi: Csw.bool(cswPrivate.tabState.Multi),
                        filterToPropId: Csw.string(cswPrivate.globalState.filterToPropId),
                        ConfigMode: cswPrivate.tabState.Config,
                        RelatedNodeId: Csw.string(cswPrivate.tabState.relatednodeid),
                        RelatedNodeTypeId: Csw.string(cswPrivate.tabState.relatednodetypeid),
                        RelatedObjectClassId: Csw.string(cswPrivate.tabState.relatedobjectclassid)
                    },
                    success: function (data) {
                        cswPrivate.IdentityTab = data.properties;

                        if (false === Csw.isNullOrEmpty(cswPrivate.IdentityTab) &&
                            false === cswPrivate.tabState.Config &&
                            cswPrivate.tabState.EditMode !== Csw.enums.editMode.PrintReport) {

                            var layoutOpts = {
                                name: cswPrivate.name + '_layout',
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
                            cswPrivate.IdentityTabId = data.tab.tabid;

                            cswPrivate.titleDiv.empty();
                            if (cswPrivate.showTitle) {
                                cswPrivate.titleDiv.append(data.node.nodename);
                                cswPrivate.titleDiv.show();
                            }
                            if (false === Csw.isNullOrEmpty(cswPrivate.IdentityTab)) {
                                cswPrivate.identityWrapDiv.addClass('CswIdentityTab');

                                cswPrivate.identityForm.empty();
                                var identityFormTbl = cswPrivate.identityForm.table({
                                    name: cswPrivate.name + '_formtbl_' + cswPrivate.IdentityTabId,
                                    width: '100%'
                                }).css({ padding: '10px' });

                                cswPrivate.identityLayoutTable = identityFormTbl.cell(1, 1).layoutTable(layoutOpts);
                                cswPrivate.handleProperties(cswPrivate.identityLayoutTable, cswPrivate.IdentityTabId, false, cswPrivate.IdentityTab);
                            }
                        }
                    }
                });
            };

            cswPrivate.getTabs = function (tabParent) {
                'use strict';
                tabParent = tabParent || cswPrivate.outerTabDiv;
                // For performance, don't bother getting tabs if we're in Add, Temp, Preview or Table
                if (cswPrivate.tabState.EditMode === Csw.enums.editMode.Add ||
                    cswPrivate.tabState.EditMode === Csw.enums.editMode.Temp ||
                    cswPrivate.tabState.EditMode === Csw.enums.editMode.Preview ||
                    cswPrivate.tabState.EditMode === Csw.enums.editMode.Table) {

                    var tabid = cswPrivate.tabState.EditMode + "_tab";
                    cswPrivate.tabs[tabid] = cswPrivate.makeTabContentDiv(tabParent, tabid, false);
                    cswPrivate.getProps(tabid);

                } else {

                    cswPrivate.ajax.tabs = Csw.ajax.post({
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

                            cswPrivate.tabState.nodetypeid = data.node.nodetypeid;

                            function makeTabs() {
                                cswPrivate.clearTabs();

                                cswPrivate.makeIdentityTab();

                                var tabIds = Csw.delimitedString();
                                Csw.iterate(data.tabs, function (tab) {
                                    tabIds.add(tab.id);
                                });
                                if (false === tabIds.contains(cswPrivate.tabState.tabid)) {
                                    cswPrivate.tabState.tabid = '';
                                    cswPrivate.tabState.tabNo = 0;
                                }

                                var tabno = 0;
                                var jqTabs = [];
                                var tabUls = [];
                                var tabLis = [];

                                var tabFunc = function (thisTab) {
                                    var tabStrip, tabUl;
                                    var thisTabId = thisTab.id;

                                    if (cswPrivate.tabState.EditMode === Csw.enums.editMode.PrintReport || jqTabs.length === 0) {
                                        tabStrip = cswPrivate.outerTabDiv.tabDiv();
                                        tabUl = tabStrip.ul();
                                        jqTabs[jqTabs.length] = tabStrip;
                                        tabUls[tabUls.length] = tabUl;
                                    } else {
                                        tabStrip = jqTabs[jqTabs.length - 1];
                                        tabUl = tabUls[tabUls.length - 1];
                                    }

                                    var tabLi = tabUl.li();
                                    tabLis.push(tabLi);
                                    tabLi.data({
                                        tabid: thisTabId,
                                        tabno: tabLis.length
                                    });
                                    tabLi.a({ href: '#' + thisTabId, text: thisTab.name });

                                    cswPrivate.makeTabContentDiv(tabStrip, thisTabId, thisTab.canEditLayout);

                                    if (Csw.string(thisTabId) === Csw.string(cswPrivate.tabState.tabid)) {
                                        cswPrivate.tabState.tabNo = tabno;
                                    }
                                    if (Csw.isNullOrEmpty(cswPrivate.tabState.tabid)) {
                                        cswPrivate.tabState.tabid = thisTabId;
                                    }
                                    tabno += 1;
                                };

                                Csw.iterate(data.tabs, tabFunc);

                                cswPrivate.tabcnt = tabno;

                                Csw.iterate(jqTabs, function (thisTabDiv) {

                                    thisTabDiv.$.tabs({
                                        active: cswPrivate.tabState.tabNo,
                                        beforeActivate: function (event, ui) {
                                            var ret = Csw.tryExec(cswPrivate.onBeforeTabSelect, cswPrivate.tabState.tabid);
                                            if (ret) {
                                                cswPrivate.tabState.tabNo = ui.newTab.index();
                                                cswPrivate.tabState.tabid = ui.newTab.data('tabid');
                                                cswPrivate.globalState.selectedTabId = cswPrivate.tabState.tabid;
                                                cswPrivate.globalState.selectedTabId = cswPrivate.tabState.tabid;
                                                Csw.tryExec(cswPrivate.onTabSelect, cswPrivate.tabState.tabid);
                                                cswPrivate.form.empty();
                                                cswPrivate.onTearDown();
                                                makeTabs();
                                            }
                                            return ret;


                                        }
                                    }); // tabs
                                    cswPrivate.getProps(cswPrivate.tabState.tabid);
                                    Csw.tryExec(cswPrivate.onTabSelect, cswPrivate.tabState.tabid);

                                }); // for(var t in tabdivs)
                            }

                            makeTabs();
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
                return cswPrivate.form.$.valid() && cswPrivate.identityForm.$.valid();
            };

            //#endregion Validator

            //#region Helper Methods

            cswPrivate.toggleConfigIcon = function (enabled) {
                if (cswPrivate.globalState.configIcn) {
                    if (enabled) {
                        cswPrivate.globalState.configIcn.show();
                    } else {
                        cswPrivate.globalState.configIcn.hide();
                    }
                }
            };

            cswPrivate.toggleMulti = function () {
                cswPrivate.Multi = !cswPrivate.Multi;
                cswPrivate.tabState.Multi = cswPrivate.Multi;
                return cswPrivate.Multi;
            };

            cswPrivate.isMultiEdit = function () {
                /// <summary>
                /// True if Multi Edit is enabled
                /// </summary>
                return (cswPrivate.tabState.EditMode === Csw.enums.editMode.Edit || cswPrivate.tabState.EditMode === Csw.enums.editMode.EditInPopup) && cswPrivate.Multi;
            };

            cswPrivate.setNode = function (node) {
                /// <summary>
                /// Set the current NodeId and NodeKey
                /// </summary>
                var nodeid = null;
                if (node) {
                    nodeid = Csw.string(node.nodeid);
                    if (nodeid !== cswPublic.getNodeId()) {
                        Csw.tryExec(cswPrivate.onNodeIdSet, nodeid);

                        cswPrivate.tabState.nodeid = nodeid;
                        cswPrivate.tabState.nodekey = node.nodekey;
                        cswPrivate.tabState.nodename = node.nodename;
                        cswPrivate.tabState.nodetypeid = node.nodetypeid;
                        
                        cswPrivate.globalState.currentNodeId = nodeid;
                        cswPrivate.globalState.currentNodeLink = node.nodelink;
                        cswPrivate.globalState.currentNodeKey = node.nodekey;
                        cswPrivate.globalState.currentNodeTypeId = node.nodetypeid;
                    }
                }
                return nodeid;
            };

            cswPrivate.setSelectedNodes = function () {
                if (false === Csw.isNullOrEmpty(cswPrivate.nodeTreeCheck)) {
                    var nodeData = cswPrivate.nodeTreeCheck.checkedNodes();
                    //It's easier to nuke the collection than to remap it
                    cswPrivate.globalState.selectedNodeIds = Csw.delimitedString();
                    cswPrivate.globalState.selectedNodeKeys = Csw.delimitedString();
                    Csw.iterate(nodeData, function (thisNode) {
                        cswPrivate.globalState.selectedNodeIds.add(thisNode.nodeid);
                        cswPrivate.globalState.selectedNodeKeys.add(thisNode.nodekey);
                    });
                }
            };

            cswPublic.getSelectedNodes = function () {
                return cswPrivate.globalState.selectedNodeIds.string();
            };

            cswPrivate.addSelectedProp = function (propid) {
                cswPrivate.globalState.selectedPropIds.add(propid);
            };

            cswPrivate.dropSelectedProp = function (propid) {
                cswPrivate.globalState.selectedPropIds.remove(propid);
            };

            cswPublic.getSelectedProps = function () {
                return cswPrivate.globalState.selectedPropIds.string();
            };

            cswPublic.getNodeId = function () {
                /// <summary>
                /// Get the current NodeId
                /// </summary>
                var nodeid = Csw.string(cswPrivate.globalState.currentNodeId, 'newnode');
                cswPrivate.tabState.nodeid = nodeid;
                return nodeid;
            };

            cswPublic.getNodeKey = function () {
                /// <summary>
                /// Get the current NodeKey
                /// </summary>
                var nodekey = Csw.string(cswPrivate.globalState.currentNodeKey);
                cswPrivate.tabState.nodekey = nodekey;
                return nodekey;
            };

            cswPublic.getPropJson = function () {
                cswPrivate.updatePropJsonFromLayoutTable();
                return cswPrivate.globalState.propertyData;
            };

            //#endregion Helper Methods

            //#region Layout Config

            cswPrivate.onRemove = function (tabid, onRemoveData) {
                'use strict';
                var propid = onRemoveData.cellSet[1][1].data('propId');
                cswPrivate.ajax.layoutRemove = Csw.ajax.post({
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
                var propIdOrig = cswPrivate.moveProp(cswPrivate.getPropertyCell(onSwapData.cellSet), tabid, onSwapData.swaprow, onSwapData.swapcolumn, onSwapData.cellSet[1][1].data('propid'));
                var propIdSwap = cswPrivate.moveProp(cswPrivate.getPropertyCell(onSwapData.swapcellset), tabid, onSwapData.row, onSwapData.column, onSwapData.swapcellset[1][1].data('propid'));
                onSwapData.cellSet[1][1].data('propid', propIdSwap);
                onSwapData.swapcellset[1][1].data('propid', propIdOrig);
            };

            // onSwap()

            cswPrivate.moveProp = function (propDiv, tabid, newrow, newcolumn, propId) {
                'use strict';
                var propid = Csw.string(propDiv.data('propid'), propId);
                if (Csw.isNullOrEmpty(propid) && propDiv.length() > 0) {
                    if (false === Csw.isNullOrEmpty(propDiv.children())) {
                        propid = propDiv.children().data('propid');
                    }
                }
                if (Csw.isNullOrEmpty(propid)) {
                    Csw.debug.error('No property selected for move.');
                } else {
                    var dataJson = {
                        PropId: propid,
                        TabId: tabid,
                        NewRow: newrow,
                        NewColumn: newcolumn,
                        EditMode: cswPrivate.tabState.EditMode
                    };

                    cswPrivate.ajax.layoutMove = Csw.ajax.post({
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
                    if (Csw.isNullOrEmpty(cswPrivate.tabgrouptables)) {
                        cswPrivate.tabgrouptables = [];
                    }
                    if (Csw.isNullOrEmpty(cswPrivate.tabgrouptables[tabgroup])) {
                        var cellSet = layoutTable.cellSet(displayrow, displaycol);
                        var propCell = cswPrivate.getPropertyCell(cellSet);
                        var fieldSet = propCell.fieldSet();
                        fieldSet.legend({ value: tabgroup });

                        var div = fieldSet.div();

                        var tabgroupLayoutTable = div.layoutTable({
                            name: tabgroup,
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
                        cswPrivate.tabgrouptables[tabgroup] = tabgroupLayoutTable;
                    }
                    ret = cswPrivate.tabgrouptables[tabgroup].cellSet(displayrow, displaycol);
                } else {
                    ret = layoutTable.cellSet(displayrow, displaycol);
                }
                return ret;
            }; // getCellSet()

            //#endregion Layout Table Cell Set

            //#region Properties

            cswPrivate.getProps = function (tabid, onSuccess) {
                'use strict';

                cswPrivate.onTearDownProps();
                if (cswPrivate.tabState.EditMode === Csw.enums.editMode.Add && cswPrivate.tabState.Config === false) {
                    // case 20970 - make sure there's room in the quota
                    cswPrivate.ajax.props = Csw.ajaxWcf.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: 'Quotas/check',
                        data: {
                            NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                            NodeKey: ''
                        },
                        success: function (data) {
                            if (data && data.HasSpace) {
                                cswPrivate.getPropsImpl(tabid, onSuccess);
                            } else {
                                //cswPrivate.tabs[tabid].append('You have used all of your purchased quota, and must purchase additional quota space in order to add more.');
                                cswPrivate.tabs[tabid].append(data.Message);
                                Csw.tryExec(cswPrivate.onInitFinish, false);
                            }
                        }
                    });
                } else {
                    cswPrivate.getPropsImpl(tabid, onSuccess);
                }

            }; // getProps()

            cswPrivate.getPropsImpl = function (tabid, onSuccess) {
                'use strict';
                cswPrivate.tabgrouptables = [];  // case 26957, 27117

                function makePropLayout() {
                    cswPrivate.form = cswPrivate.tabs[tabid].children('form');
                    cswPrivate.form.empty();

                    if (false === Csw.isNullOrEmpty(cswPrivate.globalState.title)) {
                        cswPrivate.form.append(cswPrivate.globalState.title);
                    }

                    var formTable = cswPrivate.form.table({
                        name: cswPrivate.name + '_formtbl_' + tabid,
                        width: '100%'
                    }).css({ padding: '10px' });

                    var layoutOpts = {
                        name: cswPrivate.name + '_props_' + tabid,
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
                                var cellSet = cswPrivate.getCellSet(cswPrivate.layoutTable, thisProp.tabgroup, parentCell.data('row'), parentCell.data('column'));

                                cswPrivate.layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
                                var propCell = cswPrivate.getPropertyCell(cellSet);

                                if (subTable.length > 0) {
                                    cswPrivate.updateSubProps(thisProp, propCell, tabid, configOn);
                                }
                            }
                            return false;
                        };
                        Csw.iterate(cswPrivate.globalState.propertyData, updOnSuccess);
                    }

                    cswPrivate.handleProperties(null, tabid, false);
                    if (false === Csw.isNullOrEmpty(cswPrivate.layoutTable.cellSet(1, 1)) &&
                            false === Csw.isNullOrEmpty(cswPrivate.layoutTable.cellSet(1, 1)[1][2])) {
                        cswPrivate.layoutTable.cellSet(1, 1)[1][2].trigger('focus');
                    }
                    // Validation
                    cswPrivate.initValidator();

                    if (Csw.bool(cswPrivate.tabState.Config)) {
                        cswPrivate.layoutTable.configOn();
                    } else if ( Csw.isNullOrEmpty(cswPrivate.globalState.date) &&
                                cswPrivate.tabState.EditMode !== Csw.enums.editMode.PrintReport &&
                                Csw.bool(cswPrivate.tabs[tabid].data('canEditLayout'))) {
                        /* Case 24437 */
                        var editLayoutOpt = {
                            name: cswPrivate.name,
                            globalState: cswPrivate.globalState,
                            tabState: {
                                tabid: tabid,
                                tabNo: cswPrivate.tabState.tabNo
                            },
                            Refresh: function () {
                                //Csw.tryExec(cswPrivate.Refresh);
                                cswPrivate.tabState.Config = false;
                                cswPrivate.getTabs();
                            }
                        };

                        /* Show the 'fake' config button to open the dialog */
                        cswPrivate.globalState.configIcn = formTable.cell(1, 2).icon({
                            name: cswPrivate.name + 'configbtn',
                            iconType: Csw.enums.iconType.wrench,
                            hovertext: 'Configure',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                cswPrivate.clearTabs();
                                $.CswDialog('EditLayoutDialog', editLayoutOpt);
                            }
                        });
                        cswPrivate.toggleConfigIcon(false === cswPrivate.isMultiEdit());
                    }

                    /* case 8494 */
                    if (!cswPrivate.tabState.Config && !cswPrivate.atLeastOne.Saveable && cswPrivate.tabState.EditMode === Csw.enums.editMode.Add) {
                        cswPublic.save(tabid);
                    } else {
                        Csw.tryExec(cswPrivate.onInitFinish, cswPrivate.atLeastOne.Property);
                        Csw.tryExec(onSuccess);
                    }
                }

                if (cswPrivate.tabState.Config || // case 28274 - always refresh prop data if in config mode
                    (Csw.isNullOrEmpty(cswPrivate.globalState.propertyData) ||
                     (cswPrivate.tabState.EditMode !== Csw.enums.editMode.Add &&
                      cswPrivate.tabState.EditMode !== Csw.enums.editMode.Temp))) {

                    cswPrivate.ajax.propsImpl = Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.urls.PropsUrlMethod,
                        data: {
                            EditMode: cswPrivate.tabState.EditMode,
                            NodeId: cswPublic.getNodeId(),
                            TabId: tabid,
                            SafeNodeKey: cswPublic.getNodeKey(),
                            NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                            Date: Csw.string(cswPrivate.globalState.date, new Date().toDateString()),
                            Multi: Csw.bool(cswPrivate.tabState.Multi),
                            filterToPropId: Csw.string(cswPrivate.globalState.filterToPropId),
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
                            cswPrivate.setNode(data.node);
                            cswPrivate.globalState.propertyData = data.properties;
                            makePropLayout();
                        } // success{}
                    }); // ajax
                } else {
                    if (cswPrivate.globalState.propertyData.node) {
                        cswPrivate.setNode(cswPrivate.globalState.propertyData.node);
                    }
                    makePropLayout();
                }
            }; // getPropsImpl()

            cswPrivate.handleProperties = function (layoutTable, tabid, configMode, tabPropData) {
                'use strict';
                layoutTable = layoutTable || cswPrivate.layoutTable;
                var handleSuccess = function (propObj) {
                    cswPrivate.atLeastOne.Property = true;
                    cswPrivate.handleProp(layoutTable, propObj, tabid, configMode);
                    return false;
                };
                tabPropData = tabPropData || cswPrivate.globalState.propertyData;
                Csw.iterate(tabPropData, handleSuccess);

                cswPrivate.onRenderProps(tabid);

            }; // _handleProperties()


            cswPrivate.handleProp = function (layoutTable, propData, tabid, configMode) {
                'use strict';
                if (Csw.isPlainObject(propData)) {
                    var propid = propData.id,
                        cellSet,
                        helpText = Csw.string(propData.helptext),
                        propName = Csw.string(propData.name),
                        labelCell = {};

                    if (propData.displayrow <= 0 || propData.displayrow >= Number.MAX_VALUE || 
                         propData.displaycol <= 0 || propData.displaycol >= Number.MAX_VALUE) {
                        throw new Error('Cannot make a property at these coordinate: x=' + propData.displaycol + ',y=' + propData.displayrow + '.');
                    }
                    cellSet = cswPrivate.getCellSet(layoutTable, propData.tabgroup, propData.displayrow, propData.displaycol);
                    layoutTable.addCellSetAttributes(cellSet, { propId: propid });

                    if (cswPrivate.canDisplayProp(propData, configMode) &&
                        Csw.bool(propData.showpropertyname)) {

                        labelCell = cswPrivate.getLabelCell(cellSet);

                        labelCell.addClass('propertylabelcell');
                        if (Csw.bool(propData.highlight)) {
                            labelCell.addClass('ui-state-highlight');
                        }

                        if (false === Csw.isNullOrEmpty(helpText)) {
                            labelCell.a({
                                cssclass: 'cswprop_helplink propertylabel',
                                title: helpText,
                                onClick: function () {
                                    return false;
                                },
                                value: "" //Case 29100 - the prop name will be set by setLabelText()
                            }).setLabelText(propName, propData.required, propData.readonly || cswPrivate.tabState.ReadOnly);
                        } else {
                            labelCell.setLabelText(propName, propData.required, propData.readonly || cswPrivate.tabState.ReadOnly);
                        }

                        cswPrivate.globalState.checkBoxes['check_' + propid] = labelCell.checkBox({
                            name: 'check_' + propid,
                            value: false, // Value --not defined?,
                            cssclass: cswPrivate.name + '_check',
                            onChange: function (val) {
                                if (cswPrivate.globalState.checkBoxes['check_' + propid].val()) {
                                    cswPrivate.addSelectedProp(propid);
                                } else {
                                    cswPrivate.dropSelectedProp(propid);
                                }
                            }
                        });
                        cswPrivate.globalState.checkBoxes['check_' + propid].data('propid', propid);
                        if (cswPrivate.isMultiEdit()) {
                            cswPrivate.globalState.checkBoxes['check_' + propid].show();
                        } else {
                            cswPrivate.globalState.checkBoxes['check_' + propid].hide();
                        }
                        if (false === Csw.bool(propData.copyable)) {
                            cswPrivate.globalState.checkBoxes['check_' + propid].disable();
                        }
                        if (false === Csw.bool(propData.readonly)) {
                            cswPrivate.atLeastOne.Saveable = true;
                        }

                    }

                    var propCell = cswPrivate.getPropertyCell(cellSet);
                    propCell.addClass('propertyvaluecell');

                    if (Csw.bool(propData.highlight)) {
                        propCell.addClass('ui-state-highlight');
                    }
                    cswPrivate.makeProp(propCell, propData, tabid, configMode, layoutTable);

                    if (propData.ocpname === "Owner") {
                        Csw.unsubscribe('onPropChange_' + propid);
                        Csw.subscribe('onPropChange_' + propid, function (eventObject, data) {
                            cswPrivate.propid = propid;
                            cswPrivate.onAnyPropChange(eventObject, data, tabContentDiv);

                        });
                    }
                }
            };

            cswPrivate.canDisplayProp = function (propData, configMode) {
                /*The prop is set to display or we're in layout config mode*/
                var ret = (Csw.bool(propData.display, true) || configMode);
                /* We're not excluding any OC Props or this prop has not been excluded */
                ret = ret && ((Csw.isNullOrEmpty(cswPrivate.globalState.excludeOcProps) || cswPrivate.globalState.excludeOcProps.length === 0) || false === Csw.contains(cswPrivate.globalState.excludeOcProps, Csw.string(propData.ocpname).toLowerCase()));
                return ret;
            };

            cswPrivate.makeProp = function (propCell, propData, tabid, configMode, layoutTable) {
                'use strict';
                propCell.empty();
                if (cswPrivate.canDisplayProp(propData, configMode)) {
                    var propId = propData.id;
                    var propName = propData.name;

                    var tabState = Csw.extend({}, cswPrivate.tabState, true);
                    tabState.tabid = tabid;

                    var fieldOpt = Csw.nbt.propertyOption({
                        isMulti: cswPrivate.isMultiEdit,
                        fieldtype: propData.fieldtype,
                        tabState: tabState,
                        propid: propData.id,
                        propData: propData,
                        Required: Csw.bool(propData.required),
                        onReload: function (afterReload) {
                            cswPrivate.getProps(tabid, afterReload);
                        },
                        onChange: function () {
                            if (Csw.bool(propData.hassubprops)) {
                                Csw.tryExec(cswPrivate.updateSubProps, propData, propCell, tabid, false, layoutTable);
                            }
                            Csw.tryExec(cswPrivate.onPropertyChange, fieldOpt.propid, propName, propData);
                        },
                        doSave: function (saveopts) {
                            var s = {
                                onSuccess: null
                            };
                            Csw.extend(s, saveopts);
                            cswPublic.save(tabid, s.onSuccess);
                        },
                        onEditView: cswPrivate.onEditView,
                        onAfterButtonClick: cswPrivate.onAfterButtonClick
                    }, propCell.div());

                    if (Csw.string(fieldOpt.fieldtype).toLowerCase().trim() === 'button') {
                        Object.defineProperty(fieldOpt, 'saveTheCurrentTab', {
                            value: cswPublic.save
                        });
                    }

                    cswPrivate.properties[propId] = Csw.nbt.property(fieldOpt);

                    if (Csw.contains(propData, 'subprops') && false === Csw.isNullOrEmpty(propData.subprops)) {
                        // recurse on sub-props
                        var subProps = propData.subprops;

                        var subLayoutTable = propCell.layoutTable({
                            name: fieldOpt.propid + '_subproptable',
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
                                cswPrivate.handleProp(subLayoutTable, subProp, tabid, configMode);
                                if (configMode) {
                                    subLayoutTable.configOn();
                                } else {
                                    subLayoutTable.configOff();
                                }
                            }
                            return false;
                        };
                        Csw.iterate(subProps, subOnSuccess);
                    }
                } // if (propData.display != 'false' || ConfigMode )
            }; // _makeProp()

            cswPrivate.updateSubProps = function (singlePropData, propCell, tabid, configMode, layoutTable) {
                /// <summary>Update a properties sub props</summary>
                /// <param name="fieldOpt" type="Object"> An object defining a prop's fieldtype </param>
                /// <param name="propId" type="String"> A propertyid </param>
                /// <param name="propData" type="Object"> Property definition </param>
                /// <param name="$propcell" type="JQuery"> An element to append to </param>
                /// <param name="$tabcontentdiv" type="JQuery"> A tab element </param>
                /// <param name="tabid" type="String"> TabId </param>
                /// <param name="configMode" type="Boolean"> True if config mode </param>
                /// <returns type="void"></returns>
                'use strict';

                Csw.defer(function () {
                    if (singlePropData.wasmodified) {
                        var jsonData = {
                            EditMode: Csw.string(cswPrivate.tabState.EditMode, 'Edit'),
                            NodeId: cswPublic.getNodeId(),
                            SafeNodeKey: cswPublic.getNodeKey(),
                            PropId: singlePropData.id,
                            NodeTypeId: Csw.string(cswPrivate.tabState.nodetypeid),
                            NewPropJson: JSON.stringify(singlePropData)
                        };

                        cswPrivate.ajax.subProps = Csw.ajax.post({
                            watchGlobal: cswPrivate.AjaxWatchGlobal,
                            urlMethod: cswPrivate.urls.SinglePropUrlMethod,
                            data: jsonData,
                            success: function (data) {
                                singlePropData.wasmodified = true;
                                singlePropData.subprops = data.subprops;

                                // keep the fact that the parent property was modified
                                cswPrivate.makeProp(propCell, singlePropData, tabid, configMode, layoutTable);
                                cswPrivate.onRenderProps(tabid);
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
                            nodeid: cswPublic.getNodeId(),
                            Multi: cswPrivate.isMultiEdit(),
                            nodekey: cswPublic.getNodeKey()
                        };

                        var cellSet = cswPrivate.getCellSet(layoutTable, thisProp.tabgroup, thisProp.displayrow, thisProp.displaycol);
                        layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
                        propOpt.propCell = cswPrivate.getPropertyCell(cellSet);
                        propOpt.propDiv = propOpt.propCell.children('div').first();

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
                    Csw.iterate(propData, updSuccess);
                    return propIds;
                });
            }; // updatePropJsonFromLayoutTable()

            //#endregion Properties

            //#region commit

            cswPublic.copy = function (onSuccess) {
                if (cswPrivate.isMultiEdit()) {
                    cswPrivate.setSelectedNodes();
                    var nodeids = cswPublic.getSelectedNodes();
                    var propids = cswPublic.getSelectedProps();
                    if (nodeids.length > 0 && propids.length > 0) {
                        // apply the newly saved checked property values on this node to the checked nodes
                        cswPrivate.ajax.copy = Csw.ajax.post({
                            watchGlobal: cswPrivate.AjaxWatchGlobal,
                            urlMethod: cswPrivate.urls.CopyPropValuesUrlMethod,
                            data: {
                                SourceNodeId: cswPublic.getNodeId(),
                                CopyNodeIds: nodeids,
                                PropIds: propids
                            },
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
                                Csw.tryExec(onSuccess);
                            }
                        }); // ajax
                    } else {
                        $.CswDialog('AlertDialog', 'You have not selected any properties to save.');
                    }
                }
            };

            cswPublic.save = Csw.method(function (tabid, onSuccess, async, reloadTabOnSave) {
                'use strict';
                tabid = tabid || cswPrivate.tabState.tabid;
                // This basically sets a default for reloadOnTabSave:
                // if there is no value, we default to cswPrivate.ReloadTabOnSave
                if (typeof reloadTabOnSave == 'undefined') {
                    reloadTabOnSave = cswPrivate.ReloadTabOnSave;
                }

                if (cswPrivate.isMultiEdit() || cswPublic.isFormValid()) {
                    async = Csw.bool(async, true) && false === cswPrivate.isMultiEdit();
                    //Do NOT register save for tear down. Only true gets are eligible for teardown.
                    //cswPrivate.ajax.save = Csw.ajax.post({
                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.urls.SavePropUrlMethod,
                        async: async,
                        data: {
                            EditMode: cswPrivate.tabState.EditMode,
                            NodeId: cswPublic.getNodeId(),
                            SafeNodeKey: cswPublic.getNodeKey(),
                            TabId: Csw.string(tabid),
                            NodeTypeId: cswPrivate.tabState.nodetypeid,
                            NewPropsJson: Csw.serialize(cswPrivate.globalState.propertyData),
                            IdentityTabJson: Csw.serialize(cswPrivate.IdentityTab),
                            ViewId: cswPublic.getViewId(),
                            RemoveTempStatus: cswPrivate.globalState.removeTempStatus
                        },
                        success: function (successData) {
                            //cswPrivate.enableSaveBtn();
                            var onSaveSuccess = function () {
                                var onSaveRefresh = function () {
                                    Csw.tryExec(cswPrivate.onSave, successData.nodeid, successData.nodekey, cswPrivate.tabcnt, successData.nodename, successData.nodelink, successData.physicalstatemodified);
                                    Csw.tryExec(onSuccess);
                                };

                                onSaveRefresh();
                            };
                            if (false === cswPrivate.isMultiEdit()) {
                                if (reloadTabOnSave) {
                                    // reload tab
                                    cswPrivate.globalState.propertyData = '';
                                    cswPrivate.getProps(tabid, onSaveSuccess);

                                } else {
                                    onSaveSuccess();
                                }
                            } else {
                                cswPublic.copy(onSaveSuccess);
                            }
                        }, // success
                        error: function (errorData) {
                            Csw.tryExec(cswPrivate.onSaveError, errorData);
                        }
                    }); // ajax
                } // if(cswPrivate.isValid())
            }); // Save()

            //#endregion commit

            cswPrivate.refreshLinkDiv = function () {
                if (cswPrivate.globalState.ShowAsReport &&
                    false === cswPrivate.isMultiEdit() &&
                    cswPrivate.tabState.EditMode !== Csw.enums.editMode.PrintReport) {

                    if (cswPrivate.linkDiv) {
                        cswPrivate.linkDiv.remove();
                    }
                    cswPrivate.linkDiv = cswParent.div({
                        name: cswPrivate.name + '_linkdiv',
                        align: 'right'
                    });
                    cswPrivate.linkDiv.empty();

                    cswPrivate.linkDiv.a({
                        text: 'As Report',
                        onClick: function () {
                            Csw.openPopup('NodeReport.html?nodeid=' + cswPublic.getNodeId() + '&nodekey=' + cswPublic.getNodeKey());
                        }
                    });
                }
            };

            cswPublic.getViewId = function () {
                if (Csw.isNullOrEmpty(cswPrivate.globalState.viewid)) {
                    cswPrivate.globalState.viewid = Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId);
                }
                return cswPrivate.globalState.viewid;
            };

            cswPrivate.bindSaveSub = function () {
                var saveCallBack = function _save() {
                    cswPublic.save();
                    Csw.unsubscribe('CswSaveTabsAndProp_tab' + cswPrivate.tabState.tabid + '_' + cswPublic.getNodeId(), _save);
                };
                Csw.subscribe('CswSaveTabsAndProp_tab' + cswPrivate.tabState.tabid + '_' + cswPublic.getNodeId(), saveCallBack);
            };

            cswPublic.resetTabs = function (nodeid, nodekey) {
                if (false === Csw.isNullOrEmpty(nodeid)) {
                    cswPrivate.setNode({ nodeid: nodeid, nodekey: nodekey });
                }
                cswPrivate.onTearDown();
                cswPrivate.init();
                cswPrivate.getTabs(cswPrivate.outerTabDiv);
                cswPrivate.refreshLinkDiv();
                cswPrivate.bindSaveSub();
            };

            (function _preCtor() {
                Csw.extend(cswPrivate, options, true);
                if (cswPrivate.globalState.propertyData && cswPrivate.globalState.propertyData.properties) {
                    cswPrivate.setNode(cswPrivate.globalState.propertyData.node);
                    cswPrivate.globalState.propertyData = cswPrivate.globalState.propertyData.properties;
                }


                cswPrivate.init = function () {
                    Csw.unsubscribe('CswMultiEdit', null, cswPrivate.onMultiEdit);
                    Csw.subscribe('CswMultiEdit', cswPrivate.onMultiEdit);
                    //We don't have node name yet. Init the div in the right place and polyfill later.
                    cswPrivate.titleDiv = cswParent.div({ cssclass: 'CswIdentityTabHeader' }).hide();
                    cswPrivate.identityWrapDiv = cswParent.div();

                    cswPrivate.tabsTable = cswPrivate.identityWrapDiv.table({ width: '100%' });

                    cswPrivate.identityForm = cswPrivate.tabsTable.cell(1, 1).form();

                    cswPrivate.outerTabDiv = cswPrivate.tabsTable
                        .cell(3, 1)
                        .tabDiv({});
                    cswPrivate.tabcnt = 0;
                };
                cswPrivate.init();
            }());

            (function _postCtor() {
                cswPrivate.getTabs(cswPrivate.outerTabDiv);
                cswPrivate.refreshLinkDiv();
                cswPrivate.bindSaveSub();
            }());

            return cswPublic;
        });
}());