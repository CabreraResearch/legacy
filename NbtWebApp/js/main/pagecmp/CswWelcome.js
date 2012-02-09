/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = "CswWelcome";

    var methods = {

        initTable: function (options) {
            var o = {
                Url: '/NbtWebApp/wsNBT.asmx/getWelcomeItems',
                MoveWelcomeItemUrl: '/NbtWebApp/wsNBT.asmx/moveWelcomeItems',
                RemoveWelcomeItemUrl: '/NbtWebApp/wsNBT.asmx/deleteWelcomeItem',
                onLinkClick: null, //function (optSelect) { }, //viewid, actionid, reportid
                onSearchClick: null, //function (optSelect) { }, //viewid
                onAddClick: null, //function (nodetypeid) { },
                onAddComponent: null //function () { }
            };

            if (options) {
                $.extend(o, options);
            }
            var $this = $(this);

            var jsonData = {
                RoleId: ''
            };

            Csw.ajax.post({
                url: o.Url,
                data: jsonData,
                success: function (data) {
                    var $WelcomeDiv = $('<div id="welcomediv"></div>')
                            .appendTo($this)
                            .css('text-align', 'center')
                            .css('font-size', '1.2em');

                    var layoutTable = Csw.controls.layoutTable({
                        $parent: $WelcomeDiv,
                        ID: 'welcometable',
                        cellset: { rows: 2, columns: 1 },
                        TableCssClass: 'WelcomeTable',
                        cellpadding: 10,
                        align: 'center',
                        onSwap: function (ev, onSwapData) {
                            _onSwap({
                                cellset: onSwapData.cellset,
                                row: onSwapData.row,
                                column: onSwapData.column,
                                swapcellset: onSwapData.swapcellset,
                                swaprow: onSwapData.swaprow,
                                swapcolumn: onSwapData.swapcolumn,
                                MoveWelcomeItemUrl: o.MoveWelcomeItemUrl
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
                                cellset: onRemoveData.cellset,
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
                                var $cellset = layoutTable.cellSet(thisItem.displayrow, thisItem.displaycol);
                                var $imagecell = $cellset[1][1].children('div');
                                var $textcell = $cellset[2][1].children('div');

                                if (false === Csw.isNullOrEmpty(thisItem.buttonicon))
                                    $imagecell.append($('<a href="javascript:void(0);"><img border="0" src="' + thisItem.buttonicon + '"/></a>'));

                                var clickopts = {
                                    itemData: thisItem,
                                    layoutTable: layoutTable,
                                    onAddClick: o.onAddClick,
                                    onLinkClick: o.onLinkClick,
                                    onSearchClick: o.onSearchClick
                                };

                                if (Csw.string(thisItem.linktype).toLowerCase() === 'text') {
                                    $textcell.append('<span>' + thisItem.text + '</span>');
                                } else {
                                    var onClick = Csw.makeDelegate(_clickItem, clickopts);
                                    $textcell.append($('<a href="javascript:void(0);">' + thisItem.text + '</a>'));
                                    $textcell.find('a').click(onClick);
                                    $imagecell.find('a').click(onClick);
                                }

                                var $welcomehidden = $textcell.CswInput('init', { ID: welcomeId,
                                    type: Csw.enums.inputTypes.hidden
                                });
                                $welcomehidden.CswAttrNonDom('welcomeid', welcomeId);
                            }
                        }
                    }
                } // success{}
            }); // Csw.ajax
        }, // initTable

        'getAddItemForm': function (options) {
            var o = {
                'AddWelcomeItemUrl': '/NbtWebApp/wsNBT.asmx/addWelcomeItem',
                'onAdd': function () { }
            };
            if (options) {
                $.extend(o, options);
            }

            var $parent = $(this);
            var table = Csw.controls.table({
                $parent: $parent,
                ID: 'addwelcomeitem_tbl'
            });

            /* Type Select Label */
            table.add(1, 1, '<span>Type:</span>');
            var $typeselect = $('<select id="welcome_type" name="welcome_type"></select>');
            $typeselect.append('<option value="Add" selected>Add</option>');
            $typeselect.append('<option value="Link">Link</option>');
            $typeselect.append('<option value="Search">Search</option>');
            $typeselect.append('<option value="Text">Text</option>');
            table.add(1, 2, $typeselect);
            
            var viewSelectLabel = table.add(2, 1, '<span>View:</span>');
            viewSelectLabel.$.hide();
            var viewSelectCell = table.cell(2, 2);
            var viewSelectTable = Csw.controls.table({
                $parent: viewSelectCell.$,
                ID: 'viewselecttable'
            });

            var viewSelect = viewSelectTable.cell(1, 1).$.CswViewSelect({
                ID: 'welcome_viewsel'
            }).hide();

            var searchViewSelect = viewSelectTable.cell(2, 1).$.CswViewSelect({
                ID: 'welcome_searchviewsel',
                issearchable: true,
                usesession: false
            }).hide();

            var ntSelectLabel = table.add(3, 1, '<span>Add New:</span>');
            var $ntselect = table.cell(3, 2)
                                 .$.CswNodeTypeSelect({
                                      'ID': 'welcome_ntsel'
                                  });

            /* Welcome Text Label */
            table.add(4, 1, '<span>Text:</span>');
            var welcomeTextCell = table.cell(4, 2);
            var $welcometext = welcomeTextCell.$.CswInput('init', { ID: 'welcome_text',
                type: Csw.enums.inputTypes.text
            });
            var $buttonselLabel = table.add(5, 1, '<span>Use Button:</span>');
            var $buttonsel = table.add(5, 2, '<select id="welcome_button" />');
            $buttonsel.append('<option value="blank.gif"></option>')
                                        .css('width', '100px');

            var $buttonimg = table.add(6, 2, '<img id="welcome_btnimg" />');

            var $addbutton = table.cell(7, 2).CswButton({ ID: 'welcome_add',
                enabledText: 'Add',
                disabledText: 'Adding',
                onclick: function () {
                    var viewtype = '';
                    var viewvalue = '';
                    var selectedView;
                    if (!viewSelect.is(':hidden')) {
                        selectedView = viewSelect.CswViewSelect('value');
                        viewtype = selectedView.type;
                        viewvalue = selectedView.value;
                    }
                    else if (!searchViewSelect.is(':hidden')) {
                        selectedView = searchViewSelect.CswViewSelect('value');
                        viewtype = selectedView.type;
                        viewvalue = selectedView.value;
                    }

                    _addItem({
                        AddWelcomeItemUrl: o.AddWelcomeItemUrl,
                        type: $typeselect.val(),
                        viewtype: viewtype,
                        viewvalue: viewvalue,
                        nodetypeid: $ntselect.CswNodeTypeSelect('value'),
                        text: $welcometext.val(),
                        iconfilename: $buttonsel.val(),
                        onSuccess: o.onAdd,
                        onError: function () { $addbutton.CswButton('enable'); }
                    });
                }
            });
            table.add(7, 2, $addbutton);

            $buttonsel.change(function () {
                $buttonimg.CswAttrDom('src', 'Images/biggerbuttons/' + $buttonsel.val());
            });

            $typeselect.change(function () {
                _onTypeChange({
                    $typeselect: $typeselect,
                    $viewselect_label: viewSelectLabel,
                    $viewselect: viewSelect,
                    $searchviewselect: searchViewSelect,
                    $ntselect_label: ntSelectLabel.$,
                    $ntselect: $ntselect,
                    $buttonsel_label: $buttonselLabel,
                    $buttonsel: $buttonsel,
                    $buttonimg: $buttonimg
                });
            });

            Csw.ajax.post({
                url: '/NbtWebApp/wsNBT.asmx/getWelcomeButtonIconList',
                success: function (data) {
                    for (var icon in data) {
                        var filename = icon;
                        if (filename !== 'blank.gif') {
                            $buttonsel.append('<option value="' + filename + '">' + filename + '</option>');
                        }
                        $buttonsel.css('width', '');
                    } // each
                } // success
            }); // Csw.ajax
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
            onLinkClick: null,
            onSearchClick: null
        };
        if (clickopts) $.extend(c, clickopts);

        var optSelect = {
            type: c.itemData.type,
            viewmode: c.itemData.viewmode,
            itemid: c.itemData.itemid,
            text: c.itemData.text,
            iconurl: c.itemData.iconurl,
            viewid: c.itemData.viewid,
            actionid: c.itemData.actionid,
            actionname: c.itemData.actionname,
            actionurl: c.itemData.actionurl,
            reportid: c.itemData.reportid,
            linktype: c.itemData.linktype
        };

        if (c.layoutTable.isConfig() === false) {   // case 22288
            switch (optSelect.linktype.toLowerCase()) {
                case 'add':
                    Csw.tryExec(c.onAddClick, c.itemData.nodetypeid);
                    break;
                case 'link':
                    if (Csw.isFunction(c.onLinkClick)) { c.onLinkClick(optSelect); }
                    break;
                case 'search':
                    if (Csw.isFunction(c.onSearchClick)) { c.onSearchClick(optSelect); }
                    break;
                case 'text':
                    break;
            }
        }
        return false;
    } // _clickItem()

    function _removeItem(removedata) {
        var r = {
            cellset: '',
            row: '',
            column: '',
            RemoveWelcomeItemUrl: '',
            onSuccess: function () { }
        };
        if (removedata) {
            $.extend(r, removedata);
        }
        var $textcell = $(r.cellset[2][1]),
            welcomeid, dataJson;

        if ($textcell.length > 0) {
            welcomeid = $textcell.find('input').CswAttrNonDom('welcomeid');
            if (welcomeid) {
                dataJson = {
                    RoleId: '',
                    WelcomeId: welcomeid
                };

                Csw.ajax.post({
                    url: r.RemoveWelcomeItemUrl,
                    data: dataJson,
                    success: function () {
                        r.onSuccess();
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
            $.extend(a, addoptions);
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
            url: a.AddWelcomeItemUrl,
            data: dataJson,
            success: function () {
                a.onSuccess();
            },
            error: a.onError
        });

    } // _addItem()

    function _onSwap(onSwapData) {
        var s = {
            cellset: '',
            row: '',
            column: '',
            swapcellset: '',
            swaprow: '',
            swapcolumn: '',
            MoveWelcomeItemUrl: ''
        };
        if (onSwapData) {
            $.extend(s, onSwapData);
        }

        _moveItem(s.MoveWelcomeItemUrl, s.cellset, s.swaprow, s.swapcolumn);
        _moveItem(s.MoveWelcomeItemUrl, s.swapcellset, s.row, s.column);
    } // onSwap()

    function _moveItem(MoveWelcomeItemUrl, cellset, newrow, newcolumn) {
        var $textcell = $(cellset[2][1]);
        if ($textcell.length > 0) {
            var welcomeid = $textcell.find('input').CswAttrNonDom('welcomeid');
            if (false === Csw.isNullOrEmpty(welcomeid)) {
                var dataJson = {
                    RoleId: '',
                    WelcomeId: welcomeid,
                    NewRow: newrow,
                    NewColumn: newcolumn
                };

                Csw.ajax.post({
                    url: MoveWelcomeItemUrl,
                    data: dataJson
                });
            }
        }
    } // _moveItem()

    function _onTypeChange(options) {
        var o = {
            //$table: '',
            //$typeselect_label: '',
            $typeselect: '',
            $viewselect_label: '',
            $viewselect: '',
            $searchviewselect: '',
            $ntselect_label: '',
            $ntselect: '',
            //			$welcometext_label: '',
            //			$welcometext: '',
            $buttonsel_label: '',
            $buttonsel: '',
            $buttonimg: ''//,
            //			$addbutton: '',
        };
        if (options) {
            $.extend(o, options);
        }

        switch (o.$typeselect.val()) {
            case "Add":
                o.$viewselect_label.hide();
                o.$viewselect.hide();
                o.$searchviewselect.hide();
                o.$ntselect_label.show();
                o.$ntselect.show();
                o.$buttonsel_label.show();
                o.$buttonsel.show();
                o.$buttonimg.show();
                break;
            case "Link":
                o.$viewselect_label.show();
                o.$viewselect.show();
                o.$searchviewselect.hide();
                o.$ntselect_label.hide();
                o.$ntselect.hide();
                o.$buttonsel_label.show();
                o.$buttonsel.show();
                o.$buttonimg.show();
                break;
            case "Search":
                o.$viewselect_label.show();
                o.$viewselect.hide();
                o.$searchviewselect.show();
                o.$ntselect_label.hide();
                o.$ntselect.hide();
                o.$buttonsel_label.show();
                o.$buttonsel.show();
                o.$buttonimg.show();
                break;
            case "Text":
                o.$viewselect_label.hide();
                o.$viewselect.hide();
                o.$searchviewselect.hide();
                o.$ntselect_label.hide();
                o.$ntselect.hide();
                o.$buttonsel_label.hide();
                o.$buttonsel.hide();
                o.$buttonimg.hide();
                break;
        } // switch

    } // _onTypeChange()

})(jQuery);


