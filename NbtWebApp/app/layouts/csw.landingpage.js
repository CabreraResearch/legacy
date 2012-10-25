/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.layouts.landingpage = Csw.layouts.landingpage ||
        Csw.layouts.register('landingpage', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: 'landingpage',
                Title: '',                
                RoleId: '',
                ActionId: '',
                ObjectClassId: '',
                onTitleClick: null,
                onLinkClick: null,
                onAddClick: null,
                onTabClick: null,
                onButtonClick: null,
                onAddComponent: null,
                landingPageRequestData: null,
                isConfigurable: true,
                addItemForm: {
                    table: null,
                    row: {
                        label: {},
                        control: {}
                    }
                },
                select: {
                    type: 1,
                    view: 2,
                    nodetype: 3,
                    tab: 4,
                    button: 5,
                    text: 6,
                    add: 7
                }                
            };
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var cswPublic = {};

            (function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'LandingPages/getItems',
                    data: cswPrivate.landingPageRequestData,
                    success: function (ajaxdata) {
                        cswPrivate.data = {
                            LandingPageItems: [{
                                LandingPageId: '',
                                Text: '',
                                DisplayRow: '',
                                DisplayCol: '',
                                ButtonIcon: '',
                                Type: '',
                                LinkType: '',
                                NodeTypeId: '',
                                ViewId: '',
                                ViewMode: '',
                                ActionId: '',
                                ActionName: '',
                                ActionUrl: '',
                                ReportId: ''
                            }]
                        };
                        Csw.extend(cswPrivate.data, ajaxdata);

                        cswPrivate.landingPageDiv = cswParent.div({ name: 'landingPageDiv' })
                            .css({
                                'text-align': 'center',
                                'font-size': '1.2em'
                            });

                        cswPrivate.landingPageTitle = cswPrivate.landingPageDiv.a({
                            cssclass: 'LandingPageTitle',
                            text: cswPrivate.Title,
                            onClick: cswPrivate.onTitleClick
                        });

                        cswPrivate.layoutTable = cswPrivate.landingPageDiv.layoutTable({
                            name: 'landingpagetable',
                            cellSet: { rows: 2, columns: 1 },
                            TableCssClass: 'LandingPageTable',
                            cellpadding: 10,
                            align: 'center',
                            width: null,
                            onSwap: function (ev, onSwapData) {
                                cswPrivate.onSwap(onSwapData);
                            },
                            showConfigButton: cswPrivate.isConfigurable,
                            showExpandRowButton: cswPrivate.isConfigurable,
                            showExpandColButton: cswPrivate.isConfigurable,
                            showAddButton: cswPrivate.isConfigurable,
                            showRemoveButton: cswPrivate.isConfigurable,
                            onAddClick: function () {
                                $.CswDialog('AddLandingPageItemDialog', {
                                    form: cswPrivate.getAddItemForm,
                                    onAdd: cswPrivate.onAddComponent
                                });
                            },
                            onRemove: function (ev, onRemoveData) {
                                cswPrivate.removeItem(onRemoveData);
                            }
                        });

                        Csw.each(cswPrivate.data.LandingPageItems, function (landingPageItem) {
                            var thisItem = landingPageItem;
                            if (false === Csw.isNullOrEmpty(thisItem)) {
                                var cellSet = cswPrivate.layoutTable.cellSet(thisItem.DisplayRow, thisItem.DisplayCol);
                                cswPrivate.layoutTable.addCellSetAttributes(cellSet, { landingpageid: thisItem.LandingPageId });
                                var imageCell = cellSet[1][1].children('div');
                                var textCell = cellSet[2][1].children('div');
                                var link = null;
                                if (false === Csw.isNullOrEmpty(thisItem.ButtonIcon)) {
                                    link = imageCell.a({
                                        href: 'javascript:void(0);'
                                    });
                                    link.img({
                                        src: thisItem.ButtonIcon,
                                        border: '',
                                        cssclass: 'LandingPageImage'
                                    });
                                }

                                var clickopts = {
                                    itemData: thisItem,
                                    layoutTable: cswPrivate.layoutTable,
                                    onAddClick: cswPrivate.onAddClick,
                                    onLinkClick: cswPrivate.onLinkClick,
                                    onTabClick: cswPrivate.onTabClick,
                                    onButtonClick: cswPrivate.onButtonClick
                                };

                                if (Csw.string(thisItem.LinkType).toLowerCase() === 'text') {
                                    textCell.span({ text: thisItem.Text });
                                } else {
                                    var onClick = Csw.makeDelegate(cswPrivate.clickItem, clickopts);
                                    textCell.a({
                                        href: 'javascript:void(0);',
                                        value: thisItem.Text,
                                        onClick: onClick
                                    });
                                    if (false === Csw.isNullOrEmpty(link)) {
                                        link.bind('click', onClick);
                                    }
                                }

                                var landingPageHidden = textCell.input({
                                    name: thisItem.LandingPageId,
                                    type: Csw.enums.inputTypes.hidden
                                });
                                landingPageHidden.propNonDom('landingpageid', thisItem.LandingPageId);
                            }
                        });
                    } // success{}
                }); // Csw.ajax
            } ());

            cswPrivate.clickItem = function (clickopts) {
                var itemid = clickopts.itemData.ViewId;
                itemid = Csw.string(itemid, clickopts.itemData.ActionId);
                itemid = Csw.string(itemid, clickopts.itemData.ReportId);

                var optSelect = {
                    type: clickopts.itemData.Type,
                    mode: clickopts.itemData.ViewMode,
                    itemid: itemid,
                    text: clickopts.itemData.Text,
                    name: clickopts.itemData.ActionName,
                    url: clickopts.itemData.ActionUrl,
                    linktype: clickopts.itemData.LinkType,
                    TabId: clickopts.itemData.TabId
                };

                if (clickopts.layoutTable.isConfig() === false) {
                    switch (optSelect.linktype.toLowerCase()) {
                        case 'add':
                            Csw.tryExec(clickopts.onAddClick, clickopts.itemData);
                            break;
                        case 'link':
                            Csw.tryExec(clickopts.onLinkClick, optSelect);
                            break;
                        case 'tab':
                            Csw.tryExec(clickopts.onTabClick, optSelect);
                            break;
                        case 'button':
                            Csw.tryExec(clickopts.onButtonClick, clickopts.itemData);
                            break;
                        case 'text':
                            break;
                    }
                }
            };

            cswPrivate.onSwap = function (onSwapData) {
                var landingpageidOrig = cswPrivate.moveItem(onSwapData.cellSet, onSwapData.swaprow, onSwapData.swapcolumn);
                var landingpageidSwap = cswPrivate.moveItem(onSwapData.swapcellset, onSwapData.row, onSwapData.column);
                onSwapData.cellSet[2][1].propNonDom('landingpageid', landingpageidSwap);
                onSwapData.swapcellset[2][1].propNonDom('landingpageid', landingpageidOrig);
            };

            cswPrivate.moveItem = function (cellSet, newrow, newcolumn) {
                var textCell = cellSet[2][1];
                var landingpageid = textCell.propNonDom('landingpageid');
                if (textCell.length() > 0) {
                    if (false === Csw.isNullOrEmpty(landingpageid)) {
                        var dataJson = {
                            LandingPageId: landingpageid,
                            NewRow: newrow,
                            NewColumn: newcolumn
                        };
                        Csw.ajaxWcf.post({
                            urlMethod: 'LandingPages/moveItem',
                            data: dataJson
                        });
                    }
                }
                return landingpageid;
            };

            cswPrivate.removeItem = function (removedata) {
                var textCell = removedata.cellSet[2][1],
                    landingpageid,
                    dataJson;
                if (textCell.length() > 0) {
                    landingpageid = textCell.propNonDom('landingpageid');
                    if (landingpageid) {
                        dataJson = {
                            LandingPageId: landingpageid
                        };
                        Csw.ajaxWcf.post({
                            urlMethod: 'LandingPages/deleteItem',
                            data: dataJson,
                            success: function () {
                                Csw.tryExec(removedata.onSuccess);
                            }
                        });
                    }
                }
            };

            cswPrivate.getAddItemForm = function (parentDiv, addOptions) {
                cswPrivate.addItemForm.table = parentDiv.table({ name: 'addlandingpageitem_tbl' });            
                cswPrivate.makeTypeControl();
                cswPrivate.makeViewControl();
                cswPrivate.makeNodeTypeControl();
                cswPrivate.makeTabControl();
                if (false === Csw.isNullOrEmpty(cswPrivate.ActionId) && false === Csw.isNullOrEmpty(cswPrivate.ObjectClassId)) {
                cswPrivate.makeButtonControl();
                }
                cswPrivate.makeTextControl();
                cswPrivate.makeAddControl(addOptions);
            };

            cswPrivate.makeTypeControl = function () {
                cswPrivate.resetAddItem(cswPrivate.select.type);
                cswPrivate.addItemForm[cswPrivate.select.type].label = cswPrivate.addItemForm.table.cell(cswPrivate.select.type, 1).span({ text: 'Type:' });
                cswPrivate.addItemForm[cswPrivate.select.type].control = cswPrivate.addItemForm.table.cell(cswPrivate.select.type, 2).select({
                    name: 'landingpage_type'
                });
                cswPrivate.addItemForm[cswPrivate.select.type].control.option({ value: 'Add', display: 'Add', isSelected: true });
                cswPrivate.addItemForm[cswPrivate.select.type].control.option({ value: 'Link', display: 'Link' });                
                cswPrivate.addItemForm[cswPrivate.select.type].control.option({ value: 'Tab', display: 'Tab' });
                if (false === Csw.isNullOrEmpty(cswPrivate.ActionId) && false === Csw.isNullOrEmpty(cswPrivate.ObjectClassId)) {
                    cswPrivate.addItemForm[cswPrivate.select.type].control.option({ value: 'Button', display: 'Button' });
                }
                cswPrivate.addItemForm[cswPrivate.select.type].control.option({ value: 'Text', display: 'Text' });
                cswPrivate.addItemForm[cswPrivate.select.type].control.change(function () {
                    if (cswPrivate.addItemForm[cswPrivate.select.type].control.val() == 'Tab' ||
                        cswPrivate.addItemForm[cswPrivate.select.type].control.val() == 'Add') {
                        cswPrivate.makeNodeTypeControl(cswPrivate.select.nodetype);
                    }
                    cswPrivate.onTypeChange();
                });
            };
            
            cswPrivate.makeViewControl = function () {
                cswPrivate.resetAddItem(cswPrivate.select.view);
                cswPrivate.addItemForm[cswPrivate.select.view].label = cswPrivate.addItemForm.table.cell(cswPrivate.select.view, 1).span({ text: 'View:' }).hide();
                cswPrivate.addItemForm[cswPrivate.select.view].control = cswPrivate.addItemForm.table.cell(cswPrivate.select.view, 2).viewSelect({
                    name: 'landingpage_viewsel',
                    maxHeight: '275px',
                    includeRecent: false
                });
                cswPrivate.addItemForm[cswPrivate.select.view].control.$.hide();
            };
            
            cswPrivate.makeNodeTypeControl = function () {
                cswPrivate.resetAddItem(cswPrivate.select.nodetype);
                var filter = '', text = '';
                if (cswPrivate.addItemForm[cswPrivate.select.type].control.val() == 'Add') {
                    filter = 'Create';
                    text = 'Add New:';
                } else if (cswPrivate.addItemForm[cswPrivate.select.type].control.val() == 'Tab') {
                    filter = 'Edit';
                    text = 'Select:';
                }
                cswPrivate.addItemForm[cswPrivate.select.nodetype].label = cswPrivate.addItemForm.table.cell(cswPrivate.select.nodetype, 1).span({ text: text });
                cswPrivate.addItemForm[cswPrivate.select.nodetype].control = cswPrivate.addItemForm.table.cell(cswPrivate.select.nodetype, 2).nodeTypeSelect({
                    name: 'landingpage_ntsel',
                    async: false,
                    filterToPermission: filter,
                    onSelect: function () {
                        cswPrivate.makeTabControl();
                    }
                });
            };
            
            cswPrivate.makeTabControl = function () {
                cswPrivate.resetAddItem(cswPrivate.select.tab);
                cswPrivate.addItemForm[cswPrivate.select.tab].label = cswPrivate.addItemForm.table.cell(cswPrivate.select.tab, 1).span({ text: 'Select Tab:' });
                cswPrivate.addItemForm[cswPrivate.select.tab].control = cswPrivate.addItemForm.table.cell(cswPrivate.select.tab, 2).tabSelect({
                    name: 'landingpage_tabsel',
                    nodeTypeId: cswPrivate.addItemForm[cswPrivate.select.nodetype].control.val(),
                    filterToPermission: 'Edit'
                });
                if (cswPrivate.addItemForm[cswPrivate.select.type].control.val() != 'Tab') {
                    cswPrivate.addItemForm[cswPrivate.select.tab].label.hide();
                    cswPrivate.addItemForm[cswPrivate.select.tab].control.hide();
                }
            };
            
            cswPrivate.makeTextControl = function () {
                cswPrivate.resetAddItem(cswPrivate.select.text);
                cswPrivate.addItemForm[cswPrivate.select.text].label = cswPrivate.addItemForm.table.cell(cswPrivate.select.text, 1).span({ text: 'Text:' });
                cswPrivate.addItemForm[cswPrivate.select.text].control = cswPrivate.addItemForm.table.cell(cswPrivate.select.text, 2).input({ name: 'landingpage_text' });
            };
            
            cswPrivate.makeButtonControl = function () {
                cswPrivate.resetAddItem(cswPrivate.select.button);
                cswPrivate.addItemForm[cswPrivate.select.button].label = cswPrivate.addItemForm.table.cell(cswPrivate.select.button, 1).span({ text: 'Select Action:' }).hide();
                cswPrivate.addItemForm[cswPrivate.select.button].control = cswPrivate.addItemForm.table.cell(cswPrivate.select.button, 2).select({
                    name: 'landingpage_buttonsel'
                }).hide();
                Csw.ajax.post({
                    urlMethod: 'getObjectClassButtons',
                    data: {
                        ObjectClassId: Csw.string(cswPrivate.ObjectClassId)
                    },
                    success: function (data) {
                        Csw.each(data, function (thisButton) {
                            if (Csw.contains(thisButton, 'id') && Csw.contains(thisButton, 'name')) {
                                cswPrivate.addItemForm[cswPrivate.select.button].control.option({
                                    value: thisButton.id,
                                    display: thisButton.name
                                });
                            }
                        });
                    }
                });
            };

            cswPrivate.makeAddControl = function (addOptions) {
                cswPrivate.resetAddItem(cswPrivate.select.add);
                cswPrivate.addItemForm[cswPrivate.select.add].label = cswPrivate.addItemForm.table.cell(cswPrivate.select.add, 1).span({ text: '' });
                cswPrivate.addItemForm[cswPrivate.select.add].control = cswPrivate.addItemForm.table.cell(cswPrivate.select.add, 2).button({
                    name: 'landingpage_add',
                    enabledText: 'Add',
                    disabledText: 'Adding',
                    onClick: function () {
                        var viewtype = '';
                        var pkvalue = '';
                        var selectedView;
                        if (false == cswPrivate.addItemForm[cswPrivate.select.view].control.$.is(':hidden')) {
                            selectedView = cswPrivate.addItemForm[cswPrivate.select.view].control.val();
                            viewtype = selectedView.type;
                            pkvalue = selectedView.value;
                        } else if (false == cswPrivate.addItemForm[cswPrivate.select.tab].control.$.is(':hidden')) {
                            pkvalue = cswPrivate.addItemForm[cswPrivate.select.tab].control.val();
                        } else if (false === Csw.isNullOrEmpty(cswPrivate.addItemForm[cswPrivate.select.button]) &&
                            false == cswPrivate.addItemForm[cswPrivate.select.button].control.$.is(':hidden')) {
                            pkvalue = cswPrivate.addItemForm[cswPrivate.select.button].control.val();
                        }
                        cswPrivate.addItem({
                            type: cswPrivate.addItemForm[cswPrivate.select.type].control.val(),
                            viewtype: viewtype,
                            pkvalue: pkvalue,
                            nodetypeid: cswPrivate.addItemForm[cswPrivate.select.nodetype].control.val(),
                            text: cswPrivate.addItemForm[cswPrivate.select.text].control.val(),
                            onSuccess: addOptions.onAdd,
                            onError: function () { cswPrivate.addItemForm[cswPrivate.select.add].control.enable(); }
                        });
                    }
                });
            };

            cswPrivate.resetAddItem = function(row) {
                cswPrivate.addItemForm[row] = { label: { }, control: { } };
                cswPrivate.addItemForm.table.cell(row, 1).empty();
                cswPrivate.addItemForm.table.cell(row, 2).empty();
            };

            cswPrivate.addItem = function (addOptions) {
                var dataJson = {
                    RoleId: cswPrivate.RoleId,
                    ActionId: cswPrivate.ActionId,
                    Type: addOptions.type,
                    ViewType: addOptions.viewtype,
                    PkValue: addOptions.pkvalue,
                    NodeTypeId: addOptions.nodetypeid,
                    Text: addOptions.text
                };
                Csw.ajaxWcf.post({
                    urlMethod: 'LandingPages/addItem',
                    data: dataJson,
                    success: function () {
                        Csw.tryExec(addOptions.onSuccess);
                    },
                    error: addOptions.onError
                });
            };

            cswPrivate.onTypeChange = function () {
                cswPrivate.toggleVisibility(cswPrivate.select.view, false);
                cswPrivate.toggleVisibility(cswPrivate.select.nodetype, false);
                cswPrivate.toggleVisibility(cswPrivate.select.tab, false);
                if (false === Csw.isNullOrEmpty(cswPrivate.ActionId) && false === Csw.isNullOrEmpty(cswPrivate.ObjectClassId)) {
                cswPrivate.toggleVisibility(cswPrivate.select.button, false);
                }
                switch (cswPrivate.addItemForm[cswPrivate.select.type].control.val()) {
                    case 'Add':
                        cswPrivate.toggleVisibility(cswPrivate.select.nodetype, true);
                        break;
                    case 'Link':
                        cswPrivate.toggleVisibility(cswPrivate.select.view, true);
                        break;
                    case 'Tab':
                        cswPrivate.toggleVisibility(cswPrivate.select.nodetype, true);
                        cswPrivate.toggleVisibility(cswPrivate.select.tab, true);
                        break;
                    case 'Button':
                        cswPrivate.toggleVisibility(cswPrivate.select.button, true);
                        break;
                    case 'Text':
                        break;
                }
            };

            cswPrivate.toggleVisibility = function(type, show) {
                if(show) {
                    cswPrivate.addItemForm[type].label.show();
                    cswPrivate.addItemForm[type].control.show();
                } else {
                    cswPrivate.addItemForm[type].label.hide();
                    cswPrivate.addItemForm[type].control.hide();
                }
            };

            return cswPublic;
        });
} ());

