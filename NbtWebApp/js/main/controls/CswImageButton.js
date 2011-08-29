/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

(function ($) { /// <param name="$" type="jQuery" />
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
					  .css('display', 'inline-block')
					  .click(function () { 
								var NewButtonType = o.onClick($ImageDiv);
								setButton(NewButtonType);
								return false;
							});

		setButton(o.ButtonType);

		function setButton(NewButtonType) {
			var Multiplier = -18;
			if (NewButtonType !== undefined && NewButtonType !== CswImageButton_ButtonType.None) {
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
