
; (function ($) {
    $.fn.CswImageButton = function (options) {


        var o = {
            ButtonType: CswImageButton_ButtonType.None,
            Active: false,
            AlternateText: '',
            ID: '',
            Required: false,
            onClick: function (alttext) { return CswImageButton_ButtonType.None; }
        };

        if (options) {
            $.extend(o, options);
        }

        $Div = $(this);
        //$Div.contents().remove();


        var $ImageDiv = $('<div id="' + o.ID + '" class="divbutton" title="' + o.AlternateText + '" />"')
                      .appendTo($Div)
                      .click(function () { onClick(); });

        setButton(o.ButtonType);


        function onClick() {
            var NewButtonType = o.onClick($ImageDiv);
            setButton(NewButtonType);
            return false;
        } // onClick()

        function setButton(NewButtonType) {
            var Multiplier = -18;
            if (NewButtonType != undefined && NewButtonType != CswImageButton_ButtonType.None) {
                $ImageDiv.get(0).style.background = 'url(\'Images/buttons/buttons18.gif\') 0px ' + NewButtonType * Multiplier + 'px no-repeat';
                $ImageDiv.unbind('mouseover');
                $ImageDiv.unbind('mouseout');
                $ImageDiv.unbind('mousedown');
                $ImageDiv.unbind('mouseup');
                $ImageDiv.bind('mouseover', function () { this.style.backgroundPosition = '-18px ' + NewButtonType * Multiplier + 'px'; })
                $ImageDiv.bind('mouseout', function () { this.style.backgroundPosition = '0px ' + NewButtonType * Multiplier + 'px'; })
                $ImageDiv.bind('mousedown', function () { this.style.backgroundPosition = '-36px ' + NewButtonType * Multiplier + 'px'; })
                $ImageDiv.bind('mouseup', function () { this.style.backgroundPosition = '-18px ' + NewButtonType * Multiplier + 'px'; })
            }
        } // setOffset()

        return $ImageDiv;
    };
})(jQuery);
