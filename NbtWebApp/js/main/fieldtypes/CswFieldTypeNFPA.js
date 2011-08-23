/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeNFPA';

    var methods = {
        init: function(o) {

            var $Div = $(this);

			var red = o.propData.flammability;
			var yellow = o.propData.reactivity;
			var blue = o.propData.health;
			var white = o.propData.special;

			var $outertable = $Div.CswTable('init', { ID: o.ID + '_tbl' });

			var $table = $outertable.CswTable('cell', 1, 1)
										.CswTable('init', { ID: o.ID + '_tbl' })
										.addClass('CswFieldTypeNFPA_table');

			var $cell11 = $table.CswTable('cell', 1, 1)
							.addClass('CswFieldTypeNFPA_red CswFieldTypeNFPA_cell');
			var $reddiv = $('<div class="CswFieldTypeNFPA_text"></div>')
							.appendTo($cell11);

			var $cell12 = $table.CswTable('cell', 1, 2)
							.addClass('CswFieldTypeNFPA_yellow CswFieldTypeNFPA_cell');
			var $yellowdiv = $('<div class="CswFieldTypeNFPA_text"></div>')
							.appendTo($cell12);

			var $cell21 = $table.CswTable('cell', 2, 1)
							.addClass('CswFieldTypeNFPA_blue CswFieldTypeNFPA_cell');
			var $bluediv = $('<div class="CswFieldTypeNFPA_text"></div>')
							.appendTo($cell21);

			var $cell22 = $table.CswTable('cell', 2, 2)
							.addClass('CswFieldTypeNFPA_white CswFieldTypeNFPA_cell');
			var $whitediv = $('<div class="CswFieldTypeNFPA_text"></div>')
							.appendTo($cell22);

			function setValue($div, value)
			{
				$div.text(value);

				if(value === "W")
					$div.addClass("strikethrough");
				else
					$div.removeClass("strikethrough");
			}

			setValue($reddiv, red);
			setValue($yellowdiv, yellow);
			setValue($bluediv, blue);
			setValue($whitediv, white);

            if(!o.ReadOnly)
            {
				var $edittable = $outertable.CswTable('cell', 1, 2)
												.CswTable('init', { ID: o.ID + '_edittbl',
																	FirstCellRightAlign: true });

				function makeSelect($cell, id, selected, $div)
				{
					var $sel = $('<select id="' + o.ID + '_' + id + '"></select>')
									.appendTo($cell);
					for(var i = 0; i <= 4; i++)
					{
						var $opt = $('<option value="'+ i +'">'+ i +'</option>')
										.appendTo($sel);
					}
					$sel.val(selected);
					$sel.change(function() {
						setValue($div, $sel.val());
					});
				}

				$edittable.CswTable('cell', 1, 1).append('Flammability');
				$edittable.CswTable('cell', 2, 1).append('Health');
				$edittable.CswTable('cell', 3, 1).append('Reactivity');
				$edittable.CswTable('cell', 4, 1).append('Special');

				makeSelect($edittable.CswTable('cell', 1, 2), 'red', red, $reddiv);
				makeSelect($edittable.CswTable('cell', 2, 2), 'yellow', yellow, $yellowdiv);
				makeSelect($edittable.CswTable('cell', 3, 2), 'blue', blue, $bluediv);

				var $whitesel = $('<select id="' + o.ID + '_white"></select>')
									.appendTo($edittable.CswTable('cell', 4, 2))
									.append('<option value=""></option>')
									.append('<option value="ACID">ACID</option>')
									.append('<option value="ALK">ALK</option>')
									.append('<option value="BIO">BIO</option>')
									.append('<option value="COR">COR</option>')
									.append('<option value="CRYO">CRYO</option>')
									.append('<option value="CYL">CYL</option>')
									.append('<option value="OX">OX</option>')
									.append('<option value="POI">POI</option>')
									.append('<option value="RAD">RAD</option>')
									.append('<option value="W">W</option>')
									.change(function() {
										setValue($whitediv, $whitesel.val());
									});
				$whitesel.val(white);

            } // if(!o.ReadOnly)
        },
        save: function(o) {
				o.propData.flammability = o.$propdiv.find('#' + o.ID + '_red').val();
				o.propData.reactivity = o.$propdiv.find('#' + o.ID + '_yellow').val();
				o.propData.health = o.$propdiv.find('#' + o.ID + '_blue').val();
				o.propData.special = o.$propdiv.find('#' + o.ID + '_white').val();
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeNFPA = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
