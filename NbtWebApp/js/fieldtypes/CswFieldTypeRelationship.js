/// <reference path="../jquery/jquery-1.6-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    $.fn.CswFieldTypeRelationship = function (method) {

        var PluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function(o) { //nodepk = o.nodeid, o.$propxml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey
            
                var $Div = $(this);
                $Div.contents().remove();

                var SelectedNodeId = o.$propxml.children('nodeid').text().trim();
                if( o.relatednodeid !== '' && o.relatednodeid !== undefined && 
                    ( SelectedNodeId === '' || SelectedNodeId === undefined ) )
                {
                    SelectedNodeId = o.relatednodeid;
                }
                var SelectedName = o.$propxml.children('name').text().trim();
                var NodeTypeId = o.$propxml.children('nodetypeid').text().trim();
                var AllowAdd = isTrue( o.$propxml.children('allowadd').text().trim() );
                var $Options = o.$propxml.children('options');

                if(o.ReadOnly)
                {
                    $Div.append(SelectedName);
                }
                else 
                {
                    var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

                    var $selectcell = $table.CswTable('cell', 1, 1);
                    var $SelectBox = $('<select id="'+ o.ID +'" name="'+ o.ID +'" class="selectinput" />"' )
                                        .appendTo($selectcell)
                                        .change(o.onchange);

                    $Options.children().each(function() {
                        var $this = $(this);
                        $SelectBox.append('<option value="' + $this.CswAttrXml('id') + '">' + $this.CswAttrXml('value') + '</option>');
                    });

                    $SelectBox.val( SelectedNodeId );
                    
                    if( !isNullOrEmpty( NodeTypeId ) && AllowAdd )
					{
						var $addcell = $table.CswTable('cell', 1, 2);
						var $AddButton = $('<div />').appendTo($addcell);
						$AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
													AlternateText: "Add New",
													onClick: function($ImageDiv) { 
															$.CswDialog('AddNodeDialog', {
																							'nodetypeid': NodeTypeId, 
																							'onAddNode': function(nodeid, cswnbtnodekey) { o.onReload(); }
																						});
															return CswImageButton_ButtonType.None;
														}
													});
					}

                    if(o.Required)
                    {
                        $SelectBox.addClass("required");
                    }
                }

            },
            save: function(o) {
                    var $SelectBox = o.$propdiv.find('select');
                    o.$propxml.children('nodeid').text($SelectBox.val());
                }
        };
    
        // Method calling logic
        if ( methods[method] ) {
            return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
            return methods.init.apply( this, arguments );
        } else {
            $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
