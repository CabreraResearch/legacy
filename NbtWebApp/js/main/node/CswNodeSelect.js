/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

(function ($) {
    "use strict";
    var pluginName = "CswNodeSelect";

    var methods = {
        'init': function(options) {
            var o = {
                ID: '',
                NodesUrl: '/NbtWebApp/wsNBT.asmx/getNodes',
                nodetypeid: '',
                objectclassid: '',
                objectclass: '',
                onSelect: null, // function (nodeid) {},
                onSuccess: null // function () {}
            };

            if (options) {
                $.extend(o, options);
            }

            var $parent = $(this);

            var $select = $('<select id="' + o.ID + '_nodeselect" />')
                .css('width', '100px');
                                
            $select.change(function() { if(isFunction(o.onSelect)) o.onSelect( $select.val() ); });

            var jsonData = {
                NodeTypeId: o.nodetypeid,
                ObjectClassId: o.objectclassid,
                ObjectClass: o.objectclass
            };

            CswAjaxJson({
                    url: o.NodesUrl,
                    data: jsonData,
                    success: function (data) {
                        var nodeId, nodeName;
                        for (nodeId in data) {
                            if (contains(data, nodeId)) {
                                nodeName = data[nodeId];
                                $select.append('<option value="' + nodeId + '">' + nodeName + '</option>');
                            }
                        }
                        $select.css('width', '');
                        if (isFunction(o.onSuccess)) {
                            o.onSuccess();
                        }
                    }
            });
            $parent.append($select);
            return $select;
        },
        'value': function()
            {
                var $select = $(this);
                return $select.val();
            }
    };
        // Method calling logic
    $.fn.CswNodeSelect = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };

})(jQuery);

