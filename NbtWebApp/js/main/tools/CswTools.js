/// <reference path="../../../js/thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

// adapted from http://plugins.jquery.com/project/SafeEnter
// by teedyay
; (function ($) { /// <param name="$" type="jQuery" />
    $.fn.listenForEnter = function ()
    {
        var $elements = this;
        return $elements.each(function ()
        {
	        var $element = $(this);
            $element.focus(function ()
            {
                $element.data('safeEnter_InAutocomplete', false);
            });
            $element.keypress(function (e)
            {
                var key = (e.keyCode ? e.keyCode : e.which);
                switch (key)
                {
                    case 13:
                        // Fire the event if:
                        //   - we're not currently in the browser's Autocomplete, or
                        //   - this isn't a textbox, or
                        //   - this is Opera (which provides its own protection)
                        if (!$element.data('safeEnter_InAutocomplete') || !$element.is('input[type=text]') || $.browser.opera)
                        {
                            $element.trigger('pressedEnter', e);
                        }
                        $element.data('safeEnter_InAutocomplete', false);
                        break;

                    case 40:
                    case 38:
                    case 34:
                    case 33:
                        // down=40,up=38,pgdn=34,pgup=33
                        $element.data('safeEnter_InAutocomplete', true);
                        break;

                    default:
                        $element.data('safeEnter_InAutocomplete', false);
                        break;
                }
            });
        });
    };

    $.fn.clickOnEnter = function ($target)
    {
        var $parents = this;
        return $parents.each(function ()
        {
			var $parent = $(this);
            $parent.listenForEnter()
				    .bind('pressedEnter', function ()
				    {
				        $target.click();
				    });
        });
    };
    
    $.fn.replaceText = function( search, replace ) { /// <param name="$" type="jQuery" />
        /// <summary>
        ///   Replaces text or HTML in a jQuery object
        /// </summary>
        /// <param name="search" type="RegExp|String">A RegExp object or substring to be replaced</param>
        /// <param name="replace" type="String|Function">The String that replaces the substring received from the search argument, or a function to be invoked to create the new substring. </param>
        /// <returns type="jQuery>The initial jQuery collection of elementss</returns>
    
        var $ret = this;
        if ($ret instanceof jQuery)
        {
            $ret.each(function(){
                
                var $node = this;
                var oldVal;
                var newVal = '';

                if( !isNullOrEmpty($node) )
                {
                    // 3 === Node.TEXT_NODE
                    if (!isNullOrEmpty( $node ) && $node.nodeType === 3 ) 
                    {
                        oldVal = tryParseString( $node.val(), '');
                        if( !isNullOrEmpty(oldVal) )
                        {
                            log(oldVal,true);
                            newVal = oldVal.replace( search, replace );
                            if ( newVal !== oldVal ) 
                            {
                                $node.replaceWith(newVal);
                            }
                        }
                    }
                }
                //recurse each $node
                $node.replaceText( search, replace );
            });

        }
        return $ret;
    
  };  

})(jQuery);


