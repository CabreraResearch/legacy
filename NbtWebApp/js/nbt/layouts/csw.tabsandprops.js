/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.layouts.tabsAndProps = Csw.layouts.tabsAndProps ||
        Csw.layouts.register('tabsAndProps', function (cswParent, options) {
            'use strict';
            var internal = {
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
                relatednodetypeid: '',
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
                onBeforeTabSelect: null,
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
                propertyData: {}
            };
            var external = { };
            
            (function() {
                if (options) {
                    $.extend(internal, options);
                }

                internal.outerTabDiv = cswParent.tabDiv({ ID: internal.ID + '_tabdiv' });
                internal.tabcnt = 0;
            }());

            internal.clearTabs = function () {
                internal.outerTabDiv.empty();
            };

            internal.makeTabContentDiv = function (tabParent, tabid, canEditLayout) {
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

            internal.getTabs = function (tabContentDiv) {
                'use strict';
                var jsonData = {
                    EditMode: internal.EditMode,
                    NodeId: Csw.tryParseObjByIdx(internal.nodeids, 0),
                    SafeNodeKey: Csw.tryParseObjByIdx(internal.nodekeys, 0),
                    NodeTypeId: internal.nodetypeid,
                    Date: internal.date,
                    filterToPropId: internal.filterToPropId,
                    Multi: internal.Multi
                };

                // For performance, don't bother getting tabs if we're in Add or Preview
                if (internal.EditMode === Csw.enums.editMode.Add ||
                    internal.EditMode === Csw.enums.editMode.Preview ||
                        internal.EditMode === Csw.enums.editMode.Table) {

                    var tabid = internal.EditMode + "_tab";
                    tabContentDiv = internal.makeTabContentDiv(tabContentDiv, tabid, false);
                    internal.getProps(tabContentDiv, tabid);

                } else {

                    Csw.ajax.post({
                        watchGlobal: internal.AjaxWatchGlobal,
                        urlMethod: internal.TabsUrlMethod,
                        data: jsonData,
                        success: function (data) {
                            internal.clearTabs();
                            var tabdivs = [];
                            var selectedtabno = 0;
                            var tabno = 0;
                            var tabDiv, tabUl;
                            internal.nodename = data.nodename;
                            delete data.nodename;
                            var tabFunc = function (thisTab) {
                                var thisTabId = thisTab.id;

                                if (internal.EditMode === Csw.enums.editMode.PrintReport || tabdivs.length === 0) {
                                    // For PrintReports, we're going to make a separate tabstrip for each tab
                                    tabDiv = internal.outerTabDiv.tabDiv();
                                    tabUl = tabDiv.ul();
                                    tabdivs[tabdivs.length] = tabDiv;
                                }
                                tabDiv = tabDiv || tabdivs[tabdivs.length - 1];
                                tabUl = tabUl || tabDiv.ul();
                                tabUl.li().a({ href: '#' + thisTabId, text: thisTab.name });
                                internal.makeTabContentDiv(tabDiv, thisTabId, thisTab.canEditLayout);
                                if (thisTabId === internal.tabid) {
                                    selectedtabno = tabno;
                                }
                                tabno += 1;
                                return false;
                            };
                            Csw.crawlObject(data, tabFunc, false);

                            internal.tabcnt = tabno;

                            Csw.each(tabdivs, function (thisTabDiv) {
                                thisTabDiv.tabs({
                                    selected: selectedtabno,
                                    select: function (event, ui) {
                                        var ret = false;
                                        var selectTabContentDiv = thisTabDiv.children('div:eq(' + Csw.number(ui.index) + ')');
                                        var selectTabid = selectTabContentDiv.getId();
                                        if (Csw.tryExec(internal.onBeforeTabSelect, selectedtabid)) {
                                            if (false === Csw.isNullOrEmpty(selectTabContentDiv)) {
                                                internal.getProps(selectTabContentDiv, selectTabid);
                                                Csw.tryExec(internal.onTabSelect, selectTabid);
                                                ret = true;
                                            }
                                        }
                                        return ret;
                                    } // select()
                                }); // tabs
                                var eachTabContentDiv = thisTabDiv.children('div:eq(' + Csw.number(thisTabDiv.tabs('option', 'selected')) + ')');
                                if (eachTabContentDiv.isValid) {
                                    var selectedtabid = eachTabContentDiv.getId();
                                    internal.getProps(eachTabContentDiv, selectedtabid);
                                    Csw.tryExec(internal.onTabSelect, selectedtabid);
                                }
                            }); // for(var t in tabdivs)
                        } // success
                    }); // ajax
                } // if-else editmode is add or preview
            };

            // getTabs()

            internal.getProps = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                if (internal.EditMode === Csw.enums.editMode.Add && internal.Config === false) {
                    // case 20970 - make sure there's room in the quota
                    Csw.ajax.post({
                        watchGlobal: internal.AjaxWatchGlobal,
                        urlMethod: internal.QuotaUrlMethod,
                        data: { NodeTypeId: internal.nodetypeid },
                        success: function (data) {
                            if (Csw.bool(data.result)) {
                                internal.getPropsImpl(tabContentDiv, tabid, onSuccess);
                            } else {
                                tabContentDiv.append('You have used all of your purchased quota, and must purchase additional quota space in order to add more.');
                                Csw.tryExec(internal.onInitFinish, false);
                            }
                        }
                    });
                } else {
                    internal.getPropsImpl(tabContentDiv, tabid, onSuccess);
                }
            };

            // getProps()

            internal.getPropsImpl = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                var jsonData = {
                    EditMode: internal.EditMode,
                    NodeId: Csw.tryParseObjByIdx(internal.nodeids, 0),
                    TabId: tabid,
                    SafeNodeKey: Csw.tryParseObjByIdx(internal.nodekeys, 0),
                    NodeTypeId: internal.nodetypeid,
                    Date: internal.date,
                    Multi: internal.Multi,
                    filterToPropId: internal.filterToPropId
                };

                Csw.ajax.post({
                    watchGlobal: internal.AjaxWatchGlobal,
                    urlMethod: internal.PropsUrlMethod,
                    data: jsonData,
                    success: function (data) {
                        internal.propertyData = data;
                        var form = tabContentDiv.children('form');
                        form.empty();

                        if (false === Csw.isNullOrEmpty(internal.title)) {
                            form.append(internal.title);
                        }

                        var formTable = form.table({
                            ID: internal.ID + '_formtbl',
                            width: '100%'
                        });
                        //var formTblCell11 = formTable.cell(1, 1);
                        //var formTblCell12 = formTable.cell(1, 2);

                        
                        var layoutTable = formTable.cell(1, 1).layoutTable({
                            ID: internal.ID + '_props',
                            OddCellRightAlign: true,
                            ReadOnly: (internal.EditMode === Csw.enums.editMode.PrintReport || internal.ReadOnly),
                            cellSet: {
                                rows: 1,
                                columns: 2
                            },
                            onSwap: function (e, onSwapData) {
                                internal.onSwap(onSwapData);
                            },
                            showConfigButton: false, //o.Config,
                            showExpandRowButton: internal.Config,
                            showExpandColButton: (internal.Config && internal.EditMode !== Csw.enums.editMode.Table),
                            showRemoveButton: internal.Config,
                            onConfigOn: function () {
                                doUpdateSubProps(true);
                            }, // onConfigOn
                            onConfigOff: function () {
                                doUpdateSubProps(false);
                            }, // onConfigOff
                            onRemove: function (event, onRemoveData) {
                                internal.onRemove(onRemoveData);
                            } // onRemove
                        }); // Csw.literals.layoutTable()

                        function doUpdateSubProps(configOn) {
                            var updOnSuccess = function (thisProp, key) {
                                if (Csw.bool(thisProp.hassubprops)) {
                                    var propId = key; //key
                                    var subTable = layoutTable[propId + '_subproptable'];
                                    var parentCell = subTable.parent().parent();
                                    var cellSet = layoutTable.cellSet(parentCell.propNonDom('row'), parentCell.propNonDom('column'));
                                    layoutTable.addCellSetAttributes(cellSet, { propId: propId });
                                    var propCell = internal.getPropertyCell(cellSet);

                                    if (subTable.length > 0) {
                                        var fieldOpt = {
                                            fieldtype: thisProp.fieldtype,
                                            nodeid: Csw.tryParseObjByIdx(internal.nodeids, 0),
                                            relatednodeid: internal.relatednodeid,
                                            relatednodetypeid: internal.relatednodetypeid,
                                            propid: propId,
                                            propDiv: propCell.children('div'),
                                            propData: thisProp,
                                            onChange: function () {
                                            },
                                            onReload: function () {
                                                internal.getProps(tabContentDiv, tabid);
                                            },
                                            EditMode: internal.EditMode,
                                            Multi: internal.Multi,
                                            cswnbtnodekey: Csw.tryParseObjByIdx(internal.nodekeys, 0)
                                        };

                                        internal.updateSubProps(fieldOpt, propId, thisProp, propCell, tabContentDiv, tabid, configOn, layoutTable, form, internal.propertyData);
                                    }
                                }
                                return false;
                            };
                            Csw.crawlObject(internal.propertyData, updOnSuccess, false);
                        }

                        if (internal.EditMode !== Csw.enums.editMode.PrintReport && Csw.bool(internal.showSaveButton)) {
                            internal.saveBtn = formTable.cell(2, 1).button({
                                ID: 'SaveTab',
                                enabledText: 'Save Changes',
                                disabledText: 'Saving...',
                                onClick: function () { external.save(form, layoutTable, tabContentDiv, tabid); }
                            });
                        }
                        internal.atLeastOne = internal.handleProperties(form, layoutTable, tabContentDiv, tabid, false);
                        if (false === Csw.isNullOrEmpty(layoutTable.cellSet(1, 1)) &&
                            false === Csw.isNullOrEmpty(layoutTable.cellSet(1, 1)[1][2])) {
                            layoutTable.cellSet(1, 1)[1][2].trigger('focus');
                        }
                        // Validation
                        form.$.validate({
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

                        if (Csw.bool(internal.Config)) {
                            layoutTable.configOn();
                        } else if (!internal.Config &&
                            Csw.isNullOrEmpty(internal.date) &&
                                internal.filterToPropId === '' &&
                                    internal.EditMode !== Csw.enums.editMode.PrintReport &&
                                        Csw.bool(tabContentDiv.data('canEditLayout'))) {
                            /* Case 24437 */
                            var editLayoutOpt = {
                                ID: internal.ID,
                                nodeids: internal.nodeids,
                                nodekeys: internal.nodekeys,
                                tabid: internal.tabid,
                                nodetypeid: internal.nodetypeid,
                                Refresh: function () {
                                    Csw.tryExec(internal.Refresh);
                                    internal.Config = false;
                                    internal.getTabs(tabContentDiv);
                                }
                            };

                            /* Show the 'fake' config button to open the dialog */
                            formTable.cell(1, 2).imageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Configure,
                                AlternateText: 'Configure',
                                ID: internal.ID + 'configbtn',
                                onClick: function () {
                                    internal.clearTabs();
                                    $.CswDialog('EditLayoutDialog', editLayoutOpt);
                                }
                            });
                        }

                        /* case 8494 */
                        if (!internal.Config && !internal.atLeastOne.Saveable && internal.EditMode === Csw.enums.editMode.Add) {
                            external.save(form, layoutTable, tabContentDiv, tabid);
                        } else {
                            Csw.tryExec(internal.onInitFinish, internal.atLeastOne.Property);
                            Csw.tryExec(onSuccess);
                        }
                    } // success{}
                }); // ajax
            };

            // getPropsImpl()

            internal.onRemove = function (onRemoveData) {
                'use strict';
                var propid = '';
                var propDiv = internal.getPropertyCell(onRemoveData.cellSet).children('div');
                if (false === Csw.isNullOrEmpty(propDiv)) {
                    propid = propDiv.first().propNonDom('propId');
                }

                Csw.ajax.post({
                    watchGlobal: internal.AjaxWatchGlobal,
                    urlMethod: internal.RemovePropUrlMethod,
                    data: { PropId: propid, EditMode: internal.EditMode },
                    success: function () {
                        internal.onPropertyRemove(propid);
                    }
                });

            };

            // onRemove()

            internal.onSwap = function (onSwapData) {
                internal.moveProp(internal.getPropertyCell(onSwapData.cellSet), onSwapData.swaprow, onSwapData.swapcolumn, onSwapData.cellSet[1][1].propNonDom('propid'));
                internal.moveProp(internal.getPropertyCell(onSwapData.swapcellset), onSwapData.row, onSwapData.column, onSwapData.swapcellset[1][1].propNonDom('propid'));
            };

            // onSwap()

            internal.moveProp = function (propDiv, newrow, newcolumn, propId) {
                'use strict';
                if (propDiv.length() > 0) {
                    var propid = Csw.string(propDiv.propNonDom('propid'), propId);
                    var dataJson = {
                        PropId: propid,
                        NewRow: newrow,
                        NewColumn: newcolumn,
                        EditMode: internal.EditMode
                    };

                    Csw.ajax.post({
                        watchGlobal: internal.AjaxWatchGlobal,
                        urlMethod: internal.MovePropUrlMethod,
                        data: dataJson
                    });
                }
            };

            // _moveProp()

            internal.getLabelCell = function (cellSet) {
                return cellSet[1][1].children('div');
            };

            internal.getPropertyCell = function (cellSet) {
                return cellSet[1][2].children('div');
            };

            internal.handleProperties = function (form, layoutTable, tabContentDiv, tabid, configMode) {
                'use strict';
                internal.atLeastOne = { Property: false, Saveable: false };
                var handleSuccess = function (propObj) {
                    internal.atLeastOne.Property = true;
                    internal.handleProp(form, layoutTable, propObj, tabContentDiv, tabid, configMode);
                    return false;
                };
                Csw.crawlObject(internal.propertyData, handleSuccess, false);

                if (false === Csw.isNullOrEmpty(internal.saveBtn, true)) {
                    if (internal.Config || (internal.atLeastOne.Saveable === false && internal.EditMode != Csw.enums.editMode.Add)) {
                        internal.saveBtn.hide();
                    } else {
                        internal.saveBtn.show();
                    }
                }
                return internal.atLeastOne;
            };

            // _handleProperties()

            internal.handleProp = function (form, layoutTable, thisProp, tabContentDiv, tabid, configMode) {
                'use strict';
                var propid = thisProp.id,
                    cellSet = layoutTable.cellSet(thisProp.displayrow, thisProp.displaycol),
                    helpText = Csw.string(thisProp.helptext),
                    propName = Csw.string(thisProp.name),
                    labelCell = {};

                layoutTable.addCellSetAttributes(cellSet, { propId: propid });

                if ((Csw.bool(thisProp.display, true) || configMode) &&
                    Csw.bool(thisProp.showpropertyname) &&
                        (internal.filterToPropId === '' || internal.filterToPropId === propid)) {

                    labelCell = internal.getLabelCell(cellSet);

                    labelCell.addClass('propertylabel');
                    if (Csw.bool(thisProp.highlight)) {
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
                }
                if (false === Csw.bool(thisProp.readonly)) {
                    internal.atLeastOne.Saveable = true;
                    if (internal.ShowCheckboxes && Csw.bool(thisProp.copyable)) {
                        var inpPropCheck = labelCell.input({
                            ID: 'check_' + propid,
                            type: Csw.enums.inputTypes.checkbox,
                            value: false, // Value --not defined?,
                            cssclass: internal.ID + '_check'
                        });
                        inpPropCheck.propNonDom('propid', propid);
                    }
                }


                var propCell = internal.getPropertyCell(cellSet);
                propCell.addClass('propertyvaluecell');

                if (Csw.bool(thisProp.highlight)) {
                    propCell.addClass('ui-state-highlight');
                }
                internal.makeProp(propCell, thisProp, tabContentDiv, tabid, configMode, layoutTable, form);
            };

            internal.makeProp = function (propCell, propData, tabContentDiv, tabid, configMode, layoutTable, form) {
                'use strict';
                propCell.empty();
                if ((Csw.bool(propData.display, true) || configMode) &&
                    (internal.filterToPropId === '' || internal.filterToPropId === propData.id)) {

                    var propId = propData.id;
                    var propName = propData.name;

                    var fieldOpt = {
                        fieldtype: propData.fieldtype,
                        nodeid: Csw.tryParseObjByIdx(internal.nodeids, 0),
                        nodename: internal.nodename,
                        relatednodeid: internal.relatednodeid,
                        relatednodetypeid: internal.relatednodetypeid,
                        propid: propId,
                        propDiv: propCell.div(),
                        saveBtn: internal.saveBtn,
                        propData: propData,
                        onChange: function () {
                        },
                        onReload: function () { internal.getProps(tabContentDiv, tabid); },
                        doSave: function (saveopts) {
                            var s = {
                                onSuccess: null
                            };
                            if (saveopts) $.extend(s, saveopts);
                            external.save(form, layoutTable, tabContentDiv, tabid, s.onSuccess);
                        },
                        cswnbtnodekey: Csw.tryParseObjByIdx(internal.nodekeys, 0),
                        EditMode: internal.EditMode,
                        Multi: internal.Multi,
                        onEditView: internal.onEditView,
                        onAfterButtonClick: internal.onAfterButtonClick,
                        ReadOnly: Csw.bool(propData.readonly) || internal.Config
                    };
                    fieldOpt.propDiv.propNonDom({
                        'nodeid': fieldOpt.nodeid,
                        'propid': fieldOpt.propid,
                        'cswnbtnodekey': fieldOpt.cswnbtnodekey
                    });

                    fieldOpt.onChange = function () { if (Csw.isFunction(internal.onPropertyChange)) internal.onPropertyChange(fieldOpt.propid, propName); };
                    if (Csw.bool(propData.hassubprops)) {
                        fieldOpt.onChange = function () {
                            internal.updateSubProps(fieldOpt, propId, propData, propCell, tabContentDiv, tabid, false, layoutTable, form);
                            if (Csw.isFunction(internal.onPropertyChange)) internal.onPropertyChange(fieldOpt.propid, propName);
                        };
                    } // if (Csw.bool(propData.hassubprops)) {
                    $.CswFieldTypeFactory('make', fieldOpt);

                    if (Csw.contains(propData, 'subprops')) {
                        // recurse on sub-props
                        var subProps = propData.subprops;

                        var subLayoutTable = propCell.layoutTable({
                            ID: fieldOpt.propid + '_subproptable',
                            OddCellRightAlign: true,
                            ReadOnly: (internal.EditMode === Csw.enums.editMode.PrintReport || internal.ReadOnly),
                            cellSet: {
                                rows: 1,
                                columns: 2
                            },
                            onSwap: function (e, onSwapData) {
                                internal.onSwap(onSwapData);
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
                                internal.handleProp(form, subLayoutTable, subProp, tabContentDiv, tabid, configMode);
                                if (configMode) {
                                    subLayoutTable.configOn();
                                } else {
                                    subLayoutTable.configOff();
                                }
                            }
                            return false;
                        };
                        Csw.crawlObject(subProps, subOnSuccess, true);
                    }
                } // if (propData.display != 'false' || ConfigMode )
            };

            // _makeProp()

            internal.updateSubProps = function (fieldOpt, propId, propData, propCell, tabContentDiv, tabid, configMode, layoutTable, form) {
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
                            NodeId: Csw.tryParseObjByIdx(internal.nodeids, 0),
                            SafeNodeKey: fieldOpt.cswnbtnodekey,
                            PropId: propId,
                            NodeTypeId: internal.nodetypeid,
                            NewPropJson: JSON.stringify(propData)
                        };

                        Csw.ajax.post({
                            watchGlobal: internal.AjaxWatchGlobal,
                            urlMethod: internal.SinglePropUrlMethod,
                            data: jsonData,
                            success: function (data) {
                                
                                data.wasmodified = true; // keep the fact that the parent property was modified
                                internal.makeProp(propCell, data, tabContentDiv, tabid, configMode, layoutTable, form);
                            }
                        });
                    }
                }, 150);
            }; // _updateSubProps()
            
            internal.updatePropJsonFromForm = function(layoutTable, propData) {
                'use strict';
                var propIds = [];
                var updSuccess = function(thisProp) {
                    var propOpt = {
                        propData: thisProp,
                        propDiv: '',
                        fieldtype: thisProp.fieldtype,
                        nodeid: Csw.tryParseObjByIdx(internal.nodeids, 0),
                        Multi: internal.Multi,
                        cswnbtnodekey: internal.cswnbtnodekey
                    };

                    var cellSet = layoutTable.cellSet(thisProp.displayrow, thisProp.displaycol);
                    layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
                    propOpt.propCell = internal.getPropertyCell(cellSet);
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
                            if (subTable.length() > 0) {
                                internal.updatePropJsonFromForm(subTable, subProps) ;
                            }
                        }
                    }
                    return false;
                };
                Csw.crawlObject(propData, updSuccess, false);
                return propIds;
            }; // _updatePropXmlFromForm()

            internal.enableSaveBtn = function() {
                if(false === Csw.isNullOrEmpty(internal.saveBtn, true)) {
                    internal.saveBtn.enable();
                }
            };

            external.save = function(form, layoutTable, tabContentDiv, tabid, onSuccess) {
                'use strict';
                if (form.$.valid()) {
                    var propIds = internal.updatePropJsonFromForm(layoutTable, internal.propertyData);
                    var data = {
                        EditMode: internal.EditMode,
                        NodeIds: Csw.tryParseObjByIdx(internal.nodeids, 0), // o.nodeids.join(','),
                        SafeNodeKeys: Csw.tryParseObjByIdx(internal.nodekeys, 0), // o.nodekeys.join(','),
                        TabId: tabid,
                        NodeTypeId: internal.nodetypeid,
                        NewPropsJson: JSON.stringify(internal.propertyData),
                        ViewId: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId)
                    };

                    Csw.ajax.post({
                        watchGlobal: internal.AjaxWatchGlobal,
                        urlMethod: internal.SavePropUrlMethod,
                        async: (false === internal.Multi),
                        data: data,
                        success: function(successData) {
                            var doSave = true;
                            var dataJson = {
                                SourceNodeKey: Csw.tryParseObjByIdx(internal.nodekeys, 0),
                                CopyNodeIds: [],
                                PropIds: []
                            };

                            function copyNodeProps(onSuccess) {
                                Csw.ajax.post({
                                    watchGlobal: internal.AjaxWatchGlobal,
                                    urlMethod: internal.CopyPropValuesUrlMethod,
                                    data: dataJson,
                                    success: function(copy) {
                                        Csw.tryExec(onSuccess, copy);
                                    }
                                }); // ajax						        
                            }

                            if (internal.ShowCheckboxes) {
                                // apply the newly saved checked property values on this node to the checked nodes
                                //var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                                //var nodechecks = $('#' + o.NodeCheckTreeId).CswNodeTree('checkedNodes');
                                var nodechecks = internal.nodeTreeCheck.checkedNodes();
                                var $propchecks = $('.' + internal.ID + '_check:checked');

                                if (nodechecks.length > 0 && $propchecks.length > 0) {
                                    //$nodechecks.each(function () {
                                    Csw.each(nodechecks, function(thisObj) {
                                        //var nodeid = $(this).attr('nodeid');
                                        dataJson.CopyNodeIds.push(thisObj.nodeid);
                                    });

                                    $propchecks.each(function() {
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
                            else if (internal.Multi) {
                                dataJson.CopyNodeIds = internal.nodeids;
                                dataJson.PropIds = propIds;
                                copyNodeProps(function() { window.location.reload(); });
                            }

                            internal.enableSaveBtn();

                            if (doSave) {
                                // reload tab
                                if (internal.ReloadTabOnSave) {
                                    internal.getProps(tabContentDiv, tabid, function() {
                                        Csw.tryExec(internal.onSave, successData.nodeid, successData.cswnbtnodekey, internal.tabcnt);
                                        Csw.tryExec(onSuccess);
                                    });
                                } else {
                                    // external events
                                    Csw.tryExec(internal.onSave, successData.nodeid, successData.cswnbtnodekey, internal.tabcnt);
                                    Csw.tryExec(onSuccess);
                                }
                            }

                        }, // success
                        error: internal.enableSaveBtn
                    }); // ajax
                } // if($form.valid())
                else {
                    internal.enableSaveBtn();
                }
            }; // Save()

            (function () {
                internal.getTabs(internal.outerTabDiv);

                if (internal.EditMode !== Csw.enums.editMode.PrintReport) {
                    internal.linkDiv = cswParent.div({ ID: internal.ID + '_linkdiv', align: 'right' });
                    if (internal.ShowAsReport && false === internal.Multi) {
                        internal.linkDiv.a({
                            text: 'As Report',
                            onClick: function () {
                                Csw.openPopup('NodeReport.html?nodeid=' + Csw.tryParseObjByIdx(internal.nodeids, 0) + '&cswnbtnodekey=' + Csw.tryParseObjByIdx(internal.nodekeys, 0), 600, 800);
                            }
                        });
                    }
                }
            } ());


            return external;
        }); 
} ());

