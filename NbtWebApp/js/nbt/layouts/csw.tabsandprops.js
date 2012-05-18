/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.layouts.tabsAndProps = Csw.layouts.tabsAndProps ||
        Csw.layouts.register('tabsAndProps', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
                ID: '',
                TabsUrlMethod: 'getTabs',
                SinglePropUrlMethod: 'getSingleProp',
                PropsUrlMethod: 'getProps',
                MovePropUrlMethod: 'moveProp',
                RemovePropUrlMethod: 'removeProp',
                SavePropUrlMethod: 'saveProps',
                CopyPropValuesUrlMethod: 'copyPropValues',
                NodePreviewUrlMethod: 'getNodePreview',
                QuotaUrlMethod: 'checkQuota',
                nodeids: [],
                nodepks: [],
                nodekeys: [],
                relatednodeid: '',
                relatednodename: '',
                relatednodetypeid: '',
                relatedobjectclassid: '',
                tabid: '',
                nodetypeid: '',
                filterToPropId: '',
                title: '',
                date: '',
                EditMode: Csw.enums.editMode.Edit,
                Multi: false,
                ReadOnly: false,
                onSave: null,
                ReloadTabOnSave: true,
                Refresh: null,
                onBeforeTabSelect: function () { return true; },
                onTabSelect: null,
                onPropertyChange: null,
                onPropertyRemove: null,
                onInitFinish: null,
                ShowCheckboxes: false,
                ShowAsReport: true,
                AjaxWatchGlobal: true,
                nodeTreeCheck: null,
                onEditView: null,
                onAfterButtonClick: null,
                Config: false,
                showSaveButton: true,
                atLeastOne: {},
                saveBtn: {},
                propertyData: {},
                excludeOcProps: []
            };
            var cswPublicRet = {};

            (function () {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                cswPrivateVar.outerTabDiv = cswParent.tabDiv({ ID: cswPrivateVar.ID + '_tabdiv' });
                cswPrivateVar.tabcnt = 0;
            } ());

            cswPrivateVar.clearTabs = function () {
                cswPrivateVar.outerTabDiv.empty();
            };

            cswPrivateVar.makeTabContentDiv = function (tabParent, tabid, canEditLayout) {
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

            cswPrivateVar.getTabs = function (tabContentDiv) {
                'use strict';
                var jsonData = {
                    EditMode: cswPrivateVar.EditMode,
                    NodeId: Csw.tryParseObjByIdx(cswPrivateVar.nodeids, 0),
                    SafeNodeKey: Csw.tryParseObjByIdx(cswPrivateVar.nodekeys, 0),
                    NodeTypeId: cswPrivateVar.nodetypeid,
                    Date: cswPrivateVar.date,
                    filterToPropId: cswPrivateVar.filterToPropId,
                    Multi: cswPrivateVar.Multi,
                    ConfigMode: cswPrivateVar.Config
                };

                // For performance, don't bother getting tabs if we're in Add or Preview
                if (cswPrivateVar.EditMode === Csw.enums.editMode.Add ||
                    cswPrivateVar.EditMode === Csw.enums.editMode.Preview ||
                        cswPrivateVar.EditMode === Csw.enums.editMode.Table) {

                    var tabid = cswPrivateVar.EditMode + "_tab";
                    tabContentDiv = cswPrivateVar.makeTabContentDiv(tabContentDiv, tabid, false);
                    cswPrivateVar.getProps(tabContentDiv, tabid);

                } else {

                    Csw.ajax.post({
                        watchGlobal: cswPrivateVar.AjaxWatchGlobal,
                        urlMethod: cswPrivateVar.TabsUrlMethod,
                        data: jsonData,
                        success: function (data) {
                            cswPrivateVar.clearTabs();
                            var tabdivs = [];
                            var selectedtabno = 0;
                            var tabno = 0;
                            var tabDiv, tabUl;
                            cswPrivateVar.nodename = data.nodename;
                            delete data.nodename;
                            var tabFunc = function (thisTab) {
                                var thisTabId = thisTab.id;

                                if (cswPrivateVar.EditMode === Csw.enums.editMode.PrintReport || tabdivs.length === 0) {
                                    // For PrintReports, we're going to make a separate tabstrip for each tab
                                    tabDiv = cswPrivateVar.outerTabDiv.tabDiv();
                                    tabUl = tabDiv.ul();
                                    tabdivs[tabdivs.length] = tabDiv;
                                }
                                tabDiv = tabDiv || tabdivs[tabdivs.length - 1];
                                tabUl = tabUl || tabDiv.ul();
                                tabUl.li().a({ href: '#' + thisTabId, text: thisTab.name });
                                cswPrivateVar.makeTabContentDiv(tabDiv, thisTabId, thisTab.canEditLayout);
                                if (thisTabId === cswPrivateVar.tabid) {
                                    selectedtabno = tabno;
                                }
                                tabno += 1;
                                return false;
                            };
                            Csw.crawlObject(data, tabFunc, false);

                            cswPrivateVar.tabcnt = tabno;

                            Csw.each(tabdivs, function (thisTabDiv) {
                                thisTabDiv.tabs({
                                    selected: selectedtabno,
                                    select: function (event, ui) {
                                        var ret = false;
                                        var selectTabContentDiv = thisTabDiv.children('div:eq(' + Csw.number(ui.index) + ')');
                                        var selectTabid = selectTabContentDiv.getId();
                                        if (Csw.tryExec(cswPrivateVar.onBeforeTabSelect, selectedtabid)) {
                                            if (false === Csw.isNullOrEmpty(selectTabContentDiv)) {
                                                cswPrivateVar.form.empty();
                                                cswPrivateVar.getProps(selectTabContentDiv, selectTabid);
                                                Csw.tryExec(cswPrivateVar.onTabSelect, selectTabid);
                                                ret = true;
                                            }
                                        }
                                        return ret;
                                    } // select()
                                }); // tabs
                                var eachTabContentDiv = thisTabDiv.children('div:eq(' + Csw.number(thisTabDiv.tabs('option', 'selected')) + ')');
                                if (eachTabContentDiv.isValid) {
                                    var selectedtabid = eachTabContentDiv.getId();
                                    cswPrivateVar.getProps(eachTabContentDiv, selectedtabid);
                                    Csw.tryExec(cswPrivateVar.onTabSelect, selectedtabid);
                                }
                            }); // for(var t in tabdivs)
                        } // success
                    }); // ajax
                } // if-else editmode is add or preview
            };

            // getTabs()

            cswPrivateVar.getProps = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                if (cswPrivateVar.EditMode === Csw.enums.editMode.Add && cswPrivateVar.Config === false) {
                    // case 20970 - make sure there's room in the quota
                    Csw.ajax.post({
                        watchGlobal: cswPrivateVar.AjaxWatchGlobal,
                        urlMethod: cswPrivateVar.QuotaUrlMethod,
                        data: { NodeTypeId: cswPrivateVar.nodetypeid },
                        success: function (data) {
                            if (Csw.bool(data.result)) {
                                cswPrivateVar.getPropsImpl(tabContentDiv, tabid, onSuccess);
                            } else {
                                tabContentDiv.append('You have used all of your purchased quota, and must purchase additional quota space in order to add more.');
                                Csw.tryExec(cswPrivateVar.onInitFinish, false);
                            }
                        }
                    });
                } else {
                    cswPrivateVar.getPropsImpl(tabContentDiv, tabid, onSuccess);
                }
            };

            // getProps()

            cswPrivateVar.getPropsImpl = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                var jsonData = {
                    EditMode: cswPrivateVar.EditMode,
                    NodeId: Csw.tryParseObjByIdx(cswPrivateVar.nodeids, 0),
                    TabId: tabid,
                    SafeNodeKey: Csw.tryParseObjByIdx(cswPrivateVar.nodekeys, 0),
                    NodeTypeId: cswPrivateVar.nodetypeid,
                    Date: cswPrivateVar.date,
                    Multi: cswPrivateVar.Multi,
                    filterToPropId: cswPrivateVar.filterToPropId,
                    ConfigMode: cswPrivateVar.Config
                };

                Csw.ajax.post({
                    watchGlobal: cswPrivateVar.AjaxWatchGlobal,
                    urlMethod: cswPrivateVar.PropsUrlMethod,
                    data: jsonData,
                    success: function (data) {
                        cswPrivateVar.propertyData = data;
                        cswPrivateVar.form = tabContentDiv.children('form');
                        cswPrivateVar.form.empty();

                        if (false === Csw.isNullOrEmpty(cswPrivateVar.title)) {
                            cswPrivateVar.form.append(cswPrivateVar.title);
                        }

                        var formTable = cswPrivateVar.form.table({
                            ID: cswPrivateVar.ID + '_formtbl',
                            width: '100%'
                        });
                        //var formTblCell11 = formTable.cell(1, 1);
                        //var formTblCell12 = formTable.cell(1, 2);


                        cswPrivateVar.layoutTable = formTable.cell(1, 1).layoutTable({
                            ID: cswPrivateVar.ID + '_props',
                            OddCellRightAlign: true,
                            ReadOnly: (cswPrivateVar.EditMode === Csw.enums.editMode.PrintReport || cswPrivateVar.ReadOnly),
                            cellSet: {
                                rows: 1,
                                columns: 2
                            },
                            onSwap: function (e, onSwapData) {
                                cswPrivateVar.onSwap(tabid, onSwapData);
                            },
                            showConfigButton: false, //o.Config,
                            showExpandRowButton: cswPrivateVar.Config,
                            showExpandColButton: (cswPrivateVar.Config && cswPrivateVar.EditMode !== Csw.enums.editMode.Table),
                            showRemoveButton: cswPrivateVar.Config,
                            onConfigOn: function () {
                                doUpdateSubProps(true);
                            }, // onConfigOn
                            onConfigOff: function () {
                                doUpdateSubProps(false);
                            }, // onConfigOff
                            onRemove: function (event, onRemoveData) {
                                cswPrivateVar.onRemove(tabid, onRemoveData);
                            } // onRemove
                        }); // Csw.literals.layoutTable()

                        function doUpdateSubProps(configOn) {
                            var updOnSuccess = function (thisProp, key) {
                                if (Csw.bool(thisProp.hassubprops)) {
                                    var propId = key; //key
                                    var subTable = cswPrivateVar.layoutTable[propId + '_subproptable'];
                                    var parentCell = subTable.parent().parent();
                                    var cellSet = cswPrivateVar.layoutTable.cellSet(parentCell.propNonDom('row'), parentCell.propNonDom('column'));
                                    cswPrivateVar.layoutTable.addCellSetAttributes(cellSet, { propId: propId });
                                    var propCell = cswPrivateVar.getPropertyCell(cellSet);

                                    if (subTable.length > 0) {
                                        var fieldOpt = {
                                            fieldtype: thisProp.fieldtype,
                                            nodeid: Csw.tryParseObjByIdx(cswPrivateVar.nodeids, 0),
                                            relatednodeid: cswPrivateVar.relatednodeid,
                                            relatednodename: cswPrivateVar.relatednodename,
                                            relatednodetypeid: cswPrivateVar.relatednodetypeid,
                                            relatedobjectclassid: cswPrivateVar.relatedobjectclassid,
                                            propid: propId,
                                            propDiv: propCell.children('div'),
                                            propData: thisProp,
                                            onChange: function () {
                                            },
                                            onReload: function (afterReload) {
                                                cswPrivateVar.getProps(tabContentDiv, tabid, afterReload);
                                            },
                                            EditMode: cswPrivateVar.EditMode,
                                            Multi: cswPrivateVar.Multi,
                                            cswnbtnodekey: Csw.tryParseObjByIdx(cswPrivateVar.nodekeys, 0)
                                        };

                                        cswPrivateVar.updateSubProps(fieldOpt, propId, thisProp, propCell, tabContentDiv, tabid, configOn);
                                    }
                                }
                                return false;
                            };
                            Csw.crawlObject(cswPrivateVar.propertyData, updOnSuccess, false);
                        }

                        if (cswPrivateVar.EditMode !== Csw.enums.editMode.PrintReport && Csw.bool(cswPrivateVar.showSaveButton)) {
                            cswPrivateVar.saveBtn = formTable.cell(2, 1).button({
                                ID: 'SaveTab',
                                enabledText: 'Save Changes',
                                disabledText: 'Saving...',
                                onClick: function () { cswPublicRet.save(tabContentDiv, tabid); }
                            });
                        }
                        cswPrivateVar.atLeastOne = cswPrivateVar.handleProperties(null, tabContentDiv, tabid, false);
                        if (false === Csw.isNullOrEmpty(cswPrivateVar.layoutTable.cellSet(1, 1)) &&
                            false === Csw.isNullOrEmpty(cswPrivateVar.layoutTable.cellSet(1, 1)[1][2])) {
                            cswPrivateVar.layoutTable.cellSet(1, 1)[1][2].trigger('focus');
                        }
                        // Validation
                        cswPrivateVar.form.$.validate({
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

                        if (Csw.bool(cswPrivateVar.Config)) {
                            cswPrivateVar.layoutTable.configOn();
                        } else if (!cswPrivateVar.Config &&
                            Csw.isNullOrEmpty(cswPrivateVar.date) &&
                                cswPrivateVar.filterToPropId === '' &&
                                    cswPrivateVar.EditMode !== Csw.enums.editMode.PrintReport &&
                                        Csw.bool(tabContentDiv.data('canEditLayout'))) {
                            /* Case 24437 */
                            var editLayoutOpt = {
                                ID: cswPrivateVar.ID,
                                nodeids: cswPrivateVar.nodeids,
                                nodekeys: cswPrivateVar.nodekeys,
                                tabid: cswPrivateVar.tabid,
                                nodetypeid: cswPrivateVar.nodetypeid,
                                Refresh: function () {
                                    Csw.tryExec(cswPrivateVar.Refresh);
                                    cswPrivateVar.Config = false;
                                    cswPrivateVar.getTabs(tabContentDiv);
                                }
                            };

                            /* Show the 'fake' config button to open the dialog */
                            formTable.cell(1, 2).imageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Configure,
                                AlternateText: 'Configure',
                                ID: cswPrivateVar.ID + 'configbtn',
                                onClick: function () {
                                    cswPrivateVar.clearTabs();
                                    $.CswDialog('EditLayoutDialog', editLayoutOpt);
                                }
                            });
                        }

                        /* case 8494 */
                        if (!cswPrivateVar.Config && !cswPrivateVar.atLeastOne.Saveable && cswPrivateVar.EditMode === Csw.enums.editMode.Add) {
                            cswPublicRet.save(tabContentDiv, tabid);
                        } else {
                            Csw.tryExec(cswPrivateVar.onInitFinish, cswPrivateVar.atLeastOne.Property);
                            Csw.tryExec(onSuccess);
                        }
                    } // success{}
                }); // ajax
            };

            // getPropsImpl()

            cswPrivateVar.onRemove = function (tabid, onRemoveData) {
                'use strict';
                var propid = '';
                var propDiv = cswPrivateVar.getPropertyCell(onRemoveData.cellSet).children('div');
                if (false === Csw.isNullOrEmpty(propDiv)) {
                    propid = propDiv.first().propNonDom('propId');
                }

                Csw.ajax.post({
                    watchGlobal: cswPrivateVar.AjaxWatchGlobal,
                    urlMethod: cswPrivateVar.RemovePropUrlMethod,
                    data: { PropId: propid, EditMode: cswPrivateVar.EditMode, TabId: tabid },
                    success: function () {
                        cswPrivateVar.onPropertyRemove(propid);
                    }
                });

            };

            // onRemove()

            cswPrivateVar.onSwap = function (tabid, onSwapData) {
                cswPrivateVar.moveProp(cswPrivateVar.getPropertyCell(onSwapData.cellSet), tabid, onSwapData.swaprow, onSwapData.swapcolumn, onSwapData.cellSet[1][1].propNonDom('propid'));
                cswPrivateVar.moveProp(cswPrivateVar.getPropertyCell(onSwapData.swapcellset), tabid, onSwapData.row, onSwapData.column, onSwapData.swapcellset[1][1].propNonDom('propid'));
            };

            // onSwap()

            cswPrivateVar.moveProp = function (propDiv, tabid, newrow, newcolumn, propId) {
                'use strict';
                if (propDiv.length() > 0) {
                    var propid = Csw.string(propDiv.propNonDom('propid'), propId);
                    var dataJson = {
                        PropId: propid,
                        TabId: tabid,
                        NewRow: newrow,
                        NewColumn: newcolumn,
                        EditMode: cswPrivateVar.EditMode
                    };

                    Csw.ajax.post({
                        watchGlobal: cswPrivateVar.AjaxWatchGlobal,
                        urlMethod: cswPrivateVar.MovePropUrlMethod,
                        data: dataJson
                    });
                }
            };

            // _moveProp()

            cswPrivateVar.getLabelCell = function (cellSet) {
                return cellSet[1][1].children('div');
            };

            cswPrivateVar.getPropertyCell = function (cellSet) {
                return cellSet[1][2].children('div');
            };

            cswPrivateVar.handleProperties = function (layoutTable, tabContentDiv, tabid, configMode) {
                'use strict';
                layoutTable = layoutTable || cswPrivateVar.layoutTable;
                cswPrivateVar.atLeastOne = { Property: false, Saveable: false };
                var handleSuccess = function (propObj) {
                    cswPrivateVar.atLeastOne.Property = true;
                    cswPrivateVar.handleProp(layoutTable, propObj, tabContentDiv, tabid, configMode);
                    return false;
                };
                Csw.crawlObject(cswPrivateVar.propertyData, handleSuccess, false);

                if (false === Csw.isNullOrEmpty(cswPrivateVar.saveBtn, true)) {
                    if (cswPrivateVar.Config || (cswPrivateVar.atLeastOne.Saveable === false && cswPrivateVar.EditMode != Csw.enums.editMode.Add)) {
                        cswPrivateVar.saveBtn.hide();
                    } else {
                        cswPrivateVar.saveBtn.show();
                    }
                }
                return cswPrivateVar.atLeastOne;
            };

            // _handleProperties()

            cswPrivateVar.handleProp = function (layoutTable, propData, tabContentDiv, tabid, configMode) {
                'use strict';
                var propid = propData.id,
                    cellSet = layoutTable.cellSet(propData.displayrow, propData.displaycol),
                    helpText = Csw.string(propData.helptext),
                    propName = Csw.string(propData.name),
                    labelCell = {};

                layoutTable.addCellSetAttributes(cellSet, { propId: propid });

                if (cswPrivateVar.canDisplayProp(propData, configMode) &&
                    Csw.bool(propData.showpropertyname)) {

                    labelCell = cswPrivateVar.getLabelCell(cellSet);

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
                        labelCell.append(propName);
                    }

                    /* Case 25936 */
                    if (false === Csw.bool(propData.readonly)) {
                        cswPrivateVar.atLeastOne.Saveable = true;
                        if (cswPrivateVar.ShowCheckboxes && Csw.bool(propData.copyable)) {
                            var inpPropCheck = labelCell.input({
                                ID: 'check_' + propid,
                                type: Csw.enums.inputTypes.checkbox,
                                value: false, // Value --not defined?,
                                cssclass: cswPrivateVar.ID + '_check'
                            });
                            inpPropCheck.propNonDom('propid', propid);
                        }
                    }
                }

                var propCell = cswPrivateVar.getPropertyCell(cellSet);
                propCell.addClass('propertyvaluecell');

                if (Csw.bool(propData.highlight)) {
                    propCell.addClass('ui-state-highlight');
                }
                cswPrivateVar.makeProp(propCell, propData, tabContentDiv, tabid, configMode, layoutTable);
            };

            cswPrivateVar.canDisplayProp = function (propData, configMode) {
                /*The prop is set to display or we're in layout config mode*/
                var ret = (Csw.bool(propData.display, true) || configMode);
                /*And either no filter is set or the filter is set to this property */
                ret = ret && (cswPrivateVar.filterToPropId === '' || cswPrivateVar.filterToPropId === propData.id);
                /* We're not excluding any OC Props or this prop has not been excluded */
                ret = ret && ((Csw.isNullOrEmpty(cswPrivateVar.excludeOcProps) || cswPrivateVar.excludeOcProps.length === 0) || false === Csw.contains(cswPrivateVar.excludeOcProps, Csw.string(propData.ocpname).toLowerCase()));
                return ret;
            };

            cswPrivateVar.makeProp = function (propCell, propData, tabContentDiv, tabid, configMode, layoutTable) {
                'use strict';
                propCell.empty();
                if (cswPrivateVar.canDisplayProp(propData, configMode)) {
                    var propId = propData.id;
                    var propName = propData.name;

                    var fieldOpt = {
                        fieldtype: propData.fieldtype,
                        nodeid: Csw.tryParseObjByIdx(cswPrivateVar.nodeids, 0),
                        nodename: cswPrivateVar.nodename,
                        relatednodeid: cswPrivateVar.relatednodeid,
                        relatednodename: cswPrivateVar.relatednodename,
                        relatednodetypeid: cswPrivateVar.relatednodetypeid,
                        relatedobjectclassid: cswPrivateVar.relatedobjectclassid,
                        propid: propId,
                        propDiv: propCell.div(),
                        saveBtn: cswPrivateVar.saveBtn,
                        propData: propData,
                        onChange: function () {
                        },
                        onReload: function (afterReload) {
                            cswPrivateVar.getProps(tabContentDiv, tabid, afterReload);
                        },
                        doSave: function (saveopts) {
                            var s = {
                                onSuccess: null
                            };
                            if (saveopts) $.extend(s, saveopts);
                            cswPublicRet.save(tabContentDiv, tabid, s.onSuccess);
                        },
                        cswnbtnodekey: Csw.tryParseObjByIdx(cswPrivateVar.nodekeys, 0),
                        EditMode: cswPrivateVar.EditMode,
                        Multi: cswPrivateVar.Multi,
                        onEditView: cswPrivateVar.onEditView,
                        onAfterButtonClick: cswPrivateVar.onAfterButtonClick,
                        ReadOnly: Csw.bool(propData.readonly) || cswPrivateVar.Config
                    };
                    fieldOpt.propDiv.propNonDom({
                        'nodeid': fieldOpt.nodeid,
                        'propid': fieldOpt.propid,
                        'cswnbtnodekey': fieldOpt.cswnbtnodekey
                    });

                    fieldOpt.onChange = function () { if (Csw.isFunction(cswPrivateVar.onPropertyChange)) cswPrivateVar.onPropertyChange(fieldOpt.propid, propName); };
                    if (Csw.bool(propData.hassubprops)) {
                        fieldOpt.onChange = function () {
                            cswPrivateVar.updateSubProps(fieldOpt, propId, propData, propCell, tabContentDiv, tabid, false, layoutTable);
                            if (Csw.isFunction(cswPrivateVar.onPropertyChange)) cswPrivateVar.onPropertyChange(fieldOpt.propid, propName);
                        };
                    } // if (Csw.bool(propData.hassubprops)) {
                    $.CswFieldTypeFactory('make', fieldOpt);

                    if (Csw.contains(propData, 'subprops')) {
                        // recurse on sub-props
                        var subProps = propData.subprops;

                        var subLayoutTable = propCell.layoutTable({
                            ID: fieldOpt.propid + '_subproptable',
                            OddCellRightAlign: true,
                            ReadOnly: (cswPrivateVar.EditMode === Csw.enums.editMode.PrintReport || cswPrivateVar.ReadOnly),
                            cellSet: {
                                rows: 1,
                                columns: 2
                            },
                            onSwap: function (e, onSwapData) {
                                cswPrivateVar.onSwap(tabid, onSwapData);
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
                                cswPrivateVar.handleProp(subLayoutTable, subProp, tabContentDiv, tabid, configMode);
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
            };

            // _makeProp()

            cswPrivateVar.updateSubProps = function (fieldOpt, propId, propData, propCell, tabContentDiv, tabid, configMode, layoutTable) {
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
                /*
                Case 24449: 
                $.CswFieldTypeFactory('save') depends on the result of the onChange event which triggers this method.
                Normally, the page is ready when 'Save' is clicked; however, 
                before we can evaluate subprop behavior, the governing controls must update with the result of their change event.
                */
                setTimeout(function () {
                    // do a fake 'save' to update the json with the current value
                    $.CswFieldTypeFactory('save', fieldOpt);

                    if (fieldOpt.propData.wasmodified) {
                        // update the propxml from the server
                        var jsonData = {
                            EditMode: fieldOpt.EditMode,
                            NodeId: Csw.tryParseObjByIdx(cswPrivateVar.nodeids, 0),
                            SafeNodeKey: fieldOpt.cswnbtnodekey,
                            PropId: propId,
                            NodeTypeId: cswPrivateVar.nodetypeid,
                            NewPropJson: JSON.stringify(propData)
                        };

                        Csw.ajax.post({
                            watchGlobal: cswPrivateVar.AjaxWatchGlobal,
                            urlMethod: cswPrivateVar.SinglePropUrlMethod,
                            data: jsonData,
                            success: function (data) {

                                data.wasmodified = true; // keep the fact that the parent property was modified
                                cswPrivateVar.makeProp(propCell, data, tabContentDiv, tabid, configMode, layoutTable);
                            }
                        });
                    }
                }, 150);
            }; // _updateSubProps()

            cswPrivateVar.updatePropJsonFromLayoutTable = function (layoutTable, propData) {
                /// <summary>
                /// Update the prop JSON from the main LayoutTable or from a subProp LayoutTable
                ///</summary>
                /// <param name="layoutTable" type="Csw.composites.layoutTable">(Optional) a layoutTable containing the properties to parse.</param>
                /// <param name="propData" type="Object">(Optional) an object representing CswNbt node properties.</param>
                /// <returns type="Array">An array of propIds</returns>
                'use strict';
                layoutTable = layoutTable || cswPrivateVar.layoutTable;
                propData = propData || cswPrivateVar.propertyData;

                var propIds = [];
                var updSuccess = function (thisProp) {
                    var propOpt = {
                        propData: thisProp,
                        propDiv: '',
                        fieldtype: thisProp.fieldtype,
                        nodeid: Csw.tryParseObjByIdx(cswPrivateVar.nodeids, 0),
                        Multi: cswPrivateVar.Multi,
                        cswnbtnodekey: cswPrivateVar.cswnbtnodekey
                    };

                    var cellSet = layoutTable.cellSet(thisProp.displayrow, thisProp.displaycol);
                    layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
                    propOpt.propCell = cswPrivateVar.getPropertyCell(cellSet);
                    propOpt.propDiv = propOpt.propCell.children('div').first();

                    $.CswFieldTypeFactory('save', propOpt);
                    if (propOpt.propData.wasmodified) {
                        propIds.push(propOpt.propData.id);
                    }

                    // recurse on subprops
                    if (Csw.bool(thisProp.hassubprops) && Csw.contains(thisProp, 'subprops')) {
                        var subProps = thisProp.subprops;
                        if (false === Csw.isNullOrEmpty(subProps)) { //&& $subprops.children('[display != "false"]').length > 0)
                            var subTable = layoutTable[thisProp.id + '_subproptable'];
                            if (false === Csw.isNullOrEmpty(subTable)) {
                                cswPrivateVar.updatePropJsonFromLayoutTable(subTable, subProps);
                            }
                        }
                    }
                    return false;
                };
                Csw.crawlObject(propData, updSuccess, false);
                return propIds;
            }; // updatePropJsonFromLayoutTable()

            cswPrivateVar.enableSaveBtn = function () {
                if (false === Csw.isNullOrEmpty(cswPrivateVar.saveBtn, true)) {
                    cswPrivateVar.saveBtn.enable();
                }
            };

            cswPublicRet.getPropJson = function () {
                cswPrivateVar.updatePropJsonFromLayoutTable();
                return cswPrivateVar.propertyData;
            };

            cswPublicRet.save = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                if (cswPrivateVar.form.$.valid()) {
                    var propIds = cswPrivateVar.updatePropJsonFromLayoutTable();
                    var data = {
                        EditMode: cswPrivateVar.EditMode,
                        NodeIds: cswPrivateVar.nodeids.join(','),
                        SafeNodeKeys: cswPrivateVar.nodekeys.join(','), /* Case 26134. Csw.tryParseObjByIdx(cswPrivateVar.nodekeys, 0) */
                        TabId: tabid,
                        NodeTypeId: cswPrivateVar.nodetypeid,
                        NewPropsJson: JSON.stringify(cswPrivateVar.propertyData),
                        ViewId: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId)
                    };

                    Csw.ajax.post({
                        watchGlobal: cswPrivateVar.AjaxWatchGlobal,
                        urlMethod: cswPrivateVar.SavePropUrlMethod,
                        async: (false === cswPrivateVar.Multi),
                        data: data,
                        success: function (successData) {
                            var doSave = true;
                            var dataJson = {
                                SourceNodeKey: cswPrivateVar.nodekeys.join(','), /* Case 26134. Csw.tryParseObjByIdx(cswPrivateVar.nodekeys, 0) */
                                CopyNodeIds: [],
                                PropIds: []
                            };

                            function copyNodeProps(onSuccess) {
                                Csw.ajax.post({
                                    watchGlobal: cswPrivateVar.AjaxWatchGlobal,
                                    urlMethod: cswPrivateVar.CopyPropValuesUrlMethod,
                                    data: dataJson,
                                    success: function (copy) {
                                        Csw.tryExec(onSuccess, copy);
                                    }
                                }); // ajax						        
                            }

                            if (cswPrivateVar.ShowCheckboxes) {
                                // apply the newly saved checked property values on this node to the checked nodes
                                //var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                                //var nodechecks = $('#' + o.NodeCheckTreeId).CswNodeTree('checkedNodes');
                                var nodechecks = cswPrivateVar.nodeTreeCheck.checkedNodes();
                                var $propchecks = $('.' + cswPrivateVar.ID + '_check:checked');

                                if (nodechecks.length > 0 && $propchecks.length > 0) {
                                    //$nodechecks.each(function () {
                                    Csw.each(nodechecks, function (thisObj) {
                                        //var nodeid = $(this).attr('nodeid');
                                        dataJson.CopyNodeIds.push(thisObj.nodeid);
                                    });

                                    $propchecks.each(function () {
                                        var propid = $(this).attr('propid');
                                        dataJson.PropIds.push(propid);
                                    });
                                    copyNodeProps();
                                } // if($nodechecks.length > 0 && $propchecks.length > 0)
                                else {
                                    doSave = false;
                                    $.CswDialog('AlertDialog', 'You have not selected any properties to save.');
                                }
                            } // if(o.ShowCheckboxes)
                            else if (cswPrivateVar.Multi) {
                                dataJson.CopyNodeIds = cswPrivateVar.nodeids;
                                dataJson.PropIds = propIds;
                                copyNodeProps( /* Case 26134. We're already doing a clear:all, we don't need this. 
                                                function () { Csw.window.location().reload();  } */ ); 
                            }

                            cswPrivateVar.enableSaveBtn();

                            if (doSave) {
                                // reload tab
                                if (cswPrivateVar.ReloadTabOnSave) {
                                    cswPrivateVar.getProps(tabContentDiv, tabid, function () {
                                        Csw.tryExec(cswPrivateVar.onSave, successData.nodeid, successData.cswnbtnodekey, cswPrivateVar.tabcnt, successData.nodename);
                                        Csw.tryExec(onSuccess);
                                    });
                                } else {
                                    // cswPublicRet events
                                    Csw.tryExec(cswPrivateVar.onSave, successData.nodeid, successData.cswnbtnodekey, cswPrivateVar.tabcnt, successData.nodename);
                                    Csw.tryExec(onSuccess);
                                }
                            }

                        }, // success
                        error: cswPrivateVar.enableSaveBtn
                    }); // ajax
                } // if(cswPrivateVar.form.$.valid())
                else {
                    cswPrivateVar.enableSaveBtn();
                }
            }; // Save()

            (function () {
                cswPrivateVar.getTabs(cswPrivateVar.outerTabDiv);

                if (cswPrivateVar.EditMode !== Csw.enums.editMode.PrintReport) {
                    cswPrivateVar.linkDiv = cswParent.div({ ID: cswPrivateVar.ID + '_linkdiv', align: 'right' });
                    if (cswPrivateVar.ShowAsReport && false === cswPrivateVar.Multi) {
                        cswPrivateVar.linkDiv.a({
                            text: 'As Report',
                            onClick: function () {
                                Csw.openPopup('NodeReport.html?nodeid=' + Csw.tryParseObjByIdx(cswPrivateVar.nodeids, 0) + '&cswnbtnodekey=' + Csw.tryParseObjByIdx(cswPrivateVar.nodekeys, 0), 600, 800);
                            }
                        });
                    }
                }
            } ());


            return cswPublicRet;
        });
} ());

