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
            var external = {};

            (function () {
                if (options) {
                    $.extend(internal, options);
                }

                internal.outerTabDiv = cswParent.tabDiv({ ID: internal.ID + '_tabdiv' });
                internal.tabcnt = 0;
            } ());

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
                    Multi: internal.Multi,
                    ConfigMode: internal.Config
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
                    filterToPropId: internal.filterToPropId,
                    ConfigMode: internal.Config
                };

                Csw.ajax.post({
                    watchGlobal: internal.AjaxWatchGlobal,
                    urlMethod: internal.PropsUrlMethod,
                    data: jsonData,
                    success: function (data) {
                        internal.propertyData = data;
                        internal.form = tabContentDiv.children('form');
                        internal.form.empty();

                        if (false === Csw.isNullOrEmpty(internal.title)) {
                            internal.form.append(internal.title);
                        }

                        var formTable = internal.form.table({
                            ID: internal.ID + '_formtbl',
                            width: '100%'
                        });
                        //var formTblCell11 = formTable.cell(1, 1);
                        //var formTblCell12 = formTable.cell(1, 2);


                        internal.layoutTable = formTable.cell(1, 1).layoutTable({
                            ID: internal.ID + '_props',
                            OddCellRightAlign: true,
                            ReadOnly: (internal.EditMode === Csw.enums.editMode.PrintReport || internal.ReadOnly),
                            cellSet: {
                                rows: 1,
                                columns: 2
                            },
                            onSwap: function (e, onSwapData) {
                                internal.onSwap(tabid, onSwapData);
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
                                internal.onRemove(tabid, onRemoveData);
                            } // onRemove
                        }); // Csw.literals.layoutTable()

                        function doUpdateSubProps(configOn) {
                            var updOnSuccess = function (thisProp, key) {
                                if (Csw.bool(thisProp.hassubprops)) {
                                    var propId = key; //key
                                    var subTable = internal.layoutTable[propId + '_subproptable'];
                                    var parentCell = subTable.parent().parent();
                                    var cellSet = internal.layoutTable.cellSet(parentCell.propNonDom('row'), parentCell.propNonDom('column'));
                                    internal.layoutTable.addCellSetAttributes(cellSet, { propId: propId });
                                    var propCell = internal.getPropertyCell(cellSet);

                                    if (subTable.length > 0) {
                                        var fieldOpt = {
                                            fieldtype: thisProp.fieldtype,
                                            nodeid: Csw.tryParseObjByIdx(internal.nodeids, 0),
                                            relatednodeid: internal.relatednodeid,
                                            relatednodename: internal.relatednodename,
                                            relatednodetypeid: internal.relatednodetypeid,
                                            relatedobjectclassid: internal.relatedobjectclassid,
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

                                        internal.updateSubProps(fieldOpt, propId, thisProp, propCell, tabContentDiv, tabid, configOn);
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
                                onClick: function () { external.save(tabContentDiv, tabid); }
                            });
                        }
                        internal.atLeastOne = internal.handleProperties(null, tabContentDiv, tabid, false);
                        if (false === Csw.isNullOrEmpty(internal.layoutTable.cellSet(1, 1)) &&
                            false === Csw.isNullOrEmpty(internal.layoutTable.cellSet(1, 1)[1][2])) {
                            internal.layoutTable.cellSet(1, 1)[1][2].trigger('focus');
                        }
                        // Validation
                        internal.form.$.validate({
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
                            internal.layoutTable.configOn();
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
                            external.save(tabContentDiv, tabid);
                        } else {
                            Csw.tryExec(internal.onInitFinish, internal.atLeastOne.Property);
                            Csw.tryExec(onSuccess);
                        }
                    } // success{}
                }); // ajax
            };

            // getPropsImpl()

            internal.onRemove = function (tabid, onRemoveData) {
                'use strict';
                var propid = '';
                var propDiv = internal.getPropertyCell(onRemoveData.cellSet).children('div');
                if (false === Csw.isNullOrEmpty(propDiv)) {
                    propid = propDiv.first().propNonDom('propId');
                }

                Csw.ajax.post({
                    watchGlobal: internal.AjaxWatchGlobal,
                    urlMethod: internal.RemovePropUrlMethod,
                    data: { PropId: propid, EditMode: internal.EditMode, TabId: tabid },
                    success: function () {
                        internal.onPropertyRemove(propid);
                    }
                });

            };

            // onRemove()

            internal.onSwap = function (tabid, onSwapData) {
                internal.moveProp(internal.getPropertyCell(onSwapData.cellSet), tabid, onSwapData.swaprow, onSwapData.swapcolumn, onSwapData.cellSet[1][1].propNonDom('propid'));
                internal.moveProp(internal.getPropertyCell(onSwapData.swapcellset), tabid, onSwapData.row, onSwapData.column, onSwapData.swapcellset[1][1].propNonDom('propid'));
            };

            // onSwap()

            internal.moveProp = function (propDiv, tabid, newrow, newcolumn, propId) {
                'use strict';
                if (propDiv.length() > 0) {
                    var propid = Csw.string(propDiv.propNonDom('propid'), propId);
                    var dataJson = {
                        PropId: propid,
                        TabId: tabid,
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

            internal.handleProperties = function (layoutTable, tabContentDiv, tabid, configMode) {
                'use strict';
                layoutTable = layoutTable || internal.layoutTable;
                internal.atLeastOne = { Property: false, Saveable: false };
                var handleSuccess = function (propObj) {
                    internal.atLeastOne.Property = true;
                    internal.handleProp(layoutTable, propObj, tabContentDiv, tabid, configMode);
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

            internal.handleProp = function (layoutTable, propData, tabContentDiv, tabid, configMode) {
                'use strict';
                var propid = propData.id,
                    cellSet = layoutTable.cellSet(propData.displayrow, propData.displaycol),
                    helpText = Csw.string(propData.helptext),
                    propName = Csw.string(propData.name),
                    labelCell = {};

                layoutTable.addCellSetAttributes(cellSet, { propId: propid });

                if (internal.canDisplayProp(propData, configMode) &&
                    Csw.bool(propData.showpropertyname)) {

                    labelCell = internal.getLabelCell(cellSet);

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
                        internal.atLeastOne.Saveable = true;
                        if (internal.ShowCheckboxes && Csw.bool(propData.copyable)) {
                            var inpPropCheck = labelCell.input({
                                ID: 'check_' + propid,
                                type: Csw.enums.inputTypes.checkbox,
                                value: false, // Value --not defined?,
                                cssclass: internal.ID + '_check'
                            });
                            inpPropCheck.propNonDom('propid', propid);
                        }
                    }
                }

                var propCell = internal.getPropertyCell(cellSet);
                propCell.addClass('propertyvaluecell');

                if (Csw.bool(propData.highlight)) {
                    propCell.addClass('ui-state-highlight');
                }
                internal.makeProp(propCell, propData, tabContentDiv, tabid, configMode, layoutTable);
            };

            internal.canDisplayProp = function (propData, configMode) {
                /*The prop is set to display or we're in layout config mode*/
                var ret = (Csw.bool(propData.display, true) || configMode);
                /*And either no filter is set or the filter is set to this property */
                ret = ret && (internal.filterToPropId === '' || internal.filterToPropId === propData.id);
                /* We're not excluding any OC Props or this prop has not been excluded */
                ret = ret && ((Csw.isNullOrEmpty(internal.excludeOcProps) || internal.excludeOcProps.length === 0) || false === Csw.contains(internal.excludeOcProps, Csw.string(propData.ocpname).toLowerCase()));
                return ret;
            };

            internal.makeProp = function (propCell, propData, tabContentDiv, tabid, configMode, layoutTable) {
                'use strict';
                propCell.empty();
                if (internal.canDisplayProp(propData, configMode)) {
                    var propId = propData.id;
                    var propName = propData.name;

                    var fieldOpt = {
                        fieldtype: propData.fieldtype,
                        nodeid: Csw.tryParseObjByIdx(internal.nodeids, 0),
                        nodename: internal.nodename,
                        relatednodeid: internal.relatednodeid,
                        relatednodename: internal.relatednodename,
                        relatednodetypeid: internal.relatednodetypeid,
                        relatedobjectclassid: internal.relatedobjectclassid,
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
                            external.save(tabContentDiv, tabid, s.onSuccess);
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
                            internal.updateSubProps(fieldOpt, propId, propData, propCell, tabContentDiv, tabid, false, layoutTable);
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
                                internal.onSwap(tabid, onSwapData);
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
                                internal.handleProp(subLayoutTable, subProp, tabContentDiv, tabid, configMode);
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

            internal.updateSubProps = function (fieldOpt, propId, propData, propCell, tabContentDiv, tabid, configMode, layoutTable) {
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
                                internal.makeProp(propCell, data, tabContentDiv, tabid, configMode, layoutTable);
                            }
                        });
                    }
                }, 150);
            }; // _updateSubProps()

            internal.updatePropJsonFromLayoutTable = function (layoutTable, propData) {
                /// <summary>
                /// Update the prop JSON from the main LayoutTable or from a subProp LayoutTable
                ///</summary>
                /// <param name="layoutTable" type="Csw.composites.layoutTable">(Optional) a layoutTable containing the properties to parse.</param>
                /// <param name="propData" type="Object">(Optional) an object representing CswNbt node properties.</param>
                /// <returns type="Array">An array of propIds</returns>
                'use strict';
                layoutTable = layoutTable || internal.layoutTable;
                propData = propData || internal.propertyData;

                var propIds = [];
                var updSuccess = function (thisProp) {
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
                            if (false === Csw.isNullOrEmpty(subTable)) {
                                internal.updatePropJsonFromLayoutTable(subTable, subProps);
                            }
                        }
                    }
                    return false;
                };
                Csw.crawlObject(propData, updSuccess, false);
                return propIds;
            }; // updatePropJsonFromLayoutTable()

            internal.enableSaveBtn = function () {
                if (false === Csw.isNullOrEmpty(internal.saveBtn, true)) {
                    internal.saveBtn.enable();
                }
            };

            external.getPropJson = function () {
                internal.updatePropJsonFromLayoutTable();
                return internal.propertyData;
            };

            external.save = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                if (internal.form.$.valid()) {
                    var propIds = internal.updatePropJsonFromLayoutTable();
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
                        success: function (successData) {
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
                                    success: function (copy) {
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
                            else if (internal.Multi) {
                                dataJson.CopyNodeIds = internal.nodeids;
                                dataJson.PropIds = propIds;
                                copyNodeProps(function () { window.location.reload(); });
                            }

                            internal.enableSaveBtn();

                            if (doSave) {
                                // reload tab
                                if (internal.ReloadTabOnSave) {
                                    internal.getProps(tabContentDiv, tabid, function () {
                                        Csw.tryExec(internal.onSave, successData.nodeid, successData.cswnbtnodekey, internal.tabcnt, successData.nodename);
                                        Csw.tryExec(onSuccess);
                                    });
                                } else {
                                    // external events
                                    Csw.tryExec(internal.onSave, successData.nodeid, successData.cswnbtnodekey, internal.tabcnt, successData.nodename);
                                    Csw.tryExec(onSuccess);
                                }
                            }

                        }, // success
                        error: internal.enableSaveBtn
                    }); // ajax
                } // if(internal.form.$.valid())
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

