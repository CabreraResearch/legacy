/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.layouts.landingpage = Csw.layouts.landingpage ||
        Csw.layouts.register('landingpage', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: 'landingpage',
                Title: '',
                onLinkClick: null,
                onAddClick: null,
                onAddComponent: null,
                actionData: null
            };
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var cswPublic = { };

            (function () {
                Csw.ajax.post({
                    urlMethod: 'getWelcomeItems', //TODO - replace all things 'welcome' with 'landingPage' once webservice is refactored
                    data: cswPrivate.actionData,
                    success: function (data) {
                        cswPrivate.landingPageDiv = cswParent.div({ name: 'landingPageDiv' })
                            .css({
                                'text-align': 'center',
                                'font-size': '1.2em'
                            });

                        //add title here

                        cswPrivate.layoutTable = cswPrivate.landingPageDiv.layoutTable({
                            name: 'welcometable',
                            cellSet: { rows: 2, columns: 1 },
                            TableCssClass: 'WelcomeTable',
                            cellpadding: 10,
                            align: 'center',
                            width: null,
                            onSwap: function (ev, onSwapData) {
                                cswPrivate.onSwap(onSwapData);
                            },
                            showConfigButton: true,
                            showExpandRowButton: true,
                            showExpandColButton: true,
                            showAddButton: true,
                            showRemoveButton: true,
                            onAddClick: function () {
                                $.CswDialog('AddWelcomeItemDialog', {
                                    form: cswPrivate.getAddItemForm,
                                    onAdd: cswPrivate.onAddComponent
                                });
                            },
                            onRemove: function (ev, onRemoveData) {
                                cswPrivate.removeItem(onRemoveData);
                            }
                        });

                        Csw.each(data, function (welcomeItem, welcomeId) {
                            var thisItem = welcomeItem;
                            if (false === Csw.isNullOrEmpty(thisItem)) {
                                var cellSet = cswPrivate.layoutTable.cellSet(thisItem.displayrow, thisItem.displaycol);
                                cswPrivate.layoutTable.addCellSetAttributes(cellSet, { welcomeid: welcomeId });
                                var imageCell = cellSet[1][1].children('div');
                                var textCell = cellSet[2][1].children('div');
                                var link = null;
                                if (false === Csw.isNullOrEmpty(thisItem.buttonicon)) {
                                    link = imageCell.a({
                                        href: 'javascript:void(0);'
                                    });
                                    link.img({
                                        src: thisItem.buttonicon,
                                        border: '',
                                        cssclass: 'WelcomeImage'
                                    });
                                }

                                var clickopts = {
                                    itemData: thisItem,
                                    layoutTable: cswPrivate.layoutTable,
                                    onAddClick: cswPrivate.onAddClick,
                                    onLinkClick: cswPrivate.onLinkClick
                                };

                                if (Csw.string(thisItem.linktype).toLowerCase() === 'text') {
                                    textCell.span({ text: thisItem.text });
                                } else {
                                    var onClick = Csw.makeDelegate(cswPrivate.clickItem, clickopts);
                                    textCell.a({
                                        href: 'javascript:void(0);',
                                        value: thisItem.text,
                                        onClick: onClick
                                    });
                                    if (false === Csw.isNullOrEmpty(link)) {
                                        link.bind('click', onClick);
                                    }
                                }

                                var welcomeHidden = textCell.input({
                                    name: welcomeId,
                                    type: Csw.enums.inputTypes.hidden
                                });
                                welcomeHidden.propNonDom('welcomeid', welcomeId);
                            }
                        });
                    } // success{}
                }); // Csw.ajax
            } ());

            cswPrivate.clickItem = function (clickopts) {
                var itemid = Csw.string(clickopts.itemData.itemid, clickopts.itemData.viewid);
                itemid = Csw.string(itemid, clickopts.itemData.actionid);
                itemid = Csw.string(itemid, clickopts.itemData.reportid);

                var optSelect = {
                    type: clickopts.itemData.type,
                    mode: clickopts.itemData.viewmode,
                    itemid: itemid,
                    text: clickopts.itemData.text,
                    iconurl: clickopts.itemData.iconurl,
                    name: clickopts.itemData.actionname,
                    url: clickopts.itemData.actionurl,
                    linktype: clickopts.itemData.linktype
                };

                if (clickopts.layoutTable.isConfig() === false) {
                    switch (optSelect.linktype.toLowerCase()) {
                        case 'add':
                            Csw.tryExec(clickopts.onAddClick, clickopts.itemData.nodetypeid);
                            break;
                        case 'link':
                            Csw.tryExec(clickopts.onLinkClick, optSelect);
                            break;
                        case 'text':
                            break;
                    }
                }
            };

            cswPrivate.onSwap = function (onSwapData) {
                var welcomeIdOrig = cswPrivate.moveItem(onSwapData.cellSet, onSwapData.swaprow, onSwapData.swapcolumn);
                var welcomeIdSwap = cswPrivate.moveItem(onSwapData.swapcellset, onSwapData.row, onSwapData.column);
                onSwapData.cellSet[2][1].propNonDom('welcomeid', welcomeIdSwap);
                onSwapData.swapcellset[2][1].propNonDom('welcomeid', welcomeIdOrig);
            };

            cswPrivate.moveItem = function (cellSet, newrow, newcolumn) {
                var textCell = cellSet[2][1];
                var welcomeid = textCell.propNonDom('welcomeid');
                if (textCell.length() > 0) {
                    if (false === Csw.isNullOrEmpty(welcomeid)) {
                        var dataJson = {
                            RoleId: '',
                            WelcomeId: welcomeid,
                            NewRow: newrow,
                            NewColumn: newcolumn
                        };
                        Csw.ajax.post({
                            urlMethod: 'moveWelcomeItems',
                            data: dataJson
                        });
                    }
                }
                return welcomeid;
            };

            cswPrivate.removeItem = function (removedata) {
                var textCell = removedata.cellSet[2][1],
                    welcomeid,
                    dataJson;
                if (textCell.length() > 0) {
                    welcomeid = textCell.propNonDom('welcomeid');
                    if (welcomeid) {
                        dataJson = {
                            RoleId: '',
                            WelcomeId: welcomeid
                        };

                        Csw.ajax.post({
                            urlMethod: 'deleteWelcomeItem',
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
                    name: 'addwelcomeitem_tbl'
                });

                /* Type Select Label */
                table.cell(1, 1).span({ text: 'Type:' });
                var typeSelect = table.cell(1, 2).select({
                    name: 'welcome_type'
                });
                typeSelect.option({ value: 'Add', display: 'Add', isSelected: true });
                typeSelect.option({ value: 'Link', display: 'Link' });
                typeSelect.option({ value: 'Text', display: 'Text' });

                var viewSelectLabel = table.cell(2, 1).span({ text: 'View:' }).hide();

                var viewSelectTable = table.cell(2, 2).table({
                    name: 'viewselecttable'
                });

                var viewSelect = viewSelectTable.cell(1, 1).viewSelect({
                    name: 'welcome_viewsel',
                    maxHeight: '275px',
                    includeRecent: false
                });
                viewSelect.$.hide();

                var ntSelectLabel = table.cell(3, 1).span({ text: 'Add New:' });
                var ntSelect = table.cell(3, 2)
                    .nodeTypeSelect({
                        name: 'welcome_ntsel',
                        filterToPermission: 'Create'
                    });

                /* Welcome Text Label */
                table.cell(4, 1).span({ text: 'Text:' });

                var welcomeText = table.cell(4, 2).input({ name: 'welcome_text' });

                var addButton = table.cell(7, 2).button({
                    name: 'welcome_add',
                    enabledText: 'Add',
                    disabledText: 'Adding',
                    onClick: function () {
                        var viewtype = '';
                        var viewvalue = '';
                        var selectedView;
                        if (false == viewSelect.$.is(':hidden')) {
                            selectedView = viewSelect.val();
                            viewtype = selectedView.type;
                            viewvalue = selectedView.value;
                        }

                        cswPrivate.addItem({
                            type: typeSelect.val(),
                            viewtype: viewtype,
                            viewvalue: viewvalue,
                            nodetypeid: ntSelect.val(),
                            text: welcomeText.val(),
                            iconfilename: '',
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
                    RoleId: '',
                    Type: addOptions.type,
                    ViewType: addOptions.viewtype,
                    ViewValue: addOptions.viewvalue,
                    NodeTypeId: addOptions.nodetypeid,
                    Text: addOptions.text,
                    IconFileName: addOptions.iconfilename
                };

                Csw.ajax.post({
                    urlMethod: 'addWelcomeItem',
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

