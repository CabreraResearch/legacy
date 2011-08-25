/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    $.fn.CswFieldTypeRelationship = function (method) {

        var pluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function(o) { //nodepk = o.nodeid, o.propData = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey
            
                var $Div = $(this);
                $Div.contents().remove();

                var value = o.propData.value;
                
                var selectedNodeId = tryParseString(value.nodeid).trim();
                if (!isNullOrEmpty(o.relatednodeid) && isNullOrEmpty(selectedNodeId)) {
                    selectedNodeId = o.relatednodeid;
                }
                var selectedName = tryParseString(value.name).trim();
                var nodeTypeId = tryParseString(value.nodetypeid).trim();
                var allowAdd = isTrue(value.allowadd);
                var options = value.options;

                if(o.ReadOnly) {
                    $Div.append(selectedName);
                } else {
                    var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

                    var $selectcell = $table.CswTable('cell', 1, 1);
                    var $SelectBox = $('<select id="'+ o.ID +'" name="'+ o.ID +'" class="selectinput" />"' )
                                        .appendTo($selectcell)
                                        .change(o.onchange);

                    for (var optId in options) {
                        if (options.hasOwnProperty(optId)) {
                            var optVal = options[optId];
                            $SelectBox.append('<option value="' + optId + '">' + optVal + '</option>');
                        }
                    }

                    $SelectBox.val( selectedNodeId );
                    
                    if (!isNullOrEmpty(nodeTypeId) && allowAdd) {
						var $addcell = $table.CswTable('cell', 1, 2);
						var $AddButton = $('<div />').appendTo($addcell);
						$AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
													AlternateText: "Add New",
													onClick: function($ImageDiv) { 
															$.CswDialog('AddNodeDialog', {
																							'nodetypeid': nodeTypeId, 
																							'onAddNode': function(nodeid, cswnbtnodekey) { o.onReload(); }
																						});
															return CswImageButton_ButtonType.None;
														}
													});
					}

                    if (o.Required) {
                        $SelectBox.addClass("required");
                    }
                }

            },
            save: function(o) {
                    var $SelectBox = o.$propdiv.find('select');
                    o.propData.nodeid = $SelectBox.val();
                }
        };
    
        // Method calling logic
        if ( methods[method] ) {
            return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
            return methods.init.apply( this, arguments );
        } else {
            $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
