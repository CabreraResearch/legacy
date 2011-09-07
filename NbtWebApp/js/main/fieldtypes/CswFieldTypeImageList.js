/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeImageList';

    var methods = {
        init: function(o) {
			
			var $Div = $(this);
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? tryParseString(propVals.value).trim() : CswMultiEditDefaultValue;
            var options = propVals.options;
            var width = tryParseString(propVals.width);
            var height = tryParseString(propVals.height);
            
            if (!o.ReadOnly) {
				var $select = $('<select id="' + o.ID + '"></select>')
								.append('<option value="">Select...</option>')
								.appendTo($Div);
                if (o.Multi) {
                    $select.append('<option value="' + CswMultiEditDefaultValue + ' selected="selected">' + CswMultiEditDefaultValue + '</option>');
                }
				var $HiddenValue = $('<textarea style="display: none;" name="' + o.ID + '_value" id="' + o.ID + '_value">'+ value +'</textarea>')
		                            .appendTo($Div);
                $select.change(function() { 
					var $selected = $select.children(':selected');
					addImage($selected.text(), $selected.CswAttrDom('value'), true);
					addValue($selected.CswAttrDom('value'));
					$selected.remove();
					o.onchange();
				});
                
                crawlObject(options, 
				    function(thisOpt, key) {
				        if (isTrue(thisOpt.selected)) {
						    addImage(thisOpt.text, thisOpt.value, false);
					    } else {
						    if (!o.ReadOnly) {			
							    $select.append('<option value="'+ thisOpt.value +'">'+ thisOpt.text +'</option>');
						    }
					    }
				    }, 
				    false);
			}

            var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
			var currCol = 1;

            function addImage(name, href, doAnimation) {
				var $imagecell = $table.CswTable('cell', 1, currCol)
									.css({ 'text-align': 'center',
											'padding-left': '10px' });
				var $namecell = $table.CswTable('cell', 2, currCol)
									.css({ 'text-align': 'center',
											'padding-left': '10px' });
				currCol++;

				if (doAnimation) {
					$imagecell.hide();
					$namecell.hide();
				}

				$imagecell.append('<a href="'+ href +'" target="_blank"><img src="' + href + '" alt="' + name + '" width="'+ width +'" height="'+ height +'"/></a>')
				if (name !== href) {
					$namecell.append('<a href="'+ href +'" target="_blank">'+ name +'</a>');
				}
				if (!o.ReadOnly) {
					$namecell.CswImageButton({
						ButtonType: CswImageButton_ButtonType.Delete,
						AlternateText: 'Remove',
						ID: makeId({ 'prefix': 'image_' + currCol, 'id': 'rembtn' }),
						onClick: function ($ImageDiv) {
							$namecell.fadeOut('fast');
							$imagecell.fadeOut('fast');

							removeValue(href);
							$select.append('<option value="'+ href +'">'+ name +'</option>');

							o.onchange();
							return CswImageButton_ButtonType.None; 
						} // onClick
					}); // CswImageButton
				} // if(!o.ReadOnly)

				if(doAnimation) {
					$imagecell.fadeIn('fast');
					$namecell.fadeIn('fast'); 
				}

			} // addImage()

			function addValue(valueToAdd) {
				var currentvalue = $HiddenValue.val();
				if(!isNullOrEmpty(currentvalue)) currentvalue += '\n';
				$HiddenValue.text(currentvalue + valueToAdd);
			}
            
			function removeValue(valueToRemove) {
				var currentvalue = $HiddenValue.val();
				var splitvalue = currentvalue.split('\n');
				var newvalue = '';
				for(var i = 0; i < splitvalue.length; i++) {
					if(splitvalue[i] != valueToRemove) {
						if(!isNullOrEmpty(newvalue)) newvalue += '\n';
						newvalue += splitvalue[i];
					}
				}
				$HiddenValue.text(newvalue);
			}

        },
        save: function(o) {
			var $HiddenValue = o.$propdiv.find('#' + o.ID + '_value');
			o.propData.values.value = $HiddenValue.text();
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeImageList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
