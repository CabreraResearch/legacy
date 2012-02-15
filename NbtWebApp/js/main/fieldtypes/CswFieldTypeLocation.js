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

                if (o.ReadOnly) {
                    propDiv.append(path);
                    propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, nodeId); }, Csw.nodeHoverOut);
                } else {
                    var table = propDiv.table({
                        ID: Csw.controls.dom.makeId(o.ID, 'tbl')
                    });

                    table.add(1, 1, path).propDom('colspan', '2').br();

                    var selectDiv = table.add(2, 1).div({
                        cssclass: 'locationselect',
                        value: nodeId
                    });

                    var $locationtree = selectDiv.div()
                                            .$.CswNodeTree('init', {
                                                ID: o.ID,
                                                viewid: viewId,
                                                nodeid: nodeId,
                                                cswnbtnodekey: nodeKey,
                                                onSelectNode: function (optSelect) {
                                                    onTreeSelect(selectDiv, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, o.onChange);
                                                },
                                                onInitialSelectNode: function (optSelect) {
                                                    onTreeSelect(selectDiv, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, function () { });
                                                },
                                                //SelectFirstChild: false,
                                                //UsePaging: false,
                                                UseScrollbars: false,
                                                IncludeInQuickLaunch: false,
                                                ShowToggleLink: false,
                                                DefaultSelect: Csw.enums.nodeTree_DefaultSelect.root.name
                                            });

                    selectDiv.$.CswComboBox('init', { 'ID': o.ID + '_combo',
                        'TopContent': name,
                        'SelectContent': $locationtree,
                        'Width': '290px',
                        onClick: function () {
                            var first = true;
                            return function () {
                                // only do this once
                                if (first) {
                                    $locationtree.CswNodeTree('expandAll');
                                    first = false;
                                }
                            };
                        } ()
                    });
                    propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectDiv.val()); }, Csw.nodeHoverOut);
                }
            },
            save: function (o) { //($propdiv, $xml
                var attributes = { nodeid: null };
                var selectDiv = o.propDiv.find('.locationselect');
                if (false === Csw.isNullOrEmpty(selectDiv)) {
                    attributes.nodeid = selectDiv.val();
                }
                Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
            }
        };


        function onTreeSelect(selectDiv, itemid, text, iconurl, onChange) {
            if (itemid === 'root') itemid = '';   // case 21046
            selectDiv.$.CswComboBox('TopContent', text);
            if (selectDiv.val() !== itemid) {
                selectDiv.val(itemid);
                onChange();
            }
            setTimeout(function () { selectDiv.$.CswComboBox('close'); }, 300);
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
