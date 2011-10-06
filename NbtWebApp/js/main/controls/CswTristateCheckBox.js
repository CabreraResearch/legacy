/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

(function ($) { /// <param name="$" type="jQuery" />

    var pluginName = 'CswTristateCheckBox';

    var methods = {
        init: function (options) {
            var o = {
                ID: '',
                prefix: '',
                Checked: '',
                ReadOnly: false,
                Required: false,
                Multi: false,
                cssclass: 'CswTristateCheckBox',
                onchange: function() { }
            };
            if(options) $.extend(o, options);

            var $parent = $(this).empty(),
                elementId = makeId({ prefix: o.prefix, ID: o.ID }),
                tristateVal = tryParseString(o.Checked, 'null').toLowerCase(), //Case 21769
                ret = $parent;
            
            if(o.ReadOnly) {
                if(o.Multi) {
                    $parent.append(CswMultiEditDefaultValue);
                } else {
                    switch (tristateVal) {
                        case 'true': $parent.append('Yes'); break;
                        case 'false': $parent.append('No'); break;
                    }
                }
            } else {
                ret = $parent.CswImageButton({ ID: elementId,  
                                        ButtonType: getButtonType(tristateVal), 
                                        AlternateText: tristateVal,
                                        cssclass: o.cssclass,
                                        onClick: function($ImageDiv) {
                                            o.onchange($ImageDiv); 
                                            return onClick($ImageDiv, o.Required);
                                        }
                                    });
            }
            return ret;
        },
        value: function () {
            var $CheckboxImage = $(this);
            var checked = $CheckboxImage.CswAttrDom('alt');
            return checked;
        },
        reBindClick: function (id, required, onClickEvent) {
            var $this = $(this),
                buttonType, checked;   
            if (isNullOrEmpty($this, true)) {
                $this = $('#' + id); 
            }
            if (false === isNullOrEmpty($this, true)) {
                $this.bind('click', function() {
                    buttonType = onClick($this, required);
                    $this.CswImageButton('doClick', buttonType);
                    if (isFunction(onClickEvent)) {
                        onClickEvent();
                    }
                    return false;
                });
            }
        }
    };

    function getButtonType(val) {
        var ret = CswImageButton_ButtonType.CheckboxNull;
        switch(val) {
            case 'true': ret = CswImageButton_ButtonType.CheckboxTrue; break;
            case 'false': ret = CswImageButton_ButtonType.CheckboxFalse; break;
        }
        return ret;
    }
    
    function onClick($ImageDiv, required) {
        var currentValue = $ImageDiv.CswAttrDom('alt');
        var newValue = CswImageButton_ButtonType.CheckboxNull;
        var newAltText = "null";
        if (currentValue === "null") {
            newValue = CswImageButton_ButtonType.CheckboxTrue;
            newAltText = "true";
        } else if ( currentValue === "false") {
            if ( isTrue(required) ) {
                newValue = CswImageButton_ButtonType.CheckboxTrue;
                newAltText = "true";
            } else {
                newValue = CswImageButton_ButtonType.CheckboxNull;
                newAltText = "null";
            }
        } else if (currentValue === "true") {
            newValue = CswImageButton_ButtonType.CheckboxFalse;
            newAltText = "false";
        }
        $ImageDiv.CswAttrDom('alt', newAltText);
        return newValue;
    } // onClick()

    // Method calling logic
    $.fn.CswTristateCheckBox = function (method)  {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } 
        else if (typeof method === 'object' || false === method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }
    };
})(jQuery);
