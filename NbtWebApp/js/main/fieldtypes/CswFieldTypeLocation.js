/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    $.fn.CswFieldTypeLocation = function (method) {

        var pluginName = 'CswFieldTypeLocation';

        var methods = {
            init: function (o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 
            
                var $Div = $(this);
                $Div.contents().remove();
                var propVals = o.propData.values;
                var nodeId = (false === o.Multi) ? Csw.string(propVals.nodeid).trim() : '';
                var nodeKey = (false === o.Multi) ? Csw.string(propVals.nodekey).trim() : '';
                var name = (false === o.Multi) ? Csw.string(propVals.name).trim() : Csw.enums.multiEditDefaultValue;
                var path = (false === o.Multi) ? Csw.string(propVals.path).trim() : Csw.enums.multiEditDefaultValue;
                var viewId = Csw.string(propVals.viewid).trim();

                if(o.ReadOnly) {
                    $Div.append(path);
                    $Div.hover(function (event) { Csw.nodeHoverIn(event, nodeId); }, Csw.nodeHoverOut);
                } else {
                    var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

                    var $pathcell = $table.CswTable('cell', 1, 1);
                    $pathcell.CswAttrDom('colspan', '2');
                    $pathcell.append(path + "<br/>");

                    var $selectcell = $table.CswTable('cell', 2, 1);
                    var $selectdiv = $('<div class="locationselect" value="'+ nodeId +'"/>' )
                                        .appendTo($selectcell);

                    var $locationtree = $('<div />')
                                            .CswNodeTree('init', {  
                                                ID: o.ID,
                                                viewid: viewId,
                                                nodeid: nodeId,
                                                cswnbtnodekey: nodeKey,
                                                onSelectNode: function (optSelect) {
                                                    onTreeSelect($selectdiv, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, o.onchange);
                                                },
                                                onInitialSelectNode: function (optSelect) {
                                                    onTreeSelect($selectdiv, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, function () {}); 
                                                }, 
                                                //SelectFirstChild: false,
                                                //UsePaging: false,
                                                UseScrollbars: false,
                                                IncludeInQuickLaunch: false,
                                                ShowToggleLink: false,
                                                DefaultSelect: Csw.enums.nodeTree_DefaultSelect.root.name
                                            });
    
                    $selectdiv.CswComboBox( 'init', {	'ID': o.ID + '_combo', 
                                                        'TopContent': name,
                                                        'SelectContent': $locationtree,
                                                        'Width': '290px',
                                                        onClick: function () {
                                                                    var first = true;
                                                                    return function () { 
                                                                        // only do this once
                                                                        if(first) {
                                                                            $locationtree.CswNodeTree('expandAll');
                                                                            first = false;
                                                                        }
                                                                    }
                                                                }()
                                                    });

                    $Div.hover(function (event) { Csw.nodeHoverIn(event, $selectdiv.val()); }, Csw.nodeHoverOut);
                }
            },
            save: function (o) { //($propdiv, $xml
                var attributes = { nodeid: null };
                var $selectdiv = o.$propdiv.find('.locationselect');
                if (false === Csw.isNullOrEmpty($selectdiv)) {
                    attributes.nodeid = $selectdiv.val();
                }
                Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
            }
        };
    
    
        function onTreeSelect($selectdiv, itemid, text, iconurl, onchange)
        {
            if(itemid === 'root') itemid = '';   // case 21046
            $selectdiv.CswComboBox( 'TopContent', text );
            if($selectdiv.val() !== itemid)
            {
                $selectdiv.val(itemid);
                onchange();
            }
            setTimeout(function () { $selectdiv.CswComboBox( 'close'); }, 300);
        }
        
        // Method calling logic
        if ( methods[method] ) {
            return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
            return methods.init.apply( this, arguments );
        } else {
            $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
