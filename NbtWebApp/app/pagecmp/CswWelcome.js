
/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = "CswWelcome";

    var methods = {

        initTable: function (options) {
            var o = {
                Url: 'getWelcomeItems',
                moveWelcomeItemUrl: 'moveWelcomeItems',
                RemoveWelcomeItemUrl: 'deleteWelcomeItem',
                onLinkClick: null, //function (optSelect) { }, //viewid, actionid, reportid
                //onSearchClick: null, //function (optSelect) { }, //viewid
                onAddClick: null, //function (nodetypeid) { },
                onAddComponent: null //function () { }
            };

            if (options) {
                Csw.extend(o, options);
            }
            var $parent = $(this);
            var parent = Csw.literals.factory($parent);

            var jsonData = {
                RoleId: ''
            };

            Csw.ajax.post({
                urlMethod: o.Url,
                data: jsonData,
                success: function (data) {
                    var welcomeDiv = parent.div({ ID: 'welcomediv' })
                            .css({ 'text-align': 'center',
                                'font-size': '1.2em'
                            });

                    var layoutTable = welcomeDiv.layoutTable({
                        ID: 'welcometable',
                        width: null,
                        cellSet: { rows: 2, columns: 1 },
                        TableCssClass: 'WelcomeTable',
                        cellpadding: 10,
                        align: 'center',
                        onSwap: function (ev, onSwapData) {
                            _onSwap({
                                cellSet: onSwapData.cellSet,
                                row: onSwapData.row,
                                column: onSwapData.column,
                                swapcellset: onSwapData.swapcellset,
                                swaprow: onSwapData.swaprow,
                                swapcolumn: onSwapData.swapcolumn,
                                moveWelcomeItemUrl: o.moveWelcomeItemUrl
                            });
                        },
                        showConfigButton: true,
                        showExpandRowButton: true,
                        showExpandColButton: true,
                        showAddButton: true,
                        showRemoveButton: true,
                        onAddClick: function () { $.CswDialog('AddWelcomeItemDialog', { onAdd: o.onAddComponent }); },
                        onRemove: function (ev, onRemoveData) {
                            _removeItem({
                                cellSet: onRemoveData.cellSet,
                                row: onRemoveData.row,
                                column: onRemoveData.column,
                                RemoveWelcomeItemUrl: o.RemoveWelcomeItemUrl
                            });
                        }
                    });

                    for (var welcomeId in data) {
                        if (Csw.contains(data, welcomeId)) {
                            var thisItem = data[welcomeId];
                            if (false === Csw.isNullOrEmpty(thisItem)) {
                                var cellSet = layoutTable.cellSet(thisItem.displayrow, thisItem.displaycol);
                                layoutTable.addCellSetAttributes(cellSet, { welcomeid: welcomeId });
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
                                    layoutTable: layoutTable,
                                    onAddClick: o.onAddClick,
                                    onLinkClick: o.onLinkClick//,
                                    //onSearchClick: o.onSearchClick
                                };

                                if (Csw.string(thisItem.linktype).toLowerCase() === 'text') {
                                    textCell.span({ text: thisItem.text });
                                } else {
                                    var onClick = Csw.makeDelegate(_clickItem, clickopts);
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
                                    ID: welcomeId,
                                    type: Csw.enums.inputTypes.hidden
                                });
                                welcomeHidden.propNonDom('welcomeid', welcomeId);
                            }
                        }
                    }
                } // success{}
            }); // Csw.ajax
        }, // initTable

        'getAddItemForm': function (options) {
            var o = {
                AddWelcomeItemUrl: 'addWelcomeItem',
                onAdd: function () { }
            };
            if (options) {
                Csw.extend(o, options);
            }

            var $parent = $(this);
            var parent = Csw.literals.factory($parent);
            var table = parent.table({
                ID: 'addwelcomeitem_tbl'
            });

            /* Type Select Label */
            table.cell(1, 1).span({ text: 'Type:' });
            var typeSelect = table.cell(1, 2).select({
                ID: 'welcome_type'
            });
            typeSelect.option({ value: 'Add', display: 'Add', isSelected: true });
            typeSelect.option({ value: 'Link', display: 'Link' });
            //typeSelect.option({ value: 'Search', display: 'Search' });
            typeSelect.option({ value: 'Text', display: 'Text' });


            var viewSelectLabel = table.cell(2, 1).span({ text: 'View:' }).hide();

            var viewSelectTable = table.cell(2, 2).table({
                ID: 'viewselecttable'
            });

            //            var $viewSelect = viewSelectTable.cell(1, 1).$.CswViewSelect({
            var viewSelect = viewSelectTable.cell(1, 1).viewSelect({
                //$parent: viewSelectTable.cell(1, 1).$,
                ID: 'welcome_viewsel',
                maxHeight: '275px',
                includeRecent: false
            });
            viewSelect.$.hide();

            //            var $searchViewSelect = viewSelectTable.cell(2, 1).$.CswViewSelect({
            //            var searchViewSelect = Csw.controls.viewSelect({
            //            var searchViewSelect = viewSelectTable.cell(2, 1).viewSelect({
            //                $parent: viewSelectTable.cell(2, 1).$,
            //                ID: 'welcome_searchviewsel',
            //                issearchable: true,
            //                usesession: false,
            //                maxHeight: '275px'
            //            });
            //            searchViewSelect.$.hide();

            var ntSelectLabel = table.cell(3, 1).span({ text: 'Add New:' });
            var ntSelect = table.cell(3, 2)
                                 .nodeTypeSelect({
                                     ID: 'welcome_ntsel',
                                     filterToPermission: 'Create'
                                 });

            /* Welcome Text Label */
            table.cell(4, 1).span({ text: 'Text:' });

            var welcomeText = table.cell(4, 2).input({ ID: 'welcome_text' });
            //var buttonselLabel = table.cell(5, 1).span({ text: 'Use Button:' });
            //var buttonSel = table.cell(5, 2).select({ ID: 'welcome_button' });
            //buttonSel.option({ value: 'blank.gif', display: '', isSelected: true })
            //                            .css('width', '100px');

            //var buttonImg = table.cell(6, 2).img({ ID: 'welcome_btnimg' });

            var addButton = table.cell(7, 2).button({ ID: 'welcome_add',
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
                    //                    else if (false == searchViewSelect.$.is(':hidden')) {
                    //                        selectedView = searchViewSelect.val();
                    //                        viewtype = selectedView.type;
                    //                        viewvalue = selectedView.value;
                    //                    }

                    _addItem({
                        AddWelcomeItemUrl: o.AddWelcomeItemUrl,
                        type: typeSelect.val(),
                        viewtype: viewtype,
                        viewvalue: viewvalue,
                        nodetypeid: ntSelect.val(),
                        text: welcomeText.val(),
                        iconfilename: '', //buttonSel.val(),
                        onSuccess: o.onAdd,
                        onError: function () { addButton.enable(); }
                    });
                }
            });

//            buttonSel.bind('change', function () {
//                buttonImg.propDom('src', 'Images/biggerbuttons/' + buttonSel.val());
//            });

            typeSelect.change(function () {
                _onTypeChange({
                    typeSelect: typeSelect,
                    viewSelectLabel: viewSelectLabel,
                    viewselect: viewSelect,
                    //searchviewselect: searchViewSelect,
                    ntSelectLabel: ntSelectLabel,
                    $ntselect: ntSelect//,
//                    buttonSelLabel: buttonselLabel,
//                    buttonSel: buttonSel,
//                    buttonImg: buttonImg
                });
            });

//            Csw.ajax.post({
//                url: '/NbtWebApp/wsNBT.asmx/getWelcomeButtonIconList',
//                success: function (data) {
//                    for (var filename in data) {
//                        if (false === Csw.isNullOrEmpty(filename) &&
//                            filename !== 'blank.gif') {
//                            buttonSel.option({ value: filename, display: filename });
//                        }
//                        buttonSel.css('width', '');
//                    } // each
//                } // success
//            }); // Csw.ajax
        } // getAddItemForm

    };


    // Method calling logic
    $.fn.CswWelcome = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };

    function _clickItem(clickopts) {
        var c = {
            itemData: {},
            layoutTable: {},
            onAddClick: null,
            onLinkClick: null//,
            //onSearchClick: null
        };
        if (clickopts) Csw.extend(c, clickopts);
        var itemid = Csw.string(c.itemData.itemid, c.itemData.viewid);
        itemid = Csw.string(itemid, c.itemData.actionid);
        itemid = Csw.string(itemid, c.itemData.reportid);
        
        var optSelect = {
            type: c.itemData.type,
            mode: c.itemData.viewmode,
            itemid: itemid,
            text: c.itemData.text,
            iconurl: c.itemData.iconurl,
            name: c.itemData.actionname,
            url: c.itemData.actionurl,
            linktype: c.itemData.linktype
        };

        if (c.layoutTable.isConfig() === false) {   // case 22288
            switch (optSelect.linktype.toLowerCase()) {
                case 'add':
                    Csw.tryExec(c.onAddClick, c.itemData.nodetypeid);
                    break;
                case 'link':
                    Csw.tryExec(c.onLinkClick, optSelect);
                    break;
                //                case 'search':    
                //                    Csw.tryExec(c.onSearchClick, optSelect);    
                //                    break;    
                case 'text':
                    break;
            }
        }
        return false;
    } // _clickItem()

    function _removeItem(removedata) {
        var r = {
            cellSet: '',
            row: '',
            column: '',
            RemoveWelcomeItemUrl: '',
            onSuccess: function () { }
        };
        if (removedata) {
            Csw.extend(r, removedata);
        }
        var textCell = r.cellSet[2][1],
            welcomeid, dataJson;

        if (textCell.length() > 0) {
            welcomeid = textCell.propNonDom('welcomeid');
            if (welcomeid) {
                dataJson = {
                    RoleId: '',
                    WelcomeId: welcomeid
                };

                Csw.ajax.post({
                    urlMethod: r.RemoveWelcomeItemUrl,
                    data: dataJson,
                    success: function () {
                        Csw.tryExec(r.onSuccess);
                    }
                });
            }
        } // if($textcell.length > 0)
    } // _removeItem()

    function _addItem(addoptions) {
        var a = {
            'AddWelcomeItemUrl': '',
            'type': '',
            'viewtype': '',
            'viewvalue': '',
            'nodetypeid': '',
            'text': '',
            'iconfilename': '',
            'onSuccess': function () { },
            'onError': function () { }
        };
        if (addoptions) {
            Csw.extend(a, addoptions);
        }

        var dataJson = {
            RoleId: '',
            Type: a.type,
            ViewType: a.viewtype,
            ViewValue: a.viewvalue,
            NodeTypeId: a.nodetypeid,
            Text: a.text,
            IconFileName: a.iconfilename
        };

        Csw.ajax.post({
            urlMethod: a.AddWelcomeItemUrl,
            data: dataJson,
            success: function () {
                Csw.tryExec(a.onSuccess);
            },
            error: a.onError
        });

    } // _addItem()

    function _onSwap(onSwapData) {
        var s = {
            cellSet: '',
            row: '',
            column: '',
            swapcellset: '',
            swaprow: '',
            swapcolumn: '',
            moveWelcomeItemUrl: ''
        };
        if (onSwapData) {
            Csw.extend(s, onSwapData);
        }

        var welcomeIdOrig = _moveItem(s.moveWelcomeItemUrl, s.cellSet, s.swaprow, s.swapcolumn);
        var welcomeIdSwap = _moveItem(s.moveWelcomeItemUrl, s.swapcellset, s.row, s.column);
        //Case 25958 - swap welcomeIds after swapping cells
        s.cellSet[2][1].propNonDom('welcomeid', welcomeIdSwap);
        s.swapcellset[2][1].propNonDom('welcomeid', welcomeIdOrig);
    } // onSwap()

    function _moveItem(moveWelcomeItemUrl, cellSet, newrow, newcolumn) {
        var textCell = cellSet[2][1];
        if (textCell.length() > 0) {
            var welcomeid = textCell.propNonDom('welcomeid');
            if (false === Csw.isNullOrEmpty(welcomeid)) {
                var dataJson = {
                    RoleId: '',
                    WelcomeId: welcomeid,
                    NewRow: newrow,
                    NewColumn: newcolumn
                };

                Csw.ajax.post({
                    urlMethod: moveWelcomeItemUrl,
                    data: dataJson
                });
            }
        }
        return welcomeid;
    } // _moveItem()

    function _onTypeChange(options) {
        var o = {
            typeSelect: '',
            viewSelectLabel: '',
            viewselect: '',
            //searchviewselect: '',
            ntSelectLabel: '',
            $ntselect: ''//,
//            buttonSelLabel: '',
//            buttonSel: '',
//            buttonImg: ''
        };
        if (options) {
            Csw.extend(o, options);
        }

        switch (o.typeSelect.val()) {
            case 'Add':
                o.viewSelectLabel.hide();
                o.viewselect.$.hide();
                //o.searchviewselect.$.hide();
                o.ntSelectLabel.show();
                o.$ntselect.show();
//                o.buttonSelLabel.show();
//                o.buttonSel.show();
//                o.buttonImg.show();
                break;
            case 'Link':
                o.viewSelectLabel.show();
                o.viewselect.$.show();
                //o.searchviewselect.$.hide();
                o.ntSelectLabel.hide();
                o.$ntselect.hide();
//                o.buttonSelLabel.show();
//                o.buttonSel.show();
//                o.buttonImg.show();
                break;
            //            case 'Search':    
            //                o.viewSelectLabel.show();    
            //                o.viewselect.$.hide();    
            //                o.searchviewselect.$.show();    
            //                o.ntSelectLabel.hide();    
            //                o.$ntselect.hide();    
            //                o.buttonSelLabel.show();    
            //                o.buttonSel.show();    
            //                o.buttonImg.show();    
            //                break;    
            case 'Text':
                o.viewSelectLabel.hide();
                o.viewselect.$.hide();
                //o.searchviewselect.$.hide();
                o.ntSelectLabel.hide();
                o.$ntselect.hide();
//                o.buttonSelLabel.hide();
//                o.buttonSel.hide();
//                o.buttonImg.hide();
                break;
        } // switch

    } // _onTypeChange()

})(jQuery);


