/// <reference path="~/js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = "CswNodeTypeSelect";

    var methods = {
        'init': function (options) {
            var o = {
                ID: '',
                NodeTypesUrl: '/NbtWebApp/wsNBT.asmx/getNodeTypes',
                nodetypeid: '',
                objectClassName: '',
                onSelect: null, 
                onSuccess: null, 
                width: '',
                addNewOption: false,
                excludeNodeTypeIds: ''
            };
            if (options) {
                $.extend(o, options);
            }
            
            var $parent = $(this),
                $select = $('<select id="'+ o.ID +'_sel" />').css('width', '200px');
            
            if(Csw.bool(o.addNewOption)) {
                $select.append('<option value="[Create New]">[Create New]</option>');
            }

            $select.change(function () { if (Csw.isFunction(o.onSelect)) o.onSelect( $select.val() ); });

            Csw.ajax.post({
                    url: o.NodeTypesUrl,
                    data: { ObjectClassName: Csw.string(o.objectClassName), ExcludeNodeTypeIds: o.excludeNodeTypeIds },
                    success: function (data) {
                        var ret = data;
                        ret.nodetypecount = 0;
                        //Case 24155
                        Csw.each(ret, function (thisNodeType) {
                            if(Csw.contains(thisNodeType, 'id') &&
                                    Csw.contains(thisNodeType, 'name')) {
                                var id = thisNodeType.id,
                                    name = thisNodeType.name,
                                    $thisOpt;
                                delete thisNodeType.id;
                                delete thisNodeType.name;

                                ret.nodetypecount += 1;

                                $thisOpt = $('<option value="' + id + '">' + name + '</option>');
                                Csw.each(thisNodeType, function (value, key) {
                                    $thisOpt.CswAttrNonDom(key, value);
                                });
                                $select.append($thisOpt);
                            }
                        });
                        
                        if (Csw.isFunction(o.onSuccess)) {
                            o.onSuccess(ret);
                        }
                        $select.css('width', Csw.string(o.width));    
                    }
            });
            //Case 23986: Wait until options collection is built before appending
            $parent.contents().remove();
            $parent.append($select);
            return $select;
        },
        'value': function () {
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
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };

})(jQuery);

