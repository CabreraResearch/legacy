/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.layouts.tabsAndProps = Csw.layouts.tabsAndProps ||
        Csw.layouts.register('tabsAndProps', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
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
                onNodeIdSet: null,
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
                propertyData: null,
                excludeOcProps: [],
                async: true
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                cswPrivate.outerTabDiv = cswParent.tabDiv({ ID: cswPrivate.ID + '_tabdiv' });
                cswPrivate.tabcnt = 0;
            } ());

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

            cswPrivate.getTabs = function (tabContentDiv) {
                'use strict';
                // For performance, don't bother getting tabs if we're in Add, Temp, Preview or Table
                if (cswPrivate.EditMode === Csw.enums.editMode.Add ||
                    cswPrivate.EditMode === Csw.enums.editMode.Temp ||
                    cswPrivate.EditMode === Csw.enums.editMode.Preview ||
                        cswPrivate.EditMode === Csw.enums.editMode.Table) {

                    var tabid = cswPrivate.EditMode + "_tab";
                    tabContentDiv = cswPrivate.makeTabContentDiv(tabContentDiv, tabid, false);
                    cswPrivate.getProps(tabContentDiv, tabid);

                } else {

                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.TabsUrlMethod,
                        data: {
                            EditMode: cswPrivate.EditMode,
                            NodeId: Csw.tryParseObjByIdx(cswPrivate.nodeids, 0),
                            SafeNodeKey: Csw.tryParseObjByIdx(cswPrivate.nodekeys, 0),
                            NodeTypeId: Csw.string(cswPrivate.nodetypeid),
                            Date: cswPrivate.date,
                            filterToPropId: Csw.string(cswPrivate.filterToPropId),
                            Multi: Csw.bool(cswPrivate.Multi),
                            ConfigMode: cswPrivate.Config
                        },
                        success: function (data) {
                            cswPrivate.clearTabs();
                            var tabdivs = [];
                            var selectedtabno = 0;
                            var tabno = 0;
                            var tabDiv, tabUl;
                            cswPrivate.nodename = data.nodename;
                            delete data.nodename;
                            var tabFunc = function (thisTab) {
                                var thisTabId = thisTab.id;

                                if (cswPrivate.EditMode === Csw.enums.editMode.PrintReport || tabdivs.length === 0) {
                                    // For PrintReports, we're going to make a separate tabstrip for each tab
                                    tabDiv = cswPrivate.outerTabDiv.tabDiv();
                                    tabUl = tabDiv.ul();
                                    tabdivs[tabdivs.length] = tabDiv;
                                }
                                tabDiv = tabDiv || tabdivs[tabdivs.length - 1];
                                tabUl = tabUl || tabDiv.ul();
                                tabUl.li().a({ href: '#' + thisTabId, text: thisTab.name });
                                cswPrivate.makeTabContentDiv(tabDiv, thisTabId, thisTab.canEditLayout);
                                if (thisTabId === cswPrivate.tabid) {
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

            cswPrivate.getProps = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                if (cswPrivate.EditMode === Csw.enums.editMode.Add && cswPrivate.Config === false) {
                    // case 20970 - make sure there's room in the quota
                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.QuotaUrlMethod,
                        data: {
                            NodeTypeId: cswPrivate.nodetypeid,
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

            cswPrivate.getPropsImpl = function (tabContentDiv, tabid, onSuccess) {
                'use strict';
                cswPrivate.tabgrouptables = [];  // case 26957, 27117
                
                function makePropLayout() {
                    cswPrivate.form = tabContentDiv.children('form');
                    cswPrivate.form.empty();

                    if (false === Csw.isNullOrEmpty(cswPrivate.title)) {
                        cswPrivate.form.append(cswPrivate.title);
                    }

                    var formTable = cswPrivate.form.table({
                        ID: cswPrivate.ID + '_formtbl_' + tabid + window.Ext.id(),
                        width: '100%'
                    });
                    //var formTblCell11 = formTable.cell(1, 1);
                    //var formTblCell12 = formTable.cell(1, 2);
                    var layoutOpts = {
                        ID: cswPrivate.ID + '_props_' + tabid,
                        OddCellRightAlign: true,
                        ReadOnly: (cswPrivate.EditMode === Csw.enums.editMode.PrintReport || cswPrivate.ReadOnly),
                        cellSet: {
                            rows: 1,
                            columns: 2
                        },
                        onSwap: function (e, onSwapData) {
                            cswPrivate.onSwap(tabid, onSwapData);
                        },
                        showConfigButton: false, //o.Config,
                        showExpandRowButton: cswPrivate.Config,
                        showExpandColButton: (cswPrivate.Config && (cswPrivate.EditMode !== Csw.enums.editMode.Table && cswPrivate.EditMode !== Csw.enums.editMode.Temp)),
                        showRemoveButton: cswPrivate.Config,
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

                    cswPrivate.layoutTable = formTable.cell(1, 1).layoutTable(layoutOpts); // Csw.literals.layoutTable()

                    function doUpdateSubProps(configOn) {
                        var updOnSuccess = function (thisProp, key) {
                            if (false === Csw.isNullOrEmpty(thisProp) &&
                                false === Csw.isNullOrEmpty(key) &&
                                Csw.bool(thisProp.hassubprops)) {
                                var propId = key; //key
                                var subTable = cswPrivate.layoutTable[thisProp.id + '_subproptable'];
                                //var parentCell = subTable.table.parent().parent();
                                var parentCell = Csw.literals.factory(subTable.table.$.parent().parent().parent());
                                //var cellSet = cswPrivate.layoutTable.cellSet(parentCell.propNonDom('row'), parentCell.propNonDom('column'));
                                var cellSet = cswPrivate.getCellSet(cswPrivate.layoutTable, thisProp.tabgroup, parentCell.propNonDom('row'), parentCell.propNonDom('column'));

                                cswPrivate.layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
                                var propCell = cswPrivate.getPropertyCell(cellSet);

                                if (subTable.length > 0) {
                                    var fieldOpt = {
                                        fieldtype: thisProp.fieldtype,
                                        nodeid: Csw.tryParseObjByIdx(cswPrivate.nodeids, 0),
                                        relatednodeid: cswPrivate.relatednodeid,
                                        relatednodename: cswPrivate.relatednodename,
                                        relatednodetypeid: cswPrivate.relatednodetypeid,
                                        relatedobjectclassid: cswPrivate.relatedobjectclassid,
                                        propid: thisProp.id,
                                        propDiv: propCell.children('div'),
                                        propData: thisProp,
                                        onChange: function () {
                                        },
                                        onReload: function (afterReload) {
                                            cswPrivate.getProps(tabContentDiv, tabid, afterReload);
                                        },
                                        EditMode: cswPrivate.EditMode,
                                        Multi: cswPrivate.Multi,
                                        cswnbtnodekey: Csw.tryParseObjByIdx(cswPrivate.nodekeys, 0)
                                    };

                                    cswPrivate.updateSubProps(fieldOpt, thisProp.id, thisProp, propCell, tabContentDiv, tabid, configOn);
                                }
                            }
                            return false;
                        };
                        Csw.crawlObject(cswPrivate.propertyData, updOnSuccess, false);
                    }

                    if (cswPrivate.EditMode !== Csw.enums.editMode.PrintReport && Csw.bool(cswPrivate.showSaveButton)) {
                        cswPrivate.saveBtn = formTable.cell(2, 1).buttonExt({
                            ID: 'SaveTab' + window.Ext.id,
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

                    if (Csw.bool(cswPrivate.Config)) {
                        cswPrivate.layoutTable.configOn();
                    } else if (!cswPrivate.Config &&
                            Csw.isNullOrEmpty(cswPrivate.date) &&
                                cswPrivate.filterToPropId === '' &&
                                    cswPrivate.EditMode !== Csw.enums.editMode.PrintReport &&
                                        Csw.bool(tabContentDiv.data('canEditLayout'))) {
                        /* Case 24437 */
                        var editLayoutOpt = {
                            ID: cswPrivate.ID,
                            nodeids: cswPrivate.nodeids,
                            nodekeys: cswPrivate.nodekeys,
                            tabid: cswPrivate.tabid,
                            nodetypeid: cswPrivate.nodetypeid,
                            Refresh: function () {
                                Csw.tryExec(cswPrivate.Refresh);
                                cswPrivate.Config = false;
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
                    if (!cswPrivate.Config && !cswPrivate.atLeastOne.Saveable && cswPrivate.EditMode === Csw.enums.editMode.Add) {
                        cswPublic.save(tabContentDiv, tabid);
                    } else {
                        Csw.tryExec(cswPrivate.onInitFinish, cswPrivate.atLeastOne.Property);
                        Csw.tryExec(onSuccess);
                    }
                }

                if (Csw.isNullOrEmpty(cswPrivate.propertyData) ||
                    (cswPrivate.EditMode !== Csw.enums.editMode.Add &&
                    cswPrivate.EditMode !== Csw.enums.editMode.Temp)) {

                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.PropsUrlMethod,
                        data: {
                            EditMode: cswPrivate.EditMode,
                            NodeId: Csw.tryParseObjByIdx(cswPrivate.nodeids, 0),
                            TabId: tabid,
                            SafeNodeKey: Csw.tryParseObjByIdx(cswPrivate.nodekeys, 0),
                            NodeTypeId: cswPrivate.nodetypeid,
                            Date: cswPrivate.date,
                            Multi: cswPrivate.Multi,
                            filterToPropId: cswPrivate.filterToPropId,
                            ConfigMode: cswPrivate.Config,
                            RelatedNodeId: Csw.string(cswPrivate.relatednodeid),
                            RelatedNodeTypeId: Csw.string(cswPrivate.relatednodetypeid),
                            RelatedObjectClassId: Csw.string(cswPrivate.relatedobjectclassid)
                        },
                        success: function (data) {
                            if (Csw.isNullOrEmpty(data) && cswPrivate.EditMode === Csw.enums.editMode.Edit) {
                                Csw.error.throwException({
                                    type: 'warning',
                                    message: 'No properties have been configured for this layout: ' + cswPrivate.EditMode,
                                    name: 'Csw_client_exception',
                                    fileName: 'csw.tabsandprops.js',
                                    lineNumber: 387
                                });
                            }
                            if (data.nodeid) {
                                Csw.tryExec(cswPrivate.onNodeIdSet, data.nodeid);
                                cswPrivate.nodeids[0] = data.nodeid;
                                delete data.nodeid;
                            }
                            cswPrivate.propertyData = data;
                            makePropLayout();
                        } // success{}
                    }); // ajax
                } else {
                    if (cswPrivate.propertyData.nodeid) {
                        Csw.tryExec(cswPrivate.onNodeIdSet, cswPrivate.propertyData.nodeid);
                        cswPrivate.nodeids[0] = cswPrivate.propertyData.nodeid;
                        delete cswPrivate.propertyData.nodeid;
                    }
                    makePropLayout();
                }
            };

            // getPropsImpl()

            cswPrivate.onRemove = function (tabid, onRemoveData) {
                'use strict';
                var propid = '';
                var propDiv = cswPrivate.getPropertyCell(onRemoveData.cellSet).children('div');
                if (false === Csw.isNullOrEmpty(propDiv)) {
                    propid = propDiv.first().propNonDom('propId');
                }

                Csw.ajax.post({
                    watchGlobal: cswPrivate.AjaxWatchGlobal,
                    urlMethod: cswPrivate.RemovePropUrlMethod,
                    data: { PropId: propid, EditMode: cswPrivate.EditMode, TabId: tabid },
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
                        EditMode: cswPrivate.EditMode
                    };

                    Csw.ajax.post({
                        watchGlobal: cswPrivate.AjaxWatchGlobal,
                        urlMethod: cswPrivate.MovePropUrlMethod,
                        data: dataJson
                    });
                }
                return propid;
            };

            // _moveProp()

            cswPrivate.getLabelCell = function (cellSet) {
                return cellSet[1][1].children('div');
            };

            cswPrivate.getPropertyCell = function (cellSet) {
                return cellSet[1][2].children('div');
            };

            cswPrivate.handleProperties = function (layoutTable, tabContentDiv, tabid, configMode) {
                'use strict';
                layoutTable = layoutTable || cswPrivate.layoutTable;
                cswPrivate.atLeastOne = { Property: false, Saveable: false };
                var handleSuccess = function (propObj) {
                    cswPrivate.atLeastOne.Property = true;
                    cswPrivate.handleProp(layoutTable, propObj, tabContentDiv, tabid, configMode);
                    return false;
                };
                Csw.crawlObject(cswPrivate.propertyData, handleSuccess, false);

                if (false === Csw.isNullOrEmpty(cswPrivate.saveBtn, true)) {
                    if (cswPrivate.Config || (cswPrivate.atLeastOne.Saveable === false && cswPrivate.EditMode != Csw.enums.editMode.Add)) {
                        cswPrivate.saveBtn.hide();
                    } else {
                        cswPrivate.saveBtn.show();
                    }
                }
                return cswPrivate.atLeastOne;
            }; // _handleProperties()

            cswPrivate.getCellSet = function(layoutTable, tabgroup, displayrow, displaycol) {
                var ret;
                if(false === Csw.isNullOrEmpty(tabgroup)) {
                    var safetabgroup = Csw.makeSafeId(tabgroup);
                    if(Csw.isNullOrEmpty(cswPrivate.tabgrouptables)) {
                        cswPrivate.tabgrouptables = [];
                    }
                    if(Csw.isNullOrEmpty(cswPrivate.tabgrouptables[safetabgroup])) {
                        var cellSet = layoutTable.cellSet(displayrow, displaycol);
                        var propCell = cswPrivate.getPropertyCell(cellSet);

                        var $fieldset = $('<fieldset>');
                        $fieldset.append('<legend>' + tabgroup + '</legend>');
                        propCell.append($fieldset);

                        var div = Csw.literals.div({
                            $parent: $fieldset
                        });

                        var tabgroupLayoutTable = div.layoutTable({
                            ID: safetabgroup,
                            OddCellRightAlign: true,
                            ReadOnly: (cswPrivate.EditMode === Csw.enums.editMode.PrintReport || cswPrivate.ReadOnly),
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


            cswPrivate.handleProp = function (layoutTable, propData, tabContentDiv, tabid, configMode) {
                'use strict';
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

                    /* Case 25936 */
                    if (false === Csw.bool(propData.readonly)) {
                        cswPrivate.atLeastOne.Saveable = true;
                        if (cswPrivate.ShowCheckboxes && Csw.bool(propData.copyable)) {
                            var inpPropCheck = labelCell.input({
                                ID: 'check_' + propid,
                                type: Csw.enums.inputTypes.checkbox,
                                value: false, // Value --not defined?,
                                cssclass: cswPrivate.ID + '_check'
                            });
                            inpPropCheck.propNonDom('propid', propid);
                        }
                    }
                }

                var propCell = cswPrivate.getPropertyCell(cellSet);
                propCell.addClass('propertyvaluecell');

                if (Csw.bool(propData.highlight)) {
                    propCell.addClass('ui-state-highlight');
                }
                cswPrivate.makeProp(propCell, propData, tabContentDiv, tabid, configMode, layoutTable);
            };

            cswPrivate.canDisplayProp = function (propData, configMode) {
                /*The prop is set to display or we're in layout config mode*/
                var ret = (Csw.bool(propData.display, true) || configMode);
                /*And either no filter is set or the filter is set to this property */
                ret = ret && (cswPrivate.filterToPropId === '' || cswPrivate.filterToPropId === propData.id);
                /* We're not excluding any OC Props or this prop has not been excluded */
                ret = ret && ((Csw.isNullOrEmpty(cswPrivate.excludeOcProps) || cswPrivate.excludeOcProps.length === 0) || false === Csw.contains(cswPrivate.excludeOcProps, Csw.string(propData.ocpname).toLowerCase()));
                return ret;
            };

            cswPrivate.makeProp = function (propCell, propData, tabContentDiv, tabid, configMode, layoutTable) {
                'use strict';
                propCell.empty();
                if (cswPrivate.canDisplayProp(propData, configMode)) {
                    var propId = propData.id;
                    var propName = propData.name;

                    var fieldOpt = {
                        fieldtype: propData.fieldtype,
                        nodeid: Csw.tryParseObjByIdx(cswPrivate.nodeids, 0),
                        nodename: cswPrivate.nodename,
                        relatednodeid: cswPrivate.relatednodeid,
                        relatednodename: cswPrivate.relatednodename,
                        relatednodetypeid: cswPrivate.relatednodetypeid,
                        relatedobjectclassid: cswPrivate.relatedobjectclassid,
                        propid: propId,
                        propDiv: propCell.div(),
                        saveBtn: cswPrivate.saveBtn,
                        propData: propData,
                        onChange: function () {
                        },
                        onReload: function (afterReload) {
                            cswPrivate.getProps(tabContentDiv, tabid, afterReload);
                        },
                        doSave: function (saveopts) {
                            var s = {
                                onSuccess: null
                            };
                            if (saveopts) Csw.extend(s, saveopts);
                            cswPublic.save(tabContentDiv, tabid, s.onSuccess);
                        },
                        cswnbtnodekey: Csw.tryParseObjByIdx(cswPrivate.nodekeys, 0),
                        EditMode: cswPrivate.EditMode,
                        Multi: cswPrivate.Multi,
                        onEditView: cswPrivate.onEditView,
                        onAfterButtonClick: cswPrivate.onAfterButtonClick,
                        ReadOnly: Csw.bool(propData.readonly) || cswPrivate.Config
                    };
                    fieldOpt.propDiv.propNonDom({
                        'nodeid': fieldOpt.nodeid,
                        'propid': fieldOpt.propid,
                        'cswnbtnodekey': fieldOpt.cswnbtnodekey
                    });

                    fieldOpt.onChange = function () { if (Csw.isFunction(cswPrivate.onPropertyChange)) cswPrivate.onPropertyChange(fieldOpt.propid, propName); };
                    if (Csw.bool(propData.hassubprops)) {
                        fieldOpt.onChange = function () {
                            cswPrivate.updateSubProps(fieldOpt, propId, propData, propCell, tabContentDiv, tabid, false, layoutTable);
                            if (Csw.isFunction(cswPrivate.onPropertyChange)) cswPrivate.onPropertyChange(fieldOpt.propid, propName);
                        };
                    } // if (Csw.bool(propData.hassubprops)) {
                    $.CswFieldTypeFactory('make', fieldOpt);

                    if (Csw.contains(propData, 'subprops')) {
                        // recurse on sub-props
                        var subProps = propData.subprops;

                        var subLayoutTable = propCell.layoutTable({
                            ID: fieldOpt.propid + '_subproptable',
                            OddCellRightAlign: true,
                            ReadOnly: (cswPrivate.EditMode === Csw.enums.editMode.PrintReport || cswPrivate.ReadOnly),
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
            };

            // _makeProp()

            cswPrivate.updateSubProps = function (fieldOpt, propId, propData, propCell, tabContentDiv, tabid, configMode, layoutTable) {
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
                    if (cswPrivate.EditMode == Csw.enums.editMode.Add) {
                        fieldOpt.propData.wasmodified = true;
                    }
                    if (fieldOpt.propData.wasmodified) {
                        // update the propxml from the server
                        var jsonData = {
                            EditMode: fieldOpt.EditMode,
                            NodeId: Csw.tryParseObjByIdx(cswPrivate.nodeids, 0),
                            SafeNodeKey: fieldOpt.cswnbtnodekey,
                            PropId: propId,
                            NodeTypeId: cswPrivate.nodetypeid,
                            NewPropJson: JSON.stringify(propData)
                        };

                        Csw.ajax.post({
                            watchGlobal: cswPrivate.AjaxWatchGlobal,
                            urlMethod: cswPrivate.SinglePropUrlMethod,
                            data: jsonData,
                            success: function (data) {

                                data.wasmodified = true; // keep the fact that the parent property was modified
                                cswPrivate.makeProp(propCell, data, tabContentDiv, tabid, configMode, layoutTable);
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
                    propData = propData || cswPrivate.propertyData;

                    var propIds = [];
                    var updSuccess = function (thisProp) {
                        var propOpt = {
                            propData: thisProp,
                            propDiv: '',
                            fieldtype: thisProp.fieldtype,
                            nodeid: Csw.tryParseObjByIdx(cswPrivate.nodeids, 0),
                            Multi: cswPrivate.Multi,
                            cswnbtnodekey: cswPrivate.cswnbtnodekey
                        };

                        var cellSet = cswPrivate.getCellSet(layoutTable, thisProp.tabgroup, thisProp.displayrow, thisProp.displaycol);
                        layoutTable.addCellSetAttributes(cellSet, { propId: thisProp.id });
                        propOpt.propCell = cswPrivate.getPropertyCell(cellSet);
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

            cswPrivate.enableSaveBtn = function () {
                if (false === Csw.isNullOrEmpty(cswPrivate.saveBtn, true)) {
                    cswPrivate.saveBtn.enable();
                }
            };

            cswPublic.getPropJson = function () {
                cswPrivate.updatePropJsonFromLayoutTable();
                return cswPrivate.propertyData;
            };

            cswPublic.isFormValid = function () {
                return cswPrivate.form.$.valid();
            };

            cswPublic.getNodeId = function() {
                return cswPrivate.nodeids[0];
            };

            cswPublic.save = function (tabContentDiv, tabid, onSuccess, async) {
                'use strict';
                Csw.tryExec(function () {

                    if (cswPublic.isFormValid()) {
                        var propIds = cswPrivate.updatePropJsonFromLayoutTable();
                        var sourcenodeid = Csw.tryParseObjByIdx(cswPrivate.nodeids, 0);
                        var sourcenodekey = Csw.tryParseObjByIdx(cswPrivate.nodekeys, 0);
                        async = Csw.bool(async, true) && false === cswPrivate.Multi; //
                        Csw.ajax.post({
                            watchGlobal: cswPrivate.AjaxWatchGlobal,
                            urlMethod: cswPrivate.SavePropUrlMethod,
                            async: async,
                            data: {
                                EditMode: cswPrivate.EditMode,
                                NodeId: Csw.string(sourcenodeid),
                                SafeNodeKey: Csw.string(sourcenodekey),
                                TabId: Csw.string(tabid),
                                NodeTypeId: cswPrivate.nodetypeid,
                                NewPropsJson: Csw.serialize(cswPrivate.propertyData),
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
                                        urlMethod: cswPrivate.CopyPropValuesUrlMethod,
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

                                if (cswPrivate.ShowCheckboxes) {
                                    // apply the newly saved checked property values on this node to the checked nodes
                                    //var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                                    //var nodechecks = $('#' + o.NodeCheckTreeId).CswNodeTree('checkedNodes');
                                    var nodechecks = cswPrivate.nodeTreeCheck.checkedNodes();
                                    var $propchecks = $('.' + cswPrivate.ID + '_check:checked');

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
                                else if (cswPrivate.Multi) {
                                    dataJson.CopyNodeIds = cswPrivate.nodeids;
                                    dataJson.CopyNodeKeys = cswPrivate.nodekeys;
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

                                        //                                        switch (successData.action) {
                                        //                                            case 'loadView':
                                        //                                                //Csw.ajax('getviewofnode'
                                        //                                                //success: load view(data.viewid)
                                        //                                               

                                        //                                                var dialogOptions = {
                                        //                                                    div: Csw.literals.div({ text: 'Warning: You are about to lose your work!' }),
                                        //                                                    title: '',
                                        //                                                    onOk: onSaveRefresh,
                                        //                                                    onCancel: null,
                                        //                                                    onClose: null,
                                        //                                                    height: 400,
                                        //                                                    width: 600,
                                        //                                                    okText: 'Continue Working',
                                        //                                                    cancelText: 'Go to My Feedback'
                                        //                                                };

                                        //                                                $.CswDialog('GenericDialog', dialogOptions)

                                        //                                                break;
                                        //default:
                                        onSaveRefresh();
                                        //  break;
                                        // }
                                    };
                                    if (cswPrivate.ReloadTabOnSave) {
                                        cswPrivate.getProps(tabContentDiv, tabid, onSaveSuccess);
                                    } else {
                                        // cswPublic events
                                        onSaveSuccess();
                                    }
                                }

                            }, // success
                            error: cswPrivate.enableSaveBtn
                        }); // ajax
                    } // if(cswPrivate.form.$.valid())
                    else {
                        cswPrivate.enableSaveBtn();
                    }
                });
            }; // Save()

            (function () {
                cswPrivate.getTabs(cswPrivate.outerTabDiv);

                if (cswPrivate.EditMode !== Csw.enums.editMode.PrintReport) {
                    cswPrivate.linkDiv = cswParent.div({ ID: cswPrivate.ID + '_linkdiv', align: 'right' });
                    if (cswPrivate.ShowAsReport && false === cswPrivate.Multi) {
                        cswPrivate.linkDiv.a({
                            text: 'As Report',
                            onClick: function () {
                                Csw.openPopup('NodeReport.html?nodeid=' + Csw.tryParseObjByIdx(cswPrivate.nodeids, 0) + '&cswnbtnodekey=' + Csw.tryParseObjByIdx(cswPrivate.nodekeys, 0));
                            }
                        });
                    }
                }
            } ());


            return cswPublic;
        });
} ());

