
// For CswImageButton
var CswImageButton_ButtonType = {
	None: -1,
	Add: 27,
	ArrowNorth: 28,
	ArrowEast: 29,
	ArrowSouth: 30,
	ArrowWest: 31,
	Calendar: 6,
	CheckboxFalse: 18,
	CheckboxNull: 19,
	CheckboxTrue: 20,
	Clear: 4,
	Clock: 10,
	ClockGrey: 11,
	Configure: 26,
	Delete: 4,
	Edit: 3,
	Fire: 5,
	PageFirst: 23,
	PagePrevious: 24,
	PageNext: 25,
	PageLast: 22,
	PinActive: 17,
	PinInactive: 15,
	Print: 2,
	Refresh: 9,
	SaveStatus: 13,
	Select: 32,
	ToggleActive: 1,
	ToggleInactive: 0,
	View: 8
};

(function ($) {
    $.fn.CswImageButton = function (options) {


        var o = {
            ButtonType: CswImageButton_ButtonType.None,
            Active: false,
            AlternateText: '',
            ID: '',
            Required: false,
            onClick: function ($ImageDiv) { return CswImageButton_ButtonType.None; }
        };

        if (options) {
            $.extend(o, options);
        }

        $Div = $(this);
        //$Div.contents().remove();

		//using 'title' instead of 'alt' does make the alternate text appear in Chrome, 
		//but it also screws up clicking.

        var $ImageDiv = $('<div id="' + o.ID + '" class="divbutton" alt="' + o.AlternateText + '" />"')
                      .appendTo($Div)
                      .click(function () { 
								var NewButtonType = o.onClick($ImageDiv);
								setButton(NewButtonType);
								return false;
							});

        setButton(o.ButtonType);

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
