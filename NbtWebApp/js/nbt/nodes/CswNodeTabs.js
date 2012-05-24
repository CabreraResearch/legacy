///// <reference path="~/js/CswNbt-vsdoc.js" />
///// <reference path="~/js/CswCommon-vsdoc.js" />

//(function ($) {
//    "use strict";
//    $.fn.CswNodeTabs = function (options) {
//        "use strict";
//        var o = {
//            ID: '',
//            TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
//            SinglePropUrl: '/NbtWebApp/wsNBT.asmx/getSingleProp',
//            PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
//            MovePropUrl: '/NbtWebApp/wsNBT.asmx/moveProp',
//            RemovePropUrl: '/NbtWebApp/wsNBT.asmx/removeProp',
//            SavePropUrl: '/NbtWebApp/wsNBT.asmx/saveProps',
//            CopyPropValuesUrl: '/NbtWebApp/wsNBT.asmx/copyPropValues',
//            NodePreviewUrl: '/NbtWebApp/wsNBT.asmx/getNodePreview',
//            QuotaUrl: '/NbtWebApp/wsNBT.asmx/checkQuota',
//            nodeids: [],
//            nodepks: [],
//            nodekeys: [],
//            relatednodeid: '',
//            relatednodename: '',
//            relatednodetypeid: '',
//            relatedobjectclassid: '',
//            tabid: '',
//            nodetypeid: '',
//            filterToPropId: '',
//            title: '',
//            date: '',
//            EditMode: Csw.enums.editMode.Edit,
//            Multi: false,
//            ReadOnly: false,
//            onSave: null,
//            ReloadTabOnSave: true,
//            Refresh: null,
//            onBeforeTabSelect: function(){ return true; },
//            onTabSelect: null,
//            onPropertyChange: null,
//            onPropertyRemove: null,
//            onInitFinish: null,
//            ShowCheckboxes: false,
//            ShowAsReport: true,
//            AjaxWatchGlobal: true,
//            nodeTreeCheck: null,
//            onEditView: null,
//            onAfterButtonClick: null,
//            Config: false
//        };

//        if (options) {
//            $.extend(o, options);
//        }

//        var $parent = $(this);
//        var parent = Csw.literals.factory($parent, {});

//        var outerTabDiv = parent.tabDiv({ ID: o.ID + '_tabdiv' });
//        var tabcnt = 0;

//        getTabs(outerTabDiv);

//        if (o.EditMode !== Csw.enums.editMode.PrintReport) {
//            var linkDiv = parent.div({ ID: o.ID + '_linkdiv', align: 'right' });
//            if (o.ShowAsReport && false === o.Multi) {
//                linkDiv.a({
//                    text: 'As Report',
//                    onClick: function () {
//                        Csw.openPopup('NodeReport.html?nodeid=' + Csw.tryParseObjByIdx(o.nodeids, 0) + '&cswnbtnodekey=' + Csw.tryParseObjByIdx(o.nodekeys, 0), 600, 800);
//                    }
//                });
//            }
//        }

//        function clearTabs() {
//            outerTabDiv.empty();
//        }

//        function makeTabContentDiv(tabParent, tabid, canEditLayout) {
//            var tabContentDiv = tabParent.tabDiv({
//                ID: tabid
//            });
//            tabContentDiv.form({
//                ID: tabid + '_form',
//                onsubmit: 'return false;'
//            });

//            var handle = function () {
//                tabParent.empty();
//                Csw.unsubscribe(Csw.enums.events.CswNodeDelete, handle);
//                return false;
//            };

//            if (false === Csw.isNullOrEmpty(tabParent.parent(), true)) {
//                Csw.subscribe(Csw.enums.events.CswNodeDelete, handle);
//            }
//            tabContentDiv.data('canEditLayout', canEditLayout);
//            return tabContentDiv;
//        }

//        function getTabs(tabContentDiv) {
//            var jsonData = {
//                EditMode: o.EditMode,
//                NodeId: Csw.tryParseObjByIdx(o.nodeids, 0),
//                SafeNodeKey: Csw.tryParseObjByIdx(o.nodekeys, 0),
//                NodeTypeId: o.nodetypeid,
//                Date: o.date,
//                filterToPropId: o.filterToPropId,
//                Multi: o.Multi
//            };

//            // For performance, don't bother getting tabs if we're in Add or Preview
//            if (o.EditMode === Csw.enums.editMode.Add ||
//                o.EditMode === Csw.enums.editMode.Preview ||
//                o.EditMode === Csw.enums.editMode.Table) {

//                var tabid = o.EditMode + "_tab";
//                tabContentDiv = makeTabContentDiv(parent, tabid, false);
//                getProps(tabContentDiv, tabid);

//            } else {

//                Csw.ajax.post({
//                    watchGlobal: o.AjaxWatchGlobal,
//                    url: o.TabsUrl,
//                    data: jsonData,
//                    success: function (data) {
//                        clearTabs();
//                        var tabdivs = [];
//                        var selectedtabno = 0;
//                        var tabno = 0;
//                        var tabDiv, tabUl;
//                        o.nodename = data.nodename;
//                        delete data.nodename;
//                        var tabFunc = function (thisTab) {
//                            var thisTabId = thisTab.id;

//                            if (o.EditMode === Csw.enums.editMode.PrintReport || tabdivs.length === 0) {
//                                // For PrintReports, we're going to make a separate tabstrip for each tab
//                                tabDiv = outerTabDiv.tabDiv();
//                                tabUl = tabDiv.ul();
//                                tabdivs[tabdivs.length] = tabDiv;
//                            }
//                            tabDiv = tabDiv || tabdivs[tabdivs.length - 1];
//                            tabUl = tabUl || tabDiv.ul();
//                            tabUl.li().a({ href: '#' + thisTabId, text: thisTab.name });
//                            makeTabContentDiv(tabDiv, thisTabId, thisTab.canEditLayout);
//                            if (thisTabId === o.tabid) {
//                                selectedtabno = tabno;
//                            }
//                            tabno += 1;
//                            return false;
//                        };
//                        Csw.crawlObject(data, tabFunc, false);

//                        tabcnt = tabno;

//                        Csw.each(tabdivs, function (thisTabDiv) {
//                            thisTabDiv.tabs({
//                                selected: selectedtabno,
//                                select: function (event, ui) {
//                                    var ret = false;
//                                    var selectTabContentDiv = thisTabDiv.children('div:eq(' + Csw.number(ui.index) + ')');
//                                    var selectTabid = selectTabContentDiv.getId();
//                                    if (Csw.tryExec(o.onBeforeTabSelect, selectedtabid)) {
//                                        if (false === Csw.isNullOrEmpty(selectTabContentDiv)) {
//                                            getProps(selectTabContentDiv, selectTabid);
//                                            Csw.tryExec(o.onTabSelect, selectTabid);
//                                            ret = true;
//                                        }
//                                    }
//                                    return ret;
//                                } // select()
//                            }); // tabs
//                            var eachTabContentDiv = thisTabDiv.children('div:eq(' + Csw.number(thisTabDiv.tabs('option', 'selected')) + ')');
//                            if (eachTabContentDiv.isValid) {
//                                var selectedtabid = eachTabContentDiv.getId();
//                                getProps(eachTabContentDiv, selectedtabid);
//                                Csw.tryExec(o.onTabSelect, selectedtabid);
//                            }
//                        }); // for(var t in tabdivs)
//                    } // success
//                }); // ajax
//            } // if-else editmode is add or preview
//        } // getTabs()

//        function getProps(tabContentDiv, tabid, onSuccess) {
//            if (o.EditMode === Csw.enums.editMode.Add && o.Config === false) {
//                // case 20970 - make sure there's room in the quota
//                Csw.ajax.post({
//                    watchGlobal: o.AjaxWatchGlobal,
//                    url: o.QuotaUrl,
//                    data: { NodeTypeId: o.nodetypeid },
//                    success: function (data) {
//                        if (Csw.bool(data.result)) {
//                            getPropsImpl(tabContentDiv, tabid, onSuccess);
//                        } else {
//                            tabContentDiv.append('You have used all of your purchased quota, and must purchase additional quota space in order to add more.');
//                            Csw.tryExec(o.onInitFinish, false);
//                        }
//                    }
//                });
//            } else {
//                getPropsImpl(tabContentDiv, tabid, onSuccess);
//            }
//        } // getProps()

//        function getPropsImpl(tabContentDiv, tabid, onSuccess) {
//            var jsonData = {
//                EditMode: o.EditMode,
//                NodeId: Csw.tryParseObjByIdx(o.nodeids, 0),
//                TabId: tabid,
//                SafeNodeKey: Csw.tryParseObjByIdx(o.nodekeys, 0),
//                NodeTypeId: o.nodetypeid,
//                Date: o.date,
//                Multi: o.Multi,
//                filterToPropId: o.filterToPropId,
//                ConfigMode: o.Config
//            };

//            Csw.ajax.post({
//                watchGlobal: o.AjaxWatchGlobal,
//                url: o.PropsUrl,
//                data: jsonData,
//                success: function (data) {
//                    var form = tabContentDiv.children('form');
//                    form.empty();

//                    if (false === Csw.isNullOrEmpty(o.title)) {
//                        form.append(o.title);
//                    }

//                    var formTable = form.table({
//                        ID: o.ID + '_formtbl',
//                        width: '100%'
//                    });
//                    //var formTblCell11 = formTable.cell(1, 1);
//                    //var formTblCell12 = formTable.cell(1, 2);

//                    var saveBtn = {};
//                    var layoutTable = formTable.cell(1, 1).layoutTable({
//                        ID: o.ID + '_props',
//                        OddCellRightAlign: true,
//                        ReadOnly: (o.EditMode === Csw.enums.editMode.PrintReport || o.ReadOnly),
//                        cellSet: {
//                            rows: 1,
//                            columns: 2
//                        },
//                        onSwap: function (e, onSwapData) {
//                            onSwap(onSwapData);
//                        },
//                        showConfigButton: false, //o.Config,
//                        showExpandRowButton: o.Config,
//                        showExpandColButton: (o.Config && o.EditMode !== Csw.enums.editMode.Table),
//                        showRemoveButton: o.Config,
//                        onConfigOn: function () {
//                            doUpdateSubProps(true);
//                        }, // onConfigOn
//                        onConfigOff: function () {
//                            doUpdateSubProps(false);
//                        }, // onConfigOff
//                        onRemove: function (event, onRemoveData) {
//                            onRemove(onRemoveData);
//                        } // onRemove
//                    }); // Csw.literals.layoutTable()

//                    function doUpdateSubProps(configOn) {
//                        var updOnSuccess = function (thisProp, key) {
//                            if (Csw.bool(thisProp.hassubprops)) {
//                                var propId = key; //key
//                                var subTable = layoutTable[propId + '_subproptable'];
//                                var parentCell = subTable.parent().parent();
//                                var cellSet = layoutTable.cellSet(parentCell.propNonDom('row'), parentCell.propNonDom('column'));
//                                layoutTable.addCellSetAttributes(cellSet, { propId: propId });
//                                var propCell = _getPropertyCell(cellSet);

//                                if (subTable.length > 0) {
//                                    var fieldOpt = {
//                                        fieldtype: thisProp.fieldtype,
//                                        nodeid: Csw.tryParseObjByIdx(o.nodeids, 0),
//                                        relatednodeid: o.relatednodeid,
//                                        relatednodename: o.relatednodename,
//                                        relatednodetypeid: o.relatednodetypeid,
//                                        relatedobjectclassid: o.relatedobjectclassid,
//                                        propid: propId,
//                                        propDiv: propCell.children('div'),
//                                        propData: thisProp,
//                                        onChange: function () {
//                                        },
//                                        onReload: function () {
//                                            getProps(tabContentDiv, tabid);
//                                        },
//                                        EditMode: o.EditMode,
//                                        Multi: o.Multi,
//                                        cswnbtnodekey: Csw.tryParseObjByIdx(o.nodekeys, 0)
//                                    };

//                                    _updateSubProps(fieldOpt, propId, thisProp, propCell, tabContentDiv, tabid, configOn, saveBtn, layoutTable, form, data);
//                                }
//                            }
//                            return false;
//                        };
//                        Csw.crawlObject(data, updOnSuccess, false);
//                    }

//                    if (o.EditMode !== Csw.enums.editMode.PrintReport) {
//                        saveBtn = formTable.cell(2, 1).button({ ID: 'SaveTab',
//                            enabledText: 'Save Changes',
//                            disabledText: 'Saving...',
//                            onClick: function () { save(form, layoutTable, data, saveBtn, tabContentDiv, tabid); }
//                        });
//                    }
//                    var atLeastOne = _handleProperties(form, layoutTable, data, tabContentDiv, tabid, false, saveBtn);
//                    if (false === Csw.isNullOrEmpty(layoutTable.cellSet(1, 1)) &&
//                        false === Csw.isNullOrEmpty(layoutTable.cellSet(1, 1)[1][2])) {
//                        layoutTable.cellSet(1, 1)[1][2].trigger('focus');
//                    }
//                    // Validation
//                    form.$.validate({
//                        highlight: function (element) {
//                            var $elm = $(element);
//                            $elm.attr('csw_invalid', '1');
//                            $elm.animate({ backgroundColor: '#ff6666' });
//                        },
//                        unhighlight: function (element) {
//                            var $elm = $(element);
//                            if ($elm.attr('csw_invalid') === '1')  // only unhighlight where we highlighted
//                            {
//                                $elm.css('background-color', '#66ff66');
//                                $elm.attr('csw_invalid', '0');
//                                setTimeout(function () { $elm.animate({ backgroundColor: 'transparent' }); }, 500);
//                            }
//                        }
//                    }); // validate()

//                    if (Csw.bool(o.Config)) {
//                        layoutTable.configOn();
//                    }
//                    else if (!o.Config &&
//                        Csw.isNullOrEmpty(o.date) &&
//                        o.filterToPropId === '' &&
//                        o.EditMode !== Csw.enums.editMode.PrintReport &&
//                        Csw.bool(tabContentDiv.data('canEditLayout'))) {
//                        /* Case 24437 */
//                        var editLayoutOpt = {
//                            ID: o.ID,
//                            nodeids: o.nodeids,
//                            nodekeys: o.nodekeys,
//                            tabid: o.tabid,
//                            nodetypeid: o.nodetypeid,
//                            Refresh: function () {
//                                Csw.tryExec(o.Refresh);
//                                o.Config = false;
//                                getTabs(tabContentDiv);
//                            }
//                        };

//                        /* Show the 'fake' config button to open the dialog */
//                        formTable.cell(1, 2).imageButton({
//                            ButtonType: Csw.enums.imageButton_ButtonType.Configure,
//                            AlternateText: 'Configure',
//                            ID: o.ID + 'configbtn',
//                            onClick: function () {
//                                clearTabs();
//                                $.CswDialog('EditLayoutDialog', editLayoutOpt);
//                            }
//                        });
//                    }

//                    /* case 8494 */
//                    if (!o.Config && !atLeastOne.Saveable && o.EditMode === Csw.enums.editMode.Add) {
//                        save(form, layoutTable, data, saveBtn, tabContentDiv, tabid);
//                    }
//                    else {
//                        Csw.tryExec(o.onInitFinish, atLeastOne.Property);
//                        Csw.tryExec(onSuccess);
//                    }
//                } // success{}
//            }); // ajax
//        } // getPropsImpl()

//        function onRemove(onRemoveData) {
//            var propid = '';
//            var propDiv = _getPropertyCell(onRemoveData.cellSet).children('div');
//            if (false === Csw.isNullOrEmpty(propDiv)) {
//                propid = propDiv.first().propNonDom('propId');
//            }

//            Csw.ajax.post({
//                watchGlobal: o.AjaxWatchGlobal,
//                url: o.RemovePropUrl,
//                data: { PropId: propid, EditMode: o.EditMode },
//                success: function () {
//                    o.onPropertyRemove(propid);
//                }
//            });

//        } // onRemove()

//        function onSwap(onSwapData) {
//            _moveProp(_getPropertyCell(onSwapData.cellSet), onSwapData.swaprow, onSwapData.swapcolumn, onSwapData.cellSet[1][1].propNonDom('propid'));
//            _moveProp(_getPropertyCell(onSwapData.swapcellset), onSwapData.row, onSwapData.column, onSwapData.swapcellset[1][1].propNonDom('propid'));
//        } // onSwap()

//        function _moveProp(propDiv, newrow, newcolumn, propId) {
//            if (propDiv.length() > 0) {
//                var propid = Csw.string(propDiv.propNonDom('propid'), propId);
//                var dataJson = {
//                    PropId: propid,
//                    NewRow: newrow,
//                    NewColumn: newcolumn,
//                    EditMode: o.EditMode
//                };

//                Csw.ajax.post({
//                    watchGlobal: o.AjaxWatchGlobal,
//                    url: o.MovePropUrl,
//                    data: dataJson
//                });
//            }
//        } // _moveProp()

//        function _getLabelCell(cellSet) {
//            return cellSet[1][1].children('div');
//        }
//        function _getPropertyCell(cellSet) {
//            return cellSet[1][2].children('div');
//        }

//        function _handleProperties(form, layoutTable, data, tabContentDiv, tabid, configMode, saveBtn) {
//            var atLeastOne = { Property: false, Saveable: false };
//            var handleSuccess = function (propObj) {
//                atLeastOne.Property = true;
//                handleProp(form, layoutTable, propObj, tabContentDiv, tabid, configMode, saveBtn, atLeastOne, data);
//                return false;
//            };
//            Csw.crawlObject(data, handleSuccess, false);

//            if (false === Csw.isNullOrEmpty(saveBtn, true)) {
//                if (o.Config || (atLeastOne.Saveable === false && o.EditMode != Csw.enums.editMode.Add)) {
//                    saveBtn.hide();
//                } else {
//                    saveBtn.show();
//                }
//            }
//            return atLeastOne;
//        } // _handleProperties()

//        function handleProp(form, layoutTable, thisProp, tabContentDiv, tabid, configMode, savBtn, atLeastOne, propsData) {
//            var propid = thisProp.id,
//                cellSet = layoutTable.cellSet(thisProp.displayrow, thisProp.displaycol),
//                helpText = Csw.string(thisProp.helptext),
//                propName = Csw.string(thisProp.name),
//                labelCell = {};

//            layoutTable.addCellSetAttributes(cellSet, { propId: propid });

//            if ((Csw.bool(thisProp.display, true) || configMode) &&
//                 Csw.bool(thisProp.showpropertyname) &&
//                 (o.filterToPropId === '' || o.filterToPropId === propid)) {

//                labelCell = _getLabelCell(cellSet);

//                labelCell.addClass('propertylabel');
//                if (Csw.bool(thisProp.highlight)) {
//                    labelCell.addClass('ui-state-highlight');
//                }

//                if (false === Csw.isNullOrEmpty(helpText)) {
//                    labelCell.a({
//                        cssclass: 'cswprop_helplink',
//                        title: helpText,
//                        onClick: function () {
//                            return false;
//                        },
//                        value: propName
//                    });

//                } else {
//                    labelCell.append(propName);
//                }
//            }
//            if (false === Csw.bool(thisProp.readonly)) {
//                atLeastOne.Saveable = true;
//                if (o.ShowCheckboxes && Csw.bool(thisProp.copyable)) {
//                    var inpPropCheck = labelCell.input({
//                        ID: 'check_' + propid,
//                        type: Csw.enums.inputTypes.checkbox,
//                        value: false, // Value --not defined?,
//                        cssclass: o.ID + '_check'
//                    });
//                    inpPropCheck.propNonDom('propid', propid);
//                }
//            }


//            var propCell = _getPropertyCell(cellSet);
//            propCell.addClass('propertyvaluecell');

//            if (Csw.bool(thisProp.highlight)) {
//                propCell.addClass('ui-state-highlight');
//            }
//            _makeProp(propCell, thisProp, tabContentDiv, tabid, configMode, savBtn, atLeastOne, layoutTable, form, propsData);
//        }

//        function _makeProp(propCell, propData, tabContentDiv, tabid, configMode, saveBtn, atLeastOne, layoutTable, form, propsData) {
//            propCell.empty();
//            if ((Csw.bool(propData.display, true) || configMode) &&
//                (o.filterToPropId === '' || o.filterToPropId === propData.id)) {

//                var propId = propData.id;
//                var propName = propData.name;

//                var fieldOpt = {
//                    fieldtype: propData.fieldtype,
//                    nodeid: Csw.tryParseObjByIdx(o.nodeids, 0),
//                    nodename: o.nodename,
//                    relatednodeid: o.relatednodeid,
//                    relatednodename: o.relatednodename,
//                    relatednodetypeid: o.relatednodetypeid,
//                    relatedobjectclassid: o.relatedobjectclassid,
//                    propid: propId,
//                    propDiv: propCell.div(),
//                    saveBtn: saveBtn,
//                    propData: propData,
//                    onChange: function () { },
//                    onReload: function () { getProps(tabContentDiv, tabid); },
//                    doSave: function (saveopts) {
//                        var s = {
//                            onSuccess: null
//                        };
//                        if (saveopts) $.extend(s, saveopts);
//                        save(form, layoutTable, propsData, saveBtn, tabContentDiv, tabid, s.onSuccess);
//                    },
//                    cswnbtnodekey: Csw.tryParseObjByIdx(o.nodekeys, 0),
//                    EditMode: o.EditMode,
//                    Multi: o.Multi,
//                    onEditView: o.onEditView,
//                    onAfterButtonClick: o.onAfterButtonClick,
//                    ReadOnly: Csw.bool(propData.readonly) || o.Config
//                };
//                fieldOpt.propDiv.propNonDom({
//                    'nodeid': fieldOpt.nodeid,
//                    'propid': fieldOpt.propid,
//                    'cswnbtnodekey': fieldOpt.cswnbtnodekey
//                });

//                fieldOpt.onChange = function () { if (Csw.isFunction(o.onPropertyChange)) o.onPropertyChange(fieldOpt.propid, propName); };
//                if (Csw.bool(propData.hassubprops)) {
//                    fieldOpt.onChange = function () {
//                        _updateSubProps(fieldOpt, propId, propData, propCell, tabContentDiv, tabid, false, saveBtn, layoutTable, form, propsData);
//                        if (Csw.isFunction(o.onPropertyChange)) o.onPropertyChange(fieldOpt.propid, propName);
//                    };
//                } // if (Csw.bool(propData.hassubprops)) {
//                $.CswFieldTypeFactory('make', fieldOpt);

//                if (Csw.contains(propData, 'subprops')) {
//                    // recurse on sub-props
//                    var subProps = propData.subprops;

//                    var subLayoutTable = propCell.layoutTable({
//                        ID: fieldOpt.propid + '_subproptable',
//                        OddCellRightAlign: true,
//                        ReadOnly: (o.EditMode === Csw.enums.editMode.PrintReport || o.ReadOnly),
//                        cellSet: {
//                            rows: 1,
//                            columns: 2
//                        },
//                        onSwap: function (e, onSwapData) {
//                            onSwap(onSwapData);
//                        },
//                        showConfigButton: false,
//                        showExpandRowButton: false,
//                        showExpandColButton: false,
//                        showRemoveButton: false
//                    });
//                    layoutTable[fieldOpt.propid + '_subproptable'] = subLayoutTable;
//                    var subOnSuccess = function (subProp, key) {
//                        subProp.propId = key;
//                        if (Csw.bool(subProp.display) || configMode) {
//                            handleProp(form, subLayoutTable, subProp, tabContentDiv, tabid, configMode, saveBtn, atLeastOne, propsData);
//                            if (configMode) {
//                                subLayoutTable.configOn();
//                            } else {
//                                subLayoutTable.configOff();
//                            }
//                        }
//                        return false;
//                    };
//                    Csw.crawlObject(subProps, subOnSuccess, true);
//                }
//            } // if (propData.display != 'false' || ConfigMode )
//        } // _makeProp()

//        function _updateSubProps(fieldOpt, propId, propData, propCell, tabContentDiv, tabid, configMode, saveBtn, layoutTable, form, propsData) {
//            /// <summary>Update a properties sub props</summary>
//            /// <param name="fieldOpt" type="Object"> An object defining a prop's fieldtype </param>
//            /// <param name="propId" type="String"> A propertyid </param>
//            /// <param name="propData" type="Object"> Property definition </param>
//            /// <param name="$propcell" type="JQuery"> An element to append to </param>
//            /// <param name="$tabcontentdiv" type="JQuery"> A tab element </param>
//            /// <param name="tabid" type="String"> TabId </param>
//            /// <param name="configMode" type="Boolean"> True if config mode </param>
//            /// <param name="$savebtn" type="JQuery"> A save button </param>
//            /// <returns type="void"></returns>

//            /*
//            Case 24449: 
//            $.CswFieldTypeFactory('save') depends on the result of the onChange event which triggers this method.
//            Normally, the page is ready when 'Save' is clicked; however, 
//            before we can evaluate subprop behavior, the governing controls must update with the result of their change event.
//            */
//            setTimeout(function () {
//                // do a fake 'save' to update the json with the current value
//                $.CswFieldTypeFactory('save', fieldOpt);

//                if (fieldOpt.propData.wasmodified) {
//                    // update the propxml from the server
//                    var jsonData = {
//                        EditMode: fieldOpt.EditMode,
//                        NodeId: Csw.tryParseObjByIdx(o.nodeids, 0),
//                        SafeNodeKey: fieldOpt.cswnbtnodekey,
//                        PropId: propId,
//                        NodeTypeId: o.nodetypeid,
//                        NewPropJson: JSON.stringify(propData)
//                    };

//                    Csw.ajax.post({
//                        watchGlobal: o.AjaxWatchGlobal,
//                        url: o.SinglePropUrl,
//                        data: jsonData,
//                        success: function (data) {
//                            var atLeastOne = {};
//                            data.wasmodified = true; // keep the fact that the parent property was modified
//                            _makeProp(propCell, data, tabContentDiv, tabid, configMode, saveBtn, atLeastOne, layoutTable, form, propsData);
//                        }
//                    });
//                }
//            }, 150);
//        } // _updateSubProps()

//        function save(form, layoutTable, propsData, saveBtn, tabContentDiv, tabid, onSuccess) {
//            if (form.$.valid()) {
//                var propIds = _updatePropJsonFromForm(layoutTable, propsData);
//                var data = {
//                    EditMode: o.EditMode,
//                    NodeIds: Csw.tryParseObjByIdx(o.nodeids, 0), // o.nodeids.join(','),
//                    SafeNodeKeys: Csw.tryParseObjByIdx(o.nodekeys, 0), // o.nodekeys.join(','),
//                    TabId: tabid,
//                    NodeTypeId: o.nodetypeid,
//                    NewPropsJson: JSON.stringify(propsData),
//                    ViewId: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId)
//                };

//                Csw.ajax.post({
//                    watchGlobal: o.AjaxWatchGlobal,
//                    url: o.SavePropUrl,
//                    async: (false === o.Multi),
//                    data: data,
//                    success: function (successData) {
//                        var doSave = true;
//                        var dataJson = {
//                            SourceNodeKey: Csw.tryParseObjByIdx(o.nodekeys, 0),
//                            CopyNodeIds: [],
//                            PropIds: []
//                        };
//                        function copyNodeProps(onSuccess) {
//                            Csw.ajax.post({
//                                watchGlobal: o.AjaxWatchGlobal,
//                                url: o.CopyPropValuesUrl,
//                                data: dataJson,
//                                success: function (copy) {
//                                    Csw.tryExec(onSuccess, copy);
//                                }
//                            }); // ajax						        
//                        }
//                        if (o.ShowCheckboxes) {
//                            // apply the newly saved checked property values on this node to the checked nodes
//                            //var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
//                            //var nodechecks = $('#' + o.NodeCheckTreeId).CswNodeTree('checkedNodes');
//                            var nodechecks = o.nodeTreeCheck.checkedNodes();
//                            var $propchecks = $('.' + o.ID + '_check:checked');

//                            if (nodechecks.length > 0 && $propchecks.length > 0) {
//                                //$nodechecks.each(function () {
//                                Csw.each(nodechecks, function (thisObj) {
//                                    //var nodeid = $(this).attr('nodeid');
//                                    dataJson.CopyNodeIds.push(thisObj.nodeid);
//                                });

//                                $propchecks.each(function () {
//                                    var propid = $(this).attr('propid');
//                                    dataJson.PropIds.push(propid);
//                                });
//                                copyNodeProps();
//                            } // if($nodechecks.length > 0 && $propchecks.length > 0)
//                            else {
//                                doSave = false;
//                                $.CswDialog('AlertDialog', 'You have not selected any properties to save.');
//                            }
//                        } // if(o.ShowCheckboxes)
//                        else if (o.Multi) {
//                            dataJson.CopyNodeIds = o.nodeids;
//                            dataJson.PropIds = propIds;
//                            copyNodeProps(function () { window.location.reload(); });
//                        }

//                        saveBtn.enable();

//                        if (doSave) {
//                            // reload tab
//                            if (o.ReloadTabOnSave) {
//                                getProps(tabContentDiv, tabid, function () {
//                                    Csw.tryExec(o.onSave, successData.nodeid, successData.cswnbtnodekey, tabcnt);
//                                    Csw.tryExec(onSuccess);
//                                });
//                            } else {
//                                // cswPublic events
//                                Csw.tryExec(o.onSave, successData.nodeid, successData.cswnbtnodekey, tabcnt);
//                                Csw.tryExec(onSuccess);
//                            }
//                        }

//                    }, // success
//                    error: function () {
//                        saveBtn.enable();
//                    }
//                }); // ajax
//            } // if($form.valid())
//            else {
//                saveBtn.enable();
//            }
//        } // Save()

//        function _updatePropJsonFromForm(layoutTable, propData) {
//            var propIds = [];
//            var updSuccess = function (thisProp) {
//                var propOpt = {
//                    propData: thisProp,
//                    propDiv: '',
//                    fieldtype: thisProp.fieldtype,
//                    nodeid: Csw.tryParseObjByIdx(o.nodeids, 0),
//                    Multi: o.Multi,
//                    cswnbtnodekey: o.cswnbtnodekey
//                };

//                var cellSet = layoutTable.cellSet(thisProp.displayrow, thisProp.displaycol);
//                layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
//                propOpt.propCell = _getPropertyCell(cellSet);
//                propOpt.propDiv = propOpt.propCell.children('div').first();

//                $.CswFieldTypeFactory('save', propOpt);
//                if (propOpt.propData.wasmodified) {
//                    propIds.push(propOpt.propData.id);
//                }

//                // recurse on subprops
//                if (Csw.bool(thisProp.hassubprops) && Csw.contains(thisProp, 'subprops')) {
//                    var subProps = thisProp.subprops;
//                    if (false === Csw.isNullOrEmpty(subProps)) { //&& $subprops.children('[display != "false"]').length > 0)
//                        var subTable = layoutTable[thisProp.id + '_subproptable'];
//                        if (subTable.length() > 0) {
//                            _updatePropJsonFromForm(subTable, subProps);
//                        }
//                    }
//                }
//                return false;
//            };
//            Csw.crawlObject(propData, updSuccess, false);
//            return propIds;
//        } // _updatePropXmlFromForm()

//        // For proper chaining support
//        return this;

//    }; // function (options) {
//} (jQuery));

