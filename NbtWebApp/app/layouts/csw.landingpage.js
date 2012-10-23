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
                landingPageRequestData: null
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
                            showConfigButton: true, //TODO - these flags will be contingent upon whether or not the user is an admin
                            showExpandRowButton: true,
                            showExpandColButton: true,
                            showAddButton: true,
                            showRemoveButton: true,
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
                var parent = parentDiv;
                var table = parent.table({
                    name: 'addlandingpageitem_tbl'
                });

                /* Type Select Label */
                table.cell(1, 1).span({ text: 'Type:' });
                var typeSelect = table.cell(1, 2).select({
                    name: 'landingpage_type'
                });
                typeSelect.option({ value: 'Add', display: 'Add', isSelected: true });
                typeSelect.option({ value: 'Link', display: 'Link' });
                typeSelect.option({ value: 'Text', display: 'Text' });
                //TODO - add Tab

                var viewSelectLabel = table.cell(2, 1).span({ text: 'View:' }).hide();

                var viewSelectTable = table.cell(2, 2).table({
                    name: 'viewselecttable'
                });

                var viewSelect = viewSelectTable.cell(1, 1).viewSelect({
                    name: 'landingpage_viewsel',
                    maxHeight: '275px',
                    includeRecent: false
                });
                viewSelect.$.hide();

                var ntSelectLabel = table.cell(3, 1).span({ text: 'Add New:' });
                var ntSelect = table.cell(3, 2)
                    .nodeTypeSelect({
                        name: 'landingpage_ntsel',
                        filterToPermission: 'Create'
                    });

                /* Landing Page Item Text Label */
                table.cell(4, 1).span({ text: 'Text:' });

                var landingPageText = table.cell(4, 2).input({ name: 'landingpage_text' });

                var addButton = table.cell(7, 2).button({
                    name: 'landingpage_add',
                    enabledText: 'Add',
                    disabledText: 'Adding',
                    onClick: function () {
                        var viewtype = '';
                        var pkvalue = '';
                        var selectedView;
                        if (false == viewSelect.$.is(':hidden')) {
                            selectedView = viewSelect.val();
                            viewtype = selectedView.type;
                            pkvalue = selectedView.value;
                        }

                        cswPrivate.addItem({
                            type: typeSelect.val(),
                            viewtype: viewtype,
                            pkvalue: pkvalue,
                            nodetypeid: ntSelect.val(),
                            text: landingPageText.val(),
                            onSuccess: addOptions.onAdd,
                            onError: function () { addButton.enable(); }
                        });
                    }
                });

                typeSelect.change(function () {
                    cswPrivate.onTypeChange({
                        typeSelect: typeSelect,
                        viewSelectLabel: viewSelectLabel,
                        viewselect: viewSelect,
                        ntSelectLabel: ntSelectLabel,
                        $ntselect: ntSelect
                    });
                });

            }; // getAddItemForm

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

            cswPrivate.onTypeChange = function (controlOptions) {
                switch (controlOptions.typeSelect.val()) {
                    case 'Add':
                        controlOptions.viewSelectLabel.hide();
                        controlOptions.viewselect.$.hide();
                        controlOptions.ntSelectLabel.show();
                        controlOptions.$ntselect.show();
                        break;
                    case 'Link':
                        controlOptions.viewSelectLabel.show();
                        controlOptions.viewselect.$.show();
                        controlOptions.ntSelectLabel.hide();
                        controlOptions.$ntselect.hide();
                        break;
                    case 'Text':
                        controlOptions.viewSelectLabel.hide();
                        controlOptions.viewselect.$.hide();
                        controlOptions.ntSelectLabel.hide();
                        controlOptions.$ntselect.hide();
                        break;
                }
            };

            return cswPublic;
        });
} ());

