/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";
    $.fn.CswNodeTabs = function (options) {
        "use strict";
        var o = {
            ID: '',
            TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
            SinglePropUrl: '/NbtWebApp/wsNBT.asmx/getSingleProp',
            PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
            MovePropUrl: '/NbtWebApp/wsNBT.asmx/moveProp',
            RemovePropUrl: '/NbtWebApp/wsNBT.asmx/removeProp',
            SavePropUrl: '/NbtWebApp/wsNBT.asmx/saveProps',
            CopyPropValuesUrl: '/NbtWebApp/wsNBT.asmx/copyPropValues',
            NodePreviewUrl: '/NbtWebApp/wsNBT.asmx/getNodePreview',
            QuotaUrl: '/NbtWebApp/wsNBT.asmx/checkQuota',
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
            Refresh: null, 
            onBeforeTabSelect: null,
            onTabSelect: null, 
            onPropertyChange: null,
            onPropertyRemove: null,
            onInitFinish: null, 
            ShowCheckboxes: false,
            ShowAsReport: true,
            AjaxWatchGlobal: true,
            NodeCheckTreeId: '',
            onEditView: null, 
            Config: false
        };

        if (options) {
            $.extend(o, options);
        }

        var $parent = $(this);

        var $outertabdiv = $('<div id="' + o.ID + '_tabdiv" />')
                        .appendTo($parent);
        var tabcnt = 0;

        getTabs(o);

        if (o.EditMode !== Csw.enums.editMode.PrintReport) {
            var $linkdiv = $('<div id="' + o.ID + '_linkdiv" align="right"/>')
                            .appendTo($parent);
            if (o.ShowAsReport && false === o.Multi) {
                $('<a href="#">As Report</a>')
                    .appendTo($linkdiv)
                    .click(function() { 
                        Csw.openPopup('NodeReport.html?nodeid=' + Csw.tryParseObjByIdx(o.nodeids, 0) + '&cswnbtnodekey=' + Csw.tryParseObjByIdx(o.nodekeys, 0), 600, 800); 
                    });
            }
        }

        function clearTabs() {
            $outertabdiv.contents().remove();
        }

        function makeTabContentDiv($tabParent, tabid, canEditLayout) {
            var $tabcontentdiv = $('<div id="' + tabid + '"><form onsubmit="return false;" id="' + tabid + '_form" /></div>')
                                    .appendTo($tabParent);

            var handle = function (eventObj) {
                $tabParent.remove();
                $.unsubscribe(Csw.enums.events.CswNodeDelete, handle);
                return false;
            };

            if(false === Csw.isNullOrEmpty($tabParent.parent(), true)) {
                $.subscribe(Csw.enums.events.CswNodeDelete, handle);
            }
            $tabcontentdiv.data('canEditLayout', canEditLayout);
            return $tabcontentdiv;
        }

        function getTabs() {
            var jsonData = {
                EditMode: o.EditMode,
                NodeId: Csw.tryParseObjByIdx(o.nodeids, 0),
                SafeNodeKey: Csw.tryParseObjByIdx(o.nodekeys, 0),
                NodeTypeId: o.nodetypeid,
                Date: o.date,
                filterToPropId: o.filterToPropId,
                Multi: o.Multi
            };

            // For performance, don't bother getting tabs if we're in Add or Preview
            if (o.EditMode == Csw.enums.editMode.AddInPopup ||
                o.EditMode == Csw.enums.editMode.Preview) {
                var tabid = o.EditMode + "_tab";
                var $tabcontentdiv = makeTabContentDiv($parent, tabid, false);
                getProps($tabcontentdiv, tabid);
            } else {
                Csw.ajax.post({
                    watchGlobal: o.AjaxWatchGlobal,
                    url: o.TabsUrl,
                    data: jsonData,
                    success: function (data) {
                        clearTabs();
                        var tabdivs = [];
                        var selectedtabno = 0;
                        var tabno = 0;

                        var tabFunc = function (thisTab) {
                            var thisTabId = thisTab.id;
                            if (o.EditMode === Csw.enums.editMode.PrintReport || tabdivs.length === 0) {
                                // For PrintReports, we're going to make a separate tabstrip for each tab
                                tabdivs[tabdivs.length] = $("<div><ul></ul></div>").appendTo($outertabdiv);
                            }
                            var $tabdiv = tabdivs[tabdivs.length - 1];
                            $tabdiv.children('ul').append('<li><a href="#' + thisTabId + '">' + thisTab.name + '</a></li>');
                            makeTabContentDiv($tabdiv, thisTabId, thisTab.canEditLayout);
                            if (thisTabId === o.tabid) {
                                selectedtabno = tabno;
                            }
                            tabno++;
                            return false;
                        };
                        Csw.crawlObject(data, tabFunc, false);

                        tabcnt = tabno;

                        Csw.each(tabdivs, function ($tabdiv) {
                            //var $tabdiv = tabdivs[t];
                            $tabdiv.tabs({
                                selected: selectedtabno,
                                    select: function(event, ui) {
                                    var $selectTabcontentdiv = $($tabdiv.children('div')[ui.index]);
                                    var selectTabid = $selectTabcontentdiv.CswAttrDom('id');
                                        if (Csw.isFunction(o.onBeforeTabSelect) && o.onBeforeTabSelect(selectTabid)) {
                                        getProps($selectTabcontentdiv, selectTabid);
                                            if (Csw.isFunction(o.onTabSelect)) {
                                            o.onTabSelect(selectTabid);
                                        }
                                    } else {
                                        return false;
                                    }
                                }
                            });
                            var $eachTabcontentdiv = $($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]);
                            var selectedtabid = $eachTabcontentdiv.CswAttrDom('id');
                            getProps($eachTabcontentdiv, selectedtabid);
                            if (Csw.isFunction(o.onTabSelect)) o.onTabSelect(selectedtabid);
                        }); // for(var t in tabdivs)
                    } // success
                }); // ajax
            } // if-else editmode is add or preview
        } // getTabs()

        function getProps($tabcontentdiv, tabid) {
            if (o.EditMode === Csw.enums.editMode.AddInPopup && o.Config === false) {
                // case 20970 - make sure there's room in the quota
                Csw.ajax.post({
                    watchGlobal: o.AjaxWatchGlobal,
                    url: o.QuotaUrl,
                    data: {NodeTypeId: o.nodetypeid},
                    success: function (data) {
                        if (Csw.bool(data.result)) {
                            getPropsImpl($tabcontentdiv, tabid);
                        } else {
                            $tabcontentdiv.append('You have used all of your purchased quota, and must purchase additional quota space in order to add more.');
                            Csw.tryExec(o.onInitFinish, false);
                        }
                    }                
                });
            } else {
                getPropsImpl($tabcontentdiv, tabid);
            }
        } // getProps()

        function getPropsImpl($tabcontentdiv, tabid) {
            var jsonData = {
                EditMode: o.EditMode,
                NodeId: Csw.tryParseObjByIdx(o.nodeids, 0),
                TabId: tabid,
                SafeNodeKey: Csw.tryParseObjByIdx(o.nodekeys, 0),
                NodeTypeId: o.nodetypeid,
                Date: o.date,
                Multi: o.Multi,
                filterToPropId: o.filterToPropId
            };

            Csw.ajax.post({
                watchGlobal: o.AjaxWatchGlobal,
                url: o.PropsUrl,
                data: jsonData,
                success: function (data) {
                    var $form = $tabcontentdiv.children('form');
                    $form.contents().remove();

                    if (o.title !== '') {
                        $form.append(o.title);
                    }

                    var $formtbl = $form.CswTable('init', { ID: o.ID + '_formtbl', width: '100%' });
                    var $formtblcell11 = $formtbl.CswTable('cell', 1, 1);
                    var $formtblcell12 = $formtbl.CswTable('cell', 1, 2);

                    var $savetab;
                    var $layouttable = $formtblcell11.CswLayoutTable('init', {
                        ID: o.ID + '_props',
                        OddCellRightAlign: true,
                        ReadOnly: (o.EditMode === Csw.enums.editMode.PrintReport || o.ReadOnly),
                        cellset: {
                            rows: 1,
                            columns: 2
                        },
                        onSwap: function (e, onSwapData) {
                            onSwap(onSwapData);
                        },
                        showConfigButton: false, //o.Config,
                        showRowColButtons: o.Config,
                        showRemoveButton: o.Config,
                        onConfigOn: function () {
                            doUpdateSubProps(true);
                        }, // onConfigOn
                        onConfigOff: function () {
                            doUpdateSubProps(false);
                        }, // onConfigOff
                        onRemove: function(event, onRemoveData) { 
                            onRemove(onRemoveData);
                        } // onRemove
                    }); // CswLayoutTable()

                    function doUpdateSubProps(configOn) {
                        var updOnSuccess = function (thisProp, key) {
                            if(Csw.bool(thisProp.hassubprops)) {
                                var propId = key; //key
                                var $subtable = $layouttable.find('#' + propId + '_subproptable');
                                var $parentcell = $subtable.parent().parent();
                                var $cellset = $layouttable.CswLayoutTable('cellset', $parentcell.CswAttrNonDom('row'), $parentcell.CswAttrNonDom('column'));
                                var $propcell = _getPropertyCell($cellset);

                                if ($subtable.length > 0) {
                                    var fieldOpt = {
                                        fieldtype: thisProp.fieldtype,
                                        nodeid: Csw.tryParseObjByIdx(o.nodeids, 0),
                                        relatednodeid: o.relatednodeid,
                                        relatednodetypeid: o.relatednodetypeid,
                                        propid: propId,
                                        $propdiv: $propcell.children('div'),
                                        propData: thisProp,
                                        onchange: function () { },
                                        onReload: function () { getProps($tabcontentdiv, tabid); },
                                        EditMode: o.EditMode,
                                        Multi: o.Multi,
                                        cswnbtnodekey: Csw.tryParseObjByIdx(o.nodekeys, 0)
                                    };

                                    _updateSubProps(fieldOpt, propId, thisProp, $propcell, $tabcontentdiv, tabid, configOn, $savetab);
                                }
                            }
                            return false;
                        };
                        Csw.crawlObject(data, updOnSuccess, false);
                    }

                    if (o.EditMode !== Csw.enums.editMode.PrintReport) {
                        $savetab = $formtblcell11.CswButton({ ID: 'SaveTab',
                            enabledText: 'Save Changes',
                            disabledText: 'Saving...',
                            onclick: function () { Save($form, $layouttable, data, $savetab, tabid); }
                        });
                    }
                    var AtLeastOne = _handleProperties($layouttable, data, $tabcontentdiv, tabid, false, $savetab);

                    // Validation
                    $form.validate({
                        highlight: function (element) {
                            var $elm = $(element);
                            $elm.CswAttrNonDom('csw_invalid', '1');
                            $elm.animate({ backgroundColor: '#ff6666' });
                        },
                        unhighlight: function (element) {
                            var $elm = $(element);
                            if ($elm.CswAttrNonDom('csw_invalid') === '1')  // only unhighlight where we highlighted
                            {
                                $elm.css('background-color', '#66ff66');
                                $elm.CswAttrNonDom('csw_invalid', '0');
                                setTimeout(function () { $elm.animate({ backgroundColor: 'transparent' }); }, 500);
                            }
                        }
                    }); // validate()

                    if(Csw.bool(o.Config)) {
                        $layouttable.CswLayoutTable('ConfigOn');
                    }
                    else if (!o.Config &&
                        Csw.isNullOrEmpty(o.date) && 
                        o.filterToPropId === '' &&
                        Csw.bool($tabcontentdiv.data('canEditLayout'))) {
                        /* Case 24437 */
                        var editLayoutOpt = {
                            ID: o.ID,
                            nodeids: o.nodeids,
                            nodekeys: o.nodekeys,
                            tabid: o.tabid,
                            nodetypeid: o.nodetypeid,
                            Refresh: function () {
                                    Csw.tryExec(o.Refresh);
                                o.Config = false;
                                getTabs();
                            }
                        };

                        /* Show the 'fake' config button to open the dialog */
                        $formtblcell12.CswImageButton({
                            ButtonType: Csw.enums.imageButton_ButtonType.Configure,
                            AlternateText: 'Configure',
                            ID: o.ID + 'configbtn',
                            onClick: function () {
                                clearTabs();
                                $.CswDialog('EditLayoutDialog', editLayoutOpt);
                                return Csw.enums.imageButton_ButtonType.None;
                            }
                        });
                    }

                    /* case 8494 */
                    if (!o.Config && !AtLeastOne.Saveable && o.EditMode == Csw.enums.editMode.AddInPopup) {
                        Save($form, $layouttable, data, $savetab, tabid);
                    }
                    else if (Csw.isFunction(o.onInitFinish)) {
                        o.onInitFinish(AtLeastOne.Property);
                    }
                } // success{}
            }); // ajax
        } // getPropsImpl()

        function onRemove(onRemoveData) {
            var $propdiv = _getPropertyCell(onRemoveData.cellset).children('div').first();
            var propid = $propdiv.CswAttrNonDom('propid');

            Csw.ajax.post({
                watchGlobal: o.AjaxWatchGlobal,
                url: o.RemovePropUrl,
                data: { PropId: propid, EditMode: o.EditMode },
                success: function () {
                    o.onPropertyRemove(propid);
                }
            });

        } // onRemove()

        function onSwap(onSwapData) {
            _moveProp(_getPropertyCell(onSwapData.cellset).children('div').first(), onSwapData.swaprow, onSwapData.swapcolumn);
            _moveProp(_getPropertyCell(onSwapData.swapcellset).children('div').first(), onSwapData.row, onSwapData.column);
        } // onSwap()

        function _moveProp($propdiv, newrow, newcolumn) {
            if ($propdiv.length > 0) {
                var propid = $propdiv.CswAttrNonDom('propid');

                var dataJson = {
                    PropId: propid,
                    NewRow: newrow,
                    NewColumn: newcolumn,
                    EditMode: o.EditMode
                };

                Csw.ajax.post({
                    watchGlobal: o.AjaxWatchGlobal,
                    url: o.MovePropUrl,
                    data: dataJson,
                    success: function () {

                    }
                });
            }
        } // _moveProp()

        function _getLabelCell($cellset) {
            return $cellset[1][1].children('div');
        }
        function _getPropertyCell($cellset) {
            return $cellset[1][2].children('div');
        }

        function handleProp($layouttable, thisProp, $tabcontentdiv, tabid, configMode, $savebtn, AtLeastOne) {
            var propid = thisProp.id;
            var fieldtype = thisProp.fieldtype;
            var $cellset = $layouttable.CswLayoutTable('cellset', thisProp.displayrow, thisProp.displaycol);

            if ((Csw.bool(thisProp.display, true) || configMode) &&
                fieldtype !== Csw.enums.subFieldsMap.Image.name &&
                    fieldtype !== Csw.enums.subFieldsMap.Grid.name &&
                        fieldtype !== Csw.enums.subFieldsMap.Button.name &&
                            (o.filterToPropId === '' || o.filterToPropId === propid)) {
                var $labelcell = _getLabelCell($cellset);
                $labelcell.addClass('propertylabel');

                if (Csw.bool(thisProp.highlight)) {
                    $labelcell.addClass('ui-state-highlight');
                }
            }

            var helpText = Csw.string(thisProp.helptext);
            var propName = Csw.string(thisProp.name);
            if (!Csw.isNullOrEmpty(helpText)) {
                $labelcell.CswLink('init', {
                    href: '#',
                    cssclass: 'cswprop_helplink',
                    title: helpText,
                    onclick: function () {
                        return false;
                    },
                    value: propName
                });

            } else {
                $labelcell.append(propName);
            }

            if (false === Csw.bool(thisProp.readonly)) {
                AtLeastOne.Saveable = true;
                if (o.ShowCheckboxes && Csw.bool(thisProp.copyable)) {
                    var $propcheck = $labelcell.CswInput('init', {
                        ID: 'check_' + propid,
                        type: Csw.enums.inputTypes.checkbox,
                        value: false, // Value --not defined?,
                        cssclass: o.ID + '_check'
                    });
                    $propcheck.CswAttrNonDom('propid', propid);
                }
            }


            var $propcell = _getPropertyCell($cellset);
            $propcell.addClass('propertyvaluecell');

            if (Csw.bool(thisProp.highlight)) {
                $propcell.addClass('ui-state-highlight');
            }
            _makeProp($propcell, thisProp, $tabcontentdiv, tabid, configMode, $savebtn, AtLeastOne);
        }

        function _handleProperties($layouttable, data, $tabcontentdiv, tabid, configMode, $savebtn) {
            var AtLeastOne = { Property: false, Saveable: false };
            var handleSuccess = function (propObj) {
                AtLeastOne.Property = true;
                handleProp($layouttable, propObj, $tabcontentdiv, tabid, configMode, $savebtn, AtLeastOne);
                return false;
            };
            Csw.crawlObject(data, handleSuccess, false);

            if(false === Csw.isNullOrEmpty($savebtn, true)) {
                if (o.Config || (AtLeastOne.Saveable === false && o.EditMode != Csw.enums.editMode.AddInPopup)) {
                    $savebtn.hide();
                } else {
                    $savebtn.show();
                }
            }
            return AtLeastOne;
        } // _handleProps()

        function _makeProp($propcell, propData, $tabcontentdiv, tabid, configMode, $savebtn, AtLeastOne) {
            $propcell.empty();
            if ((Csw.bool(propData.display, true) || configMode ) &&
                (o.filterToPropId === '' || o.filterToPropId === propData.id)) {

                var propId = propData.id;
                var propName = propData.name;

                var fieldOpt = {
                    fieldtype: propData.fieldtype,
                    nodeid: Csw.tryParseObjByIdx(o.nodeids, 0),
                    relatednodeid: o.relatednodeid,
                    relatednodetypeid: o.relatednodetypeid,
                    propid: propId,
                    $propdiv: $('<div/>').appendTo($propcell),
                    $savebtn: $savebtn,
                    propData: propData,
                    onchange: function () { },
                    onReload: function () { getProps($tabcontentdiv, tabid); },
                    cswnbtnodekey: Csw.tryParseObjByIdx(o.nodekeys, 0),
                    EditMode: o.EditMode,
                    Multi: o.Multi,
                    onEditView: o.onEditView,
                    ReadOnly: Csw.bool(propData.readonly)
                };
                fieldOpt.$propdiv.CswAttrNonDom('nodeid', fieldOpt.nodeid);
                fieldOpt.$propdiv.CswAttrNonDom('propid', fieldOpt.propid);
                fieldOpt.$propdiv.CswAttrNonDom('cswnbtnodekey', fieldOpt.cswnbtnodekey);

                fieldOpt.onchange = function () { if(Csw.isFunction(o.onPropertyChange)) o.onPropertyChange(fieldOpt.propid, propName); };
                if (Csw.bool(propData.hassubprops)) {
                    fieldOpt.onchange = function () {
                        _updateSubProps(fieldOpt, propId, propData, $propcell, $tabcontentdiv, tabid, false, $savebtn);
                        if(Csw.isFunction(o.onPropertyChange)) o.onPropertyChange(fieldOpt.propid, propName);
                    };
                } // if (Csw.bool(propData.hassubprops)) {

                $.CswFieldTypeFactory('make', fieldOpt);

                if (propData.hasOwnProperty('subprops')) {
                    // recurse on sub-props
                    var subProps = propData.subprops;

                    var $subtable = $propcell.CswLayoutTable('init', {
                        ID: fieldOpt.propid + '_subproptable',
                        OddCellRightAlign: true,
                        ReadOnly: (o.EditMode === Csw.enums.editMode.PrintReport || o.ReadOnly),
                        cellset: {
                            rows: 1,
                            columns: 2
                        },
                        onSwap: function (e, onSwapData) {
                            onSwap(onSwapData);
                        },
                        showConfigButton: false,
                        showRowColButtons: false,
                        showRemoveButton: false
                    });

                    var subOnSuccess = function (subProp, key) {
                        subProp.propId = key;
                        if (Csw.bool(subProp.display) || configMode) {
                            handleProp($subtable, subProp, $tabcontentdiv, tabid, configMode, $savebtn, AtLeastOne);
                            if (configMode) {
                                $subtable.CswLayoutTable('ConfigOn');
                            } else {
                                $subtable.CswLayoutTable('ConfigOff');
                            }
                        }
                        return false;
                    };
                    Csw.crawlObject(subProps, subOnSuccess, true);
                }
            } // if (propData.display != 'false' || ConfigMode )
        } // _makeProp()

        function _updateSubProps(fieldOpt, propId, propData, $propcell, $tabcontentdiv, tabid, configMode, $savebtn) {
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
                        NodeId: Csw.tryParseObjByIdx(o.nodeids, 0),
                        SafeNodeKey: fieldOpt.cswnbtnodekey,
                        PropId: propId,
                        NodeTypeId: o.nodetypeid,
                        NewPropJson: JSON.stringify(propData)
                    };

                    Csw.ajax.post({
                        watchGlobal: o.AjaxWatchGlobal,
                        url: o.SinglePropUrl,
                        data: jsonData,
                            success: function(data) {
                            var AtLeastOne = {};
                            data.wasmodified = true; // keep the fact that the parent property was modified
                            _makeProp($propcell, data, $tabcontentdiv, tabid, configMode, $savebtn, AtLeastOne);
                        }
                    });
                }
            }, 150);
        } // _updateSubProps()

        function Save($form, $layouttable, propsData, $savebtn, tabid) {
            if ($form.valid()) {
                var propIds = _updatePropJsonFromForm($layouttable, propsData);
                var data = {
                    EditMode: o.EditMode,
                    NodeIds: Csw.tryParseObjByIdx(o.nodeids, 0), // o.nodeids.join(','),
                    SafeNodeKeys: Csw.tryParseObjByIdx(o.nodekeys, 0), // o.nodekeys.join(','),
                    TabId: tabid,
                    NodeTypeId: o.nodetypeid,
                    NewPropsJson: JSON.stringify(propsData),
                    ViewId: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId)
                };

                Csw.ajax.post({
                    watchGlobal: o.AjaxWatchGlobal,
                    url: o.SavePropUrl,
                    async: (false === o.Multi),
                    data: data,
                    success: function (successData) {
                        var doSave = true;
                        var dataJson = {
                            SourceNodeKey: Csw.tryParseObjByIdx(o.nodekeys, 0),
                            CopyNodeIds: [],
                            PropIds: []
                        };
                        function copyNodeProps(onSuccess) {
                            Csw.ajax.post({
                                watchGlobal: o.AjaxWatchGlobal,
                                url: o.CopyPropValuesUrl,
                                data: dataJson,
                                success: function (copy) {
                                    if(Csw.isFunction(onSuccess)) {
                                        onSuccess(copy);
                                    }
                                }
                            }); // ajax						        
                        }
                        if (o.ShowCheckboxes) {
                            // apply the newly saved checked property values on this node to the checked nodes
                            var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                            var $propchecks = $('.' + o.ID + '_check:checked');

                            if ($nodechecks.length > 0 && $propchecks.length > 0) {
                                $nodechecks.each(function() { 
                                    var nodeid = $(this).CswAttrNonDom('nodeid');
                                    dataJson.CopyNodeIds.push(nodeid);
                                });

                                $propchecks.each(function() { 
                                    var propid = $(this).CswAttrNonDom('propid');
                                    dataJson.PropIds.push(propid);
                                });
                                copyNodeProps();
                            } // if($nodechecks.length > 0 && $propchecks.length > 0)
                            else {
                                doSave = false;
                                $.CswDialog('AlertDialog', 'You have not selected any properties to save.');
                            }
                        } // if(o.ShowCheckboxes)
                        else if (o.Multi) {
                            dataJson.CopyNodeIds = o.nodeids;
                            dataJson.PropIds = propIds;
                            copyNodeProps(function () { window.location.reload(); });
                        }
                        if (Csw.isFunction(o.onSave) && doSave) o.onSave(successData.nodeid, successData.cswnbtnodekey, tabcnt);
                        $savebtn.CswButton('enable');
                    }, // success
                    error: function () {
                        $savebtn.CswButton('enable');
                    }
                }); // ajax
            } // if($form.valid())
            else {
                $savebtn.CswButton('enable');
            }
        } // Save()

        function _updatePropJsonFromForm($layouttable, propData) {
            var propIds = [];
            var updSuccess = function (thisProp) {
                var propOpt = {
                    propData: thisProp,
                    $propdiv: '',
                    $propCell: '',
                    fieldtype: thisProp.fieldtype,
                    nodeid: Csw.tryParseObjByIdx(o.nodeids, 0),
                    Multi: o.Multi,
                    cswnbtnodekey: o.cswnbtnodekey
                };

                var $cellset = $layouttable.CswLayoutTable('cellset', thisProp.displayrow, thisProp.displaycol);
                propOpt.$propcell = _getPropertyCell($cellset);
                propOpt.$propdiv = propOpt.$propcell.children('div').first();

                $.CswFieldTypeFactory('save', propOpt);
                if (propOpt.propData.wasmodified) {
                    propIds.push(propOpt.propData.id);
                }

                // recurse on subprops
                if (Csw.bool(thisProp.hassubprops) && Csw.contains(thisProp, 'subprops')) {
                    var subProps = thisProp.subprops;
                    if (false === Csw.isNullOrEmpty(subProps)) { //&& $subprops.children('[display != "false"]').length > 0)
                        var $subtable = propOpt.$propcell.children('#' + thisProp.id + '_subproptable').first();
                        if ($subtable.length > 0) {
                            _updatePropJsonFromForm($subtable, subProps);
                        }
                    }
                }
                return false;
            };
            Csw.crawlObject(propData, updSuccess, false);
            return propIds;
        } // _updatePropXmlFromForm()

        // For proper chaining support
        return this;

    }; // function (options) {
})(jQuery);

