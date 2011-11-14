/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

; (function ($)
{
    var pluginName = "CswNodeTypeSelect";

    var methods = {
        'init': function(options) {
            var o = {
                ID: '',
                NodeTypesUrl: '/NbtWebApp/wsNBT.asmx/getNodeTypes',
                nodetypeid: '',
                objectClassName: '',
                onSelect: null, //function (nodetypeid) {},
                onSuccess: null, //function () {}
                width: '',
                excludeNodeTypeIds: ''
            };
            if (options) {
                $.extend(o, options);
            }
            
            var $parent = $(this),
                $select = $('<select id="'+ o.ID +'_sel" />').css('width', '200px');
            
            $select.change(function() { if (isFunction(o.onSelect)) o.onSelect( $select.val() ); });

            CswAjaxJson({
                    url: o.NodeTypesUrl,
                    data: { ObjectClassName: tryParseString(o.objectClassName), ExcludeNodeTypeIds: o.excludeNodeTypeIds },
                    success: function (data) {
                        //Case 24155
                        each(data, function(thisNodeType) {
                            var id = thisNodeType.id,
                                name = thisNodeType.name,
                                $thisOpt;
                            delete thisNodeType.id;
                            delete thisNodeType.name;

                            $thisOpt = $('<option value="' + id + '">' + name + '</option>');
                            each(thisNodeType, function(value, key) {
                                $thisOpt.CswAttrXml(key, value);
                            });
                            $select.append($thisOpt);
                        });
                        if (isFunction(o.onSuccess)) {
                            o.onSuccess();
                        }
                        $select.css('width', tryParseString(o.width));    
                    }
            });
            //Case 23986: Wait until options collection is built before appending
            $parent.contents().remove();
            $parent.append($select);
            return $select;
        },
        'value': function() {
            var $select = $(this);
            return $select.val();
        }
    };
        // Method calling logic
    $.fn.CswNodeTypeSelect = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };

})(jQuery);

