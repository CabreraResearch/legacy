/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    $.fn.CswFieldTypeLocation = function (method) {

        var pluginName = 'CswFieldTypeLocation';

        var methods = {
            init: function (o) {

                var propDiv = o.propDiv;
                propDiv.empty();
                var propVals = o.propData.values;
                var nodeId = (false === o.Multi) ? Csw.string(propVals.nodeid, o.relatednodeid).trim() : '';
                var nodeKey = ''; //(false === o.Multi) ? Csw.string(propVals.nodekey).trim() : '';
                var name = (false === o.Multi) ? Csw.string(propVals.name, o.relatednodename).trim() : Csw.enums.multiEditDefaultValue;
                var path = (false === o.Multi) ? Csw.string(propVals.path, o.relatednodename).trim() : Csw.enums.multiEditDefaultValue;
                var viewId = Csw.string(propVals.viewid).trim();
                var comboBox;

                var table = propDiv.table({
                    ID: Csw.makeId(o.ID, 'tbl')
                });

                table.cell(1, 1).text(path);

                var selectDiv = table.cell(2, 1).div({
                    cssclass: 'locationselect',
                    value: nodeId
                });

                table.cell(2, 1).hide();
                if (false === o.ReadOnly) {

                    table.cell(1, 2).imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        ID: Csw.makeId(o.ID, 'toggle'),
                        onClick: function () {

                            table.cell(1, 1).hide();
                            table.cell(1, 2).hide();
                            table.cell(2, 1).show();

                            comboBox = selectDiv.comboBox({
                                ID: o.ID + '_combo',
                                topContent: name,
                                selectContent: '',
                                width: '290px',
                                onClick: (function () {
                                    var first = true;
                                    return function () {
                                        if (first) {      // only do this once
                                            locationTree.expandAll();
                                            first = false;
                                        }
                                        comboBox.open(); // ensure we're open on click
                                        return false;    // but only close when onTreeSelect fires, below
                                    };
                                })()
                            });

                            var locationTree = Csw.nbt.nodeTree({
                                ID: o.ID,
                                parent: comboBox.pickList,
                                onInitialSelectNode: function (optSelect) {
                                    onTreeSelect(selectDiv, comboBox, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, function () { });
                                },
                                onSelectNode: function (optSelect) {
                                    onTreeSelect(selectDiv, comboBox, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, o.onChange);
                                },
                                UseScrollbars: false,
                                ShowToggleLink: false
                            });

                            locationTree.init({
                                viewid: viewId,
                                nodeid: nodeId,
                                cswnbtnodekey: nodeKey,
                                IncludeInQuickLaunch: false,
                                DefaultSelect: Csw.enums.nodeTree_DefaultSelect.root.name
                            });
                        } // onClick
                    }); // imageButton

                    propDiv.$.hover(function (event) {
                        Csw.nodeHoverIn(event, selectDiv.val());
                    }, Csw.nodeHoverOut);
                }
            },
            save: function (o) { //($propdiv, $xml
                var attributes = { nodeid: null };
                var selectDiv = o.propDiv.find('.locationselect');
                attributes.nodeid = selectDiv.propNonDom('value');
                Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
            }
        };


        function onTreeSelect(selectDiv, comboBox, itemid, text, iconurl, onChange) {
            if (itemid === 'root' || itemid === undefined) itemid = '';   // case 21046
            comboBox.topContent(text, itemid);
            if (comboBox.val() !== itemid) {
                comboBox.val(itemid);
                onChange();
            }
            selectDiv.propNonDom('value', itemid);
            setTimeout(function () { comboBox.close(); }, 100);
        }

        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
