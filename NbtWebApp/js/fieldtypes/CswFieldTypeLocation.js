/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    $.fn.CswFieldTypeLocation = function (method) {

        var pluginName = 'CswFieldTypeLocation';

        var methods = {
            init: function (o) {

                var propDiv = o.propDiv;
                propDiv.empty();
                var propVals = o.propData.values;
                var nodeId = (false === o.Multi) ? Csw.string(propVals.nodeid).trim() : '';
                var nodeKey = (false === o.Multi) ? Csw.string(propVals.nodekey).trim() : '';
                var name = (false === o.Multi) ? Csw.string(propVals.name).trim() : Csw.enums.multiEditDefaultValue;
                var path = (false === o.Multi) ? Csw.string(propVals.path).trim() : Csw.enums.multiEditDefaultValue;
                var viewId = Csw.string(propVals.viewid).trim();
                var comboBox;

                if (o.ReadOnly) {
                    propDiv.append(path);
                    propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, nodeId); }, Csw.nodeHoverOut);
                } else {
                    var table = propDiv.table({
                        ID: Csw.controls.dom.makeId(o.ID, 'tbl')
                    });

                    table.cell(1, 1).text(path).propDom('colspan', '2').br();

                    var selectDiv = table.cell(2, 1).div({
                        cssclass: 'locationselect',
                        value: nodeId
                    });

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
                            onTreeSelect(comboBox, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, function () { });
                        },
                        onSelectNode: function (optSelect) {
                            onTreeSelect(comboBox, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, o.onChange);
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

                    propDiv.$.hover(function (event) {
                        Csw.nodeHoverIn(event, selectDiv.val());
                    }, Csw.nodeHoverOut);
                }
            },
            save: function (o) { //($propdiv, $xml
                var attributes = { nodeid: null };
                var selectDiv = o.propDiv.find('.locationselect');
                if (false === Csw.isNullOrEmpty(selectDiv.children())) {
                    attributes.nodeid = selectDiv.children().val();
                }
                Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
            }
        };


        function onTreeSelect(comboBox, itemid, text, iconurl, onChange) {
            if (itemid === 'root') itemid = '';   // case 21046
            comboBox.topContent(text, itemid);
            if (comboBox.val() !== itemid) {
                comboBox.val(itemid);
                onChange();
            }
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
