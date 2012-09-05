/// <reference path="~app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    $.fn.CswFieldTypeLocation = function (method) {

        var pluginName = 'CswFieldTypeLocation';

        var methods = {
            init: function (o) {

                var propDiv, propVals, locationobjectclassid, locationnodetypeids, relatedmatch, 
                    nodeId, name, path, nodeKey, viewId, comboBox, table, selectDiv;

                function makeLocationCombo() {
                    var locationTree;
                    
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

                    locationTree = Csw.nbt.nodeTree({
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
                } // makeLocationCombo()

                propDiv = o.propDiv;
                propDiv.empty();
                propVals = o.propData.values;

                locationobjectclassid = propVals.locationobjectclassid;
                locationnodetypeids = propVals.locationnodetypeids;  // array

                relatedmatch = (o.relatedobjectclassid === locationobjectclassid);
                Csw.each(locationnodetypeids, function (thisNTid) {
                    relatedmatch = (relatedmatch || thisNTid === o.relatednodetypeid);
                });

                nodeKey = ''; //(false === o.Multi) ? Csw.string(propVals.nodekey).trim() : '';
                if (relatedmatch) {
                    nodeId = (false === o.Multi) ? Csw.string(propVals.nodeid, o.relatednodeid).trim() : '';
                    name = (false === o.Multi) ? Csw.string(propVals.name, o.relatednodename).trim() : Csw.enums.multiEditDefaultValue;
                    path = (false === o.Multi) ? Csw.string(propVals.path, o.relatednodename).trim() : Csw.enums.multiEditDefaultValue;
                } else {
                    nodeId = (false === o.Multi) ? Csw.string(propVals.nodeid, '').trim() : '';
                    name = (false === o.Multi) ? Csw.string(propVals.name, '').trim() : Csw.enums.multiEditDefaultValue;
                    path = (false === o.Multi) ? Csw.string(propVals.path, '').trim() : Csw.enums.multiEditDefaultValue;
                }
                viewId = Csw.string(propVals.viewid).trim();
                
                table = propDiv.table({
                    ID: Csw.makeId(o.ID, 'tbl')
                });

                table.cell(1, 1).text(path);

                selectDiv = table.cell(2, 1).div({
                    cssclass: 'locationselect',
                    value: nodeId
                });

                table.cell(2, 1).hide();
                if (false === o.ReadOnly) {


                    if(o.EditMode === Csw.enums.editMode.Add) {
                        makeLocationCombo();
                    } else {
                        table.cell(1, 2).icon({
                            ID: Csw.makeId(o.ID, 'toggle'),
                            iconType: Csw.enums.iconType.pencil,
                            hovertext: 'Edit',
                            size: 16,
                            isButton: true,
                            onClick: makeLocationCombo
                        });
                    }

                    propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectDiv.propNonDom('value')); }, 
                                    function (event) { Csw.nodeHoverOut(event, selectDiv.propNonDom('value')); });
                }
            },
            save: function (o) { //($propdiv, $xml
                var attributes = { nodeid: null };
                var compare = {};
                var selectDiv = o.propDiv.find('.locationselect');
                if (false === Csw.isNullOrEmpty(selectDiv)) {
                    attributes.nodeid = selectDiv.propNonDom('value');
                    compare = attributes;
                }
                Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
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
