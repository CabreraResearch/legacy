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
                addNewOption: false,
                excludeNodeTypeIds: ''
            };
            if (options) {
                $.extend(o, options);
            }
            
            var $parent = $(this),
                $select = $('<select id="'+ o.ID +'_sel" />').css('width', '200px');
            
            if(isTrue(o.addNewOption)) {
                $select.append('<option value="[Create New]">[Create New]</option>');
            }

            $select.change(function() { if (isFunction(o.onSelect)) o.onSelect( $select.val() ); });

            CswAjaxJson({
                    url: o.NodeTypesUrl,
                    data: { ObjectClassName: tryParseString(o.objectClassName), ExcludeNodeTypeIds: o.excludeNodeTypeIds },
                    success: function (data) {
                        var ret = data;
                        ret.nodetypecount = 0;
                        //Case 24155
                        each(ret, function(thisNodeType) {
                            if(contains(thisNodeType, 'id') &&
                                    contains(thisNodeType, 'name')) {
                                var id = thisNodeType.id,
                                    name = thisNodeType.name,
                                    $thisOpt;
                                delete thisNodeType.id;
                                delete thisNodeType.name;

                                ret.nodetypecount += 1;

                                $thisOpt = $('<option value="' + id + '">' + name + '</option>');
                                each(thisNodeType, function(value, key) {
                                    $thisOpt.CswAttrXml(key, value);
                                });
                                $select.append($thisOpt);
                            }
                        });
                        
                        if (isFunction(o.onSuccess)) {
                            o.onSuccess(ret);
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

