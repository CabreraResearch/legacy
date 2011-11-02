/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />

; (function ($) {
	$.fn.CswMenuHeader = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getHeaderMenu',
			onLogout: function() { },
			onQuotas: function () { },
			onSessions: function () { },
			onSuccess: function () { }
		};

		if (options) {
			$.extend(o, options);
		}

		var $MenuDiv = $(this);

		CswAjaxJson({
			url: o.Url,
			data: {},
			success: function (data) {
				var $ul = $('<ul class="topnav"></ul>');
				$MenuDiv.text('')
						.append($ul);
                
			    for (var menuItem in data) {
			        if (data.hasOwnProperty(menuItem)) {
                        var thisItem = data[menuItem];
			            if (!isNullOrEmpty(menuItem))
			            {
			                var $li = HandleMenuItem({ $ul: $ul, 
			                                            itemKey: menuItem,
			                                            itemJson: thisItem, 
			                                            onLogout: o.onLogout,
														onQuotas: o.onQuotas,
														onSessions: o.onSessions });

			                if (isTrue(thisItem.haschildren)) {
			                    delete thisItem.haschildren;
			                    var $subul = $('<ul class="subnav"></ul>')
    			                    .appendTo($li);
			                    for (var childItem in thisItem) {
			                        if (thisItem.hasOwnProperty(childItem)) {
			                            var thisChild = thisItem[childItem];
			                            HandleMenuItem({ $ul: $subul, 
			                                             itemKey: childItem,
			                                             itemJson: thisChild, 
			                                             onLogout: o.onLogout,
														 onQuotas: o.onQuotas,
														 onSessions: o.onSessions });
			                        }
			                    }
			                }
			            }
			        }
			    }
			    $ul.CswMenu();

				o.onSuccess();

			} // success{}
		}); // $.ajax({

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

