/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";        
    var pluginName = 'CswFieldTypeImageList';

    var methods = {
        init: function (o) {
            
            var $Div = $(this);
            var propVals = o.propData.values;

            var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue;
            var options = propVals.options;
            var width = Csw.string(propVals.width);
            var height = Csw.string(propVals.height);
            var table = Csw.controls.table({
                $parent: $Div,
                ID: Csw.controls.dom.makeId(o.ID, 'tbl')
            });
            var currCol = 1;

            
            if (false === o.ReadOnly) {
                var $select = $('<select id="' + o.ID + '"></select>')
                                .append('<option value="">Select...</option>')
                                .appendTo($Div);
                if (o.Multi) {
                    $select.append('<option value="' + Csw.enums.multiEditDefaultValue + ' selected="selected">' + Csw.enums.multiEditDefaultValue + '</option>');
                }
                var $HiddenValue = $('<textarea style="display: none;" name="' + o.ID + '_value" id="' + o.ID + '_value">'+ value +'</textarea>')
                                    .appendTo($Div);
                $select.change(function () { 
                    var $selected = $select.children(':selected');
                    addImage($selected.text(), $selected.CswAttrDom('value'), true);
                    addValue($selected.CswAttrDom('value'));
                    $selected.remove();
                    o.onChange();
                });
                
                Csw.crawlObject(options, 
                    function (thisOpt) {
                        if (Csw.bool(thisOpt.selected)) {
                            addImage(thisOpt.text, thisOpt.value, false);
                        } else {
                            if (false === o.ReadOnly) {			
                                $select.append('<option value="'+ thisOpt.value +'">'+ thisOpt.text +'</option>');
                            }
                        }
                    }, 
                    false);
            }

            function addImage(name, href, doAnimation) {
                var imageCell = table.add(1, currCol, '<a href="' + href + '" target="_blank"><img src="' + href + '" alt="' + name + '" width="' + width + '" height="' + height + '"/></a>')
                                    .css({ 'text-align': 'center',
                                            'padding-left': '10px' });
                var nameCell = table.cell(2, currCol)
                                    .css({ 'text-align': 'center',
                                            'padding-left': '10px' });
                currCol += 1;

                if (doAnimation) {
                    imageCell.hide();
                    nameCell.hide();
                }

                
                if (name !== href) {
                    nameCell.append('<a href="'+ href +'" target="_blank">'+ name +'</a>');
                }
                if (false === o.ReadOnly) {
                    nameCell.$.CswImageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                        AlternateText: 'Remove',
                        ID: Csw.controls.dom.makeId({ 'prefix': 'image_' + currCol, 'id': 'rembtn' }),
                        onClick: function () {
                            nameCell.$.fadeOut('fast');
                            imageCell.$.fadeOut('fast');

                            removeValue(href);
                            $select.append('<option value="'+ href +'">'+ name +'</option>');

                            o.onChange();
                            return Csw.enums.imageButton_ButtonType.None; 
                        } // onClick
                    }); // CswImageButton
                } // if(!o.ReadOnly)

                if(doAnimation) {
                    imageCell.$.fadeIn('fast');
                    nameCell.$.fadeIn('fast'); 
                }

            } // addImage()

            function addValue(valueToAdd) {
                var currentvalue = $HiddenValue.val();
                if(!Csw.isNullOrEmpty(currentvalue)) currentvalue += '\n';
                $HiddenValue.text(currentvalue + valueToAdd);
            }
            
            function removeValue(valueToRemove) {
                var currentvalue = $HiddenValue.val();
                var splitvalue = currentvalue.split('\n');
                var newvalue = '';
                for(var i = 0; i < splitvalue.length; i++) {
                    if(splitvalue[i] != valueToRemove) {
                        if(!Csw.isNullOrEmpty(newvalue)) newvalue += '\n';
                        newvalue += splitvalue[i];
                    }
                }
                $HiddenValue.text(newvalue);
            }

        },
        save: function (o) {
            var imageList = null;
            var $HiddenValue = o.$propdiv.find('#' + o.ID + '_value');
            if (false === Csw.isNullOrEmpty($HiddenValue)) {
                imageList = $HiddenValue.text();
            }
            var attributes = { value: imageList };
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeImageList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
