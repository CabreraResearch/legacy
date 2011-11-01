/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />

(function ($) { /// <param name="$" type="jQuery" />
    var pluginName = 'CswImageButton';

    var methods = {

        init: function (options) {
            var o = {
                ButtonType: CswImageButton_ButtonType.None,
                Active: false,
                AlternateText: '',
                ID: '',
                cssclass: '',
                Required: false,
                onClick: function () { return CswImageButton_ButtonType.None; }
            };

            if (options) {
                $.extend(o, options);
            }

            //$Div.contents().remove();
            //using 'title' instead of 'alt' does make the alternate text appear in Chrome, 
            //but it also screws up clicking.

            var $Div = $(this),
                $ImageDiv = $('<div id="' + o.ID + '" class="divbutton ' + tryParseString(o.cssclass) + '" alt="' + o.AlternateText + '" />"')
                    .css('display', 'inline-block');

            setButton(o.ButtonType, $ImageDiv);
            $ImageDiv.bind('click', function () {
                var newButtonType = o.onClick($ImageDiv);
                return setButton(newButtonType, $ImageDiv);
            });

            $Div.append($ImageDiv);
            return $ImageDiv;
        },
        reBindClick: function (newButtonType, id, onClickEvent) {
            var $this = $(this);
            if (isNullOrEmpty($this, true)) {
                $this = $('#' + id);
            }
            if (false === isNullOrEmpty($this, true)) {
                $this.bind('click', function () {
                    if (isFunction(onClickEvent)) {
                        onClickEvent();
                    }
                    return setButton(newButtonType, $this);
                });
            }
        },
        doClick: function (newButtonType, id) {
            var $this = $(this);
            if (isNullOrEmpty($this, true)) {
                $this = $('#' + id);
            }
            return setButton(newButtonType, $this);
        }
    };

    function setButton(newButtonType, $ImageDiv) {
        var multiplier = -18;
        //Case 24112: IE7 processes url() using https but randles the response as http--prompting the security dialog.
        var port = document.location.port;
        var prefix = document.location.protocol + "//" + document.location.hostname;
        if (false === isNullOrEmpty(port) && port !== 80) {
            prefix += ':' + port;
        }
        prefix += '/NbtWebApp';
        if (newButtonType !== undefined && newButtonType !== CswImageButton_ButtonType.None) {
            $ImageDiv.get(0).style.background = 'url(\'' + prefix + '/Images/buttons/buttons18.gif\') 0px ' + newButtonType * multiplier + 'px no-repeat';
            $ImageDiv.unbind('mouseover');
            $ImageDiv.unbind('mouseout');
            $ImageDiv.unbind('mousedown');
            $ImageDiv.unbind('mouseup');
            $ImageDiv.bind('mouseover', function () {
                this.style.backgroundPosition = '-18px ' + newButtonType * multiplier + 'px';
            });
            $ImageDiv.bind('mouseout', function () {
                this.style.backgroundPosition = '0px ' + newButtonType * multiplier + 'px';
            });
            $ImageDiv.bind('mousedown', function () {
                this.style.backgroundPosition = '-36px ' + newButtonType * multiplier + 'px';
            });
            $ImageDiv.bind('mouseup', function () {
                this.style.backgroundPosition = '-18px ' + newButtonType * multiplier + 'px';
            });
        }
        return false;
    } // setOffset()

    $.fn.CswImageButton = function (method) {
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
