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
            var propVals = o.propData.values;
			var red = propVals.flammability;
			var yellow = propVals.reactivity;
			var blue = propVals.health;
			var white = propVals.special;

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
					var $sel = $cell.CswSelect({
											'ID': makeId({ ID: o.ID, suffix: id }),
											'selected': selected,
											'values': [{ value: '0', display: '0'},
													   { value: '1', display: '1'},
													   { value: '2', display: '2'},
													   { value: '3', display: '3'},
													   { value: '4', display: '4'}],
											'cssclass': '',
											'onChange': function () {
												setValue($div, $sel.val());
											}
										});
				} // makeSelect()

				$edittable.CswTable('cell', 1, 1).append('Flammability');
				$edittable.CswTable('cell', 2, 1).append('Health');
				$edittable.CswTable('cell', 3, 1).append('Reactivity');
				$edittable.CswTable('cell', 4, 1).append('Special');

				makeSelect($edittable.CswTable('cell', 1, 2), 'red', red, $reddiv);
				makeSelect($edittable.CswTable('cell', 2, 2), 'yellow', yellow, $yellowdiv);
				makeSelect($edittable.CswTable('cell', 3, 2), 'blue', blue, $bluediv);

				var $whitesel = $edittable.CswTable('cell', 4, 2).CswSelect({
										'ID': makeId({ ID: o.ID, suffix: 'white' }),
										'selected': white,
										'values': [ { value: 'ACID', display: 'ACID'},
													{ value: 'ALK', display: 'ALK'},
													{ value: 'BIO', display: 'BIO'},
													{ value: 'COR', display: 'COR'},
													{ value: 'CRYO', display: 'CRYO'},
													{ value: 'CYL', display: 'CYL'},
													{ value: 'OX', display: 'OX'},
													{ value: 'POI', display: 'POI'},
													{ value: 'RAD', display: 'RAD'},
													{ value: 'W', display: 'W'}],
										'cssclass': '',
										'onChange': function () {
											setValue($whitediv, $whitesel.val());
										}
									});

            } // if(!o.ReadOnly)
        },
        save: function(o) {
			var propVals = o.propData.values;	
            propVals.flammability = o.$propdiv.find('#' + o.ID + '_red').val();
			propVals.reactivity = o.$propdiv.find('#' + o.ID + '_yellow').val();
			propVals.health = o.$propdiv.find('#' + o.ID + '_blue').val();
			propVals.special = o.$propdiv.find('#' + o.ID + '_white').val();
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
